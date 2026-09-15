using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseWorkflowEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeeConcessionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeeStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    RequestedValue = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_FeeConcessionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_AspNetUsers_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeConcessionRequests_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeRefunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeePaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GatewayRefundId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_FeeRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeRefunds_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeRefunds_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeRefunds_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeRefunds_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeRefunds_FeePayments_FeePaymentId",
                        column: x => x.FeePaymentId,
                        principalTable: "FeePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReminderOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_FeeReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeReminders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeReminders_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeReminders_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelAllocationTransferRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromBedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToBedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_HostelAllocationTransferRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_HostelBeds_FromBedId",
                        column: x => x.FromBedId,
                        principalTable: "HostelBeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_HostelBeds_ToBedId",
                        column: x => x.ToBedId,
                        principalTable: "HostelBeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_HostelRooms_FromRoomId",
                        column: x => x.FromRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelAllocationTransferRequests_HostelRooms_ToRoomId",
                        column: x => x.ToRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelMaintenanceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HostelRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HostelBedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReportedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_HostelMaintenanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceRequests_AspNetUsers_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceRequests_HostelBeds_HostelBedId",
                        column: x => x.HostelBedId,
                        principalTable: "HostelBeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceRequests_HostelBlocks_HostelBlockId",
                        column: x => x.HostelBlockId,
                        principalTable: "HostelBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelMaintenanceRequests_HostelRooms_HostelRoomId",
                        column: x => x.HostelRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostelVisitorLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostelAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VisitorName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CheckInOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdDocumentNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
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
                    table.PrimaryKey("PK_HostelVisitorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelVisitorLogs_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelVisitorLogs_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelVisitorLogs_HostelAllocations_HostelAllocationId",
                        column: x => x.HostelAllocationId,
                        principalTable: "HostelAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostelVisitorLogs_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LedgerExportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExportType = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LedgerExportBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerExportBatches_AspNetUsers_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerExportBatches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryBarcodeScans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScanCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    LibraryBookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScannedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LibraryBarcodeScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryBarcodeScans_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryBarcodeScans_LibraryBookCopies_LibraryBookCopyId",
                        column: x => x.LibraryBookCopyId,
                        principalTable: "LibraryBookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryOverdueNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NotifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LibraryOverdueNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryOverdueNotifications_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryOverdueNotifications_LibraryBookIssues_LibraryBookIssueId",
                        column: x => x.LibraryBookIssueId,
                        principalTable: "LibraryBookIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryOverdueNotifications_LibraryMembers_LibraryMemberId",
                        column: x => x.LibraryMemberId,
                        principalTable: "LibraryMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryRenewals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RenewedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NewDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RenewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_LibraryRenewals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryRenewals_AspNetUsers_RenewedByUserId",
                        column: x => x.RenewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryRenewals_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryRenewals_LibraryBookIssues_LibraryBookIssueId",
                        column: x => x.LibraryBookIssueId,
                        principalTable: "LibraryBookIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LibraryMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FulfilledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LibraryReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryReservations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryReservations_LibraryBookCopies_LibraryBookCopyId",
                        column: x => x.LibraryBookCopyId,
                        principalTable: "LibraryBookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryReservations_LibraryBooks_LibraryBookId",
                        column: x => x.LibraryBookId,
                        principalTable: "LibraryBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryReservations_LibraryMembers_LibraryMemberId",
                        column: x => x.LibraryMemberId,
                        principalTable: "LibraryMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveryAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotificationMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationRecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextRetryOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProviderResponseJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_NotificationDeliveryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveryAttempts_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveryAttempts_NotificationMessages_NotificationMessageId",
                        column: x => x.NotificationMessageId,
                        principalTable: "NotificationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveryAttempts_NotificationRecipients_NotificationRecipientId",
                        column: x => x.NotificationRecipientId,
                        principalTable: "NotificationRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnlinePaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatewayProviderKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    GatewayOrderId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    GatewayPaymentId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InitiatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallbackPayloadJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_OnlinePaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentTransactions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentTransactions_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportDocumentReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReminderType = table.Column<int>(type: "int", nullable: false),
                    DueOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReminderSentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_TransportDocumentReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportDocumentReminders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportDocumentReminders_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportMaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DueOn = table.Column<DateOnly>(type: "date", nullable: false),
                    CompletedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    OdometerReading = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReminderSentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_TransportMaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportMaintenanceRecords_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportMaintenanceRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleGpsPings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransportRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    SpeedKmph = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    HeadingDegrees = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    RawPayloadJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_VehicleGpsPings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleGpsPings_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleGpsPings_TransportRoutes_TransportRouteId",
                        column: x => x.TransportRouteId,
                        principalTable: "TransportRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleGpsPings_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBookCopies_TenantId_BranchId_Barcode",
                table: "LibraryBookCopies",
                columns: new[] { "TenantId", "BranchId", "Barcode" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_BranchId",
                table: "FeeConcessionRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_DecidedByUserId",
                table: "FeeConcessionRequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_FeeInvoiceId",
                table: "FeeConcessionRequests",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_FeeStructureId",
                table: "FeeConcessionRequests",
                column: "FeeStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_RequestedByUserId",
                table: "FeeConcessionRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_StudentProfileId",
                table: "FeeConcessionRequests",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessionRequests_TenantId_BranchId_Status",
                table: "FeeConcessionRequests",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_ApprovedByUserId",
                table: "FeeRefunds",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_BranchId",
                table: "FeeRefunds",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_FeeInvoiceId",
                table: "FeeRefunds",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_FeePaymentId",
                table: "FeeRefunds",
                column: "FeePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_RequestedByUserId",
                table: "FeeRefunds",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRefunds_TenantId_BranchId_Status",
                table: "FeeRefunds",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FeeReminders_BranchId",
                table: "FeeReminders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReminders_FeeInvoiceId",
                table: "FeeReminders",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReminders_StudentProfileId",
                table: "FeeReminders",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeReminders_TenantId_BranchId_Status_ReminderOn",
                table: "FeeReminders",
                columns: new[] { "TenantId", "BranchId", "Status", "ReminderOn" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_ApprovedByUserId",
                table: "HostelAllocationTransferRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_BranchId",
                table: "HostelAllocationTransferRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_FromBedId",
                table: "HostelAllocationTransferRequests",
                column: "FromBedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_FromRoomId",
                table: "HostelAllocationTransferRequests",
                column: "FromRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_HostelAllocationId",
                table: "HostelAllocationTransferRequests",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_RequestedByUserId",
                table: "HostelAllocationTransferRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_TenantId_BranchId_Status",
                table: "HostelAllocationTransferRequests",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_ToBedId",
                table: "HostelAllocationTransferRequests",
                column: "ToBedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocationTransferRequests_ToRoomId",
                table: "HostelAllocationTransferRequests",
                column: "ToRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_BranchId",
                table: "HostelMaintenanceRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_HostelBedId",
                table: "HostelMaintenanceRequests",
                column: "HostelBedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_HostelBlockId",
                table: "HostelMaintenanceRequests",
                column: "HostelBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_HostelRoomId",
                table: "HostelMaintenanceRequests",
                column: "HostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_ReportedByUserId",
                table: "HostelMaintenanceRequests",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelMaintenanceRequests_TenantId_BranchId_Status_Priority",
                table: "HostelMaintenanceRequests",
                columns: new[] { "TenantId", "BranchId", "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_HostelVisitorLogs_ApprovedByUserId",
                table: "HostelVisitorLogs",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelVisitorLogs_BranchId",
                table: "HostelVisitorLogs",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelVisitorLogs_HostelAllocationId",
                table: "HostelVisitorLogs",
                column: "HostelAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelVisitorLogs_StudentProfileId",
                table: "HostelVisitorLogs",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelVisitorLogs_TenantId_BranchId_CheckInOn",
                table: "HostelVisitorLogs",
                columns: new[] { "TenantId", "BranchId", "CheckInOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerExportBatches_BranchId",
                table: "LedgerExportBatches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerExportBatches_GeneratedByUserId",
                table: "LedgerExportBatches",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerExportBatches_TenantId_BranchId_ExportType_RequestedOn",
                table: "LedgerExportBatches",
                columns: new[] { "TenantId", "BranchId", "ExportType", "RequestedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBarcodeScans_BranchId",
                table: "LibraryBarcodeScans",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBarcodeScans_LibraryBookCopyId",
                table: "LibraryBarcodeScans",
                column: "LibraryBookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBarcodeScans_TenantId_BranchId_ScanCode_ScannedOn",
                table: "LibraryBarcodeScans",
                columns: new[] { "TenantId", "BranchId", "ScanCode", "ScannedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryOverdueNotifications_BranchId",
                table: "LibraryOverdueNotifications",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryOverdueNotifications_LibraryBookIssueId",
                table: "LibraryOverdueNotifications",
                column: "LibraryBookIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryOverdueNotifications_LibraryMemberId",
                table: "LibraryOverdueNotifications",
                column: "LibraryMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryOverdueNotifications_TenantId_BranchId_Status",
                table: "LibraryOverdueNotifications",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRenewals_BranchId",
                table: "LibraryRenewals",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRenewals_LibraryBookIssueId",
                table: "LibraryRenewals",
                column: "LibraryBookIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRenewals_RenewedByUserId",
                table: "LibraryRenewals",
                column: "RenewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRenewals_TenantId_LibraryBookIssueId_RenewedOn",
                table: "LibraryRenewals",
                columns: new[] { "TenantId", "LibraryBookIssueId", "RenewedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryReservations_BranchId",
                table: "LibraryReservations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryReservations_LibraryBookCopyId",
                table: "LibraryReservations",
                column: "LibraryBookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryReservations_LibraryBookId",
                table: "LibraryReservations",
                column: "LibraryBookId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryReservations_LibraryMemberId",
                table: "LibraryReservations",
                column: "LibraryMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryReservations_TenantId_BranchId_Status_ExpiresOn",
                table: "LibraryReservations",
                columns: new[] { "TenantId", "BranchId", "Status", "ExpiresOn" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_BranchId",
                table: "NotificationDeliveryAttempts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_NotificationMessageId",
                table: "NotificationDeliveryAttempts",
                column: "NotificationMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_NotificationRecipientId",
                table: "NotificationDeliveryAttempts",
                column: "NotificationRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_TenantId_BranchId_Status_NextRetryOn",
                table: "NotificationDeliveryAttempts",
                columns: new[] { "TenantId", "BranchId", "Status", "NextRetryOn" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_TenantId_NotificationMessageId_NotificationRecipientId_AttemptNumber",
                table: "NotificationDeliveryAttempts",
                columns: new[] { "TenantId", "NotificationMessageId", "NotificationRecipientId", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentTransactions_BranchId",
                table: "OnlinePaymentTransactions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentTransactions_FeeInvoiceId",
                table: "OnlinePaymentTransactions",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentTransactions_TenantId_BranchId_Status",
                table: "OnlinePaymentTransactions",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePaymentTransactions_TenantId_GatewayProviderKey_GatewayOrderId",
                table: "OnlinePaymentTransactions",
                columns: new[] { "TenantId", "GatewayProviderKey", "GatewayOrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocumentReminders_BranchId",
                table: "TransportDocumentReminders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocumentReminders_TenantId_BranchId_Status_DueOn",
                table: "TransportDocumentReminders",
                columns: new[] { "TenantId", "BranchId", "Status", "DueOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocumentReminders_VehicleId",
                table: "TransportDocumentReminders",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMaintenanceRecords_BranchId",
                table: "TransportMaintenanceRecords",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMaintenanceRecords_TenantId_BranchId_Status_DueOn",
                table: "TransportMaintenanceRecords",
                columns: new[] { "TenantId", "BranchId", "Status", "DueOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TransportMaintenanceRecords_VehicleId",
                table: "TransportMaintenanceRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGpsPings_BranchId",
                table: "VehicleGpsPings",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGpsPings_TenantId_VehicleId_RecordedOn",
                table: "VehicleGpsPings",
                columns: new[] { "TenantId", "VehicleId", "RecordedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGpsPings_TransportRouteId",
                table: "VehicleGpsPings",
                column: "TransportRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGpsPings_VehicleId",
                table: "VehicleGpsPings",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeeConcessionRequests");

            migrationBuilder.DropTable(
                name: "FeeRefunds");

            migrationBuilder.DropTable(
                name: "FeeReminders");

            migrationBuilder.DropTable(
                name: "HostelAllocationTransferRequests");

            migrationBuilder.DropTable(
                name: "HostelMaintenanceRequests");

            migrationBuilder.DropTable(
                name: "HostelVisitorLogs");

            migrationBuilder.DropTable(
                name: "LedgerExportBatches");

            migrationBuilder.DropTable(
                name: "LibraryBarcodeScans");

            migrationBuilder.DropTable(
                name: "LibraryOverdueNotifications");

            migrationBuilder.DropTable(
                name: "LibraryRenewals");

            migrationBuilder.DropTable(
                name: "LibraryReservations");

            migrationBuilder.DropTable(
                name: "NotificationDeliveryAttempts");

            migrationBuilder.DropTable(
                name: "OnlinePaymentTransactions");

            migrationBuilder.DropTable(
                name: "TransportDocumentReminders");

            migrationBuilder.DropTable(
                name: "TransportMaintenanceRecords");

            migrationBuilder.DropTable(
                name: "VehicleGpsPings");

            migrationBuilder.DropIndex(
                name: "IX_LibraryBookCopies_TenantId_BranchId_Barcode",
                table: "LibraryBookCopies");
        }
    }
}
