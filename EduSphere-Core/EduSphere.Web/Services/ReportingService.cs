using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Text;
using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed record ReportData(string Title, IReadOnlyList<string> Columns, IReadOnlyList<IReadOnlyDictionary<string, string>> Rows);
public sealed record ReportFile(string FileName, string ContentType, byte[] Content, int RowCount);

public interface IReportingService
{
    IReadOnlyList<string> AvailableFields(ReportDataSource source);
    Task EnsureCatalogAsync(CancellationToken token = default);
    Task<ReportData> ExecuteAsync(ReportDefinition definition, CancellationToken token = default);
    Task<ReportExecution> ExecuteAndStoreAsync(Guid definitionId, ReportOutputFormat format, CancellationToken token = default);
    ReportFile Export(ReportData data, ReportOutputFormat format);
    Task CaptureSnapshotsAsync(DateOnly date, CancellationToken token = default);
    Task<int> ProcessSchedulesAsync(CancellationToken token = default);
}

public sealed class ReportingService(TenantDbContext db) : IReportingService
{
    private static readonly IReadOnlyDictionary<ReportDataSource, string[]> Fields = new Dictionary<ReportDataSource, string[]>
    {
        [ReportDataSource.Student360] = ["AdmissionNumber", "Student", "Branch", "Course", "Section", "AttendancePercent", "AverageResult", "FeeBalance", "LibraryIssues"],
        [ReportDataSource.TeacherWorkload] = ["EmployeeNumber", "Teacher", "Branch", "Subjects", "Sections", "WeeklyPeriods", "AttendanceSessions", "StudentResultAverage"],
        [ReportDataSource.AttendancePerformance] = ["AdmissionNumber", "Student", "Branch", "Course", "Section", "AttendancePercent", "AverageResult", "PassedExams"],
        [ReportDataSource.BranchComparison] = ["Branch", "Students", "AttendancePercent", "AverageResult", "PassPercent", "FeeCollectionPercent"],
        [ReportDataSource.CourseComparison] = ["Course", "Students", "AttendancePercent", "AverageResult", "PassPercent"],
        [ReportDataSource.ClassComparison] = ["Section", "Students", "AttendancePercent", "AverageResult", "PassPercent"],
        [ReportDataSource.Students] = ["AdmissionNumber", "Student", "Branch", "Section", "Email", "Status"],
        [ReportDataSource.Teachers] = ["EmployeeNumber", "Teacher", "Branch", "Email", "Status"],
        [ReportDataSource.Attendance] = ["Date", "Branch", "Section", "Student", "Status", "Subject"],
        [ReportDataSource.Results] = ["Student", "Branch", "Course", "Section", "Percentage", "Grade", "Passed", "PublishedOn"],
        [ReportDataSource.Finance] = ["Invoice", "Branch", "Student", "InvoiceDate", "DueDate", "Total", "Paid", "Balance", "Status"]
    };
    public IReadOnlyList<string> AvailableFields(ReportDataSource source) => Fields[source];

    public async Task EnsureCatalogAsync(CancellationToken token = default)
    {
        var seeds = new[]
        {
            ("student-360", "Student 360-degree profile", ReportDataSource.Student360),
            ("teacher-workload", "Teacher workload and performance", ReportDataSource.TeacherWorkload),
            ("attendance-performance", "Attendance versus performance", ReportDataSource.AttendancePerformance),
            ("branch-comparison", "Branch comparison", ReportDataSource.BranchComparison),
            ("course-comparison", "Course comparison", ReportDataSource.CourseComparison),
            ("class-comparison", "Class comparison", ReportDataSource.ClassComparison)
        };
        foreach (var seed in seeds)
            if (!await db.ReportDefinitions.AnyAsync(x => x.Code == seed.Item1, token))
                db.ReportDefinitions.Add(new() { Code = seed.Item1, Name = seed.Item2, DataSource = seed.Item3, SelectedFieldsJson = JsonSerializer.Serialize(Fields[seed.Item3]), IsSystem = true, IsPublished = true });
        await db.SaveChangesAsync(token);
    }

