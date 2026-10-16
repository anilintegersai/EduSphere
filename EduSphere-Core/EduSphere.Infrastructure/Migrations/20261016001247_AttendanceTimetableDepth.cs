using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceTimetableDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceAlertSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ThresholdPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LookbackDays = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NextRunOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRunOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RunEveryHours = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceAlertSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceAlertSchedules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceCondonationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EvidenceFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EvidenceStoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EvidenceContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_AttendanceCondonationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceCondonationRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCondonationRequests_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCondonationRequests_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DeviceIdentifier = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    EndpointUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApiKeyProtected = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastSynchronizedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDevices_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceLeaveReconciliations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    AppliedStatus = table.Column<int>(type: "int", nullable: false),
                    ReconciledOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReconciledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceLeaveReconciliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceLeaveReconciliations_AspNetUsers_ReconciledByUserId",
                        column: x => x.ReconciledByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceLeaveReconciliations_AttendanceRecords_AttendanceRecordId",
                        column: x => x.AttendanceRecordId,
                        principalTable: "AttendanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceLeaveReconciliations_LeaveApplications_LeaveApplicationId",
                        column: x => x.LeaveApplicationId,
                        principalTable: "LeaveApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransportRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransportRouteStopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_BusAttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_TransportRouteStops_TransportRouteStopId",
                        column: x => x.TransportRouteStopId,
                        principalTable: "TransportRouteStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_TransportRoutes_TransportRouteId",
                        column: x => x.TransportRouteId,
                        principalTable: "TransportRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusAttendanceRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentAbsenceExplanations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ParentAbsenceExplanations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentAbsenceExplanations_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentAbsenceExplanations_AspNetUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentAbsenceExplanations_AttendanceRecords_AttendanceRecordId",
                        column: x => x.AttendanceRecordId,
                        principalTable: "AttendanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentAbsenceExplanations_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckIn = table.Column<TimeOnly>(type: "time", nullable: true),
                    CheckOut = table.Column<TimeOnly>(type: "time", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TeacherAttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAttendanceRecords_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherAttendanceRecords_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDeviceImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ImportedRows = table.Column<int>(type: "int", nullable: false),
                    RejectedRows = table.Column<int>(type: "int", nullable: false),
                    ErrorSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceDeviceImportBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDeviceImportBatches_AttendanceDevices_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDevicePunches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDeviceImportBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalRecordId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    PersonCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PunchedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StaffProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_AttendanceDevicePunches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDevicePunches_AttendanceDeviceImportBatches_AttendanceDeviceImportBatchId",
                        column: x => x.AttendanceDeviceImportBatchId,
                        principalTable: "AttendanceDeviceImportBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceDevicePunches_AttendanceDevices_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceDevicePunches_StaffProfiles_StaffProfileId",
                        column: x => x.StaffProfileId,
                        principalTable: "StaffProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceDevicePunches_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceDevicePunches_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceAlertSchedules_BranchId",
                table: "AttendanceAlertSchedules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceAlertSchedules_IsActive_NextRunOn",
                table: "AttendanceAlertSchedules",
                columns: new[] { "IsActive", "NextRunOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCondonationRequests_RequestedByUserId",
                table: "AttendanceCondonationRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCondonationRequests_ReviewedByUserId",
                table: "AttendanceCondonationRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCondonationRequests_StudentProfileId",
                table: "AttendanceCondonationRequests",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCondonationRequests_TenantId_StudentProfileId_Status",
                table: "AttendanceCondonationRequests",
                columns: new[] { "TenantId", "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDeviceImportBatches_AttendanceDeviceId",
                table: "AttendanceDeviceImportBatches",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDeviceImportBatches_TenantId_AttendanceDeviceId_UploadedOn",
                table: "AttendanceDeviceImportBatches",
                columns: new[] { "TenantId", "AttendanceDeviceId", "UploadedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_AttendanceDeviceId",
                table: "AttendanceDevicePunches",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_AttendanceDeviceImportBatchId",
                table: "AttendanceDevicePunches",
                column: "AttendanceDeviceImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_StaffProfileId",
                table: "AttendanceDevicePunches",
                column: "StaffProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_StudentProfileId",
                table: "AttendanceDevicePunches",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_TeacherProfileId",
                table: "AttendanceDevicePunches",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_TenantId_AttendanceDeviceId_ExternalRecordId",
                table: "AttendanceDevicePunches",
                columns: new[] { "TenantId", "AttendanceDeviceId", "ExternalRecordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevicePunches_TenantId_PersonCode_PunchedOn",
                table: "AttendanceDevicePunches",
                columns: new[] { "TenantId", "PersonCode", "PunchedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevices_BranchId",
                table: "AttendanceDevices",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevices_TenantId_DeviceIdentifier",
                table: "AttendanceDevices",
                columns: new[] { "TenantId", "DeviceIdentifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeaveReconciliations_AttendanceRecordId",
                table: "AttendanceLeaveReconciliations",
                column: "AttendanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeaveReconciliations_LeaveApplicationId",
                table: "AttendanceLeaveReconciliations",
                column: "LeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeaveReconciliations_ReconciledByUserId",
                table: "AttendanceLeaveReconciliations",
                column: "ReconciledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeaveReconciliations_TenantId_LeaveApplicationId_AttendanceRecordId",
                table: "AttendanceLeaveReconciliations",
                columns: new[] { "TenantId", "LeaveApplicationId", "AttendanceRecordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_BranchId",
                table: "BusAttendanceRecords",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_RecordedByUserId",
                table: "BusAttendanceRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_StudentProfileId",
                table: "BusAttendanceRecords",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_TenantId_StudentProfileId_AttendanceDate_Status",
                table: "BusAttendanceRecords",
                columns: new[] { "TenantId", "StudentProfileId", "AttendanceDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_TransportRouteId",
                table: "BusAttendanceRecords",
                column: "TransportRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_TransportRouteStopId",
                table: "BusAttendanceRecords",
                column: "TransportRouteStopId");

            migrationBuilder.CreateIndex(
                name: "IX_BusAttendanceRecords_VehicleId",
                table: "BusAttendanceRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentAbsenceExplanations_AttendanceRecordId",
                table: "ParentAbsenceExplanations",
                column: "AttendanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentAbsenceExplanations_ReviewedByUserId",
                table: "ParentAbsenceExplanations",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentAbsenceExplanations_StudentProfileId",
                table: "ParentAbsenceExplanations",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentAbsenceExplanations_SubmittedByUserId",
                table: "ParentAbsenceExplanations",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentAbsenceExplanations_TenantId_StudentProfileId_Status",
                table: "ParentAbsenceExplanations",
                columns: new[] { "TenantId", "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendanceRecords_BranchId",
                table: "TeacherAttendanceRecords",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendanceRecords_TeacherProfileId",
                table: "TeacherAttendanceRecords",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendanceRecords_TenantId_TeacherProfileId_AttendanceDate",
                table: "TeacherAttendanceRecords",
                columns: new[] { "TenantId", "TeacherProfileId", "AttendanceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceAlertSchedules");

            migrationBuilder.DropTable(
                name: "AttendanceCondonationRequests");

            migrationBuilder.DropTable(
                name: "AttendanceDevicePunches");

            migrationBuilder.DropTable(
                name: "AttendanceLeaveReconciliations");

            migrationBuilder.DropTable(
                name: "BusAttendanceRecords");

            migrationBuilder.DropTable(
                name: "ParentAbsenceExplanations");

            migrationBuilder.DropTable(
                name: "TeacherAttendanceRecords");

            migrationBuilder.DropTable(
                name: "AttendanceDeviceImportBatches");

            migrationBuilder.DropTable(
                name: "AttendanceDevices");
        }
    }
}
