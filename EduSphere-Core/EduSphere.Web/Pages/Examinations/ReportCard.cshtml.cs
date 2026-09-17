using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Examinations;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class ReportCardModel : PageModel
{
    private readonly ICrudService<Result> _results;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly ICrudService<MarkEntry> _marks;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<ResultRankingRule> _rankingRules;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public ReportCardModel(
        ICrudService<Result> results,
        ICrudService<Exam> exams,
        ICrudService<StudentProfile> students,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Section> sections,
        ICrudService<ExamSchedule> schedules,
        ICrudService<MarkEntry> marks,
        ICrudService<Subject> subjects,
        ICrudService<ResultRankingRule> rankingRules,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _results = results;
        _exams = exams;
        _students = students;
        _branches = branches;
        _years = years;
        _sections = sections;
        _schedules = schedules;
        _marks = marks;
        _subjects = subjects;
        _rankingRules = rankingRules;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public Result Result { get; private set; } = null!;
    public Exam Exam { get; private set; } = null!;
    public StudentProfile Student { get; private set; } = null!;
    public Branch Branch { get; private set; } = null!;
    public AcademicYear? AcademicYear { get; private set; }
    public Section? Section { get; private set; }
    public IReadOnlyList<MarkSheetRow> MarkRows { get; private set; } = new List<MarkSheetRow>();
    public bool ShowRank { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!_tenant.HasTenant) return NotFound();
        var result = await _results.GetAsync(id);
        if (result is null || !await _branchAccess.CanAccessBranchAsync(User, result.BranchId)) return NotFound();
        if (result.Status != Domain.Enums.ResultStatus.Published) return Forbid();

        var exam = await _exams.GetAsync(result.ExamId);
        var student = await _students.GetAsync(result.StudentProfileId);
        var branch = await _branches.GetAsync(result.BranchId);
        if (exam is null || student is null || branch is null) return NotFound();

        Result = result;
        Exam = exam;
        Student = student;
        Branch = branch;
        AcademicYear = await _years.GetAsync(result.AcademicYearId);
        Section = result.SectionId.HasValue ? await _sections.GetAsync(result.SectionId.Value) : null;

        var schedules = await _schedules.ListAsync(s => s.ExamId == result.ExamId && (!result.SectionId.HasValue || s.SectionId == result.SectionId.Value));
        var scheduleIds = schedules.Select(s => s.Id).ToHashSet();
        var marks = await _marks.ListAsync(m => m.StudentProfileId == result.StudentProfileId && scheduleIds.Contains(m.ExamScheduleId));
        var subjects = await _subjects.ListAsync();
        MarkRows = schedules.OrderBy(s => s.ExamDate).ThenBy(s => subjects.FirstOrDefault(x => x.Id == s.SubjectId)?.Name)
            .Select(schedule =>
            {
                var mark = marks.OrderByDescending(m => m.EnteredOn).FirstOrDefault(m => m.ExamScheduleId == schedule.Id);
                return new MarkSheetRow(
                    subjects.FirstOrDefault(s => s.Id == schedule.SubjectId)?.Name ?? "Subject",
                    schedule.MaximumMarks,
                    schedule.PassingMarks,
                    mark?.MarksObtained ?? 0,
                    mark?.IsAbsent == true,
                    mark?.IsAbsent != true && mark is not null && mark.MarksObtained >= schedule.PassingMarks,
                    mark?.Grade,
                    mark?.Remarks);
            }).ToList();

        var rule = (await _rankingRules.ListAsync(r => r.ExamId == result.ExamId && r.SectionId == result.SectionId)).FirstOrDefault()
            ?? (await _rankingRules.ListAsync(r => r.ExamId == result.ExamId && r.SectionId == null)).FirstOrDefault();
        ShowRank = rule?.ShowRankOnReportCard ?? true;
        return Page();
    }

    public sealed record MarkSheetRow(string Subject, decimal MaximumMarks, decimal PassingMarks, decimal MarksObtained, bool IsAbsent, bool IsPassed, string? Grade, string? Remarks);
}
