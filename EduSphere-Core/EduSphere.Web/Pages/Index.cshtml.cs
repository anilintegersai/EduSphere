using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainTimetable = EduSphere.Domain.Entities.Timetable;

namespace EduSphere.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ITenantContext _tenant;
    private readonly ITenantService _tenants;
    private readonly IBranchService _branches;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchAccessService _branchAccess;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<SyllabusUnit> _syllabusUnits;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<TeacherSubjectAssignment> _teacherAssignments;
    private readonly ICrudService<AdmissionApplication> _admissions;
    private readonly ICrudService<AttendanceSession> _attendanceSessions;
    private readonly ICrudService<AttendanceRecord> _attendanceRecords;
    private readonly ICrudService<DomainTimetable> _timetables;
    private readonly ICrudService<TimetableEntry> _timetableEntries;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<QuestionBankItem> _questionBankItems;
    private readonly ICrudService<QuestionPaper> _questionPapers;
    private readonly ICrudService<MarkEntry> _markEntries;
    private readonly ICrudService<Result> _results;

    public IndexModel(
        ITenantContext tenant,
        ITenantService tenants,
        IBranchService branches,
        UserManager<ApplicationUser> userManager,
        IBranchAccessService branchAccess,
        ICrudService<AcademicYear> years,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        ICrudService<Course> courses,
        ICrudService<Subject> subjects,
        ICrudService<SyllabusUnit> syllabusUnits,
        ICrudService<StudentProfile> students,
        ICrudService<TeacherProfile> teachers,
        ICrudService<TeacherSubjectAssignment> teacherAssignments,
        ICrudService<AdmissionApplication> admissions,
        ICrudService<AttendanceSession> attendanceSessions,
        ICrudService<AttendanceRecord> attendanceRecords,
        ICrudService<DomainTimetable> timetables,
        ICrudService<TimetableEntry> timetableEntries,
        ICrudService<Exam> exams,
        ICrudService<QuestionBankItem> questionBankItems,
        ICrudService<QuestionPaper> questionPapers,
        ICrudService<MarkEntry> markEntries,
        ICrudService<Result> results)
    {
        _tenant = tenant;
        _tenants = tenants;
        _branches = branches;
        _userManager = userManager;
        _branchAccess = branchAccess;
        _years = years;
        _batches = batches;
        _sections = sections;
        _courses = courses;
        _subjects = subjects;
        _syllabusUnits = syllabusUnits;
        _students = students;
        _teachers = teachers;
        _teacherAssignments = teacherAssignments;
        _admissions = admissions;
        _attendanceSessions = attendanceSessions;
        _attendanceRecords = attendanceRecords;
        _timetables = timetables;
        _timetableEntries = timetableEntries;
        _exams = exams;
        _questionBankItems = questionBankItems;
        _questionPapers = questionPapers;
        _markEntries = markEntries;
        _results = results;
    }

    public bool IsAuthenticated { get; private set; }
    public bool IsSuper { get; private set; }
    public bool IsTenantAdmin { get; private set; }
    public bool IsBranchAdmin { get; private set; }
    public bool IsTeacher { get; private set; }
    public bool IsStudent { get; private set; }
    public bool CanManageTenantWorkspace => IsSuper || IsTenantAdmin;
    public bool CanManageBranchWorkspace => CanManageTenantWorkspace || IsBranchAdmin;
    public bool HasTenant => _tenant.HasTenant;
    public string? CurrentTenant => _tenant.TenantIdentifier;

    public int TenantCount { get; private set; }
    public int BranchCount { get; private set; }
    public int AcademicYearCount { get; private set; }
    public int CourseCount { get; private set; }
    public int SubjectCount { get; private set; }
    public int StudentCount { get; private set; }
    public int TeacherCount { get; private set; }
    public int AdmissionApplicationCount { get; private set; }
    public int AttendanceSessionCount { get; private set; }
    public int TimetableEntryCount { get; private set; }
    public int ExamCount { get; private set; }
    public int QuestionPaperCount { get; private set; }
    public int ResultCount { get; private set; }
    public string RoleLabel { get; private set; } = "User";
    public string DashboardEyebrow { get; private set; } = "Workspace";
    public string DashboardDescription { get; private set; } =
        "Welcome to your EduSphere workspace.";
    public string? DashboardNotice { get; private set; }
    public IReadOnlyList<DashboardMetric> Metrics { get; private set; } = [];

    public async Task OnGetAsync()
    {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        if (!IsAuthenticated) return;

        IsSuper = User.IsInRole(Roles.SuperAdmin);
        IsTenantAdmin = User.IsInRole(Roles.TenantAdmin);
        IsBranchAdmin = User.IsInRole(Roles.BranchAdmin);
        IsTeacher = User.IsInRole(Roles.Teacher);
        IsStudent = User.IsInRole(Roles.Student);
        RoleLabel = ResolveRoleLabel();

        var currentUser = await _userManager.GetUserAsync(User);
        await ResolveTenantFromSignedInUserAsync(currentUser);

        if (IsSuper)
            TenantCount = (await _tenants.GetAllTenantsAsync()).Count();

        DashboardEyebrow = ResolveDashboardEyebrow();
        DashboardDescription = ResolveDashboardDescription();

        var metrics = new List<DashboardMetric>();
        Metrics = metrics;

        if (CanManageTenantWorkspace)
        {
            await BuildAdminMetricsAsync(metrics);
            return;
        }

        if (_tenant.TenantId is Guid tenantId)
        {
            if (IsBranchAdmin && currentUser is not null)
            {
                await BuildBranchAdminMetricsAsync(metrics, tenantId);
            }
            else if (IsTeacher && currentUser is not null)
            {
                await BuildTeacherMetricsAsync(metrics, currentUser.Id);
            }
            else if (IsStudent && currentUser is not null)
            {
                await BuildStudentMetricsAsync(metrics, currentUser.Id);
            }
            else
            {
                await BuildTenantSummaryMetricsAsync(metrics, tenantId);
            }
        }
        else
        {
            DashboardNotice = "Your tenant workspace is not resolved yet. Ask an administrator to confirm your tenant assignment.";
        }
    }

    private async Task ResolveTenantFromSignedInUserAsync(ApplicationUser? currentUser)
    {
        if (_tenant.HasTenant || currentUser is null || IsSuper || currentUser.TenantId == Guid.Empty)
            return;

        var userTenant = await _tenants.GetTenantByIdAsync(currentUser.TenantId);
        if (userTenant is { IsActive: true })
            _tenant.SetTenant(userTenant.Id, userTenant.TenantIdentifier);
    }

    private async Task BuildAdminMetricsAsync(List<DashboardMetric> metrics)
    {
        if (IsSuper)
        {
            metrics.Add(new DashboardMetric(
                "Tenants",
                FormatCount(TenantCount),
                "Institutions in the platform registry",
                "metric-card-blue",
                "/Tenants/Index"));
        }

        if (_tenant.TenantId is not Guid tenantId)
        {
            if (IsSuper)
            {
                DashboardNotice = TenantCount == 0
                    ? "No tenants exist yet. Create one to begin tenant operations."
                    : "Select a tenant from the switcher in the top bar to manage branches, academic structure, and people.";
            }
            else
            {
                DashboardNotice = "Your tenant workspace is not resolved yet. Ask a platform administrator to confirm your tenant assignment.";
            }

            return;
        }

        await BuildTenantSummaryMetricsAsync(metrics, tenantId);
    }

    private async Task BuildTenantSummaryMetricsAsync(List<DashboardMetric> metrics, Guid tenantId)
    {
        BranchCount = (await _branches.GetBranchesByTenantAsync(tenantId)).Count();
        AcademicYearCount = (await _years.ListAsync()).Count;
        CourseCount = (await _courses.ListAsync()).Count;
        SubjectCount = (await _subjects.ListAsync()).Count;
        StudentCount = (await _students.ListAsync()).Count;
        TeacherCount = (await _teachers.ListAsync()).Count;
        AdmissionApplicationCount = (await _admissions.ListAsync()).Count;
        AttendanceSessionCount = (await _attendanceSessions.ListAsync()).Count;
        TimetableEntryCount = (await _timetableEntries.ListAsync()).Count;
        ExamCount = (await _exams.ListAsync()).Count;
        QuestionPaperCount = (await _questionPapers.ListAsync()).Count;
        ResultCount = (await _results.ListAsync()).Count;

        metrics.AddRange([
            new DashboardMetric("Branches", FormatCount(BranchCount), "Campuses configured for this tenant", "metric-card-teal", "/Branches/Index"),
            new DashboardMetric("Courses", FormatCount(CourseCount), "Programs available for enrollment", "metric-card-gold", "/Academic/Courses"),
            new DashboardMetric("Subjects", FormatCount(SubjectCount), "Subject catalog prepared for syllabus", "metric-card-rose", "/Academic/Subjects"),
            new DashboardMetric("Academic Years", FormatCount(AcademicYearCount), "Calendar periods available for batches", "metric-card-green", "/Academic/AcademicYears"),
            new DashboardMetric("Students", FormatCount(StudentCount), "Learner profiles ready for admissions and attendance", "metric-card-blue", "/People/Students"),
            new DashboardMetric("Teachers", FormatCount(TeacherCount), "Faculty profiles ready for timetable and exams", "metric-card-teal", "/People/Teachers"),
            new DashboardMetric("Applications", FormatCount(AdmissionApplicationCount), "Admission pipeline records under review", "metric-card-gold", "/Admissions/Applications"),
            new DashboardMetric("Attendance Sessions", FormatCount(AttendanceSessionCount), "Marked or planned attendance sessions", "metric-card-green", "/Attendance/Mark"),
            new DashboardMetric("Timetable Entries", FormatCount(TimetableEntryCount), "Teacher, subject, room, and slot allocations", "metric-card-rose", "/Timetable/Schedule"),
            new DashboardMetric("Exams", FormatCount(ExamCount), "Exam plans, schedules, question papers, marks, and results", "metric-card-blue", "/Examinations/Exams"),
            new DashboardMetric("Question Papers", FormatCount(QuestionPaperCount), "Manual and generated papers prepared for exams", "metric-card-gold", "/Examinations/Exams"),
            new DashboardMetric("Published Results", FormatCount(ResultCount), "Computed academic outcomes ready for reporting", "metric-card-green", "/Examinations/Exams")
        ]);
    }

    private async Task BuildBranchAdminMetricsAsync(List<DashboardMetric> metrics, Guid tenantId)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        if (!assignedBranchId.HasValue)
        {
            DashboardNotice = "Your branch administrator account is not assigned to a branch yet. Ask a tenant administrator to assign a branch.";
            return;
        }

        var branch = (await _branches.GetBranchesByTenantAsync(tenantId))
            .FirstOrDefault(b => b.Id == assignedBranchId.Value);
        if (branch is null)
        {
            DashboardNotice = "Your assigned branch was not found in this tenant. Ask a tenant administrator to review your account.";
            return;
        }

        BranchCount = 1;
        StudentCount = (await _students.ListAsync(s => s.BranchId == branch.Id)).Count;
        TeacherCount = (await _teachers.ListAsync(t => t.BranchId == branch.Id)).Count;
        AdmissionApplicationCount = (await _admissions.ListAsync(a => a.BranchId == branch.Id)).Count;
        AttendanceSessionCount = (await _attendanceSessions.ListAsync(s => s.BranchId == branch.Id)).Count;
        var branchTimetables = await _timetables.ListAsync(t => t.BranchId == branch.Id);
        var timetableIds = branchTimetables.Select(t => t.Id).ToHashSet();
        TimetableEntryCount = timetableIds.Count == 0
            ? 0
            : (await _timetableEntries.ListAsync(e => timetableIds.Contains(e.TimetableId))).Count;
        ExamCount = (await _exams.ListAsync(e => e.BranchId == branch.Id)).Count;
        QuestionPaperCount = (await _questionPapers.ListAsync(p => p.BranchId == branch.Id)).Count;
        ResultCount = (await _results.ListAsync(r => r.BranchId == branch.Id)).Count;

        metrics.AddRange([
            new DashboardMetric("Assigned Branch", branch.Code ?? branch.Name, branch.Name, "metric-card-teal"),
            new DashboardMetric("Students", FormatCount(StudentCount), "Learner profiles assigned to your branch", "metric-card-blue", "/People/Students"),
            new DashboardMetric("Teachers", FormatCount(TeacherCount), "Faculty profiles assigned to your branch", "metric-card-teal", "/People/Teachers"),
            new DashboardMetric("Applications", FormatCount(AdmissionApplicationCount), "Admission pipeline records for your branch", "metric-card-gold", "/Admissions/Applications"),
            new DashboardMetric("Attendance Sessions", FormatCount(AttendanceSessionCount), "Marked or planned attendance sessions for your branch", "metric-card-green", "/Attendance/Mark"),
            new DashboardMetric("Timetable Entries", FormatCount(TimetableEntryCount), "Scheduled branch classes by room, slot, subject, and teacher", "metric-card-rose", "/Timetable/Schedule"),
            new DashboardMetric("Exams", FormatCount(ExamCount), "Exam schedules, question papers, marks, and results for your branch", "metric-card-blue", "/Examinations/Exams"),
            new DashboardMetric("Question Papers", FormatCount(QuestionPaperCount), "Manual and generated papers prepared for branch exams", "metric-card-gold", "/Examinations/Exams"),
            new DashboardMetric("Results", FormatCount(ResultCount), "Computed outcomes for branch students", "metric-card-green", "/Examinations/Exams")
        ]);
    }

    private async Task BuildTeacherMetricsAsync(List<DashboardMetric> metrics, Guid userId)
    {
        var teacherProfile = (await _teachers.ListAsync(t => t.UserId == userId)).FirstOrDefault();
        if (teacherProfile is null)
        {
            DashboardNotice = "Your teacher profile has not been linked yet. Ask a tenant administrator to connect your login with a teacher profile.";
            return;
        }

        var assignments = await _teacherAssignments.ListAsync(a => a.TeacherProfileId == teacherProfile.Id);
        var subjectIds = assignments.Select(a => a.SubjectId).Distinct().ToList();
        var sectionIds = assignments
            .Where(a => a.SectionId.HasValue)
            .Select(a => a.SectionId!.Value)
            .Distinct()
            .ToList();

        var classTeacherSections = await _sections.ListAsync(s => s.ClassTeacherUserId == userId);
        sectionIds = sectionIds
            .Concat(classTeacherSections.Select(s => s.Id))
            .Distinct()
            .ToList();

        var studentCoverage = sectionIds.Count == 0
            ? 0
            : (await _students.ListAsync(s => s.SectionId.HasValue && sectionIds.Contains(s.SectionId.Value))).Count;

        var syllabusUnits = subjectIds.Count == 0
            ? 0
            : (await _syllabusUnits.ListAsync(u => subjectIds.Contains(u.SubjectId))).Count;
        var timetableEntries = await _timetableEntries.ListAsync(e => e.TeacherProfileId == teacherProfile.Id);
        var markedSessions = await _attendanceSessions.ListAsync(s => s.MarkedByUserId == userId);

        metrics.AddRange([
            new DashboardMetric("Profile Status", teacherProfile.Status.ToString(), $"Employee {teacherProfile.EmployeeNumber}", "metric-card-teal"),
            new DashboardMetric("Teaching Assignments", FormatCount(assignments.Count), "Subject and section responsibilities assigned to you", "metric-card-blue"),
            new DashboardMetric("Assigned Subjects", FormatCount(subjectIds.Count), "Subjects currently mapped to your timetable foundation", "metric-card-gold"),
            new DashboardMetric("Class Sections", FormatCount(sectionIds.Count), "Sections where you are assigned or class teacher", "metric-card-green"),
            new DashboardMetric("Students In Classes", FormatCount(studentCoverage), "Learners visible through your assigned sections", "metric-card-rose"),
            new DashboardMetric("Syllabus Units", FormatCount(syllabusUnits), "Teaching units linked to your assigned subjects", "metric-card-blue"),
            new DashboardMetric("Timetable Entries", FormatCount(timetableEntries.Count), "Scheduled classes assigned to you", "metric-card-teal"),
            new DashboardMetric("Marked Sessions", FormatCount(markedSessions.Count), "Attendance sessions marked from your login", "metric-card-green", "/Attendance/Mark")
        ]);
    }

    private async Task BuildStudentMetricsAsync(List<DashboardMetric> metrics, Guid userId)
    {
        var studentProfile = (await _students.ListAsync(s => s.UserId == userId)).FirstOrDefault();
        if (studentProfile is null)
        {
            DashboardNotice = "Your student profile has not been linked yet. Ask a tenant administrator to connect your login with a student profile.";
            return;
        }

        Section? section = null;
        Batch? batch = null;
        if (studentProfile.SectionId.HasValue)
        {
            section = await _sections.GetAsync(studentProfile.SectionId.Value);
            if (section is not null)
                batch = await _batches.GetAsync(section.BatchId);
        }

        var subjects = batch is null
            ? new List<Subject>()
            : (await _subjects.ListAsync(s => s.CourseId == batch.CourseId)).ToList();
        var subjectIds = subjects.Select(s => s.Id).Distinct().ToList();

        var classmates = studentProfile.SectionId.HasValue
            ? (await _students.ListAsync(s => s.SectionId == studentProfile.SectionId && s.Id != studentProfile.Id)).Count
            : 0;

        var syllabusUnits = subjectIds.Count == 0
            ? 0
            : (await _syllabusUnits.ListAsync(u => subjectIds.Contains(u.SubjectId))).Count;
        var attendanceRecords = await _attendanceRecords.ListAsync(r => r.StudentProfileId == studentProfile.Id);
        var attendanceRate = CalculateAttendanceRate(attendanceRecords);

        var teacherAssignments = studentProfile.SectionId.HasValue
            ? await _teacherAssignments.ListAsync(a =>
                (a.SectionId.HasValue && a.SectionId.Value == studentProfile.SectionId.Value) ||
                subjectIds.Contains(a.SubjectId))
            : new List<TeacherSubjectAssignment>();

        metrics.AddRange([
            new DashboardMetric("Profile Status", studentProfile.Status.ToString(), $"Admission {studentProfile.AdmissionNumber}", "metric-card-blue"),
            new DashboardMetric("My Section", section?.Name ?? "Pending", "Class section linked to your student profile", "metric-card-teal"),
            new DashboardMetric("My Subjects", FormatCount(subjectIds.Count), "Subjects attached to your course structure", "metric-card-gold"),
            new DashboardMetric("Syllabus Units", FormatCount(syllabusUnits), "Learning units available through your subjects", "metric-card-green"),
            new DashboardMetric("Classmates", FormatCount(classmates), "Other active learner profiles in your section", "metric-card-rose"),
            new DashboardMetric("Assigned Teachers", FormatCount(teacherAssignments.Select(a => a.TeacherProfileId).Distinct().Count()), "Faculty mapped to your section or subjects", "metric-card-teal"),
            new DashboardMetric("Attendance Rate", attendanceRate, "Present, late, and excused marks counted from available records", "metric-card-green"),
            new DashboardMetric("Attendance Marks", FormatCount(attendanceRecords.Count), "Attendance records currently captured for you", "metric-card-blue")
        ]);
    }

    private string ResolveRoleLabel()
    {
        if (IsSuper) return "Super Admin";
        if (IsTenantAdmin) return "Tenant Admin";
        if (IsBranchAdmin) return "Branch Admin";
        if (IsTeacher) return "Teacher";
        if (IsStudent) return "Student";
        return "User";
    }

    private string ResolveDashboardEyebrow()
    {
        if (IsSuper) return _tenant.HasTenant ? "Tenant Workspace" : "Platform Console";
        if (IsTenantAdmin) return "Tenant Operations";
        if (IsBranchAdmin) return "Branch Operations";
        if (IsTeacher) return "Teacher Workspace";
        if (IsStudent) return "Student Workspace";
        return "Workspace";
    }

    private string ResolveDashboardDescription()
    {
        if (IsSuper && !_tenant.HasTenant)
            return "Manage the platform registry and switch into a tenant workspace when you need to inspect institution operations.";
        if (IsSuper)
            return "Review the selected tenant's branches, academic structure, and people registry from the platform console.";
        if (IsTenantAdmin)
            return "Monitor your institution's campuses, academic masters, people, attendance, timetable, exams, question papers, and results from one operations view.";
        if (IsBranchAdmin)
            return "Manage admissions, people, attendance, timetable, exams, question papers, marks, and results for your assigned branch.";
        if (IsTeacher)
            return "Track your teaching profile, assigned subjects, class coverage, and syllabus workload.";
        if (IsStudent)
            return "See your academic profile, class context, subjects, syllabus coverage, and assigned faculty.";
        return "Welcome to your EduSphere workspace.";
    }

    private static string FormatCount(int count) => count.ToString("N0");

    private static string CalculateAttendanceRate(IReadOnlyList<AttendanceRecord> records)
    {
        if (records.Count == 0)
            return "N/A";

        var attended = records.Count(r =>
            r.Status is AttendanceStatus.Present or
                AttendanceStatus.Late or
                AttendanceStatus.Excused or
                AttendanceStatus.HalfDay);
        return $"{attended * 100m / records.Count:0.#}%";
    }

    public sealed record DashboardMetric(
        string Label,
        string Value,
        string Hint,
        string CssClass,
        string? PagePath = null);
}
