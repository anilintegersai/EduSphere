using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceTimetableWorkflowEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceCorrectionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    RequestedStatus = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_AttendanceCorrectionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_AttendanceRecords_AttendanceRecordId",
                        column: x => x.AttendanceRecordId,
                        principalTable: "AttendanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_AttendanceSessions_AttendanceSessionId",
                        column: x => x.AttendanceSessionId,
                        principalTable: "AttendanceSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetableSubstitutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimetableEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubstitutionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OriginalTeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubstituteTeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TimetableSubstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_TeacherProfiles_OriginalTeacherProfileId",
                        column: x => x.OriginalTeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_TeacherProfiles_SubstituteTeacherProfileId",
                        column: x => x.SubstituteTeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_TimetableEntries_TimetableEntryId",
                        column: x => x.TimetableEntryId,
                        principalTable: "TimetableEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_AttendanceRecordId",
                table: "AttendanceCorrectionRequests",
                column: "AttendanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_AttendanceSessionId",
                table: "AttendanceCorrectionRequests",
                column: "AttendanceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_RequestedByUserId",
                table: "AttendanceCorrectionRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_ReviewedByUserId",
                table: "AttendanceCorrectionRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_StudentProfileId",
                table: "AttendanceCorrectionRequests",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_TenantId_AttendanceRecordId_Status",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "TenantId", "AttendanceRecordId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_TenantId_AttendanceSessionId_Status",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "TenantId", "AttendanceSessionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_BranchId",
                table: "TimetableSubstitutions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_OriginalTeacherProfileId",
                table: "TimetableSubstitutions",
                column: "OriginalTeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_RequestedByUserId",
                table: "TimetableSubstitutions",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_ReviewedByUserId",
                table: "TimetableSubstitutions",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_SubstituteTeacherProfileId",
                table: "TimetableSubstitutions",
                column: "SubstituteTeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_TenantId_BranchId_SubstitutionDate_Status",
                table: "TimetableSubstitutions",
                columns: new[] { "TenantId", "BranchId", "SubstitutionDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_TenantId_TimetableEntryId_SubstitutionDate",
                table: "TimetableSubstitutions",
                columns: new[] { "TenantId", "TimetableEntryId", "SubstitutionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_TimetableEntryId",
                table: "TimetableSubstitutions",
                column: "TimetableEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceCorrectionRequests");

            migrationBuilder.DropTable(
                name: "TimetableSubstitutions");
        }
    }
}
