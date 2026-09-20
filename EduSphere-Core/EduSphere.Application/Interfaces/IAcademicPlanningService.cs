namespace EduSphere.Application.Interfaces;

public sealed record TimetableGenerationRequest(Guid BranchId, Guid AcademicYearId, Guid SectionId, Guid TemplateId, string Name, DateOnly EffectiveFrom, DateOnly? EffectiveTo);
public sealed record TimetableGenerationResult(bool Succeeded, Guid RunId, Guid? TimetableId, int ScheduledPeriods, int UnscheduledPeriods, IReadOnlyList<string> Diagnostics);
public sealed record AcademicRolloverRequest(Guid SourceAcademicYearId, Guid TargetAcademicYearId, bool CopyCalendar, bool CopyTemplates);
public sealed record AcademicRolloverResult(int TermsCopied, int CalendarEventsCopied, int TemplatesCopied);

public interface IAcademicPlanningService
{
    Task<TimetableGenerationResult> GenerateTimetableAsync(TimetableGenerationRequest request, CancellationToken cancellationToken = default);
    Task<AcademicRolloverResult> RolloverAsync(AcademicRolloverRequest request, CancellationToken cancellationToken = default);
}
