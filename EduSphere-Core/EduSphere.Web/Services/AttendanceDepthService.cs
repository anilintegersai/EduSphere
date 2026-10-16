using System.Globalization;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace EduSphere.Web.Services;

public sealed class AttendanceDepthService(TenantDbContext db)
{
    public async Task<AttendanceDeviceImportBatch> ImportDeviceCsvAsync(AttendanceDevice device, Stream stream, string fileName, Guid? actorId, CancellationToken token)
    {
        var batch = new AttendanceDeviceImportBatch { AttendanceDeviceId = device.Id, FileName = Path.GetFileName(fileName), Status = AttendanceImportStatus.Processing };
        db.AttendanceDeviceImportBatches.Add(batch);
        await db.SaveChangesAsync(token);
        var errors = new List<string>();
        using var parser = new TextFieldParser(stream) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = true };
        parser.SetDelimiters(",");
        var headers = parser.EndOfData ? [] : parser.ReadFields() ?? [];
        var lookup = headers.Select((value, index) => (value, index)).ToDictionary(x => x.value.Trim(), x => x.index, StringComparer.OrdinalIgnoreCase);
        string Value(string[] row, string name) => lookup.TryGetValue(name, out var index) && index < row.Length ? row[index].Trim() : string.Empty;
        if (!lookup.ContainsKey("ExternalRecordId") || !lookup.ContainsKey("PersonCode") || !lookup.ContainsKey("PunchedOn"))
            throw new InvalidOperationException("CSV headers must include ExternalRecordId, PersonCode and PunchedOn. Direction is optional.");

        while (!parser.EndOfData)
        {
            token.ThrowIfCancellationRequested();
            var row = parser.ReadFields() ?? [];
            batch.TotalRows++;
            try
            {
                var externalId = Value(row, "ExternalRecordId");
                var personCode = Value(row, "PersonCode");
                if (string.IsNullOrWhiteSpace(externalId) || string.IsNullOrWhiteSpace(personCode) ||
                    !DateTime.TryParse(Value(row, "PunchedOn"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var punchedOn))
                    throw new InvalidOperationException("External record, person code or timestamp is invalid.");
                if (await db.AttendanceDevicePunches.AnyAsync(x => x.AttendanceDeviceId == device.Id && x.ExternalRecordId == externalId, token))
                    throw new InvalidOperationException("Duplicate external record.");
                var direction = Enum.TryParse<AttendancePunchDirection>(Value(row, "Direction"), true, out var parsedDirection) ? parsedDirection : AttendancePunchDirection.CheckIn;
                var student = await db.StudentProfiles.FirstOrDefaultAsync(x => x.AdmissionNumber == personCode || x.RollNumber == personCode, token);
                var teacher = student is null ? await db.TeacherProfiles.FirstOrDefaultAsync(x => x.EmployeeNumber == personCode, token) : null;
                var staff = student is null && teacher is null ? await db.StaffProfiles.FirstOrDefaultAsync(x => x.EmployeeNumber == personCode, token) : null;
                var punch = new AttendanceDevicePunch { AttendanceDeviceId = device.Id, AttendanceDeviceImportBatchId = batch.Id, ExternalRecordId = externalId, PersonCode = personCode, PunchedOn = punchedOn.ToUniversalTime(), Direction = direction, StudentProfileId = student?.Id, TeacherProfileId = teacher?.Id, StaffProfileId = staff?.Id, RawPayload = string.Join(',', row) };
                if (student is not null) await ApplyStudentPunchAsync(punch, student, device.BranchId, actorId, token);
                else if (teacher is not null) await ApplyTeacherPunchAsync(punch, teacher, device.BranchId, token);
                else if (staff is not null) await ApplyStaffPunchAsync(punch, staff, device.BranchId, token);
                else punch.ProcessingNotes = "No student admission/roll number or staff employee number matched.";
                punch.IsProcessed = student is not null || teacher is not null || staff is not null;
                db.AttendanceDevicePunches.Add(punch);
                batch.ImportedRows++;
            }
            catch (Exception ex)
            {
                batch.RejectedRows++;
                if (errors.Count < 20) errors.Add($"Row {batch.TotalRows + 1}: {ex.Message}");
            }
        }
        batch.Status = batch.RejectedRows == 0 ? AttendanceImportStatus.Completed : AttendanceImportStatus.CompletedWithErrors;
        batch.ErrorSummary = errors.Count == 0 ? null : string.Join(Environment.NewLine, errors);
        batch.CompletedOn = DateTime.UtcNow;
        device.LastSynchronizedOn = DateTime.UtcNow;
        await db.SaveChangesAsync(token);
        return batch;
    }

