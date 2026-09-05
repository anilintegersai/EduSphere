using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Examinations;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class ExamsModel : PageModel
{
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly ICrudService<GradingScheme> _schemes;
    private readonly ICrudService<GradingSchemeBand> _bands;
    private readonly ICrudService<QuestionBankItem> _bankItems;
    private readonly ICrudService<QuestionPaper> _papers;
    private readonly ICrudService<QuestionPaperSection> _paperSections;
    private readonly ICrudService<QuestionPaperQuestion> _paperQuestions;
    private readonly ICrudService<MarkEntry> _marks;
    private readonly ICrudService<Result> _results;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<SyllabusUnit> _units;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<Room> _rooms;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExamsModel(
        ICrudService<Exam> exams,
        ICrudService<ExamSchedule> schedules,
        ICrudService<GradingScheme> schemes,
        ICrudService<GradingSchemeBand> bands,
        ICrudService<QuestionBankItem> bankItems,
        ICrudService<QuestionPaper> papers,
        ICrudService<QuestionPaperSection> paperSections,
        ICrudService<QuestionPaperQuestion> paperQuestions,
        ICrudService<MarkEntry> marks,
        ICrudService<Result> results,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Section> sections,
        ICrudService<Batch> batches,
        ICrudService<Course> courses,
        ICrudService<Subject> subjects,
        ICrudService<SyllabusUnit> units,
        ICrudService<TeacherProfile> teachers,
        ICrudService<StudentProfile> students,
        ICrudService<Room> rooms,
        ITenantContext tenant,
        IBranchAccessService branchAccess,
        UserManager<ApplicationUser> userManager)
    {
        _exams = exams;
        _schedules = schedules;
        _schemes = schemes;
        _bands = bands;
        _bankItems = bankItems;
        _papers = papers;
        _paperSections = paperSections;
        _paperQuestions = paperQuestions;
        _marks = marks;
        _results = results;
        _branches = branches;
        _years = years;
        _sections = sections;
        _batches = batches;
        _courses = courses;
        _subjects = subjects;
        _units = units;
        _teachers = teachers;
        _students = students;
        _rooms = rooms;
        _tenant = tenant;
        _branchAccess = branchAccess;
        _userManager = userManager;
    }

    public bool HasTenant => _tenant.HasTenant;
    public bool IsBranchAdminOnly => _branchAccess.IsBranchAdminOnly(User);
    public string? Notice { get; private set; }

    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<Subject> Subjects { get; private set; } = new List<Subject>();
    public IReadOnlyList<SyllabusUnit> Units { get; private set; } = new List<SyllabusUnit>();
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = new List<TeacherProfile>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();
    public IReadOnlyList<Room> Rooms { get; private set; } = new List<Room>();
    public IReadOnlyList<Exam> Exams { get; private set; } = new List<Exam>();
    public IReadOnlyList<ExamSchedule> Schedules { get; private set; } = new List<ExamSchedule>();
    public IReadOnlyList<GradingScheme> Schemes { get; private set; } = new List<GradingScheme>();
    public IReadOnlyList<GradingSchemeBand> Bands { get; private set; } = new List<GradingSchemeBand>();
    public IReadOnlyList<QuestionBankItem> BankItems { get; private set; } = new List<QuestionBankItem>();
    public IReadOnlyList<QuestionPaper> Papers { get; private set; } = new List<QuestionPaper>();
    public IReadOnlyList<QuestionPaperSection> PaperSections { get; private set; } = new List<QuestionPaperSection>();
    public IReadOnlyList<QuestionPaperQuestion> PaperQuestions { get; private set; } = new List<QuestionPaperQuestion>();
    public IReadOnlyList<MarkEntry> Marks { get; private set; } = new List<MarkEntry>();
    public IReadOnlyList<Result> Results { get; private set; } = new List<Result>();

    [BindProperty] public ExamInputModel ExamInput { get; set; } = new();
    [BindProperty] public ScheduleInputModel ScheduleInput { get; set; } = new();
    [BindProperty] public SchemeInputModel SchemeInput { get; set; } = new();
    [BindProperty] public BandInputModel BandInput { get; set; } = new();
    [BindProperty] public QuestionInputModel QuestionInput { get; set; } = new();
    [BindProperty] public PaperInputModel PaperInput { get; set; } = new();
    [BindProperty] public PaperSectionInputModel PaperSectionInput { get; set; } = new();
    [BindProperty] public PaperQuestionInputModel PaperQuestionInput { get; set; } = new();
    [BindProperty] public MarkInputModel MarkInput { get; set; } = new();
    [BindProperty] public ComputeInputModel ComputeInput { get; set; } = new();

    public bool IsEditingExam => ExamInput.Id != Guid.Empty;
    public bool IsEditingSchedule => ScheduleInput.Id != Guid.Empty;
    public bool IsEditingScheme => SchemeInput.Id != Guid.Empty;
    public bool IsEditingBand => BandInput.Id != Guid.Empty;
    public bool IsEditingQuestion => QuestionInput.Id != Guid.Empty;
    public bool IsEditingPaper => PaperInput.Id != Guid.Empty;
    public bool IsEditingPaperSection => PaperSectionInput.Id != Guid.Empty;
    public bool IsEditingPaperQuestion => PaperQuestionInput.Id != Guid.Empty;
    public bool IsEditingMark => MarkInput.Id != Guid.Empty;

    public class ExamInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Academic year"), Required] public Guid? AcademicYearId { get; set; }
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        public ExamType Type { get; set; } = ExamType.UnitTest;
        [Display(Name = "Start date")] public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "End date")] public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Range(0, 100), Display(Name = "Weightage %")] public decimal WeightagePercentage { get; set; } = 100;
        public ExamStatus Status { get; set; } = ExamStatus.Draft;
        [StringLength(1000)] public string? Instructions { get; set; }
    }

    public class ScheduleInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Exam"), Required] public Guid? ExamId { get; set; }
        [Display(Name = "Section"), Required] public Guid? SectionId { get; set; }
        [Display(Name = "Subject"), Required] public Guid? SubjectId { get; set; }
        [Display(Name = "Teacher")] public Guid? TeacherProfileId { get; set; }
        [Display(Name = "Room")] public Guid? RoomId { get; set; }
        [Display(Name = "Exam date")] public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Start")] public TimeOnly StartsAt { get; set; } = new(9, 0);
        [Display(Name = "End")] public TimeOnly EndsAt { get; set; } = new(12, 0);
        [Range(1, 1440), Display(Name = "Duration")] public int DurationMinutes { get; set; } = 180;
        [Range(1, 10000), Display(Name = "Max marks")] public decimal MaximumMarks { get; set; } = 100;
        [Range(0, 10000), Display(Name = "Passing marks")] public decimal PassingMarks { get; set; } = 35;
        public ExamScheduleStatus Status { get; set; } = ExamScheduleStatus.Draft;
        [StringLength(1000), Display(Name = "Seating notes")] public string? SeatingPlanNotes { get; set; }
        [StringLength(500)] public string? Instructions { get; set; }
    }

    public class SchemeInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        public GradingSchemeType Type { get; set; } = GradingSchemeType.Percentage;
        [Display(Name = "Effective from")] public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Default")] public bool IsDefault { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
    }

    public class BandInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Scheme"), Required] public Guid? GradingSchemeId { get; set; }
        [Required, StringLength(20)] public string Grade { get; set; } = string.Empty;
        [Range(0, 100), Display(Name = "Min %")] public decimal MinimumPercentage { get; set; }
        [Range(0, 100), Display(Name = "Max %")] public decimal MaximumPercentage { get; set; }
        [Range(0, 10), Display(Name = "Point")] public decimal GradePoint { get; set; }
        [Range(0, 1000), Display(Name = "Order")] public int SortOrder { get; set; }
        [StringLength(200)] public string? Remarks { get; set; }
    }

    public class QuestionInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Subject"), Required] public Guid? SubjectId { get; set; }
        [Display(Name = "Syllabus unit")] public Guid? SyllabusUnitId { get; set; }
        [Display(Name = "Author")] public Guid? AuthorTeacherProfileId { get; set; }
        [Display(Name = "Question type")] public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
        [Display(Name = "Bloom level")] public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
        public GenerationSource Source { get; set; } = GenerationSource.Manual;
        [Range(0.1, 1000)] public decimal Marks { get; set; } = 2;
        [Required, StringLength(4000), Display(Name = "Question")] public string QuestionText { get; set; } = string.Empty;
        [StringLength(4000), Display(Name = "Answer")] public string? ExpectedAnswer { get; set; }
        [StringLength(500)] public string? Tags { get; set; }
        [Display(Name = "Approval")] public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    }

    public class PaperInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Subject"), Required] public Guid? SubjectId { get; set; }
        [Display(Name = "Exam")] public Guid? ExamId { get; set; }
        [Display(Name = "Schedule")] public Guid? ExamScheduleId { get; set; }
        [Required, StringLength(150)] public string Title { get; set; } = string.Empty;
        [Range(1, 10000), Display(Name = "Total marks")] public decimal TotalMarks { get; set; } = 100;
        [Range(1, 1440), Display(Name = "Duration")] public int DurationMinutes { get; set; } = 180;
        [StringLength(1000)] public string? Instructions { get; set; }
        public GenerationSource Source { get; set; } = GenerationSource.Manual;
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    }

    public class PaperSectionInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Paper"), Required] public Guid? QuestionPaperId { get; set; }
        [Required, StringLength(20)] public string Code { get; set; } = "A";
        [Required, StringLength(120)] public string Title { get; set; } = "Section A";
        [Range(0, 1000), Display(Name = "Order")] public int SortOrder { get; set; }
        [Range(0, 10000)] public decimal Marks { get; set; } = 20;
        [StringLength(500)] public string? Instructions { get; set; }
    }

    public class PaperQuestionInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Paper section"), Required] public Guid? QuestionPaperSectionId { get; set; }
        [Display(Name = "Bank item")] public Guid? QuestionBankItemId { get; set; }
        [Range(0, 1000), Display(Name = "Order")] public int SortOrder { get; set; }
        [Display(Name = "Type")] public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
        [Display(Name = "Bloom")] public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
        [Range(0.1, 1000)] public decimal Marks { get; set; } = 2;
        [Required, StringLength(4000), Display(Name = "Question")] public string QuestionText { get; set; } = string.Empty;
        [StringLength(4000), Display(Name = "Solution")] public string? SolutionText { get; set; }
    }

    public class MarkInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Schedule"), Required] public Guid? ExamScheduleId { get; set; }
        [Display(Name = "Student"), Required] public Guid? StudentProfileId { get; set; }
        [Range(0, 10000), Display(Name = "Marks")] public decimal MarksObtained { get; set; }
        [Display(Name = "Absent")] public bool IsAbsent { get; set; }
        [StringLength(20)] public string? Grade { get; set; }
        [Range(0, 10), Display(Name = "Point")] public decimal? GradePoint { get; set; }
        [StringLength(500)] public string? Remarks { get; set; }
    }

    public class ComputeInputModel
    {
        [Display(Name = "Exam"), Required] public Guid? ExamId { get; set; }
        [Display(Name = "Student"), Required] public Guid? StudentProfileId { get; set; }
        [Display(Name = "Scheme")] public Guid? GradingSchemeId { get; set; }
        [Display(Name = "Publish now")] public bool Publish { get; set; }
        [StringLength(500)] public string? Remarks { get; set; }
    }

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string YearName(Guid id) => Years.FirstOrDefault(y => y.Id == id)?.Name ?? "-";
    public string SectionName(Guid id) => Sections.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string CourseName(Guid? id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "-";
    public string SubjectName(Guid id) => Subjects.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string UnitTitle(Guid? id) => Units.FirstOrDefault(u => u.Id == id)?.Title ?? "-";
    public string TeacherName(Guid? id)
    {
        var teacher = Teachers.FirstOrDefault(t => t.Id == id);
        return teacher is null ? "-" : $"{teacher.FirstName} {teacher.LastName}";
    }
    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }
    public string RoomName(Guid? id) => Rooms.FirstOrDefault(r => r.Id == id)?.Name ?? "-";
    public string ExamName(Guid id) => Exams.FirstOrDefault(e => e.Id == id)?.Name ?? "-";
    public string SchemeName(Guid id) => Schemes.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string PaperTitle(Guid id) => Papers.FirstOrDefault(p => p.Id == id)?.Title ?? "-";
    public string PaperSectionTitle(Guid id)
    {
        var section = PaperSections.FirstOrDefault(s => s.Id == id);
        return section is null ? "-" : $"{PaperTitle(section.QuestionPaperId)} / {section.Code}";
    }
    public string QuestionLabel(QuestionBankItem question)
        => $"{SubjectName(question.SubjectId)} / {question.Marks:0.##} marks / {Brief(question.QuestionText, 70)}";
    public string ScheduleName(Guid id)
    {
        var schedule = Schedules.FirstOrDefault(s => s.Id == id);
        return schedule is null ? "-" : $"{ExamName(schedule.ExamId)} - {SubjectName(schedule.SubjectId)} - {SectionName(schedule.SectionId)}";
    }
    public static string Brief(string? value, int length = 90)
        => string.IsNullOrWhiteSpace(value)
            ? "-"
            : value.Length <= length
                ? value
                : value[..length] + "...";

    public async Task OnGetAsync(
        Guid? examEditId,
        Guid? scheduleEditId,
        Guid? schemeEditId,
        Guid? bandEditId,
        Guid? questionEditId,
        Guid? paperEditId,
        Guid? paperSectionEditId,
        Guid? paperQuestionEditId,
        Guid? markEditId)
    {
        if (!HasTenant) return;
        await LoadAsync();

        if (examEditId is Guid examId && await _exams.GetAsync(examId) is { } exam && await CanUseBranchAsync(exam.BranchId))
            ExamInput = Map(exam);
        if (scheduleEditId is Guid scheduleId && await _schedules.GetAsync(scheduleId) is { } schedule && await CanUseScheduleAsync(schedule))
            ScheduleInput = Map(schedule);
        if (schemeEditId is Guid schemeId && await _schemes.GetAsync(schemeId) is { } scheme && await CanUseSchemeAsync(scheme))
            SchemeInput = Map(scheme);
        if (bandEditId is Guid bandId && await _bands.GetAsync(bandId) is { } band && await CanUseBandAsync(band))
            BandInput = Map(band);
        if (questionEditId is Guid questionId && await _bankItems.GetAsync(questionId) is { } question && await CanUseBranchAsync(question.BranchId))
            QuestionInput = Map(question);
        if (paperEditId is Guid paperId && await _papers.GetAsync(paperId) is { } paper && await CanUseBranchAsync(paper.BranchId))
            PaperInput = Map(paper);
        if (paperSectionEditId is Guid paperSectionId && await _paperSections.GetAsync(paperSectionId) is { } paperSection && await CanUsePaperAsync(paperSection.QuestionPaperId))
            PaperSectionInput = Map(paperSection);
        if (paperQuestionEditId is Guid paperQuestionId && await _paperQuestions.GetAsync(paperQuestionId) is { } paperQuestion && await CanUsePaperQuestionAsync(paperQuestion))
            PaperQuestionInput = Map(paperQuestion);
        if (markEditId is Guid markId && await _marks.GetAsync(markId) is { } mark && await CanUseMarkAsync(mark))
            MarkInput = Map(mark);
    }

    public async Task<IActionResult> OnPostSaveExamAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ExamInput));
        var branchId = ExamInput.BranchId ?? Guid.Empty;
        var yearId = ExamInput.AcademicYearId ?? Guid.Empty;
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("ExamInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("ExamInput.BranchId", "You can manage exams only for your assigned branch.");
        if (yearId == Guid.Empty || await _years.GetAsync(yearId) is null)
            ModelState.AddModelError("ExamInput.AcademicYearId", "Selected academic year was not found.");
        if (ExamInput.EndDate < ExamInput.StartDate)
            ModelState.AddModelError("ExamInput.EndDate", "End date must be on or after start date.");
        if (ExamInput.Id != Guid.Empty && await _exams.GetAsync(ExamInput.Id) is { } existing && !await CanUseBranchAsync(existing.BranchId))
            ModelState.AddModelError(string.Empty, "You can update exams only in your assigned branch.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (ExamInput.Id == Guid.Empty)
            await _exams.CreateAsync(Apply(new Exam(), branchId, yearId));
        else
            await _exams.UpdateAsync(ExamInput.Id, e => Apply(e, branchId, yearId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveScheduleAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ScheduleInput));
        var validationError = await ValidateScheduleAsync(ScheduleInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (ScheduleInput.Id == Guid.Empty)
            await _schedules.CreateAsync(Apply(new ExamSchedule()));
        else
            await _schedules.UpdateAsync(ScheduleInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveSchemeAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(SchemeInput));
        var validationError = await ValidateSchemeAsync(SchemeInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (SchemeInput.Id == Guid.Empty)
            await _schemes.CreateAsync(Apply(new GradingScheme()));
        else
            await _schemes.UpdateAsync(SchemeInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveBandAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(BandInput));
        var validationError = await ValidateBandAsync(BandInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (BandInput.Id == Guid.Empty)
            await _bands.CreateAsync(Apply(new GradingSchemeBand()));
        else
            await _bands.UpdateAsync(BandInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveQuestionAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(QuestionInput));
        var validationError = await ValidateQuestionAsync(QuestionInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (QuestionInput.Id == Guid.Empty)
            await _bankItems.CreateAsync(Apply(new QuestionBankItem()));
        else
            await _bankItems.UpdateAsync(QuestionInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSavePaperAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(PaperInput));
        var validationError = await ValidatePaperAsync(PaperInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (PaperInput.Id == Guid.Empty)
            await _papers.CreateAsync(Apply(new QuestionPaper()));
        else
            await _papers.UpdateAsync(PaperInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSavePaperSectionAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(PaperSectionInput));
        var validationError = await ValidatePaperSectionAsync(PaperSectionInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (PaperSectionInput.Id == Guid.Empty)
            await _paperSections.CreateAsync(Apply(new QuestionPaperSection()));
        else
            await _paperSections.UpdateAsync(PaperSectionInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSavePaperQuestionAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(PaperQuestionInput));
        var validationError = await ValidatePaperQuestionAsync(PaperQuestionInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (PaperQuestionInput.Id == Guid.Empty)
            await _paperQuestions.CreateAsync(Apply(new QuestionPaperQuestion()));
        else
            await _paperQuestions.UpdateAsync(PaperQuestionInput.Id, e => Apply(e));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveMarkAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(MarkInput));
        var validationError = await ValidateMarkAsync(MarkInput);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (MarkInput.Id == Guid.Empty)
            await _marks.CreateAsync(Apply(new MarkEntry(), CurrentUserId()));
        else
            await _marks.UpdateAsync(MarkInput.Id, e => Apply(e, CurrentUserId()));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostComputeResultAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ComputeInput));
        var result = await ComputeResultAsync(ComputeInput);
        if (result is null)
        {
            if (ModelState.IsValid)
                ModelState.AddModelError(string.Empty, "Result could not be computed from the selected exam and student.");
            await LoadAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Result {result.Grade} ({result.Percentage:0.##}%) computed for {StudentName(result.StudentProfileId)}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteExamAsync(Guid id) => await DeleteIfAllowedAsync(id, _exams, e => CanUseBranchAsync(e.BranchId));
    public async Task<IActionResult> OnPostDeleteScheduleAsync(Guid id) => await DeleteIfAllowedAsync(id, _schedules, CanUseScheduleAsync);
    public async Task<IActionResult> OnPostDeleteSchemeAsync(Guid id) => await DeleteIfAllowedAsync(id, _schemes, CanUseSchemeAsync);
    public async Task<IActionResult> OnPostDeleteBandAsync(Guid id) => await DeleteIfAllowedAsync(id, _bands, CanUseBandAsync);
    public async Task<IActionResult> OnPostDeleteQuestionAsync(Guid id) => await DeleteIfAllowedAsync(id, _bankItems, q => CanUseBranchAsync(q.BranchId));
    public async Task<IActionResult> OnPostDeletePaperAsync(Guid id) => await DeleteIfAllowedAsync(id, _papers, p => CanUseBranchAsync(p.BranchId));
    public async Task<IActionResult> OnPostDeletePaperSectionAsync(Guid id) => await DeleteIfAllowedAsync(id, _paperSections, s => CanUsePaperAsync(s.QuestionPaperId));
    public async Task<IActionResult> OnPostDeletePaperQuestionAsync(Guid id) => await DeleteIfAllowedAsync(id, _paperQuestions, CanUsePaperQuestionAsync);
    public async Task<IActionResult> OnPostDeleteMarkAsync(Guid id) => await DeleteIfAllowedAsync(id, _marks, CanUseMarkAsync);

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Subjects = (await _subjects.ListAsync()).OrderBy(s => s.Name).ToList();
        Units = (await _units.ListAsync()).OrderBy(u => u.Title).ToList();
        Teachers = (await _teachers.ListAsync(t =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)))
            .OrderBy(t => t.FirstName).ThenBy(t => t.LastName).ToList();
        Students = (await _students.ListAsync(s =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)))
            .OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();
        Rooms = (await _rooms.ListAsync(r =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && r.BranchId == assignedBranchId.Value)))
            .OrderBy(r => r.Code).ToList();
        Exams = (await _exams.ListAsync(e =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && e.BranchId == assignedBranchId.Value)))
            .OrderByDescending(e => e.StartDate).ThenBy(e => e.Name).ToList();

        var examIds = Exams.Select(e => e.Id).ToHashSet();
        Schedules = (await _schedules.ListAsync(s => !IsBranchAdminOnly || examIds.Contains(s.ExamId)))
            .OrderByDescending(s => s.ExamDate).ThenBy(s => s.StartsAt).ToList();
        Schemes = (await _schemes.ListAsync(s =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)))
            .OrderBy(s => s.Name).ToList();
        var schemeIds = Schemes.Select(s => s.Id).ToHashSet();
        Bands = (await _bands.ListAsync(b => schemeIds.Contains(b.GradingSchemeId)))
            .OrderBy(b => SchemeName(b.GradingSchemeId)).ThenBy(b => b.SortOrder).ToList();
        BankItems = (await _bankItems.ListAsync(q =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && q.BranchId == assignedBranchId.Value)))
            .OrderBy(q => SubjectName(q.SubjectId)).ThenBy(q => q.BloomLevel).ToList();
        Papers = (await _papers.ListAsync(p =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && p.BranchId == assignedBranchId.Value)))
            .OrderByDescending(p => p.CreatedOn).ToList();
        var paperIds = Papers.Select(p => p.Id).ToHashSet();
        PaperSections = (await _paperSections.ListAsync(s => paperIds.Contains(s.QuestionPaperId)))
            .OrderBy(s => PaperTitle(s.QuestionPaperId)).ThenBy(s => s.SortOrder).ToList();
        var paperSectionIds = PaperSections.Select(s => s.Id).ToHashSet();
        PaperQuestions = (await _paperQuestions.ListAsync(q => paperSectionIds.Contains(q.QuestionPaperSectionId)))
            .OrderBy(q => PaperSectionTitle(q.QuestionPaperSectionId)).ThenBy(q => q.SortOrder).ToList();
        var scheduleIds = Schedules.Select(s => s.Id).ToHashSet();
        Marks = (await _marks.ListAsync(m => scheduleIds.Contains(m.ExamScheduleId)))
            .OrderBy(m => ScheduleName(m.ExamScheduleId)).ThenBy(m => StudentName(m.StudentProfileId)).ToList();
        Results = (await _results.ListAsync(r =>
                !IsBranchAdminOnly || (assignedBranchId.HasValue && r.BranchId == assignedBranchId.Value)))
            .OrderByDescending(r => r.ComputedOn).ToList();

        if (IsBranchAdminOnly && assignedBranchId.HasValue)
        {
            ExamInput.BranchId ??= assignedBranchId.Value;
            SchemeInput.BranchId ??= assignedBranchId.Value;
            QuestionInput.BranchId ??= assignedBranchId.Value;
            PaperInput.BranchId ??= assignedBranchId.Value;
        }
    }

    private void KeepOnlyModelStateFor(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(k => !k.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);
    }

    private async Task<IActionResult> DeleteIfAllowedAsync<T>(
        Guid id,
        ICrudService<T> service,
        Func<T, Task<bool>> canUse)
        where T : class
    {
        if (await service.GetAsync(id) is { } entity && await canUse(entity))
            await service.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    private async Task<string?> ValidateScheduleAsync(ScheduleInputModel input)
    {
        if (input.ExamId is not Guid examId || input.SectionId is not Guid sectionId || input.SubjectId is not Guid subjectId)
            return "Exam, section, and subject are required.";
        var exam = await _exams.GetAsync(examId);
        if (exam is null) return "Selected exam was not found.";
        if (!await CanUseBranchAsync(exam.BranchId)) return "You can manage schedules only for your assigned branch.";
        if (input.ExamDate < exam.StartDate || input.ExamDate > exam.EndDate) return "Exam date must fall inside the selected exam range.";
        if (input.EndsAt <= input.StartsAt) return "End time must be after start time.";
        if (input.PassingMarks > input.MaximumMarks) return "Passing marks cannot exceed maximum marks.";
        if (await _sections.GetAsync(sectionId) is null) return "Selected section was not found.";
        if (await _subjects.GetAsync(subjectId) is null) return "Selected subject was not found.";
        if (input.TeacherProfileId is Guid teacherId && await _teachers.GetAsync(teacherId) is not { } teacher) return "Selected teacher was not found.";
        if (input.TeacherProfileId is Guid checkedTeacherId && await _teachers.GetAsync(checkedTeacherId) is { } checkedTeacher && checkedTeacher.BranchId != exam.BranchId) return "Selected teacher must belong to the exam branch.";
        if (input.RoomId is Guid roomId && await _rooms.GetAsync(roomId) is not { } room) return "Selected room was not found.";
        if (input.RoomId is Guid checkedRoomId && await _rooms.GetAsync(checkedRoomId) is { } checkedRoom && checkedRoom.BranchId != exam.BranchId) return "Selected room must belong to the exam branch.";
        if (input.Id != Guid.Empty && await _schedules.GetAsync(input.Id) is { } existing && !await CanUseScheduleAsync(existing)) return "You can update schedules only in your assigned branch.";
        return null;
    }

    private async Task<string?> ValidateSchemeAsync(SchemeInputModel input)
    {
        if (IsBranchAdminOnly && !input.BranchId.HasValue) return "Branch administrators must create branch-specific grading schemes.";
        if (input.BranchId is Guid branchId)
        {
            if (await _branches.GetAsync(branchId) is null) return "Selected branch was not found.";
            if (!await CanUseBranchAsync(branchId)) return "You can manage grading schemes only for your assigned branch.";
        }
        if (input.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null) return "Selected course was not found.";
        return null;
    }

    private async Task<string?> ValidateBandAsync(BandInputModel input)
    {
        if (input.GradingSchemeId is not Guid schemeId) return "Grading scheme is required.";
        if (input.MaximumPercentage < input.MinimumPercentage) return "Maximum percentage must be at least the minimum percentage.";
        var scheme = await _schemes.GetAsync(schemeId);
        if (scheme is null) return "Selected grading scheme was not found.";
        if (!await CanUseSchemeAsync(scheme)) return "You can manage grading bands only for your assigned branch.";
        return null;
    }

    private async Task<string?> ValidateQuestionAsync(QuestionInputModel input)
    {
        if (input.BranchId is not Guid branchId || input.SubjectId is not Guid subjectId) return "Branch and subject are required.";
        if (await _branches.GetAsync(branchId) is null) return "Selected branch was not found.";
        if (!await CanUseBranchAsync(branchId)) return "You can manage questions only for your assigned branch.";
        if (await _subjects.GetAsync(subjectId) is null) return "Selected subject was not found.";
        if (input.SyllabusUnitId is Guid unitId && await _units.GetAsync(unitId) is not { } unit) return "Selected syllabus unit was not found.";
        if (input.SyllabusUnitId is Guid checkedUnitId && await _units.GetAsync(checkedUnitId) is { } checkedUnit && checkedUnit.SubjectId != subjectId) return "Selected syllabus unit must belong to the selected subject.";
        if (input.AuthorTeacherProfileId is Guid teacherId && await _teachers.GetAsync(teacherId) is not { } teacher) return "Selected author was not found.";
        if (input.AuthorTeacherProfileId is Guid checkedTeacherId && await _teachers.GetAsync(checkedTeacherId) is { } checkedTeacher && checkedTeacher.BranchId != branchId) return "Selected author must belong to the selected branch.";
        return null;
    }

    private async Task<string?> ValidatePaperAsync(PaperInputModel input)
    {
        if (input.BranchId is not Guid branchId || input.SubjectId is not Guid subjectId) return "Branch and subject are required.";
        if (await _branches.GetAsync(branchId) is null) return "Selected branch was not found.";
        if (!await CanUseBranchAsync(branchId)) return "You can manage papers only for your assigned branch.";
        if (await _subjects.GetAsync(subjectId) is null) return "Selected subject was not found.";
        if (input.ExamId is Guid examId && await _exams.GetAsync(examId) is not { } exam) return "Selected exam was not found.";
        if (input.ExamId is Guid checkedExamId && await _exams.GetAsync(checkedExamId) is { } checkedExam && checkedExam.BranchId != branchId) return "Selected exam must belong to the paper branch.";
        if (input.ExamScheduleId is Guid scheduleId && await _schedules.GetAsync(scheduleId) is not { } schedule) return "Selected schedule was not found.";
        if (input.ExamScheduleId is Guid checkedScheduleId && await _schedules.GetAsync(checkedScheduleId) is { } checkedSchedule)
        {
            var scheduleExam = await _exams.GetAsync(checkedSchedule.ExamId);
            if (scheduleExam is null) return "Selected schedule has no valid exam.";
            if (scheduleExam.BranchId != branchId) return "Selected schedule must belong to the paper branch.";
            if (checkedSchedule.SubjectId != subjectId) return "Selected schedule must use the paper subject.";
            if (input.ExamId.HasValue && checkedSchedule.ExamId != input.ExamId.Value) return "Selected schedule must belong to the selected exam.";
        }
        return null;
    }

    private async Task<string?> ValidatePaperSectionAsync(PaperSectionInputModel input)
    {
        if (input.QuestionPaperId is not Guid paperId) return "Question paper is required.";
        return await CanUsePaperAsync(paperId) ? null : "Selected question paper was not found for your branch.";
    }

    private async Task<string?> ValidatePaperQuestionAsync(PaperQuestionInputModel input)
    {
        if (input.QuestionPaperSectionId is not Guid sectionId) return "Paper section is required.";
        var section = await _paperSections.GetAsync(sectionId);
        if (section is null) return "Selected paper section was not found.";
        var paper = await _papers.GetAsync(section.QuestionPaperId);
        if (paper is null || !await CanUseBranchAsync(paper.BranchId)) return "Selected paper section was not found for your branch.";
        if (input.QuestionBankItemId is Guid bankItemId)
        {
            var bankItem = await _bankItems.GetAsync(bankItemId);
            if (bankItem is null) return "Selected bank item was not found.";
            if (bankItem.BranchId != paper.BranchId) return "Bank item must belong to the paper branch.";
            if (bankItem.SubjectId != paper.SubjectId) return "Bank item must belong to the paper subject.";
        }
        return null;
    }

    private async Task<string?> ValidateMarkAsync(MarkInputModel input)
    {
        if (input.ExamScheduleId is not Guid scheduleId || input.StudentProfileId is not Guid studentId)
            return "Schedule and student are required.";
        var schedule = await _schedules.GetAsync(scheduleId);
        if (schedule is null) return "Selected schedule was not found.";
        var exam = await _exams.GetAsync(schedule.ExamId);
        if (exam is null) return "Selected schedule has no valid exam.";
        if (!await CanUseBranchAsync(exam.BranchId)) return "You can enter marks only for your assigned branch.";
        if (input.MarksObtained > schedule.MaximumMarks) return "Marks cannot exceed schedule maximum marks.";
        var student = await _students.GetAsync(studentId);
        if (student is null) return "Selected student was not found.";
        if (student.BranchId != exam.BranchId) return "Student must belong to the exam branch.";
        if (student.SectionId != schedule.SectionId) return "Student must belong to the schedule section.";
        return null;
    }

    private async Task<Result?> ComputeResultAsync(ComputeInputModel input)
    {
        if (input.ExamId is not Guid examId || input.StudentProfileId is not Guid studentId)
        {
            ModelState.AddModelError(string.Empty, "Exam and student are required.");
            return null;
        }

        var exam = await _exams.GetAsync(examId);
        var student = await _students.GetAsync(studentId);
        if (exam is null || student is null)
        {
            ModelState.AddModelError(string.Empty, "Selected exam or student was not found.");
            return null;
        }
        if (!await CanUseBranchAsync(exam.BranchId) || student.BranchId != exam.BranchId)
        {
            ModelState.AddModelError(string.Empty, "Selected exam and student must belong to your branch.");
            return null;
        }

        Section? section = null;
        Batch? batch = null;
        if (student.SectionId.HasValue)
        {
            section = await _sections.GetAsync(student.SectionId.Value);
            if (section is not null)
                batch = await _batches.GetAsync(section.BatchId);
        }

        var schedules = await _schedules.ListAsync(s => s.ExamId == examId && (!student.SectionId.HasValue || s.SectionId == student.SectionId.Value));
        var scheduleIds = schedules.Select(s => s.Id).ToHashSet();
        var markEntries = await _marks.ListAsync(m => m.StudentProfileId == studentId && scheduleIds.Contains(m.ExamScheduleId));
        if (schedules.Count == 0 || markEntries.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Enter marks for at least one schedule before computing a result.");
            return null;
        }

        var total = schedules.Sum(s => s.MaximumMarks);
        var marksBySchedule = markEntries.GroupBy(m => m.ExamScheduleId).ToDictionary(g => g.Key, g => g.OrderByDescending(m => m.EnteredOn).First());
        var obtained = schedules.Sum(s => marksBySchedule.TryGetValue(s.Id, out var mark) && !mark.IsAbsent ? mark.MarksObtained : 0);
        var percentage = total <= 0 ? 0 : Math.Round(obtained * 100 / total, 2);
        var grade = await ResolveGradeAsync(input.GradingSchemeId, exam.BranchId, batch?.CourseId, percentage);
        var currentUserId = CurrentUserId();
        var existing = (await _results.ListAsync(r => r.ExamId == examId && r.StudentProfileId == studentId)).FirstOrDefault();

        if (existing is null)
        {
            return await _results.CreateAsync(new Result
            {
                ExamId = examId,
                StudentProfileId = studentId,
                BranchId = exam.BranchId,
                AcademicYearId = exam.AcademicYearId,
                CourseId = batch?.CourseId,
                BatchId = batch?.Id,
                SectionId = student.SectionId,
                TotalMarks = total,
                MarksObtained = obtained,
                Percentage = percentage,
                Grade = grade.Grade,
                GradePoint = grade.GradePoint,
                Status = input.Publish ? ResultStatus.Published : ResultStatus.Computed,
                ComputedOn = DateTime.UtcNow,
                PublishedByUserId = input.Publish ? currentUserId : null,
                PublishedOn = input.Publish ? DateTime.UtcNow : null,
                Remarks = input.Remarks
            });
        }

        await _results.UpdateAsync(existing.Id, result =>
        {
            result.BranchId = exam.BranchId;
            result.AcademicYearId = exam.AcademicYearId;
            result.CourseId = batch?.CourseId;
            result.BatchId = batch?.Id;
            result.SectionId = student.SectionId;
            result.TotalMarks = total;
            result.MarksObtained = obtained;
            result.Percentage = percentage;
            result.Grade = grade.Grade;
            result.GradePoint = grade.GradePoint;
            result.Status = input.Publish ? ResultStatus.Published : ResultStatus.Computed;
            result.ComputedOn = DateTime.UtcNow;
            result.PublishedByUserId = input.Publish ? currentUserId : result.PublishedByUserId;
            result.PublishedOn = input.Publish ? DateTime.UtcNow : result.PublishedOn;
            result.Remarks = input.Remarks;
        });
        return await _results.GetAsync(existing.Id);
    }

    private async Task<(string Grade, decimal? GradePoint)> ResolveGradeAsync(Guid? schemeId, Guid branchId, Guid? courseId, decimal percentage)
    {
        GradingScheme? scheme = schemeId.HasValue ? await _schemes.GetAsync(schemeId.Value) : null;
        if (scheme is not null && !await CanUseSchemeAsync(scheme))
            scheme = null;
        if (scheme is null)
        {
            var defaults = await _schemes.ListAsync(s =>
                s.IsDefault &&
                (s.BranchId == branchId || s.BranchId == null) &&
                (!courseId.HasValue || s.CourseId == courseId.Value || s.CourseId == null));
            scheme = defaults
                .OrderByDescending(s => s.BranchId == branchId)
                .ThenByDescending(s => courseId.HasValue && s.CourseId == courseId.Value)
                .ThenByDescending(s => s.EffectiveFrom)
                .FirstOrDefault();
        }

        if (scheme is not null)
        {
            var band = (await _bands.ListAsync(b =>
                    b.GradingSchemeId == scheme.Id &&
                    percentage >= b.MinimumPercentage &&
                    percentage <= b.MaximumPercentage))
                .OrderBy(b => b.SortOrder)
                .FirstOrDefault();
            if (band is not null)
                return (band.Grade, band.GradePoint);
        }

        return percentage switch
        {
            >= 90 => ("A+", 10),
            >= 80 => ("A", 9),
            >= 70 => ("B+", 8),
            >= 60 => ("B", 7),
            >= 50 => ("C", 6),
            >= 35 => ("D", 5),
            _ => ("F", 0)
        };
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);
    private async Task<bool> CanUseScheduleAsync(ExamSchedule schedule) => await _exams.GetAsync(schedule.ExamId) is { } exam && await CanUseBranchAsync(exam.BranchId);
    private async Task<bool> CanUseSchemeAsync(GradingScheme scheme)
    {
        if (!IsBranchAdminOnly)
            return !scheme.BranchId.HasValue || await CanUseBranchAsync(scheme.BranchId.Value);

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        return assignedBranchId.HasValue && scheme.BranchId == assignedBranchId.Value;
    }
    private async Task<bool> CanUseBandAsync(GradingSchemeBand band) => await _schemes.GetAsync(band.GradingSchemeId) is { } scheme && await CanUseSchemeAsync(scheme);
    private async Task<bool> CanUsePaperAsync(Guid paperId) => await _papers.GetAsync(paperId) is { } paper && await CanUseBranchAsync(paper.BranchId);
    private async Task<bool> CanUsePaperQuestionAsync(QuestionPaperQuestion question) => await _paperSections.GetAsync(question.QuestionPaperSectionId) is { } section && await CanUsePaperAsync(section.QuestionPaperId);
    private async Task<bool> CanUseMarkAsync(MarkEntry mark) => await _schedules.GetAsync(mark.ExamScheduleId) is { } schedule && await CanUseScheduleAsync(schedule);
    private Guid? CurrentUserId() => Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;

    private Exam Apply(Exam entity, Guid branchId, Guid yearId)
    {
        entity.BranchId = branchId;
        entity.AcademicYearId = yearId;
        entity.Name = ExamInput.Name;
        entity.Type = ExamInput.Type;
        entity.StartDate = ExamInput.StartDate;
        entity.EndDate = ExamInput.EndDate;
        entity.WeightagePercentage = ExamInput.WeightagePercentage;
        entity.Status = ExamInput.Status;
        entity.Instructions = ExamInput.Instructions;
        return entity;
    }

    private ExamSchedule Apply(ExamSchedule entity)
    {
        entity.ExamId = ScheduleInput.ExamId!.Value;
        entity.SectionId = ScheduleInput.SectionId!.Value;
        entity.SubjectId = ScheduleInput.SubjectId!.Value;
        entity.TeacherProfileId = ScheduleInput.TeacherProfileId;
        entity.RoomId = ScheduleInput.RoomId;
        entity.ExamDate = ScheduleInput.ExamDate;
        entity.StartsAt = ScheduleInput.StartsAt;
        entity.EndsAt = ScheduleInput.EndsAt;
        entity.DurationMinutes = ScheduleInput.DurationMinutes;
        entity.MaximumMarks = ScheduleInput.MaximumMarks;
        entity.PassingMarks = ScheduleInput.PassingMarks;
        entity.Status = ScheduleInput.Status;
        entity.SeatingPlanNotes = ScheduleInput.SeatingPlanNotes;
        entity.Instructions = ScheduleInput.Instructions;
        return entity;
    }

    private GradingScheme Apply(GradingScheme entity)
    {
        entity.BranchId = SchemeInput.BranchId;
        entity.CourseId = SchemeInput.CourseId;
        entity.Name = SchemeInput.Name;
        entity.Type = SchemeInput.Type;
        entity.EffectiveFrom = SchemeInput.EffectiveFrom;
        entity.IsDefault = SchemeInput.IsDefault;
        entity.Notes = SchemeInput.Notes;
        return entity;
    }

    private GradingSchemeBand Apply(GradingSchemeBand entity)
    {
        entity.GradingSchemeId = BandInput.GradingSchemeId!.Value;
        entity.Grade = BandInput.Grade;
        entity.MinimumPercentage = BandInput.MinimumPercentage;
        entity.MaximumPercentage = BandInput.MaximumPercentage;
        entity.GradePoint = BandInput.GradePoint;
        entity.SortOrder = BandInput.SortOrder;
        entity.Remarks = BandInput.Remarks;
        return entity;
    }

    private QuestionBankItem Apply(QuestionBankItem entity)
    {
        entity.BranchId = QuestionInput.BranchId!.Value;
        entity.SubjectId = QuestionInput.SubjectId!.Value;
        entity.SyllabusUnitId = QuestionInput.SyllabusUnitId;
        entity.AuthorTeacherProfileId = QuestionInput.AuthorTeacherProfileId;
        entity.QuestionType = QuestionInput.QuestionType;
        entity.Difficulty = QuestionInput.Difficulty;
        entity.BloomLevel = QuestionInput.BloomLevel;
        entity.Source = QuestionInput.Source;
        entity.Marks = QuestionInput.Marks;
        entity.QuestionText = QuestionInput.QuestionText;
        entity.ExpectedAnswer = QuestionInput.ExpectedAnswer;
        entity.Tags = QuestionInput.Tags;
        entity.ApprovalStatus = QuestionInput.ApprovalStatus;
        return entity;
    }

    private QuestionPaper Apply(QuestionPaper entity)
    {
        entity.BranchId = PaperInput.BranchId!.Value;
        entity.SubjectId = PaperInput.SubjectId!.Value;
        entity.ExamId = PaperInput.ExamId;
        entity.ExamScheduleId = PaperInput.ExamScheduleId;
        entity.Title = PaperInput.Title;
        entity.TotalMarks = PaperInput.TotalMarks;
        entity.DurationMinutes = PaperInput.DurationMinutes;
        entity.Instructions = PaperInput.Instructions;
        entity.Source = PaperInput.Source;
        entity.Status = PaperInput.Status;
        return entity;
    }

    private QuestionPaperSection Apply(QuestionPaperSection entity)
    {
        entity.QuestionPaperId = PaperSectionInput.QuestionPaperId!.Value;
        entity.Code = PaperSectionInput.Code;
        entity.Title = PaperSectionInput.Title;
        entity.SortOrder = PaperSectionInput.SortOrder;
        entity.Marks = PaperSectionInput.Marks;
        entity.Instructions = PaperSectionInput.Instructions;
        return entity;
    }

    private QuestionPaperQuestion Apply(QuestionPaperQuestion entity)
    {
        entity.QuestionPaperSectionId = PaperQuestionInput.QuestionPaperSectionId!.Value;
        entity.QuestionBankItemId = PaperQuestionInput.QuestionBankItemId;
        entity.SortOrder = PaperQuestionInput.SortOrder;
        entity.QuestionType = PaperQuestionInput.QuestionType;
        entity.Difficulty = PaperQuestionInput.Difficulty;
        entity.BloomLevel = PaperQuestionInput.BloomLevel;
        entity.Marks = PaperQuestionInput.Marks;
        entity.QuestionText = PaperQuestionInput.QuestionText;
        entity.SolutionText = PaperQuestionInput.SolutionText;
        return entity;
    }

    private MarkEntry Apply(MarkEntry entity, Guid? currentUserId)
    {
        entity.ExamScheduleId = MarkInput.ExamScheduleId!.Value;
        entity.StudentProfileId = MarkInput.StudentProfileId!.Value;
        entity.IsAbsent = MarkInput.IsAbsent;
        entity.MarksObtained = MarkInput.IsAbsent ? 0 : MarkInput.MarksObtained;
        entity.Grade = MarkInput.Grade;
        entity.GradePoint = MarkInput.GradePoint;
        entity.EnteredByUserId = currentUserId;
        entity.EnteredOn = DateTime.UtcNow;
        entity.Remarks = MarkInput.Remarks;
        return entity;
    }

    private static ExamInputModel Map(Exam e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        Name = e.Name,
        Type = e.Type,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        WeightagePercentage = e.WeightagePercentage,
        Status = e.Status,
        Instructions = e.Instructions
    };

    private static ScheduleInputModel Map(ExamSchedule e) => new()
    {
        Id = e.Id,
        ExamId = e.ExamId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        TeacherProfileId = e.TeacherProfileId,
        RoomId = e.RoomId,
        ExamDate = e.ExamDate,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        DurationMinutes = e.DurationMinutes,
        MaximumMarks = e.MaximumMarks,
        PassingMarks = e.PassingMarks,
        Status = e.Status,
        SeatingPlanNotes = e.SeatingPlanNotes,
        Instructions = e.Instructions
    };

    private static SchemeInputModel Map(GradingScheme e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        CourseId = e.CourseId,
        Name = e.Name,
        Type = e.Type,
        EffectiveFrom = e.EffectiveFrom,
        IsDefault = e.IsDefault,
        Notes = e.Notes
    };

    private static BandInputModel Map(GradingSchemeBand e) => new()
    {
        Id = e.Id,
        GradingSchemeId = e.GradingSchemeId,
        Grade = e.Grade,
        MinimumPercentage = e.MinimumPercentage,
        MaximumPercentage = e.MaximumPercentage,
        GradePoint = e.GradePoint,
        SortOrder = e.SortOrder,
        Remarks = e.Remarks
    };

    private static QuestionInputModel Map(QuestionBankItem e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        SubjectId = e.SubjectId,
        SyllabusUnitId = e.SyllabusUnitId,
        AuthorTeacherProfileId = e.AuthorTeacherProfileId,
        QuestionType = e.QuestionType,
        Difficulty = e.Difficulty,
        BloomLevel = e.BloomLevel,
        Source = e.Source,
        Marks = e.Marks,
        QuestionText = e.QuestionText,
        ExpectedAnswer = e.ExpectedAnswer,
        Tags = e.Tags,
        ApprovalStatus = e.ApprovalStatus
    };

    private static PaperInputModel Map(QuestionPaper e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        SubjectId = e.SubjectId,
        ExamId = e.ExamId,
        ExamScheduleId = e.ExamScheduleId,
        Title = e.Title,
        TotalMarks = e.TotalMarks,
        DurationMinutes = e.DurationMinutes,
        Instructions = e.Instructions,
        Source = e.Source,
        Status = e.Status
    };

    private static PaperSectionInputModel Map(QuestionPaperSection e) => new()
    {
        Id = e.Id,
        QuestionPaperId = e.QuestionPaperId,
        Code = e.Code,
        Title = e.Title,
        SortOrder = e.SortOrder,
        Marks = e.Marks,
        Instructions = e.Instructions
    };

    private static PaperQuestionInputModel Map(QuestionPaperQuestion e) => new()
    {
        Id = e.Id,
        QuestionPaperSectionId = e.QuestionPaperSectionId,
        QuestionBankItemId = e.QuestionBankItemId,
        SortOrder = e.SortOrder,
        QuestionType = e.QuestionType,
        Difficulty = e.Difficulty,
        BloomLevel = e.BloomLevel,
        Marks = e.Marks,
        QuestionText = e.QuestionText,
        SolutionText = e.SolutionText
    };

    private static MarkInputModel Map(MarkEntry e) => new()
    {
        Id = e.Id,
        ExamScheduleId = e.ExamScheduleId,
        StudentProfileId = e.StudentProfileId,
        MarksObtained = e.MarksObtained,
        IsAbsent = e.IsAbsent,
        Grade = e.Grade,
        GradePoint = e.GradePoint,
        Remarks = e.Remarks
    };
}
