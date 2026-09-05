using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Attendance;

[Authorize(Policy = AuthorizationPolicies.AttendanceMarker)]
public class MarkModel : PageModel
{
    private readonly ICrudService<AttendanceSession> _sessions;
    private readonly ICrudService<AttendanceRecord> _records;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<TeacherSubjectAssignment> _teacherAssignments;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public MarkModel(
        ICrudService<AttendanceSession> sessions,
        ICrudService<AttendanceRecord> records,
        ICrudService<Branch> branches,
        ICrudService<Section> sections,
        ICrudService<Subject> subjects,
        ICrudService<StudentProfile> students,
        ICrudService<TeacherProfile> teachers,
        ICrudService<TeacherSubjectAssignment> teacherAssignments,
        UserManager<ApplicationUser> userManager,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _sessions = sessions;
        _records = records;
        _branches = branches;
        _sections = sections;
        _subjects = subjects;
        _students = students;
        _teachers = teachers;
        _teacherAssignments = teacherAssignments;
        _userManager = userManager;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public bool IsTeacherOnly => User.IsInRole(Roles.Teacher) &&
        !User.IsInRole(Roles.SuperAdmin) &&
        !User.IsInRole(Roles.TenantAdmin) &&
        !User.IsInRole(Roles.BranchAdmin) &&
        !User.IsInRole(Roles.Principal);
    public bool IsBranchAdminOnly => _branchAccess.IsBranchAdminOnly(User);
    public string? Notice { get; private set; }
    public IReadOnlyList<AttendanceSession> Sessions { get; private set; } = new List<AttendanceSession>();
    public IReadOnlyList<AttendanceRecord> Records { get; private set; } = new List<AttendanceRecord>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<Subject> Subjects { get; private set; } = new List<Subject>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();

    [BindProperty] public SessionInputModel SessionInput { get; set; } = new();
    [BindProperty] public RecordInputModel RecordInput { get; set; } = new();
    public bool IsEditingSession => SessionInput.Id != Guid.Empty;
    public bool IsEditingRecord => RecordInput.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string SectionName(Guid id) => Sections.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string SubjectName(Guid? id) => Subjects.FirstOrDefault(s => s.Id == id)?.Name ?? "Daily";
    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }
    public string SessionLabel(Guid id)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == id);
        return session is null
            ? "-"
            : $"{session.AttendanceDate} / {SectionName(session.SectionId)} / {SubjectName(session.SubjectId)}";
    }

    public class SessionInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Section"), Required] public Guid? SectionId { get; set; }
        [Display(Name = "Subject")] public Guid? SubjectId { get; set; }
        [Required, Display(Name = "Date")] public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Type")] public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.Daily;
        [Display(Name = "Period")] public int? PeriodNumber { get; set; }
        [Display(Name = "Start")] public TimeOnly? StartsAt { get; set; }
        [Display(Name = "End")] public TimeOnly? EndsAt { get; set; }
        public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Draft;
        [StringLength(500)] public string? Notes { get; set; }
    }

    public class RecordInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Session"), Required] public Guid? AttendanceSessionId { get; set; }
        [Display(Name = "Student"), Required] public Guid? StudentProfileId { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        [StringLength(500)] public string? Remarks { get; set; }
    }

    public async Task OnGetAsync(Guid? sessionEditId, Guid? recordEditId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (sessionEditId is Guid sessionId &&
            await _sessions.GetAsync(sessionId) is { } session &&
            await CanUseBranchAsync(session.BranchId) &&
            await CanUseSectionAsync(session.SectionId, session.SubjectId))
        {
            SessionInput = Map(session);
        }

        if (recordEditId is Guid recordId &&
            await _records.GetAsync(recordId) is { } record &&
            await _sessions.GetAsync(record.AttendanceSessionId) is { } recordSession &&
            await CanUseBranchAsync(recordSession.BranchId))
        {
            RecordInput = Map(record);
        }
    }

    public async Task<IActionResult> OnPostSaveSessionAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(SessionInput));
        var branchId = SessionInput.BranchId ?? Guid.Empty;
        var sectionId = SessionInput.SectionId ?? Guid.Empty;

        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("SessionInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("SessionInput.BranchId", "You can manage attendance only for your assigned branch.");
        if (sectionId == Guid.Empty || await _sections.GetAsync(sectionId) is null)
            ModelState.AddModelError("SessionInput.SectionId", "Selected section was not found.");
        if (SessionInput.SubjectId is Guid subjectId && await _subjects.GetAsync(subjectId) is null)
            ModelState.AddModelError("SessionInput.SubjectId", "Selected subject was not found.");
        if (!await CanUseSectionAsync(sectionId, SessionInput.SubjectId))
            ModelState.AddModelError("SessionInput.SectionId", "You can mark attendance only for sections assigned to you.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (SessionInput.Id == Guid.Empty)
            await _sessions.CreateAsync(Apply(new AttendanceSession(), branchId, sectionId));
        else
            await _sessions.UpdateAsync(SessionInput.Id, e => Apply(e, branchId, sectionId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveRecordAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(RecordInput));
        var sessionId = RecordInput.AttendanceSessionId ?? Guid.Empty;
        var studentId = RecordInput.StudentProfileId ?? Guid.Empty;
        var session = sessionId == Guid.Empty ? null : await _sessions.GetAsync(sessionId);
        var student = studentId == Guid.Empty ? null : await _students.GetAsync(studentId);

        if (session is null)
            ModelState.AddModelError("RecordInput.AttendanceSessionId", "Selected attendance session was not found.");
        else if (!await CanUseBranchAsync(session.BranchId))
            ModelState.AddModelError("RecordInput.AttendanceSessionId", "You can manage attendance only for your assigned branch.");
        if (student is null)
            ModelState.AddModelError("RecordInput.StudentProfileId", "Selected student was not found.");
        else if (!await CanUseBranchAsync(student.BranchId))
            ModelState.AddModelError("RecordInput.StudentProfileId", "You can manage students only for your assigned branch.");
        if (session is not null && student is not null && student.SectionId != session.SectionId)
            ModelState.AddModelError("RecordInput.StudentProfileId", "Student does not belong to the selected session section.");
        if (session is not null && !await CanUseSectionAsync(session.SectionId, session.SubjectId))
            ModelState.AddModelError("RecordInput.AttendanceSessionId", "You can mark attendance only for sections assigned to you.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (RecordInput.Id == Guid.Empty)
            await _records.CreateAsync(Apply(new AttendanceRecord(), sessionId, studentId));
        else
            await _records.UpdateAsync(RecordInput.Id, e => Apply(e, sessionId, studentId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteSessionAsync(Guid id)
    {
        if (await _sessions.GetAsync(id) is { } session && await CanUseBranchAsync(session.BranchId))
            await _sessions.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteRecordAsync(Guid id)
    {
        if (await _records.GetAsync(id) is { } record &&
            await _sessions.GetAsync(record.AttendanceSessionId) is { } session &&
            await CanUseBranchAsync(session.BranchId))
        {
            await _records.SoftDeleteAsync(id);
        }

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Subjects = (await _subjects.ListAsync()).OrderBy(s => s.Name).ToList();
        Sections = await LoadSectionsAsync(assignedBranchId);
        var sectionIds = Sections.Select(s => s.Id).ToHashSet();
        Sessions = (await _sessions.ListAsync(s =>
                (!IsBranchAdminOnly || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)) &&
                (!IsTeacherOnly || sectionIds.Contains(s.SectionId))))
            .OrderByDescending(s => s.AttendanceDate)
            .ThenBy(s => s.PeriodNumber)
            .ToList();
        Students = (await _students.ListAsync(s =>
                (!IsBranchAdminOnly || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)) &&
                s.SectionId.HasValue &&
                sectionIds.Contains(s.SectionId.Value)))
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .ToList();
        var sessionIds = Sessions.Select(s => s.Id).ToHashSet();
        Records = (await _records.ListAsync(r => sessionIds.Contains(r.AttendanceSessionId)))
            .OrderByDescending(r => r.MarkedOn)
            .ToList();

        if (IsTeacherOnly && Sections.Count == 0)
            Notice = "No sections are assigned to your teacher profile yet.";
        if (IsBranchAdminOnly && !assignedBranchId.HasValue)
            Notice = "Your branch administrator account is not assigned to a branch yet.";
        if (IsBranchAdminOnly && assignedBranchId.HasValue && SessionInput.BranchId is null)
            SessionInput.BranchId = assignedBranchId.Value;
    }

    private void KeepOnlyModelStateFor(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(k => !k.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);
    }

    private async Task<IReadOnlyList<Section>> LoadSectionsAsync(Guid? assignedBranchId)
    {
        var allSections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        if (IsBranchAdminOnly)
        {
            if (!assignedBranchId.HasValue)
                return new List<Section>();

            var studentSectionIds = (await _students.ListAsync(s =>
                    s.BranchId == assignedBranchId.Value && s.SectionId.HasValue))
                .Select(s => s.SectionId!.Value);
            var sessionSectionIds = (await _sessions.ListAsync(s => s.BranchId == assignedBranchId.Value))
                .Select(s => s.SectionId);
            var scopedSectionIds = studentSectionIds.Concat(sessionSectionIds).Distinct().ToHashSet();

            return allSections.Where(s => scopedSectionIds.Contains(s.Id)).ToList();
        }

        if (!IsTeacherOnly)
            return allSections;

        if (!Guid.TryParse(_userManager.GetUserId(User), out var userId))
            return new List<Section>();

        var teacher = (await _teachers.ListAsync(t => t.UserId == userId)).FirstOrDefault();
        if (teacher is null)
            return new List<Section>();

        var assignments = await _teacherAssignments.ListAsync(a => a.TeacherProfileId == teacher.Id);
        var assignedSectionIds = assignments
            .Where(a => a.SectionId.HasValue)
            .Select(a => a.SectionId!.Value)
            .Concat(allSections.Where(s => s.ClassTeacherUserId == userId).Select(s => s.Id))
            .Distinct()
            .ToHashSet();

        return allSections.Where(s => assignedSectionIds.Contains(s.Id)).ToList();
    }

    private async Task<bool> CanUseSectionAsync(Guid sectionId, Guid? subjectId)
    {
        if (!IsTeacherOnly)
            return true;

        if (!Guid.TryParse(_userManager.GetUserId(User), out var userId))
            return false;

        var teacher = (await _teachers.ListAsync(t => t.UserId == userId)).FirstOrDefault();
        if (teacher is null)
            return false;

        var assignments = await _teacherAssignments.ListAsync(a => a.TeacherProfileId == teacher.Id);
        return assignments.Any(a =>
                   (a.SectionId == sectionId || !a.SectionId.HasValue) &&
                   (!subjectId.HasValue || a.SubjectId == subjectId.Value)) ||
               (await _sections.ListAsync(s => s.Id == sectionId && s.ClassTeacherUserId == userId)).Any();
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private AttendanceSession Apply(AttendanceSession entity, Guid branchId, Guid sectionId)
    {
        entity.BranchId = branchId;
        entity.SectionId = sectionId;
        entity.SubjectId = SessionInput.SubjectId;
        entity.AttendanceDate = SessionInput.AttendanceDate;
        entity.SessionType = SessionInput.SessionType;
        entity.PeriodNumber = SessionInput.PeriodNumber;
        entity.StartsAt = SessionInput.StartsAt;
        entity.EndsAt = SessionInput.EndsAt;
        entity.MarkedByUserId = Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;
        entity.Status = SessionInput.Status;
        entity.Notes = SessionInput.Notes;
        return entity;
    }

    private AttendanceRecord Apply(AttendanceRecord entity, Guid sessionId, Guid studentId)
    {
        entity.AttendanceSessionId = sessionId;
        entity.StudentProfileId = studentId;
        entity.Status = RecordInput.Status;
        entity.MarkedByUserId = Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;
        entity.MarkedOn = DateTime.UtcNow;
        entity.Remarks = RecordInput.Remarks;
        return entity;
    }

    private static SessionInputModel Map(AttendanceSession e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        AttendanceDate = e.AttendanceDate,
        SessionType = e.SessionType,
        PeriodNumber = e.PeriodNumber,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        Status = e.Status,
        Notes = e.Notes
    };

    private static RecordInputModel Map(AttendanceRecord e) => new()
    {
        Id = e.Id,
        AttendanceSessionId = e.AttendanceSessionId,
        StudentProfileId = e.StudentProfileId,
        Status = e.Status,
        Remarks = e.Remarks
    };
}