    public async Task<ReportData> ExecuteAsync(ReportDefinition definition, CancellationToken token = default)
    {
        var selected = ParseFields(definition);
        var rows = definition.DataSource switch
        {
            ReportDataSource.Student360 => await Student360(definition, token),
            ReportDataSource.TeacherWorkload => await TeacherWorkload(definition, token),
            ReportDataSource.AttendancePerformance => await AttendancePerformance(definition, token),
            ReportDataSource.BranchComparison => await Comparisons(definition, "Branch", token),
            ReportDataSource.CourseComparison => await Comparisons(definition, "Course", token),
            ReportDataSource.ClassComparison => await Comparisons(definition, "Section", token),
            ReportDataSource.Students => await Students(definition, token),
            ReportDataSource.Teachers => await Teachers(definition, token),
            ReportDataSource.Attendance => await Attendance(definition, token),
            ReportDataSource.Results => await Results(definition, token),
            ReportDataSource.Finance => await Finance(definition, token),
            _ => []
        };
        var projected = rows.Select(row => (IReadOnlyDictionary<string, string>)selected.ToDictionary(field => field, field => row.GetValueOrDefault(field, ""))).ToList();
        return new(definition.Name, selected, projected);
    }

    public async Task<ReportExecution> ExecuteAndStoreAsync(Guid definitionId, ReportOutputFormat format, CancellationToken token = default)
    {
        var definition = await db.ReportDefinitions.SingleAsync(x => x.Id == definitionId, token);
        var execution = new ReportExecution { TenantId = definition.TenantId, BranchId = definition.BranchId, ReportDefinitionId = definition.Id, Format = format, Status = ReportExecutionStatus.Running, StartedOn = DateTime.UtcNow };
        db.ReportExecutions.Add(execution); await db.SaveChangesAsync(token);
        try { var data = await ExecuteAsync(definition, token); var file = Export(data, format); execution.Status = ReportExecutionStatus.Succeeded; execution.CompletedOn = DateTime.UtcNow; execution.RowCount = file.RowCount; execution.FileName = file.FileName; execution.ContentType = file.ContentType; execution.OutputContent = file.Content; }
        catch (Exception ex) { execution.Status = ReportExecutionStatus.Failed; execution.CompletedOn = DateTime.UtcNow; execution.ErrorMessage = ex.Message; }
        await db.SaveChangesAsync(token); return execution;
    }

    public ReportFile Export(ReportData data, ReportOutputFormat format) => format switch
    {
        ReportOutputFormat.Excel => new(Safe(data.Title) + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Excel(data), data.Rows.Count),
        ReportOutputFormat.Pdf => new(Safe(data.Title) + ".pdf", "application/pdf", Pdf(data), data.Rows.Count),
        _ => new(Safe(data.Title) + ".csv", "text/csv", Csv(data), data.Rows.Count)
    };

    public async Task CaptureSnapshotsAsync(DateOnly date, CancellationToken token = default)
    {
        if (await db.AnalyticsSnapshots.AnyAsync(x => x.SnapshotDate == date && x.MetricKey == "students.active", token)) return;
        var branches = await db.Branches.AsNoTracking().ToListAsync(token);
        foreach (var branch in branches)
        {
            var studentCount = await db.StudentProfiles.CountAsync(x => x.BranchId == branch.Id && x.Status == StudentStatus.Active, token);
            var resultAvg = await db.Results.Where(x => x.BranchId == branch.Id).Select(x => (decimal?)x.Percentage).AverageAsync(token) ?? 0;
            var records = await db.AttendanceRecords.Include(x => x.AttendanceSession).Where(x => x.AttendanceSession!.BranchId == branch.Id).Select(x => x.Status).ToListAsync(token);
            var attendance = records.Count == 0 ? 0 : records.Count(x => x is AttendanceStatus.Present or AttendanceStatus.Late) * 100m / records.Count;
            db.AnalyticsSnapshots.AddRange(Snapshot(branch, date, "students.active", studentCount), Snapshot(branch, date, "results.average", resultAvg), Snapshot(branch, date, "attendance.percent", attendance));
        }
        await db.SaveChangesAsync(token);
    }

