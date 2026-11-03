using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ReportDefinition : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    [Required, StringLength(80)] public string Code { get; set; } = null!;
    [Required, StringLength(180)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    public ReportDataSource DataSource { get; set; }
    public string SelectedFieldsJson { get; set; } = "[]";
    public string FiltersJson { get; set; } = "{}";
    public string GroupingJson { get; set; } = "[]";
    public string SortJson { get; set; } = "[]";
    public bool IsSystem { get; set; }
    public bool IsPublished { get; set; } = true;
    public ICollection<ReportSchedule> Schedules { get; set; } = [];
}

public class ReportExecution : TenantEntityBase
{
    public Guid ReportDefinitionId { get; set; }
    public ReportDefinition? ReportDefinition { get; set; }
    public Guid? BranchId { get; set; }
    public ReportOutputFormat Format { get; set; }
    public ReportExecutionStatus Status { get; set; } = ReportExecutionStatus.Queued;
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public DateTime? StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public int RowCount { get; set; }
    [StringLength(500)] public string? FileName { get; set; }
    [StringLength(120)] public string? ContentType { get; set; }
    public byte[]? OutputContent { get; set; }
    [StringLength(2000)] public string? ErrorMessage { get; set; }
}

public class ReportSchedule : TenantEntityBase
{
    public Guid ReportDefinitionId { get; set; }
    public ReportDefinition? ReportDefinition { get; set; }
    public Guid? BranchId { get; set; }
    public ReportScheduleFrequency Frequency { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public TimeOnly DeliveryTime { get; set; } = new(7, 0);
    [Required, StringLength(100)] public string TimeZoneId { get; set; } = "Asia/Kolkata";
    public ReportOutputFormat Format { get; set; } = ReportOutputFormat.Excel;
    [Required, StringLength(2000)] public string Recipients { get; set; } = null!;
    public DateTime NextRunOn { get; set; }
    public DateTime? LastRunOn { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AnalyticsSnapshot : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public DateOnly SnapshotDate { get; set; }
    [Required, StringLength(120)] public string MetricKey { get; set; } = null!;
    public decimal MetricValue { get; set; }
    public string DimensionsJson { get; set; } = "{}";
}

public class DashboardWidgetConfiguration : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    [Required, StringLength(80)] public string RoleName { get; set; } = null!;
    [Required, StringLength(100)] public string WidgetKey { get; set; } = null!;
    [Required, StringLength(150)] public string DisplayName { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public DashboardWidgetSize Size { get; set; } = DashboardWidgetSize.Small;
    public bool IsVisible { get; set; } = true;
    public string SettingsJson { get; set; } = "{}";
}

public class BiDashboard : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    [Required, StringLength(160)] public string Name { get; set; } = null!;
    public BiProvider Provider { get; set; }
    [Required, StringLength(1000)] public string EmbedUrl { get; set; } = null!;
    [StringLength(160)] public string? ExternalReportId { get; set; }
    public Guid? TenantIntegrationSettingId { get; set; }
    public TenantIntegrationSetting? TenantIntegrationSetting { get; set; }
    [StringLength(1000)] public string? AllowedRolesCsv { get; set; }
    public bool IsActive { get; set; } = true;
}
