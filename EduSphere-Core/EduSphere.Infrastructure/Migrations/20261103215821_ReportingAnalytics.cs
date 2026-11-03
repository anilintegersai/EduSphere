using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportingAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyticsSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MetricKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DimensionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiDashboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    EmbedUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExternalReportId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    TenantIntegrationSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowedRolesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiDashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiDashboards_TenantIntegrationSettings_TenantIntegrationSettingId",
                        column: x => x.TenantIntegrationSettingId,
                        principalTable: "TenantIntegrationSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardWidgetConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    WidgetKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardWidgetConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataSource = table.Column<int>(type: "int", nullable: false),
                    SelectedFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiltersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupingJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Format = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    OutputContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportExecutions_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    DeliveryTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    Recipients = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NextRunOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRunOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportSchedules_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsSnapshots_TenantId_BranchId_SnapshotDate_MetricKey",
                table: "AnalyticsSnapshots",
                columns: new[] { "TenantId", "BranchId", "SnapshotDate", "MetricKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BiDashboards_TenantId_BranchId_Name",
                table: "BiDashboards",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BiDashboards_TenantIntegrationSettingId",
                table: "BiDashboards",
                column: "TenantIntegrationSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidgetConfigurations_TenantId_BranchId_RoleName_WidgetKey",
                table: "DashboardWidgetConfigurations",
                columns: new[] { "TenantId", "BranchId", "RoleName", "WidgetKey" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitions_TenantId_Code",
                table: "ReportDefinitions",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutions_ReportDefinitionId",
                table: "ReportExecutions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutions_TenantId_Status_RequestedOn",
                table: "ReportExecutions",
                columns: new[] { "TenantId", "Status", "RequestedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportSchedules_ReportDefinitionId",
                table: "ReportSchedules",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSchedules_TenantId_IsActive_NextRunOn",
                table: "ReportSchedules",
                columns: new[] { "TenantId", "IsActive", "NextRunOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsSnapshots");

            migrationBuilder.DropTable(
                name: "BiDashboards");

            migrationBuilder.DropTable(
                name: "DashboardWidgetConfigurations");

            migrationBuilder.DropTable(
                name: "ReportExecutions");

            migrationBuilder.DropTable(
                name: "ReportSchedules");

            migrationBuilder.DropTable(
                name: "ReportDefinitions");
        }
    }
}
