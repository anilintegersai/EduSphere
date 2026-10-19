using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HostelOperationsDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "HostelVisitorLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CheckedInByUserId",
                table: "HostelVisitorLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CheckedOutByUserId",
                table: "HostelVisitorLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedVisitOn",
                table: "HostelVisitorLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "HostelVisitorLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId",
                table: "HostelMaintenanceRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalationLevel",
                table: "HostelMaintenanceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstResponseOn",
                table: "HostelMaintenanceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlaMinutes",
                table: "HostelMaintenanceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FeeInvoiceId",
                table: "HostelFees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HostelClearanceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VacationFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedReturnOn = table.Column<DateOnly>(type: "date", nullable: true),
                    IsFinalCheckout = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HostelClearanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelClearanceRequests_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelGatePasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PlannedOutOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedReturnOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualOutOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualReturnOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_HostelGatePasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelGatePasses_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelMaintenanceEscalations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelMaintenanceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    EscalatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EscalatedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HostelMaintenanceEscalations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceEscalations_HostelMaintenanceRequests_HostelMaintenanceRequestId",
                        column: x => x.HostelMaintenanceRequestId,
                        principalTable: "HostelMaintenanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelMessAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MealType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_HostelMessAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelMessAttendances_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelMessBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    ChargeableMeals = table.Column<int>(type: "int", nullable: false),
                    RatePerMeal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_HostelMessBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelMessBills_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelMessBills_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelMessMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MenuDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MealType = table.Column<int>(type: "int", nullable: false),
                    MenuItems = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsVegetarian = table.Column<bool>(type: "bit", nullable: false),
                    AllergenNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GuestPrice = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_HostelMessMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostelRoomInspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InspectorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CleanlinessScore = table.Column<int>(type: "int", nullable: false),
                    SafetyScore = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
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
                    table.PrimaryKey("PK_HostelRoomInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelRoomInspections_HostelRooms_HostelRoomId",
                        column: x => x.HostelRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelClearanceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelClearanceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Area = table.Column<int>(type: "int", nullable: false),
                    IsCleared = table.Column<bool>(type: "bit", nullable: false),
                    ClearedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClearedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HostelClearanceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelClearanceItems_HostelClearanceRequests_HostelClearanceRequestId",
                        column: x => x.HostelClearanceRequestId,
                        principalTable: "HostelClearanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelDamageAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelRoomInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ChargedAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    StudentLiable = table.Column<bool>(type: "bit", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_HostelDamageAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelDamageAssessments_HostelRoomInspections_HostelRoomInspectionId",
                        column: x => x.HostelRoomInspectionId,
                        principalTable: "HostelRoomInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), "f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1", "Warden platform role", "Warden", "WARDEN" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelFees_FeeInvoiceId",
                table: "HostelFees",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelClearanceItems_HostelClearanceRequestId",
                table: "HostelClearanceItems",
                column: "HostelClearanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelClearanceItems_TenantId_HostelClearanceRequestId_Area",
                table: "HostelClearanceItems",
                columns: new[] { "TenantId", "HostelClearanceRequestId", "Area" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostelClearanceRequests_HostelAllocationId",
                table: "HostelClearanceRequests",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelClearanceRequests_TenantId_BranchId_Status",
                table: "HostelClearanceRequests",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelDamageAssessments_HostelRoomInspectionId",
                table: "HostelDamageAssessments",
                column: "HostelRoomInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelGatePasses_HostelAllocationId",
                table: "HostelGatePasses",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelGatePasses_TenantId_BranchId_Status_PlannedReturnOn",
                table: "HostelGatePasses",
                columns: new[] { "TenantId", "BranchId", "Status", "PlannedReturnOn" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceEscalations_HostelMaintenanceRequestId",
                table: "HostelMaintenanceEscalations",
                column: "HostelMaintenanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessAttendances_HostelAllocationId",
                table: "HostelMessAttendances",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessAttendances_TenantId_HostelAllocationId_AttendanceDate_MealType",
                table: "HostelMessAttendances",
                columns: new[] { "TenantId", "HostelAllocationId", "AttendanceDate", "MealType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessBills_FeeInvoiceId",
                table: "HostelMessBills",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessBills_HostelAllocationId",
                table: "HostelMessBills",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessBills_TenantId_HostelAllocationId_PeriodStart_PeriodEnd",
                table: "HostelMessBills",
                columns: new[] { "TenantId", "HostelAllocationId", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostelMessMenus_TenantId_BranchId_HostelBlockId_MenuDate_MealType",
                table: "HostelMessMenus",
                columns: new[] { "TenantId", "BranchId", "HostelBlockId", "MenuDate", "MealType" },
                unique: true,
                filter: "[HostelBlockId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomInspections_HostelRoomId",
                table: "HostelRoomInspections",
                column: "HostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomInspections_TenantId_BranchId_Status_ScheduledOn",
                table: "HostelRoomInspections",
                columns: new[] { "TenantId", "BranchId", "Status", "ScheduledOn" });

            migrationBuilder.AddForeignKey(
                name: "FK_HostelFees_FeeInvoices_FeeInvoiceId",
                table: "HostelFees",
                column: "FeeInvoiceId",
                principalTable: "FeeInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HostelFees_FeeInvoices_FeeInvoiceId",
                table: "HostelFees");

            migrationBuilder.DropTable(
                name: "HostelClearanceItems");

            migrationBuilder.DropTable(
                name: "HostelDamageAssessments");

            migrationBuilder.DropTable(
                name: "HostelGatePasses");

            migrationBuilder.DropTable(
                name: "HostelMaintenanceEscalations");

            migrationBuilder.DropTable(
                name: "HostelMessAttendances");

            migrationBuilder.DropTable(
                name: "HostelMessBills");

            migrationBuilder.DropTable(
                name: "HostelMessMenus");

            migrationBuilder.DropTable(
                name: "HostelClearanceRequests");

            migrationBuilder.DropTable(
                name: "HostelRoomInspections");

            migrationBuilder.DropIndex(
                name: "IX_HostelFees_FeeInvoiceId",
                table: "HostelFees");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"));

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "HostelVisitorLogs");

            migrationBuilder.DropColumn(
                name: "CheckedInByUserId",
                table: "HostelVisitorLogs");

            migrationBuilder.DropColumn(
                name: "CheckedOutByUserId",
                table: "HostelVisitorLogs");

            migrationBuilder.DropColumn(
                name: "RequestedVisitOn",
                table: "HostelVisitorLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HostelVisitorLogs");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "HostelMaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "EscalationLevel",
                table: "HostelMaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "FirstResponseOn",
                table: "HostelMaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "SlaMinutes",
                table: "HostelMaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "FeeInvoiceId",
                table: "HostelFees");
        }
    }
}