    public async Task<int> ProcessSchedulesAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow; var due = await db.ReportSchedules.Include(x => x.ReportDefinition).Where(x => x.IsActive && x.NextRunOn <= now).ToListAsync(token); var count = 0;
        foreach (var schedule in due)
        {
            var execution = await ExecuteAndStoreAsync(schedule.ReportDefinitionId, schedule.Format, token);
            foreach (var recipient in schedule.Recipients.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                db.NotificationMessages.Add(new() { TenantId = schedule.TenantId, BranchId = schedule.BranchId, Channel = CommunicationChannel.Email, Subject = $"Scheduled report: {schedule.ReportDefinition!.Name}", Body = execution.Status == ReportExecutionStatus.Succeeded ? $"Your scheduled report is ready. Open Reporting > Executions to download {execution.FileName}." : $"The scheduled report failed: {execution.ErrorMessage}", Status = NotificationStatus.Queued, ScheduledOn = now, Recipients = [new() { TenantId = schedule.TenantId, BranchId = schedule.BranchId, DisplayName = recipient, DestinationAddress = recipient, Status = NotificationStatus.Queued }] });
            schedule.LastRunOn = now; schedule.NextRunOn = NextRun(schedule, now); count++;
        }
        await db.SaveChangesAsync(token); return count;
    }

