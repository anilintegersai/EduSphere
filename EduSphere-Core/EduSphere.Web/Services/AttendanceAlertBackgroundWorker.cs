using System.Text.Json;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AttendanceAlertBackgroundWorker(IServiceScopeFactory scopes, ILogger<AttendanceAlertBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunCycleAsync(stoppingToken); }
            catch (Exception ex) { logger.LogError(ex, "Scheduled low-attendance alert cycle failed."); }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        using var discoveryScope = scopes.CreateScope();
        var discoveryDb = discoveryScope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var due = await discoveryDb.AttendanceAlertSchedules.IgnoreQueryFilters().AsNoTracking().Where(x => !x.IsDeleted && x.IsActive && x.NextRunOn <= DateTime.UtcNow).Select(x => new { x.Id, x.TenantId }).Take(25).ToListAsync(token);
        foreach (var item in due)
        {
            using var scope = scopes.CreateScope();
            var tenant = scope.ServiceProvider.GetRequiredService<TenantContext>(); tenant.SetTenant(item.TenantId, item.TenantId.ToString());
            var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            var schedule = await db.AttendanceAlertSchedules.SingleOrDefaultAsync(x => x.Id == item.Id, token); if (schedule is null) continue;
            var job = new BackgroundJobLog { JobType = "attendance.low-alert", JobKey = $"schedule:{schedule.Id:N}:{DateTime.UtcNow:yyyyMMddHH}", PayloadJson = JsonSerializer.Serialize(new { schedule.Id, schedule.ThresholdPercentage, schedule.LookbackDays }), BranchId = schedule.BranchId, ScheduledOn = DateTime.UtcNow, StartedOn = DateTime.UtcNow, Status = BackgroundJobStatus.Running, AttemptCount = 1, MaxAttempts = 3, CorrelationId = Guid.NewGuid().ToString("N") };
            db.BackgroundJobLogs.Add(job); await db.SaveChangesAsync(token);
            try
            {
                await ProcessScheduleAsync(db, schedule, token);
                schedule.LastRunOn = DateTime.UtcNow; schedule.NextRunOn = DateTime.UtcNow.AddHours(Math.Max(1, schedule.RunEveryHours)); job.Status = BackgroundJobStatus.Succeeded; job.CompletedOn = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                job.Status = BackgroundJobStatus.Failed; job.CompletedOn = DateTime.UtcNow; job.LastError = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message; schedule.NextRunOn = DateTime.UtcNow.AddMinutes(15); logger.LogError(ex, "Attendance alert schedule {ScheduleId} failed.", schedule.Id);
            }
            await db.SaveChangesAsync(token);
        }
    }

    private static async Task ProcessScheduleAsync(TenantDbContext db, AttendanceAlertSchedule schedule, CancellationToken token)
    {
        var end = DateOnly.FromDateTime(DateTime.UtcNow); var start = end.AddDays(-Math.Max(1, schedule.LookbackDays) + 1);
        var records = await db.AttendanceRecords.Include(x => x.AttendanceSession).Where(x => x.AttendanceSession != null && x.AttendanceSession.AttendanceDate >= start && x.AttendanceSession.AttendanceDate <= end && (!schedule.BranchId.HasValue || x.AttendanceSession.BranchId == schedule.BranchId)).ToListAsync(token);
        foreach (var group in records.GroupBy(x => x.StudentProfileId))
        {
            var total = group.Count(); if (total == 0) continue;
            var attended = group.Count(x => x.Status is AttendanceStatus.Present or AttendanceStatus.Late or AttendanceStatus.HalfDay or AttendanceStatus.EarlyDeparture);
            var percentage = Math.Round(attended * 100m / total, 2); if (percentage >= schedule.ThresholdPercentage) continue;
            if (await db.AttendanceAlerts.AnyAsync(x => x.StudentProfileId == group.Key && x.PeriodStart == start && x.PeriodEnd == end, token)) continue;
            var student = await db.StudentProfiles.FindAsync([group.Key], token); if (student is null) continue;
            var alert = new AttendanceAlert { StudentProfileId = student.Id, PeriodStart = start, PeriodEnd = end, AttendancePercentage = percentage, ThresholdPercentage = schedule.ThresholdPercentage, Status = AttendanceAlertStatus.Pending, Message = $"{student.FirstName}'s attendance is {percentage:0.##}% for {start:dd MMM} to {end:dd MMM}, below the {schedule.ThresholdPercentage:0.##}% threshold." };
            db.AttendanceAlerts.Add(alert);
            var guardians = await db.StudentGuardians.Where(x => x.StudentProfileId == student.Id && x.HasPortalAccess).ToListAsync(token);
            foreach (var guardian in guardians)
            {
                var address = schedule.Channel switch { CommunicationChannel.Email => guardian.Email, CommunicationChannel.Sms or CommunicationChannel.WhatsApp => guardian.PhoneNumber, _ => guardian.ParentUserId?.ToString() };
                if (string.IsNullOrWhiteSpace(address)) continue;
                db.NotificationMessages.Add(new NotificationMessage { BranchId = student.BranchId, Channel = schedule.Channel, Subject = "Low attendance alert", Body = alert.Message, Status = NotificationStatus.Queued, ScheduledOn = DateTime.UtcNow, CreatedForUserId = guardian.ParentUserId, Recipients = [new NotificationRecipient { BranchId = student.BranchId, UserId = guardian.ParentUserId, DisplayName = guardian.FullName, DestinationAddress = address, Status = NotificationStatus.Queued }] });
            }
        }
    }
}
