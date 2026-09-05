using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Examinations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Examinations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/results")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class ResultsController : ApiControllerBase
{
    private readonly ICrudService<Result> _results;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly ICrudService<MarkEntry> _marks;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<GradingScheme> _schemes;
    private readonly ICrudService<GradingSchemeBand> _bands;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchAccessService _branchAccess;

    public ResultsController(
        ICrudService<Result> results,
        ICrudService<Exam> exams,
        ICrudService<ExamSchedule> schedules,
        ICrudService<MarkEntry> marks,
        ICrudService<StudentProfile> students,
        ICrudService<Section> sections,
        ICrudService<Batch> batches,
        ICrudService<GradingScheme> schemes,
        ICrudService<GradingSchemeBand> bands,
        UserManager<ApplicationUser> userManager,
        IBranchAccessService branchAccess)
    {
        _results = results;
        _exams = exams;
        _schedules = schedules;
        _marks = marks;
        _students = students;
        _sections = sections;
        _batches = batches;
        _schemes = schemes;
        _bands = bands;
        _userManager = userManager;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? examId,
        [FromQuery] Guid? studentProfileId,
        [FromQuery] ResultStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _results.ListAsync(r =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && r.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || r.BranchId == branchId.Value) &&
            (!examId.HasValue || r.ExamId == examId.Value) &&
            (!studentProfileId.HasValue || r.StudentProfileId == studentProfileId.Value) &&
            (!status.HasValue || r.Status == status.Value));

        return Ok(ApiResponse<IEnumerable<ResultDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _results.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<ResultDto>.Fail($"Result {id} was not found."))
            : Ok(ApiResponse<ResultDto>.Ok(Map(entity)));
    }

    [HttpPost("compute")]
    public async Task<IActionResult> Compute([FromBody] ComputeResultRequest request)
    {
        var computed = await ComputeResultAsync(request);
        if (computed.Error is not null)
            return BadRequest(ApiResponse<object>.Fail(computed.Error));
        if (computed.Forbidden)
            return Forbid();

        return Ok(ApiResponse<ResultDto>.Ok(Map(computed.Result!)));
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, [FromBody] PublishResultRequest request)
    {
        var existing = await _results.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Result {id} was not found."));

        var currentUserId = CurrentUserId();
        await _results.UpdateAsync(id, result =>
        {
            result.Status = ResultStatus.Published;
            result.PublishedByUserId = currentUserId;
            result.PublishedOn = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Remarks))
                result.Remarks = request.Remarks;
        });

        var published = await _results.GetAsync(id);
        return Ok(ApiResponse<ResultDto>.Ok(Map(published!)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _results.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Result {id} was not found."));

        return await _results.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Result {id} was not found."));
    }

    private async Task<ComputeResultOutcome> ComputeResultAsync(ComputeResultRequest request)
    {
        var exam = await _exams.GetAsync(request.ExamId);
        if (exam is null)
            return ComputeResultOutcome.Failed($"Exam {request.ExamId} was not found in this tenant.");
        if (!await _branchAccess.CanAccessBranchAsync(User, exam.BranchId))
            return ComputeResultOutcome.AccessDenied();

        var student = await _students.GetAsync(request.StudentProfileId);
        if (student is null)
            return ComputeResultOutcome.Failed($"Student {request.StudentProfileId} was not found in this tenant.");
        if (student.BranchId != exam.BranchId)
            return ComputeResultOutcome.Failed("Student must belong to the exam branch.");

        Section? section = null;
        Batch? batch = null;
        if (student.SectionId.HasValue)
        {
            section = await _sections.GetAsync(student.SectionId.Value);
            if (section is not null)
                batch = await _batches.GetAsync(section.BatchId);
        }

        var schedules = await _schedules.ListAsync(s =>
            s.ExamId == exam.Id &&
            (!student.SectionId.HasValue || s.SectionId == student.SectionId.Value));
        if (schedules.Count == 0)
            return ComputeResultOutcome.Failed("No exam schedules were found for this student.");

        var scheduleIds = schedules.Select(s => s.Id).ToHashSet();
        var markEntries = await _marks.ListAsync(m =>
            m.StudentProfileId == student.Id &&
            scheduleIds.Contains(m.ExamScheduleId));
        if (markEntries.Count == 0)
            return ComputeResultOutcome.Failed("No marks have been entered for this student and exam.");

        var totalMarks = schedules.Sum(s => s.MaximumMarks);
        var marksBySchedule = markEntries
            .GroupBy(m => m.ExamScheduleId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(m => m.EnteredOn).First());
        var obtained = schedules.Sum(s =>
            marksBySchedule.TryGetValue(s.Id, out var mark) && !mark.IsAbsent
                ? mark.MarksObtained
                : 0);
        var percentage = totalMarks <= 0 ? 0 : Math.Round(obtained * 100 / totalMarks, 2);
        var grade = await ResolveGradeAsync(request.GradingSchemeId, exam.BranchId, batch?.CourseId, percentage);
        var currentUserId = CurrentUserId();
        var status = request.Publish ? ResultStatus.Published : ResultStatus.Computed;

        var existing = (await _results.ListAsync(r => r.ExamId == exam.Id && r.StudentProfileId == student.Id))
            .FirstOrDefault();
        if (existing is null)
        {
            existing = await _results.CreateAsync(new Result
            {
                ExamId = exam.Id,
                StudentProfileId = student.Id,
                BranchId = exam.BranchId,
                AcademicYearId = exam.AcademicYearId,
                CourseId = batch?.CourseId,
                BatchId = batch?.Id,
                SectionId = student.SectionId,
                TotalMarks = totalMarks,
                MarksObtained = obtained,
                Percentage = percentage,
                Grade = grade.Grade,
                GradePoint = grade.GradePoint,
                Status = status,
                ComputedOn = DateTime.UtcNow,
                PublishedByUserId = request.Publish ? currentUserId : null,
                PublishedOn = request.Publish ? DateTime.UtcNow : null,
                Remarks = request.Remarks
            });
        }
        else
        {
            await _results.UpdateAsync(existing.Id, result =>
            {
                result.BranchId = exam.BranchId;
                result.AcademicYearId = exam.AcademicYearId;
                result.CourseId = batch?.CourseId;
                result.BatchId = batch?.Id;
                result.SectionId = student.SectionId;
                result.TotalMarks = totalMarks;
                result.MarksObtained = obtained;
                result.Percentage = percentage;
                result.Grade = grade.Grade;
                result.GradePoint = grade.GradePoint;
                result.Status = status;
                result.ComputedOn = DateTime.UtcNow;
                result.PublishedByUserId = request.Publish ? currentUserId : result.PublishedByUserId;
                result.PublishedOn = request.Publish ? DateTime.UtcNow : result.PublishedOn;
                result.Remarks = request.Remarks;
            });
            existing = await _results.GetAsync(existing.Id);
        }

        return ComputeResultOutcome.Success(existing!);
    }

    private async Task<(string Grade, decimal? GradePoint)> ResolveGradeAsync(
        Guid? requestedSchemeId,
        Guid branchId,
        Guid? courseId,
        decimal percentage)
    {
        GradingScheme? scheme = null;
        if (requestedSchemeId.HasValue)
        {
            scheme = await _schemes.GetAsync(requestedSchemeId.Value);
            if (scheme is not null &&
                scheme.BranchId.HasValue &&
                !await _branchAccess.CanAccessBranchAsync(User, scheme.BranchId.Value))
            {
                scheme = null;
            }
        }

        if (scheme is null)
        {
            var schemes = await _schemes.ListAsync(s =>
                s.IsDefault &&
                (s.BranchId == branchId || s.BranchId == null) &&
                (!courseId.HasValue || s.CourseId == courseId.Value || s.CourseId == null));
            scheme = schemes
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

    private Guid? CurrentUserId()
        => Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;

    private static ResultDto Map(Result e) => new ResultDto
    {
        ExamId = e.ExamId,
        StudentProfileId = e.StudentProfileId,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        SectionId = e.SectionId,
        TotalMarks = e.TotalMarks,
        MarksObtained = e.MarksObtained,
        Percentage = e.Percentage,
        Grade = e.Grade,
        GradePoint = e.GradePoint,
        Status = e.Status,
        ComputedOn = e.ComputedOn,
        PublishedByUserId = e.PublishedByUserId,
        PublishedOn = e.PublishedOn,
        Remarks = e.Remarks
    }.WithMetadata(e);

    private sealed record ComputeResultOutcome(Result? Result, string? Error, bool Forbidden)
    {
        public static ComputeResultOutcome Success(Result result) => new(result, null, false);
        public static ComputeResultOutcome Failed(string error) => new(null, error, false);
        public static ComputeResultOutcome AccessDenied() => new(null, null, true);
    }
}