    private List<string> ParseFields(ReportDefinition definition)
    {
        var allowed = Fields[definition.DataSource]; string[]? requested = null; try { requested = JsonSerializer.Deserialize<string[]>(definition.SelectedFieldsJson); } catch { }
        var selected = requested?.Where(allowed.Contains).Distinct().ToList() ?? []; return selected.Count == 0 ? allowed.ToList() : selected;
    }
    private Dictionary<string, string> Filters(ReportDefinition definition) { try { return JsonSerializer.Deserialize<Dictionary<string, string>>(definition.FiltersJson) ?? []; } catch { return []; } }
    private async Task<List<Dictionary<string, string>>> Student360(ReportDefinition d, CancellationToken t) { var f = Filters(d); var q = db.StudentProfiles.Include(x => x.Branch).Include(x => x.Section)!.ThenInclude(x => x!.Batch)!.ThenInclude(x => x!.Course).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); if (f.TryGetValue("studentId", out var s) && Guid.TryParse(s, out var id)) q = q.Where(x => x.Id == id); var students = await q.Take(500).ToListAsync(t); return await StudentRows(students, true, t); }
    private async Task<List<Dictionary<string, string>>> AttendancePerformance(ReportDefinition d, CancellationToken t) { var q = db.StudentProfiles.Include(x => x.Branch).Include(x => x.Section)!.ThenInclude(x => x!.Batch)!.ThenInclude(x => x!.Course).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); return await StudentRows(await q.Take(2000).ToListAsync(t), false, t); }
    private async Task<List<Dictionary<string, string>>> StudentRows(List<StudentProfile> students, bool full, CancellationToken t)
    {
        var ids = students.Select(x => x.Id).ToList(); var attendance = await db.AttendanceRecords.Where(x => ids.Contains(x.StudentProfileId)).Select(x => new { x.StudentProfileId, x.Status }).ToListAsync(t); var results = await db.Results.Where(x => ids.Contains(x.StudentProfileId)).Select(x => new { x.StudentProfileId, x.Percentage, x.IsPassed }).ToListAsync(t); var fees = full ? await db.FeeInvoices.Where(x => x.StudentProfileId.HasValue && ids.Contains(x.StudentProfileId.Value)).Select(x => new { Id = x.StudentProfileId!.Value, Balance = x.TotalAmount - x.PaidAmount }).ToListAsync(t) : []; var issues = full ? await db.LibraryMembers.Where(x => x.StudentProfileId.HasValue && ids.Contains(x.StudentProfileId.Value)).Select(x => new { Id = x.StudentProfileId!.Value, Member = x.Id }).ToListAsync(t) : []; var memberIds = issues.Select(x => x.Member).ToList(); var issueCounts = full ? await db.LibraryBookIssues.Where(x => memberIds.Contains(x.LibraryMemberId) && x.Status != LibraryIssueStatus.Returned).GroupBy(x => x.LibraryMemberId).Select(x => new { x.Key, Count = x.Count() }).ToListAsync(t) : [];
        return students.Select(x => { var a = attendance.Where(y => y.StudentProfileId == x.Id).ToList(); var r = results.Where(y => y.StudentProfileId == x.Id).ToList(); var member = issues.FirstOrDefault(y => y.Id == x.Id); return Row(("AdmissionNumber", x.AdmissionNumber), ("Student", $"{x.FirstName} {x.LastName}"), ("Branch", x.Branch?.Name), ("Course", x.Section?.Batch?.Course?.Name), ("Section", x.Section?.Name), ("AttendancePercent", Percent(a.Count(y => y.Status is AttendanceStatus.Present or AttendanceStatus.Late), a.Count)), ("AverageResult", Number(r.Select(y => y.Percentage).DefaultIfEmpty().Average())), ("PassedExams", r.Count(y => y.IsPassed).ToString()), ("FeeBalance", Number(fees.Where(y => y.Id == x.Id).Sum(y => y.Balance))), ("LibraryIssues", (member is null ? 0 : issueCounts.FirstOrDefault(y => y.Key == member.Member)?.Count ?? 0).ToString())); }).ToList();
    }
    private async Task<List<Dictionary<string, string>>> TeacherWorkload(ReportDefinition d, CancellationToken t) { var q = db.TeacherProfiles.Include(x => x.Branch).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); var teachers = await q.Take(1000).ToListAsync(t); var ids = teachers.Select(x => x.Id).ToList(); var assignments = await db.TeacherSubjectAssignments.Include(x => x.Subject).Include(x => x.Section).Where(x => ids.Contains(x.TeacherProfileId)).ToListAsync(t); var sectionIds = assignments.Where(x => x.SectionId.HasValue).Select(x => x.SectionId!.Value).Distinct().ToList(); var results = await db.Results.Where(x => x.SectionId.HasValue && sectionIds.Contains(x.SectionId.Value)).Select(x => new { x.SectionId, x.Percentage }).ToListAsync(t); var periods = await db.TimetableEntries.Where(x => ids.Contains(x.TeacherProfileId)).GroupBy(x => x.TeacherProfileId).Select(x => new { x.Key, Count = x.Count() }).ToListAsync(t); var sessions = await db.AttendanceSessions.Where(x => x.MarkedByUserId.HasValue).ToListAsync(t); return teachers.Select(x => { var a = assignments.Where(y => y.TeacherProfileId == x.Id).ToList(); var assignedSections = a.Where(y => y.SectionId.HasValue).Select(y => y.SectionId).ToHashSet(); var teacherResults = results.Where(y => assignedSections.Contains(y.SectionId)).Select(y => y.Percentage).ToList(); return Row(("EmployeeNumber", x.EmployeeNumber), ("Teacher", $"{x.FirstName} {x.LastName}"), ("Branch", x.Branch?.Name), ("Subjects", string.Join(", ", a.Select(y => y.Subject?.Name).Where(y => y != null).Distinct())), ("Sections", string.Join(", ", a.Select(y => y.Section?.Name).Where(y => y != null).Distinct())), ("WeeklyPeriods", (periods.FirstOrDefault(y => y.Key == x.Id)?.Count ?? 0).ToString()), ("AttendanceSessions", sessions.Count(y => y.MarkedByUserId == x.UserId).ToString()), ("StudentResultAverage", Number(teacherResults.DefaultIfEmpty().Average()))); }).ToList(); }
    private async Task<List<Dictionary<string, string>>> Comparisons(ReportDefinition d, string group, CancellationToken t) { var students = await db.StudentProfiles.Include(x => x.Branch).Include(x => x.Section)!.ThenInclude(x => x!.Batch)!.ThenInclude(x => x!.Course).ToListAsync(t); if (d.BranchId is Guid b) students = students.Where(x => x.BranchId == b).ToList(); var perf = await StudentRows(students, false, t); var invoices = group == "Branch" ? await db.FeeInvoices.Include(x => x.Branch).Select(x => new { Branch = x.Branch!.Name, x.TotalAmount, x.PaidAmount }).ToListAsync(t) : []; return perf.GroupBy(x => group == "Branch" ? x["Branch"] : group == "Course" ? x["Course"] : x["Section"]).Select(g => { var branchInvoices = invoices.Where(x => x.Branch == g.Key).ToList(); var billed = branchInvoices.Sum(x => x.TotalAmount); var collection = billed == 0 ? 0 : branchInvoices.Sum(x => x.PaidAmount) * 100m / billed; return Row((group, g.Key), ("Students", g.Count().ToString()), ("AttendancePercent", Number(g.Average(x => Decimal(x["AttendancePercent"])))), ("AverageResult", Number(g.Average(x => Decimal(x["AverageResult"])))), ("PassPercent", Percent(g.Count(x => int.TryParse(x["PassedExams"], out var p) && p > 0), g.Count())), ("FeeCollectionPercent", Number(collection))); }).ToList(); }
    private async Task<List<Dictionary<string, string>>> Students(ReportDefinition d, CancellationToken t) { var q = db.StudentProfiles.Include(x => x.Branch).Include(x => x.Section).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); return (await q.Take(5000).ToListAsync(t)).Select(x => Row(("AdmissionNumber", x.AdmissionNumber), ("Student", $"{x.FirstName} {x.LastName}"), ("Branch", x.Branch?.Name), ("Section", x.Section?.Name), ("Email", x.Email), ("Status", x.Status.ToString()))).ToList(); }
    private async Task<List<Dictionary<string, string>>> Teachers(ReportDefinition d, CancellationToken t) { var q = db.TeacherProfiles.Include(x => x.Branch).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); return (await q.Take(5000).ToListAsync(t)).Select(x => Row(("EmployeeNumber", x.EmployeeNumber), ("Teacher", $"{x.FirstName} {x.LastName}"), ("Branch", x.Branch?.Name), ("Email", x.Email), ("Status", x.Status.ToString()))).ToList(); }
    private async Task<List<Dictionary<string, string>>> Attendance(ReportDefinition d, CancellationToken t) { var q = db.AttendanceRecords.Include(x => x.StudentProfile).Include(x => x.AttendanceSession)!.ThenInclude(x => x!.Branch).Include(x => x.AttendanceSession)!.ThenInclude(x => x!.Section).Include(x => x.AttendanceSession)!.ThenInclude(x => x!.Subject).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.AttendanceSession!.BranchId == b); return (await q.OrderByDescending(x => x.AttendanceSession!.AttendanceDate).Take(10000).ToListAsync(t)).Select(x => Row(("Date", x.AttendanceSession?.AttendanceDate.ToString("yyyy-MM-dd")), ("Branch", x.AttendanceSession?.Branch?.Name), ("Section", x.AttendanceSession?.Section?.Name), ("Student", $"{x.StudentProfile?.FirstName} {x.StudentProfile?.LastName}"), ("Status", x.Status.ToString()), ("Subject", x.AttendanceSession?.Subject?.Name))).ToList(); }
    private async Task<List<Dictionary<string, string>>> Results(ReportDefinition d, CancellationToken t) { var q = db.Results.Include(x => x.StudentProfile).Include(x => x.Branch).Include(x => x.Course).Include(x => x.Section).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); return (await q.Take(10000).ToListAsync(t)).Select(x => Row(("Student", $"{x.StudentProfile?.FirstName} {x.StudentProfile?.LastName}"), ("Branch", x.Branch?.Name), ("Course", x.Course?.Name), ("Section", x.Section?.Name), ("Percentage", Number(x.Percentage)), ("Grade", x.Grade), ("Passed", x.IsPassed ? "Yes" : "No"), ("PublishedOn", x.PublishedOn?.ToString("u")))).ToList(); }
    private async Task<List<Dictionary<string, string>>> Finance(ReportDefinition d, CancellationToken t) { var q = db.FeeInvoices.Include(x => x.Branch).Include(x => x.StudentProfile).AsQueryable(); if (d.BranchId is Guid b) q = q.Where(x => x.BranchId == b); return (await q.Take(10000).ToListAsync(t)).Select(x => Row(("Invoice", x.InvoiceNumber), ("Branch", x.Branch?.Name), ("Student", $"{x.StudentProfile?.FirstName} {x.StudentProfile?.LastName}"), ("InvoiceDate", x.InvoiceDate.ToString("yyyy-MM-dd")), ("DueDate", x.DueDate.ToString("yyyy-MM-dd")), ("Total", Number(x.TotalAmount)), ("Paid", Number(x.PaidAmount)), ("Balance", Number(x.TotalAmount - x.PaidAmount)), ("Status", x.Status.ToString()))).ToList(); }
    private static Dictionary<string, string> Row(params (string Key, object? Value)[] values) => values.ToDictionary(x => x.Key, x => Convert.ToString(x.Value, CultureInfo.InvariantCulture) ?? "");
    private static string Number(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture); private static decimal Decimal(string value) => decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ? x : 0; private static string Percent(int n, int d) => d == 0 ? "0.00" : Number(n * 100m / d);
    private static AnalyticsSnapshot Snapshot(Branch b, DateOnly date, string key, decimal value) => new() { TenantId = b.TenantId, BranchId = b.Id, SnapshotDate = date, MetricKey = key, MetricValue = value, DimensionsJson = JsonSerializer.Serialize(new { branch = b.Name }) };
    private static DateTime NextRun(ReportSchedule s, DateTime from) => s.Frequency switch { ReportScheduleFrequency.Daily => from.AddDays(1), ReportScheduleFrequency.Weekly => from.AddDays(7), _ => from.AddMonths(1) };
    private static string Safe(string value) => string.Concat(value.Select(c => char.IsLetterOrDigit(c) ? c : '-')).Trim('-').ToLowerInvariant();
    private static byte[] Csv(ReportData data) { static string Q(string s) => $"\"{s.Replace("\"", "\"\"")}\""; return Encoding.UTF8.GetBytes(string.Join("\r\n", new[] { string.Join(",", data.Columns.Select(Q)) }.Concat(data.Rows.Select(r => string.Join(",", data.Columns.Select(c => Q(r[c]))))))); }
    private static byte[] Excel(ReportData data) { using var output = new MemoryStream(); using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true)) { Write(zip, "[Content_Types].xml", "<?xml version=\"1.0\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>"); Write(zip, "_rels/.rels", "<?xml version=\"1.0\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>"); Write(zip, "xl/workbook.xml", "<?xml version=\"1.0\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Report\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>"); Write(zip, "xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>"); var rows = new[] { data.Columns.ToArray() }.Concat(data.Rows.Select(r => data.Columns.Select(c => r[c]).ToArray())); var xml = string.Join("", rows.Select((r, i) => $"<row r=\"{i + 1}\">{string.Join("", r.Select(v => $"<c t=\"inlineStr\"><is><t>{SecurityElement.Escape(v)}</t></is></c>"))}</row>")); Write(zip, "xl/worksheets/sheet1.xml", $"<?xml version=\"1.0\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>{xml}</sheetData></worksheet>"); } return output.ToArray(); }
    private static byte[] Pdf(ReportData data) { var lines = new[] { data.Title, $"Generated {DateTime.UtcNow:u}", string.Join(" | ", data.Columns), "" }.Concat(data.Rows.Take(45).Select(r => string.Join(" | ", data.Columns.Select(c => r[c])))); var text = string.Join("\n", lines).Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)"); var stream = $"BT /F1 8 Tf 28 800 Td 10 TL ({text.Replace("\n", ") Tj T* (")}) Tj ET"; var objs = new[] { "<< /Type /Catalog /Pages 2 0 R >>", "<< /Type /Pages /Kids [3 0 R] /Count 1 >>", "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 842 595] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>", $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>" }; var sb = new StringBuilder("%PDF-1.4\n"); var offsets = new List<int> { 0 }; for (var i = 0; i < objs.Length; i++) { offsets.Add(Encoding.ASCII.GetByteCount(sb.ToString())); sb.Append($"{i + 1} 0 obj\n{objs[i]}\nendobj\n"); } var xref = Encoding.ASCII.GetByteCount(sb.ToString()); sb.Append($"xref\n0 {objs.Length + 1}\n0000000000 65535 f \n"); foreach (var o in offsets.Skip(1)) sb.Append($"{o:0000000000} 00000 n \n"); sb.Append($"trailer << /Size {objs.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF"); return Encoding.ASCII.GetBytes(sb.ToString()); }
    private static void Write(ZipArchive zip, string path, string text) { var entry = zip.CreateEntry(path); using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)); writer.Write(text); }
}
