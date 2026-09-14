using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdmissionsWorkflowCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "StudentProfileId",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentFeeAssignmentId",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AdmissionApplicationId",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdmissionInterviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    MeetingLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OutcomeNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_AdmissionInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionInterviews_AdmissionApplications_AdmissionApplicationId",
                        column: x => x.AdmissionApplicationId,
                        principalTable: "AdmissionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionInterviews_AspNetUsers_InterviewerUserId",
                        column: x => x.InterviewerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionInterviews_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeeInvoices_AdmissionApplicationId",
                table: "FeeInvoices",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviews_AdmissionApplicationId",
                table: "AdmissionInterviews",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviews_BranchId",
                table: "AdmissionInterviews",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviews_InterviewerUserId",
                table: "AdmissionInterviews",
                column: "InterviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviews_TenantId_BranchId_StartsOn_Status",
                table: "AdmissionInterviews",
                columns: new[] { "TenantId", "BranchId", "StartsOn", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviews_TenantId_InterviewerUserId_StartsOn_EndsOn",
                table: "AdmissionInterviews",
                columns: new[] { "TenantId", "InterviewerUserId", "StartsOn", "EndsOn" });

            migrationBuilder.AddForeignKey(
                name: "FK_FeeInvoices_AdmissionApplications_AdmissionApplicationId",
                table: "FeeInvoices",
                column: "AdmissionApplicationId",
                principalTable: "AdmissionApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeeInvoices_AdmissionApplications_AdmissionApplicationId",
                table: "FeeInvoices");

            migrationBuilder.DropTable(
                name: "AdmissionInterviews");

            migrationBuilder.DropIndex(
                name: "IX_FeeInvoices_AdmissionApplicationId",
                table: "FeeInvoices");

            migrationBuilder.DropColumn(
                name: "AdmissionApplicationId",
                table: "FeeInvoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentProfileId",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentFeeAssignmentId",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