    public async Task<int> CopyPreviousDayAsync(Guid sourceSessionId, DateOnly targetDate, Guid? actorId, CancellationToken token)
    {
        var source = await db.AttendanceSessions.Include(x => x.Records).SingleAsync(x => x.Id == sourceSessionId, token);
        var target = await db.AttendanceSessions.Include(x => x.Records).FirstOrDefaultAsync(x => x.SectionId == source.SectionId && x.SubjectId == source.SubjectId && x.AttendanceDate == targetDate && x.SessionType == source.SessionType && x.PeriodNumber == source.PeriodNumber, token);
        if (target is null)
        {
            target = new AttendanceSession { BranchId = source.BranchId, SectionId = source.SectionId, SubjectId = source.SubjectId, TimetableEntryId = source.TimetableEntryId, AttendanceDate = targetDate, SessionType = source.SessionType, PeriodNumber = source.PeriodNumber, StartsAt = source.StartsAt, EndsAt = source.EndsAt, MarkedByUserId = actorId, Status = AttendanceSessionStatus.Draft, Notes = $"Copied from {source.AttendanceDate:yyyy-MM-dd}" };
            db.AttendanceSessions.Add(target);
        }
        var existing = target.Records.Select(x => x.StudentProfileId).ToHashSet();
        var copies = source.Records.Where(x => !existing.Contains(x.StudentProfileId)).Select(x => new AttendanceRecord { AttendanceSession = target, StudentProfileId = x.StudentProfileId, Status = x.Status, MarkedByUserId = actorId, MarkedOn = DateTime.UtcNow, Remarks = $"Copied from {source.AttendanceDate:yyyy-MM-dd}" }).ToList();
        db.AttendanceRecords.AddRange(copies);
        await db.SaveChangesAsync(token);
        return copies.Count;
    }

    public async Task<int> BulkMarkAsync(Guid sessionId, AttendanceStatus status, Guid? actorId, bool overwrite, CancellationToken token)
    {
        var session = await db.AttendanceSessions.Include(x => x.Records).SingleAsync(x => x.Id == sessionId, token);
        var students = await db.StudentProfiles.Where(x => x.SectionId == session.SectionId && x.Status == StudentStatus.Active).ToListAsync(token);
        foreach (var student in students)
        {
            var record = session.Records.FirstOrDefault(x => x.StudentProfileId == student.Id);
            if (record is null) db.AttendanceRecords.Add(new AttendanceRecord { AttendanceSessionId = session.Id, StudentProfileId = student.Id, Status = status, MarkedByUserId = actorId, MarkedOn = DateTime.UtcNow, Remarks = "Bulk attendance" });
            else if (overwrite) { record.Status = status; record.MarkedByUserId = actorId; record.MarkedOn = DateTime.UtcNow; record.Remarks = "Bulk attendance update"; }
        }
        await db.SaveChangesAsync(token);
        return students.Count;
    }

    public async Task<int> ReconcileLeaveAsync(Guid leaveId, Guid? actorId, CancellationToken token)
    {
        var leave = await db.LeaveApplications.SingleAsync(x => x.Id == leaveId, token);
        if (leave.Status != LeaveApplicationStatus.Approved) throw new InvalidOperationException("Only approved leave can be reconciled.");
        var records = await db.AttendanceRecords.Include(x => x.AttendanceSession).Where(x => x.StudentProfileId == leave.StudentProfileId && x.AttendanceSession != null && x.AttendanceSession.AttendanceDate >= leave.FromDate && x.AttendanceSession.AttendanceDate <= leave.ToDate).ToListAsync(token);
        var existing = await db.AttendanceLeaveReconciliations.Where(x => x.LeaveApplicationId == leaveId).Select(x => x.AttendanceRecordId).ToListAsync(token);
        foreach (var record in records.Where(x => !existing.Contains(x.Id)))
        {
            db.AttendanceLeaveReconciliations.Add(new AttendanceLeaveReconciliation { LeaveApplicationId = leave.Id, AttendanceRecordId = record.Id, PreviousStatus = record.Status, AppliedStatus = AttendanceStatus.Leave, ReconciledByUserId = actorId });
            record.Status = AttendanceStatus.Leave;
            record.Remarks = string.IsNullOrWhiteSpace(record.Remarks) ? "Reconciled with approved leave" : $"{record.Remarks}; reconciled with approved leave";
        }
        await db.SaveChangesAsync(token);
        return records.Count(x => !existing.Contains(x.Id));
    }

