using System.ComponentModel.DataAnnotations;
using System.Text;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace EduSphere.Web.Pages.Academic;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public sealed class PlanningModel(TenantDbContext db, ITenantContext tenant, IAcademicPlanningService planning) : PageModel
{
    public bool HasTenant => tenant.HasTenant;
    public IReadOnlyList<AcademicYear> Years { get; private set; } = [];
    public IReadOnlyList<AcademicTerm> Terms { get; private set; } = [];
    public IReadOnlyList<AcademicCalendarEvent> Events { get; private set; } = [];
    public IReadOnlyList<Subject> Subjects { get; private set; } = [];
    public IReadOnlyList<SyllabusUnit> Units { get; private set; } = [];
    public IReadOnlyList<SyllabusTopic> Topics { get; private set; } = [];
    public IReadOnlyList<SubjectReferenceBook> Books { get; private set; } = [];
    public IReadOnlyList<LearningOutcome> Outcomes { get; private set; } = [];
    public IReadOnlyList<Branch> Branches { get; private set; } = [];
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = [];
    public IReadOnlyList<Section> Sections { get; private set; } = [];
    public IReadOnlyList<TeacherLessonPlan> LessonPlans { get; private set; } = [];
    public IReadOnlyList<TeacherAvailabilityRule> Availability { get; private set; } = [];
    public IReadOnlyList<TimetableTemplate> Templates { get; private set; } = [];
    public IReadOnlyList<TimetableGenerationRun> Runs { get; private set; } = [];
    public IReadOnlyList<ImportPreviewRow> ImportPreview { get; private set; } = [];
    [TempData] public string? Feedback { get; set; }

    [BindProperty] public TermInput Term { get; set; } = new();
    [BindProperty] public CalendarInput Calendar { get; set; } = new();
    [BindProperty] public TopicInput Topic { get; set; } = new();
    [BindProperty] public BookInput Book { get; set; } = new();
    [BindProperty] public OutcomeInput Outcome { get; set; } = new();
    [BindProperty] public LessonInput Lesson { get; set; } = new();
    [BindProperty] public AvailabilityInput AvailabilityRule { get; set; } = new();
    [BindProperty] public TemplateInput Template { get; set; } = new();
    [BindProperty] public TemplateItemInput TemplateItem { get; set; } = new();
    [BindProperty] public GenerationInput Generation { get; set; } = new();
    [BindProperty] public RolloverInput Rollover { get; set; } = new();
    [BindProperty] public IFormFile? ImportFile { get; set; }

    public sealed class TermInput { [Required] public Guid? AcademicYearId { get; set; } [Required, StringLength(100)] public string Name { get; set; } = ""; public AcademicTermType Type { get; set; } [Range(1, 20)] public int Sequence { get; set; } = 1; public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(5)); }
    public sealed class CalendarInput { [Required] public Guid? AcademicYearId { get; set; } public Guid? BranchId { get; set; } public Guid? AcademicTermId { get; set; } [Required, StringLength(160)] public string Title { get; set; } = ""; public AcademicCalendarEventType EventType { get; set; } public DateOnly StartsOn { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly EndsOn { get; set; } = DateOnly.FromDateTime(DateTime.Today); public bool IsInstructionalDay { get; set; } }
    public sealed class TopicInput { [Required] public Guid? SyllabusUnitId { get; set; } [Range(1, 1000)] public int Order { get; set; } = 1; [Required, StringLength(200)] public string Title { get; set; } = ""; [Range(0, 100000)] public int EstimatedMinutes { get; set; } = 45; }
    public sealed class BookInput { [Required] public Guid? SubjectId { get; set; } [Required, StringLength(250)] public string Title { get; set; } = ""; [StringLength(200)] public string? Authors { get; set; } [StringLength(30)] public string? Isbn { get; set; } public bool IsPrimary { get; set; } }
    public sealed class OutcomeInput { [Required] public Guid? SubjectId { get; set; } public Guid? SyllabusUnitId { get; set; } [Required, StringLength(40)] public string Code { get; set; } = ""; [Required, StringLength(1000)] public string Description { get; set; } = ""; [StringLength(80)] public string? BloomLevel { get; set; } }
    public sealed class LessonInput { [Required] public Guid? TeacherProfileId { get; set; } [Required] public Guid? SectionId { get; set; } [Required] public Guid? SubjectId { get; set; } [Required] public Guid? AcademicTermId { get; set; } public Guid? SyllabusTopicId { get; set; } public DateOnly PlannedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); [Required, StringLength(200)] public string Title { get; set; } = ""; [StringLength(2000)] public string? Objectives { get; set; } public LessonPlanStatus Status { get; set; } }
    public sealed class AvailabilityInput { [Required] public Guid? TeacherProfileId { get; set; } [Required] public Guid? BranchId { get; set; } public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday; public TimeOnly StartsAt { get; set; } = new(9, 0); public TimeOnly EndsAt { get; set; } = new(17, 0); public AvailabilityRuleType RuleType { get; set; } = AvailabilityRuleType.Unavailable; [StringLength(300)] public string? Reason { get; set; } }
    public sealed class TemplateInput { [Required] public Guid? BranchId { get; set; } [Required, StringLength(120)] public string Name { get; set; } = ""; [StringLength(500)] public string? Description { get; set; } }
    public sealed class TemplateItemInput { [Required] public Guid? TimetableTemplateId { get; set; } [Required] public Guid? SubjectId { get; set; } [Range(1, 20)] public int PeriodsPerWeek { get; set; } = 3; [Range(1, 10)] public int MaximumPeriodsPerDay { get; set; } = 1; public Guid? PreferredRoomId { get; set; } }
    public sealed class GenerationInput { [Required] public Guid? BranchId { get; set; } [Required] public Guid? AcademicYearId { get; set; } [Required] public Guid? SectionId { get; set; } [Required] public Guid? TemplateId { get; set; } [Required, StringLength(120)] public string Name { get; set; } = "Generated timetable"; public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly? EffectiveTo { get; set; } }
    public sealed class RolloverInput { [Required] public Guid? SourceAcademicYearId { get; set; } [Required] public Guid? TargetAcademicYearId { get; set; } public bool CopyCalendar { get; set; } = true; public bool CopyTemplates { get; set; } = true; }
    public sealed record ImportPreviewRow(int Row, string Type, string Code, string Name, string ParentCode, bool Valid, string Message);

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostSaveTermAsync() => await SaveAsync(nameof(Term), async () =>
    {
        if (Term.EndDate < Term.StartDate) throw new InvalidOperationException("Term end date cannot precede its start date.");
        db.AcademicTerms.Add(new AcademicTerm { AcademicYearId = Term.AcademicYearId!.Value, Name = Term.Name.Trim(), Type = Term.Type, Sequence = Term.Sequence, StartDate = Term.StartDate, EndDate = Term.EndDate }); await db.SaveChangesAsync();
    });
    public async Task<IActionResult> OnPostSaveCalendarAsync() => await SaveAsync(nameof(Calendar), async () => { if (Calendar.EndsOn < Calendar.StartsOn) throw new InvalidOperationException("Event end date cannot precede its start date."); db.AcademicCalendarEvents.Add(new AcademicCalendarEvent { AcademicYearId = Calendar.AcademicYearId!.Value, BranchId = Calendar.BranchId, AcademicTermId = Calendar.AcademicTermId, Title = Calendar.Title.Trim(), EventType = Calendar.EventType, StartsOn = Calendar.StartsOn, EndsOn = Calendar.EndsOn, IsInstructionalDay = Calendar.IsInstructionalDay }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveTopicAsync() => await SaveAsync(nameof(Topic), async () => { db.SyllabusTopics.Add(new SyllabusTopic { SyllabusUnitId = Topic.SyllabusUnitId!.Value, Order = Topic.Order, Title = Topic.Title.Trim(), EstimatedMinutes = Topic.EstimatedMinutes }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveBookAsync() => await SaveAsync(nameof(Book), async () => { db.SubjectReferenceBooks.Add(new SubjectReferenceBook { SubjectId = Book.SubjectId!.Value, Title = Book.Title.Trim(), Authors = Book.Authors, Isbn = Book.Isbn, IsPrimary = Book.IsPrimary }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveOutcomeAsync() => await SaveAsync(nameof(Outcome), async () => { db.LearningOutcomes.Add(new LearningOutcome { SubjectId = Outcome.SubjectId!.Value, SyllabusUnitId = Outcome.SyllabusUnitId, Code = Outcome.Code.Trim(), Description = Outcome.Description.Trim(), BloomLevel = Outcome.BloomLevel }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveLessonAsync() => await SaveAsync(nameof(Lesson), async () => { db.TeacherLessonPlans.Add(new TeacherLessonPlan { TeacherProfileId = Lesson.TeacherProfileId!.Value, SectionId = Lesson.SectionId!.Value, SubjectId = Lesson.SubjectId!.Value, AcademicTermId = Lesson.AcademicTermId!.Value, SyllabusTopicId = Lesson.SyllabusTopicId, PlannedDate = Lesson.PlannedDate, Title = Lesson.Title.Trim(), Objectives = Lesson.Objectives, Status = Lesson.Status }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveAvailabilityAsync() => await SaveAsync(nameof(AvailabilityRule), async () => { if (AvailabilityRule.EndsAt <= AvailabilityRule.StartsAt) throw new InvalidOperationException("Availability end time must be after start time."); db.TeacherAvailabilityRules.Add(new TeacherAvailabilityRule { TeacherProfileId = AvailabilityRule.TeacherProfileId!.Value, BranchId = AvailabilityRule.BranchId!.Value, DayOfWeek = AvailabilityRule.DayOfWeek, StartsAt = AvailabilityRule.StartsAt, EndsAt = AvailabilityRule.EndsAt, RuleType = AvailabilityRule.RuleType, Reason = AvailabilityRule.Reason }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveTemplateAsync() => await SaveAsync(nameof(Template), async () => { db.TimetableTemplates.Add(new TimetableTemplate { BranchId = Template.BranchId!.Value, Name = Template.Name.Trim(), Description = Template.Description }); await db.SaveChangesAsync(); });
    public async Task<IActionResult> OnPostSaveTemplateItemAsync() => await SaveAsync(nameof(TemplateItem), async () => { db.TimetableTemplateItems.Add(new TimetableTemplateItem { TimetableTemplateId = TemplateItem.TimetableTemplateId!.Value, SubjectId = TemplateItem.SubjectId!.Value, PeriodsPerWeek = TemplateItem.PeriodsPerWeek, MaximumPeriodsPerDay = TemplateItem.MaximumPeriodsPerDay, PreferredRoomId = TemplateItem.PreferredRoomId }); await db.SaveChangesAsync(); });

    public async Task<IActionResult> OnPostGenerateAsync()
    {
        KeepOnly(nameof(Generation)); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var result = await planning.GenerateTimetableAsync(new(Generation.BranchId!.Value, Generation.AcademicYearId!.Value, Generation.SectionId!.Value, Generation.TemplateId!.Value, Generation.Name, Generation.EffectiveFrom, Generation.EffectiveTo));
        Feedback = $"Generated {result.ScheduledPeriods} periods; {result.UnscheduledPeriods} remain unscheduled.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRolloverAsync()
    {
        KeepOnly(nameof(Rollover));
        if (Rollover.SourceAcademicYearId == Rollover.TargetAcademicYearId) ModelState.AddModelError("Rollover.TargetAcademicYearId", "Choose a different target academic year.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var result = await planning.RolloverAsync(new(Rollover.SourceAcademicYearId!.Value, Rollover.TargetAcademicYearId!.Value, Rollover.CopyCalendar, Rollover.CopyTemplates));
        Feedback = $"Rollover copied {result.TermsCopied} terms, {result.CalendarEventsCopied} calendar events and {result.TemplatesCopied} templates."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostImportAsync(bool commit)
    {
        ModelState.Clear();
        if (ImportFile is null || ImportFile.Length == 0) { ModelState.AddModelError(nameof(ImportFile), "Choose a CSV file."); await LoadAsync(); return Page(); }
        var rows = await ParseImportAsync(ImportFile);
        ImportPreview = rows;
        if (!commit || rows.Any(x => !x.Valid)) { await LoadAsync(keepPreview: true); return Page(); }
        foreach (var row in rows.Where(x => x.Type.Equals("Department", StringComparison.OrdinalIgnoreCase)))
            db.Departments.Add(new Department { Code = row.Code, Name = row.Name });
        await db.SaveChangesAsync();
        var departments = await db.Departments.ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows.Where(x => x.Type.Equals("Course", StringComparison.OrdinalIgnoreCase)))
            db.Courses.Add(new Course { Code = row.Code, Name = row.Name, DepartmentId = departments[row.ParentCode].Id });
        await db.SaveChangesAsync();
        var courses = await db.Courses.ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows.Where(x => x.Type.Equals("Subject", StringComparison.OrdinalIgnoreCase)))
            db.Subjects.Add(new Subject { Code = row.Code, Name = row.Name, CourseId = string.IsNullOrWhiteSpace(row.ParentCode) ? null : courses[row.ParentCode].Id });
        await db.SaveChangesAsync(); Feedback = $"Imported {rows.Count} academic master records."; return RedirectToPage();
    }

    public async Task<FileContentResult> OnGetExportAsync()
    {
        var lines = new List<string> { "Type,Code,Name,ParentCode" };
        lines.AddRange(await db.Departments.OrderBy(x => x.Code).Select(x => $"Department,{Csv(x.Code)},{Csv(x.Name)},").ToListAsync());
        var departments = await db.Departments.ToDictionaryAsync(x => x.Id, x => x.Code);
        var courses = await db.Courses.OrderBy(x => x.Code).ToListAsync();
        lines.AddRange(courses.Select(x => $"Course,{Csv(x.Code)},{Csv(x.Name)},{Csv(x.DepartmentId.HasValue ? departments.GetValueOrDefault(x.DepartmentId.Value) : "")}"));
        var courseCodes = courses.ToDictionary(x => x.Id, x => x.Code);
        lines.AddRange((await db.Subjects.OrderBy(x => x.Code).ToListAsync()).Select(x => $"Subject,{Csv(x.Code)},{Csv(x.Name)},{Csv(x.CourseId.HasValue ? courseCodes.GetValueOrDefault(x.CourseId.Value) : "")}"));
        return File(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)), "text/csv", $"academic-masters-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
    public FileContentResult OnGetImportTemplate() => File(Encoding.UTF8.GetBytes("Type,Code,Name,ParentCode\r\nDepartment,SCI,Science,\r\nCourse,BSC-CS,BSc Computer Science,SCI\r\nSubject,CS101,Programming Fundamentals,BSC-CS"), "text/csv", "academic-master-import-template.csv");

    public async Task<IActionResult> OnPostDeleteAsync(string entity, Guid id)
    {
        EduSphere.Domain.Common.ISoftDeletable? item = entity switch { "term" => await db.AcademicTerms.FindAsync(id), "event" => await db.AcademicCalendarEvents.FindAsync(id), "topic" => await db.SyllabusTopics.FindAsync(id), "book" => await db.SubjectReferenceBooks.FindAsync(id), "outcome" => await db.LearningOutcomes.FindAsync(id), "lesson" => await db.TeacherLessonPlans.FindAsync(id), "availability" => await db.TeacherAvailabilityRules.FindAsync(id), "template" => await db.TimetableTemplates.FindAsync(id), "template-item" => await db.TimetableTemplateItems.FindAsync(id), _ => null };
        if (item is not null) { item.IsDeleted = true; item.DeletedOn = DateTime.UtcNow; await db.SaveChangesAsync(); }
        return RedirectToPage();
    }

    public string YearName(Guid id) => Years.FirstOrDefault(x => x.Id == id)?.Name ?? "-";
    public string SubjectName(Guid id) => Subjects.FirstOrDefault(x => x.Id == id)?.Name ?? "-";
    public string UnitName(Guid id) => Units.FirstOrDefault(x => x.Id == id)?.Title ?? "-";
    public string TeacherName(Guid id) { var x = Teachers.FirstOrDefault(t => t.Id == id); return x is null ? "-" : $"{x.FirstName} {x.LastName}"; }

    private async Task<IActionResult> SaveAsync(string prefix, Func<Task> action)
    {
        KeepOnly(prefix); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        try { await action(); Feedback = "Academic planning record saved."; return RedirectToPage(); }
        catch (Exception ex) when (ex is InvalidOperationException or DbUpdateException) { ModelState.AddModelError(string.Empty, ex is DbUpdateException ? "This record conflicts with an existing academic record." : ex.Message); await LoadAsync(); return Page(); }
    }
    private void KeepOnly(string prefix) { foreach (var key in ModelState.Keys.Where(x => !x.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList()) ModelState.Remove(key); }
    private async Task LoadAsync(bool keepPreview = false)
    {
        if (!HasTenant) return;
        Years = await db.AcademicYears.OrderByDescending(x => x.StartDate).ToListAsync(); Terms = await db.AcademicTerms.OrderBy(x => x.AcademicYearId).ThenBy(x => x.Sequence).ToListAsync(); Events = await db.AcademicCalendarEvents.OrderByDescending(x => x.StartsOn).Take(100).ToListAsync();
        Subjects = await db.Subjects.OrderBy(x => x.Name).ToListAsync(); Units = await db.SyllabusUnits.OrderBy(x => x.SubjectId).ThenBy(x => x.Order).ToListAsync(); Topics = await db.SyllabusTopics.OrderBy(x => x.SyllabusUnitId).ThenBy(x => x.Order).ToListAsync(); Books = await db.SubjectReferenceBooks.OrderBy(x => x.Title).ToListAsync(); Outcomes = await db.LearningOutcomes.OrderBy(x => x.Code).ToListAsync();
        Branches = await db.Branches.OrderBy(x => x.Name).ToListAsync(); Teachers = await db.TeacherProfiles.OrderBy(x => x.FirstName).ToListAsync(); Sections = await db.Sections.OrderBy(x => x.Name).ToListAsync(); LessonPlans = await db.TeacherLessonPlans.OrderByDescending(x => x.PlannedDate).Take(100).ToListAsync(); Availability = await db.TeacherAvailabilityRules.OrderBy(x => x.TeacherProfileId).ThenBy(x => x.DayOfWeek).ToListAsync(); Templates = await db.TimetableTemplates.Include(x => x.Items).OrderBy(x => x.Name).ToListAsync(); Runs = await db.TimetableGenerationRuns.OrderByDescending(x => x.StartedOn).Take(20).ToListAsync();
    }
    private async Task<List<ImportPreviewRow>> ParseImportAsync(IFormFile file)
    {
        var rows = new List<ImportPreviewRow>(); using var parser = new TextFieldParser(file.OpenReadStream()) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true }; parser.SetDelimiters(",");
        _ = parser.ReadFields(); var rowNumber = 1;
        var existingDepartments = (await db.Departments.Select(x => x.Code).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingCourses = (await db.Courses.Select(x => x.Code).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingSubjects = (await db.Subjects.Select(x => x.Code).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var seenDepartments = new HashSet<string>(StringComparer.OrdinalIgnoreCase); var seenCourses = new HashSet<string>(StringComparer.OrdinalIgnoreCase); var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (!parser.EndOfData) { rowNumber++; var f = parser.ReadFields() ?? []; var type = f.ElementAtOrDefault(0)?.Trim() ?? ""; var code = f.ElementAtOrDefault(1)?.Trim() ?? ""; var name = f.ElementAtOrDefault(2)?.Trim() ?? ""; var parent = f.ElementAtOrDefault(3)?.Trim() ?? ""; var validType = type.Equals("Department", StringComparison.OrdinalIgnoreCase) || type.Equals("Course", StringComparison.OrdinalIgnoreCase) || type.Equals("Subject", StringComparison.OrdinalIgnoreCase); var alreadyExists = type.Equals("Department", StringComparison.OrdinalIgnoreCase) ? existingDepartments.Contains(code) : type.Equals("Course", StringComparison.OrdinalIgnoreCase) ? existingCourses.Contains(code) : existingSubjects.Contains(code); var valid = validType && code.Length > 0 && name.Length > 0 && !alreadyExists && seenCodes.Add($"{type}:{code}"); var parentValid = type.Equals("Department", StringComparison.OrdinalIgnoreCase) || (type.Equals("Course", StringComparison.OrdinalIgnoreCase) ? existingDepartments.Contains(parent) || seenDepartments.Contains(parent) : string.IsNullOrWhiteSpace(parent) || existingCourses.Contains(parent) || seenCourses.Contains(parent)); valid &= parentValid; var message = valid ? "Ready" : alreadyExists ? "Code already exists." : !validType ? "Type must be Department, Course or Subject." : !parentValid ? "Parent code was not found or appears after this row." : "Code/name is missing or duplicated."; rows.Add(new(rowNumber, type, code, name, parent, valid, message)); if (valid && type.Equals("Department", StringComparison.OrdinalIgnoreCase)) seenDepartments.Add(code); if (valid && type.Equals("Course", StringComparison.OrdinalIgnoreCase)) seenCourses.Add(code); }
        return rows;
    }
    private static string Csv(string? value) => $"\"{(value ?? "").Replace("\"", "\"\"")}\"";
}
