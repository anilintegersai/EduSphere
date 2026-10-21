using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LibraryOperationsDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AvailabilityNotifiedOn",
                table: "LibraryReservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QueuePosition",
                table: "LibraryReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FeeInvoiceId",
                table: "LibraryFineRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerifiedOn",
                table: "LibraryBookCopies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LibraryCirculationPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MemberType = table.Column<int>(type: "int", nullable: false),
                    MaxIssues = table.Column<int>(type: "int", nullable: false),
                    LoanDays = table.Column<int>(type: "int", nullable: false),
                    MaxRenewals = table.Column<int>(type: "int", nullable: false),
                    GraceDays = table.Column<int>(type: "int", nullable: false),
                    FinePerDay = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    MaximumFine = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    LostBookMultiplier = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    DamageChargePercent = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ReservationHoldDays = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LibraryCirculationPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryDigitalResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ResourceType = table.Column<int>(type: "int", nullable: false),
                    ResourceUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LicenseName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LicenseExpiresOn = table.Column<DateOnly>(type: "date", nullable: true),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LibraryDigitalResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryDigitalResources_LibraryBooks_LibraryBookId",
                        column: x => x.LibraryBookId,
                        principalTable: "LibraryBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryItemIncidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AssessedAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FeeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LibraryItemIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryItemIncidents_FeeInvoices_FeeInvoiceId",
                        column: x => x.FeeInvoiceId,
                        principalTable: "FeeInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryItemIncidents_LibraryBookCopies_LibraryBookCopyId",
                        column: x => x.LibraryBookCopyId,
                        principalTable: "LibraryBookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryItemIncidents_LibraryBookIssues_LibraryBookIssueId",
                        column: x => x.LibraryBookIssueId,
                        principalTable: "LibraryBookIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryItemIncidents_LibraryMembers_LibraryMemberId",
                        column: x => x.LibraryMemberId,
                        principalTable: "LibraryMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryStockVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LibraryStockVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
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
                    table.PrimaryKey("PK_LibraryVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryStockVerificationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryStockVerificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScanCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    ScannedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LibraryStockVerificationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryStockVerificationItems_LibraryBookCopies_LibraryBookCopyId",
                        column: x => x.LibraryBookCopyId,
                        principalTable: "LibraryBookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryStockVerificationItems_LibraryStockVerifications_LibraryStockVerificationId",
                        column: x => x.LibraryStockVerificationId,
                        principalTable: "LibraryStockVerifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LibraryPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryVendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_LibraryPurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryPurchaseOrders_LibraryVendors_LibraryVendorId",
                        column: x => x.LibraryVendorId,
                        principalTable: "LibraryVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryPurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryPurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReceivedQuantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_LibraryPurchaseOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryPurchaseOrderLines_LibraryBooks_LibraryBookId",
                        column: x => x.LibraryBookId,
                        principalTable: "LibraryBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryPurchaseOrderLines_LibraryPurchaseOrders_LibraryPurchaseOrderId",
                        column: x => x.LibraryPurchaseOrderId,
                        principalTable: "LibraryPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryFineRecords_FeeInvoiceId",
                table: "LibraryFineRecords",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryCirculationPolicies_TenantId_BranchId_MemberType",
                table: "LibraryCirculationPolicies",
                columns: new[] { "TenantId", "BranchId", "MemberType" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryDigitalResources_LibraryBookId",
                table: "LibraryDigitalResources",
                column: "LibraryBookId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryDigitalResources_TenantId_BranchId_IsActive",
                table: "LibraryDigitalResources",
                columns: new[] { "TenantId", "BranchId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItemIncidents_FeeInvoiceId",
                table: "LibraryItemIncidents",
                column: "FeeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItemIncidents_LibraryBookCopyId",
                table: "LibraryItemIncidents",
                column: "LibraryBookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItemIncidents_LibraryBookIssueId",
                table: "LibraryItemIncidents",
                column: "LibraryBookIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItemIncidents_LibraryMemberId",
                table: "LibraryItemIncidents",
                column: "LibraryMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItemIncidents_TenantId_BranchId_Status",
                table: "LibraryItemIncidents",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryPurchaseOrderLines_LibraryBookId",
                table: "LibraryPurchaseOrderLines",
                column: "LibraryBookId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryPurchaseOrderLines_LibraryPurchaseOrderId",
                table: "LibraryPurchaseOrderLines",
                column: "LibraryPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryPurchaseOrders_LibraryVendorId",
                table: "LibraryPurchaseOrders",
                column: "LibraryVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryPurchaseOrders_TenantId_BranchId_OrderNumber",
                table: "LibraryPurchaseOrders",
                columns: new[] { "TenantId", "BranchId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryStockVerificationItems_LibraryBookCopyId",
                table: "LibraryStockVerificationItems",
                column: "LibraryBookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryStockVerificationItems_LibraryStockVerificationId",
                table: "LibraryStockVerificationItems",
                column: "LibraryStockVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryStockVerificationItems_TenantId_LibraryStockVerificationId_ScanCode",
                table: "LibraryStockVerificationItems",
                columns: new[] { "TenantId", "LibraryStockVerificationId", "ScanCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryStockVerifications_TenantId_BranchId_Status",
                table: "LibraryStockVerifications",
                columns: new[] { "TenantId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryVendors_TenantId_BranchId_Name",
                table: "LibraryVendors",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryFineRecords_FeeInvoices_FeeInvoiceId",
                table: "LibraryFineRecords",
                column: "FeeInvoiceId",
                principalTable: "FeeInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryFineRecords_FeeInvoices_FeeInvoiceId",
                table: "LibraryFineRecords");

            migrationBuilder.DropTable(
                name: "LibraryCirculationPolicies");

            migrationBuilder.DropTable(
                name: "LibraryDigitalResources");

            migrationBuilder.DropTable(
                name: "LibraryItemIncidents");

            migrationBuilder.DropTable(
                name: "LibraryPurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "LibraryStockVerificationItems");

            migrationBuilder.DropTable(
                name: "LibraryPurchaseOrders");

            migrationBuilder.DropTable(
                name: "LibraryStockVerifications");

            migrationBuilder.DropTable(
                name: "LibraryVendors");

            migrationBuilder.DropIndex(
                name: "IX_LibraryFineRecords_FeeInvoiceId",
                table: "LibraryFineRecords");

            migrationBuilder.DropColumn(
                name: "AvailabilityNotifiedOn",
                table: "LibraryReservations");

            migrationBuilder.DropColumn(
                name: "QueuePosition",
                table: "LibraryReservations");

            migrationBuilder.DropColumn(
                name: "FeeInvoiceId",
                table: "LibraryFineRecords");

            migrationBuilder.DropColumn(
                name: "LastVerifiedOn",
                table: "LibraryBookCopies");
        }
    }
}