    private async Task ApplyStudentPunchAsync(AttendanceDevicePunch punch, StudentProfile student, Guid branchId, Guid? actorId, CancellationToken token)
    {
        if (punch.Direction is AttendancePunchDirection.BusBoarding or AttendancePunchDirection.BusAlighting)
        {
            var assignment = await db.StudentTransportAssignments.FirstOrDefaultAsync(x => x.StudentProfileId == student.Id && x.Status == TransportAssignmentStatus.Active, token);
            if (assignment is not null) db.BusAttendanceRecords.Add(new BusAttendanceRecord { BranchId = student.BranchId, StudentProfileId = student.Id, TransportRouteId = assignment.TransportRouteId, VehicleId = assignment.VehicleId, TransportRouteStopId = assignment.TransportRouteStopId, AttendanceDate = DateOnly.FromDateTime(punch.PunchedOn), Status = punch.Direction == AttendancePunchDirection.BusBoarding ? BusAttendanceStatus.Boarded : BusAttendanceStatus.Alighted, RecordedOn = punch.PunchedOn, RecordedByUserId = actorId, Notes = "Imported from attendance device" });
            return;
        }
        if (student.SectionId is not Guid sectionId || student.BranchId != branchId) return;
        var date = DateOnly.FromDateTime(punch.PunchedOn);
        var session = await db.AttendanceSessions.FirstOrDefaultAsync(x => x.SectionId == sectionId && x.AttendanceDate == date && x.SessionType == AttendanceSessionType.Daily, token);
        if (session is null) { session = new AttendanceSession { BranchId = branchId, SectionId = sectionId, AttendanceDate = date, SessionType = AttendanceSessionType.Daily, MarkedByUserId = actorId, Status = AttendanceSessionStatus.Draft, Notes = "Created from device import" }; db.AttendanceSessions.Add(session); }
        if (!await db.AttendanceRecords.AnyAsync(x => x.AttendanceSessionId == session.Id && x.StudentProfileId == student.Id, token)) db.AttendanceRecords.Add(new AttendanceRecord { AttendanceSession = session, StudentProfileId = student.Id, Status = AttendanceStatus.Present, MarkedByUserId = actorId, MarkedOn = punch.PunchedOn, Remarks = "Imported from attendance device" });
    }

    private async Task ApplyStaffPunchAsync(AttendanceDevicePunch punch, StaffProfile staff, Guid branchId, CancellationToken token)
    {
        if (staff.BranchId != branchId) return;
        var date = DateOnly.FromDateTime(punch.PunchedOn);
        var time = TimeOnly.FromDateTime(punch.PunchedOn);
        var record = await db.StaffAttendanceRecords.FirstOrDefaultAsync(x => x.StaffProfileId == staff.Id && x.AttendanceDate == date, token);
        if (record is null) { record = new StaffAttendanceRecord { StaffProfileId = staff.Id, BranchId = branchId, AttendanceDate = date, Status = StaffAttendanceStatus.Present, CheckIn = time, Notes = "Imported from attendance device" }; db.StaffAttendanceRecords.Add(record); }
        else if (punch.Direction == AttendancePunchDirection.CheckOut) record.CheckOut = time;
        else if (!record.CheckIn.HasValue || time < record.CheckIn) record.CheckIn = time;
    }

    private async Task ApplyTeacherPunchAsync(AttendanceDevicePunch punch, TeacherProfile teacher, Guid branchId, CancellationToken token)
    {
        if (teacher.BranchId != branchId) return;
        var date = DateOnly.FromDateTime(punch.PunchedOn); var time = TimeOnly.FromDateTime(punch.PunchedOn);
        var record = await db.TeacherAttendanceRecords.FirstOrDefaultAsync(x => x.TeacherProfileId == teacher.Id && x.AttendanceDate == date, token);
        if (record is null) { record = new TeacherAttendanceRecord { TeacherProfileId = teacher.Id, BranchId = branchId, AttendanceDate = date, Status = StaffAttendanceStatus.Present, CheckIn = time, Notes = "Imported from attendance device" }; db.TeacherAttendanceRecords.Add(record); }
        else if (punch.Direction == AttendancePunchDirection.CheckOut) record.CheckOut = time;
        else if (!record.CheckIn.HasValue || time < record.CheckIn) record.CheckIn = time;
    }
}
