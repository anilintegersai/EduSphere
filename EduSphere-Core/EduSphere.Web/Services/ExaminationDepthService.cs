using System.IO.Compression;
using System.Xml.Linq;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class ExaminationDepthService(TenantDbContext db)
{
    public byte[] BuildMarksTemplate()
    {
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, true))
        {
            WriteEntry(archive, "[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
            WriteEntry(archive, "_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
            WriteEntry(archive, "xl/workbook.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Sheet1\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
            WriteEntry(archive, "xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");
            WriteEntry(archive, "xl/worksheets/sheet1.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData><row r=\"1\"><c t=\"inlineStr\"><is><t>StudentIdentifier</t></is></c><c t=\"inlineStr\"><is><t>MarksObtained</t></is></c><c t=\"inlineStr\"><is><t>IsAbsent</t></is></c></row><row r=\"2\"><c t=\"inlineStr\"><is><t>ADMISSION-OR-ROLL-NUMBER</t></is></c><c><v>75</v></c><c t=\"inlineStr\"><is><t>false</t></is></c></row></sheetData></worksheet>");
        }
        return output.ToArray();
    }

    private static void WriteEntry(ZipArchive archive, string name, string content)
    {
        using var writer = new StreamWriter(archive.CreateEntry(name).Open(), new System.Text.UTF8Encoding(false));
        writer.Write(content);
    }

    public async Task<ExamSeatingPlan> GenerateSeatingPlanAsync(Guid scheduleId, Guid? userId, CancellationToken token)
    {
        var schedule = await db.ExamSchedules.Include(x => x.Exam).Include(x => x.Section).SingleAsync(x => x.Id == scheduleId, token);
        var students = await db.StudentProfiles.Where(x => x.SectionId == schedule.SectionId && x.Status == StudentStatus.Active).OrderBy(x => x.RollNumber).ThenBy(x => x.AdmissionNumber).ToListAsync(token);
        var rooms = await db.Rooms.Where(x => x.BranchId == schedule.Exam!.BranchId && x.IsActive && x.Capacity > 0).OrderBy(x => x.Code).ToListAsync(token);
        if (rooms.Sum(x => x.Capacity) < students.Count) throw new InvalidOperationException("Available exam-room capacity is lower than the candidate count.");
        var plan = new ExamSeatingPlan { ExamScheduleId = schedule.Id, Name = $"{schedule.Exam!.Name} - {schedule.Section?.Name} - {schedule.ExamDate:dd MMM yyyy}", GeneratedByUserId = userId, CandidateCount = students.Count, Instructions = schedule.SeatingPlanNotes };
        var roomIndex = 0; var seat = 1;
        foreach (var student in students)
        {
            while (seat > rooms[roomIndex].Capacity) { roomIndex++; seat = 1; }
            plan.Assignments.Add(new ExamSeatAssignment { StudentProfileId = student.Id, RoomId = rooms[roomIndex].Id, SeatNumber = $"{rooms[roomIndex].Code}-{seat:000}", CandidateStatus = ExamCandidateStatus.Eligible }); seat++;
        }
        db.ExamSeatingPlans.Add(plan); await db.SaveChangesAsync(token); return plan;
    }

    public async Task<int> GenerateHallTicketsAsync(Guid examId, CancellationToken token)
    {
        var exam = await db.Exams.Include(x => x.Schedules).SingleAsync(x => x.Id == examId, token);
        var sectionIds = exam.Schedules.Select(x => x.SectionId).Distinct().ToList();
        var students = await db.StudentProfiles.Where(x => x.SectionId.HasValue && sectionIds.Contains(x.SectionId.Value) && x.Status == StudentStatus.Active).OrderBy(x => x.AdmissionNumber).ToListAsync(token);
        var existing = await db.ExamHallTickets.Where(x => x.ExamId == examId).Select(x => x.StudentProfileId).ToListAsync(token); var count = 0;
        foreach (var student in students.Where(x => !existing.Contains(x.Id))) { db.ExamHallTickets.Add(new ExamHallTicket { ExamId = exam.Id, StudentProfileId = student.Id, TicketNumber = $"HT-{exam.StartDate.Year}-{student.AdmissionNumber}-{student.Id.ToString("N")[..6]}".ToUpperInvariant() }); count++; }
        await db.SaveChangesAsync(token); return count;
    }

    public async Task<MarksImportBatch> PreviewMarksAsync(Guid scheduleId, Stream stream, string fileName, Guid userId, CancellationToken token)
    {
        var schedule = await db.ExamSchedules.Include(x => x.Section).SingleAsync(x => x.Id == scheduleId, token);
        var rows = ReadWorksheet(stream);
        var batch = new MarksImportBatch { ExamScheduleId = scheduleId, FileName = Path.GetFileName(fileName), UploadedByUserId = userId, Status = MarksImportStatus.Previewed };
        var rowNumber = 1;
        foreach (var values in rows.Skip(1))
        {
            rowNumber++; if (values.All(string.IsNullOrWhiteSpace)) continue;
            var identifier = values.ElementAtOrDefault(0)?.Trim() ?? ""; var marksText = values.ElementAtOrDefault(1)?.Trim(); var absent = string.Equals(values.ElementAtOrDefault(2)?.Trim(), "yes", StringComparison.OrdinalIgnoreCase) || string.Equals(values.ElementAtOrDefault(2)?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
            var student = await db.StudentProfiles.FirstOrDefaultAsync(x => x.SectionId == schedule.SectionId && (x.AdmissionNumber == identifier || x.RollNumber == identifier), token);
            var validMark = absent || (decimal.TryParse(marksText, out var mark) && mark >= 0 && mark <= schedule.MaximumMarks);
            var valid = student is not null && validMark;
            batch.Rows.Add(new MarksImportRow { RowNumber = rowNumber, StudentIdentifier = identifier, StudentProfileId = student?.Id, MarksObtained = absent ? 0 : decimal.TryParse(marksText, out var parsed) ? parsed : null, IsAbsent = absent, IsValid = valid, ValidationMessage = student is null ? "Student was not found in the scheduled section." : !validMark ? $"Marks must be between 0 and {schedule.MaximumMarks}." : "Ready to import." });
        }
        batch.TotalRows = batch.Rows.Count; batch.ValidRows = batch.Rows.Count(x => x.IsValid); batch.ErrorRows = batch.TotalRows - batch.ValidRows;
        db.MarksImportBatches.Add(batch); await db.SaveChangesAsync(token); return batch;
    }

    public async Task<MarksImportBatch> ExecuteMarksImportAsync(Guid batchId, Guid userId, CancellationToken token)
    {
        var batch = await db.MarksImportBatches.Include(x => x.Rows).Include(x => x.ExamSchedule).SingleAsync(x => x.Id == batchId, token); if (batch.Status != MarksImportStatus.Previewed) throw new InvalidOperationException("Only a previewed import can be executed.");
        batch.Status = MarksImportStatus.Processing;
        foreach (var row in batch.Rows.Where(x => x.IsValid && x.StudentProfileId.HasValue))
        {
            var mark = await db.MarkEntries.FirstOrDefaultAsync(x => x.ExamScheduleId == batch.ExamScheduleId && x.StudentProfileId == row.StudentProfileId, token);
            if (mark is null) db.MarkEntries.Add(new MarkEntry { ExamScheduleId = batch.ExamScheduleId, StudentProfileId = row.StudentProfileId!.Value, MarksObtained = row.MarksObtained ?? 0, IsAbsent = row.IsAbsent, EnteredByUserId = userId, EnteredOn = DateTime.UtcNow, Remarks = $"Imported from {batch.FileName}" });
            else { mark.MarksObtained = row.MarksObtained ?? 0; mark.IsAbsent = row.IsAbsent; mark.EnteredByUserId = userId; mark.EnteredOn = DateTime.UtcNow; mark.Remarks = $"Updated from {batch.FileName}"; }
            row.IsImported = true; batch.ImportedRows++;
        }
        batch.Status = batch.ErrorRows == 0 ? MarksImportStatus.Completed : MarksImportStatus.CompletedWithErrors; batch.CompletedOn = DateTime.UtcNow; await db.SaveChangesAsync(token); return batch;
    }

    public async Task<StudentTranscript> GenerateTranscriptAsync(Guid studentId, CancellationToken token)
    {
        var student = await db.StudentProfiles.SingleAsync(x => x.Id == studentId, token);
        var results = await db.Results.Include(x => x.Exam).Where(x => x.StudentProfileId == studentId && x.Status == ResultStatus.Published).OrderBy(x => x.Exam!.StartDate).ToListAsync(token);
        if (results.Count == 0) throw new InvalidOperationException("The student has no published results.");
        var transcript = new StudentTranscript { StudentProfileId = student.Id, AcademicYearId = results.Last().AcademicYearId, CourseId = results.Last().CourseId, TranscriptNumber = $"TR-{DateTime.UtcNow:yyyy}-{student.AdmissionNumber}-{Guid.NewGuid().ToString("N")[..6]}".ToUpperInvariant(), IssuedOn = DateOnly.FromDateTime(DateTime.UtcNow), AggregatePercentage = Math.Round(results.Average(x => x.Percentage), 2), FinalGrade = results.Last().Grade, IsFinal = true, Lines = results.Select(x => new StudentTranscriptLine { AssessmentName = x.Exam?.Name ?? "Assessment", MaximumMarks = x.TotalMarks, MarksObtained = x.MarksObtained, Percentage = x.Percentage, Grade = x.Grade, IsPassed = x.IsPassed }).ToList() };
        db.StudentTranscripts.Add(transcript); await db.SaveChangesAsync(token); return transcript;
    }

    public async Task ApplyModerationDecisionAsync(Guid paperId, Guid reviewerId, ApprovalStatus decision, string? notes, CancellationToken token)
    {
        if (decision is not (ApprovalStatus.Approved or ApprovalStatus.Rejected)) throw new InvalidOperationException("Decision must be approved or rejected.");
        if (await db.QuestionPaperModerations.AnyAsync(x => x.QuestionPaperId == paperId && x.ReviewedByUserId == reviewerId, token)) throw new InvalidOperationException("This reviewer has already submitted a decision.");
        var paper = await db.QuestionPapers.SingleAsync(x => x.Id == paperId, token); var policy = await db.QuestionPaperReviewPolicies.Where(x => x.BranchId == paper.BranchId || x.BranchId == null).OrderByDescending(x => x.BranchId.HasValue).ThenByDescending(x => x.IsDefault).FirstOrDefaultAsync(token) ?? new QuestionPaperReviewPolicy { RequiredReviewerCount = 1, RequiredApprovalCount = 1, RejectOnAnyRejection = true };
        db.QuestionPaperModerations.Add(new QuestionPaperModeration { QuestionPaperId = paperId, ReviewedByUserId = reviewerId, Decision = decision, Notes = notes, ReviewedOn = DateTime.UtcNow }); await db.SaveChangesAsync(token);
        var reviews = await db.QuestionPaperModerations.Where(x => x.QuestionPaperId == paperId).ToListAsync(token); var approvals = reviews.Count(x => x.Decision == ApprovalStatus.Approved); var rejected = reviews.Any(x => x.Decision == ApprovalStatus.Rejected);
        paper.Status = rejected && policy.RejectOnAnyRejection ? ApprovalStatus.Rejected : reviews.Count >= policy.RequiredReviewerCount && approvals >= policy.RequiredApprovalCount ? ApprovalStatus.Approved : ApprovalStatus.UnderReview;
        if (paper.Status == ApprovalStatus.Approved) { paper.ApprovedByUserId = reviewerId; paper.ApprovedOn = DateTime.UtcNow; } await db.SaveChangesAsync(token);
    }

    public static IReadOnlyList<string[]> ReadWorksheet(Stream stream)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true); var shared = new List<string>();
        var sharedEntry = archive.GetEntry("xl/sharedStrings.xml"); if (sharedEntry is not null) { using var reader = sharedEntry.Open(); var doc = XDocument.Load(reader); shared.AddRange(doc.Descendants().Where(x => x.Name.LocalName == "si").Select(x => string.Concat(x.Descendants().Where(t => t.Name.LocalName == "t").Select(t => t.Value)))); }
        var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml") ?? throw new InvalidOperationException("Excel workbook must contain Sheet1."); using var sheetReader = sheetEntry.Open(); var sheet = XDocument.Load(sheetReader);
        return sheet.Descendants().Where(x => x.Name.LocalName == "row").Select(row => row.Elements().Where(x => x.Name.LocalName == "c").Select(cell => { var value = cell.Descendants().FirstOrDefault(x => x.Name.LocalName == "v")?.Value ?? cell.Descendants().FirstOrDefault(x => x.Name.LocalName == "t")?.Value ?? ""; return cell.Attribute("t")?.Value == "s" && int.TryParse(value, out var index) && index < shared.Count ? shared[index] : value; }).ToArray()).ToList();
    }
}
