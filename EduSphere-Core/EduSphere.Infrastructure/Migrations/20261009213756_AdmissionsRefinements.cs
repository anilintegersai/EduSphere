using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdmissionsRefinements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarProvider",
                table: "AdmissionInterviews",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalCalendarEventId",
                table: "AdmissionInterviews",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedByDocumentId",
                table: "AdmissionDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacesDocumentId",
                table: "AdmissionDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewFeedback",
                table: "AdmissionDocuments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewStatus",
                table: "AdmissionDocuments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantAccountId",
                table: "AdmissionApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationSource",
                table: "AdmissionApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeeDueOn",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedOn",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OfferExpiresOn",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReAdmissionStudentProfileId",
                table: "AdmissionApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedOn",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaitlistPosition",
                table: "AdmissionApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdmissionApplicantAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AdmissionApplicantAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionAutomationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                    table.PrimaryKey("PK_AdmissionAutomationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionAutomationLogs_AdmissionApplications_AdmissionApplicationId",
                        column: x => x.AdmissionApplicationId,
                        principalTable: "AdmissionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionInterviewerAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
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
                    table.PrimaryKey("PK_AdmissionInterviewerAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionInterviewerAvailabilities_AspNetUsers_InterviewerUserId",
                        column: x => x.InterviewerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionInterviewerAvailabilities_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchAdmissionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationNumberFormat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdmissionNumberFormat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NextApplicationSequence = table.Column<int>(type: "int", nullable: false),
                    NextAdmissionSequence = table.Column<int>(type: "int", nullable: false),
                    OfferValidityDays = table.Column<int>(type: "int", nullable: false),
                    FeeDeadlineDays = table.Column<int>(type: "int", nullable: false),
                    AutoPromoteWaitlist = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_BranchAdmissionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchAdmissionSettings_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocuments_ReplacesDocumentId",
                table: "AdmissionDocuments",
                column: "ReplacesDocumentId",
                unique: true,
                filter: "[ReplacesDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_ApplicantAccountId",
                table: "AdmissionApplications",
                column: "ApplicantAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_ReAdmissionStudentProfileId",
                table: "AdmissionApplications",
                column: "ReAdmissionStudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_TenantId_ApplicantAccountId_Status",
                table: "AdmissionApplications",
                columns: new[] { "TenantId", "ApplicantAccountId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_TenantId_BranchId_CourseId_WaitlistPosition",
                table: "AdmissionApplications",
                columns: new[] { "TenantId", "BranchId", "CourseId", "WaitlistPosition" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplicantAccounts_TenantId_Email",
                table: "AdmissionApplicantAccounts",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionAutomationLogs_AdmissionApplicationId",
                table: "AdmissionAutomationLogs",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionAutomationLogs_TenantId_AdmissionApplicationId_OccurredOn",
                table: "AdmissionAutomationLogs",
                columns: new[] { "TenantId", "AdmissionApplicationId", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviewerAvailabilities_BranchId",
                table: "AdmissionInterviewerAvailabilities",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviewerAvailabilities_InterviewerUserId",
                table: "AdmissionInterviewerAvailabilities",
                column: "InterviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionInterviewerAvailabilities_TenantId_InterviewerUserId_DayOfWeek_StartsAt_EndsAt",
                table: "AdmissionInterviewerAvailabilities",
                columns: new[] { "TenantId", "InterviewerUserId", "DayOfWeek", "StartsAt", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchAdmissionSettings_BranchId",
                table: "BranchAdmissionSettings",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchAdmissionSettings_TenantId_BranchId",
                table: "BranchAdmissionSettings",
                columns: new[] { "TenantId", "BranchId" },
                unique: true);

            migrationBuilder.Sql("""
                UPDATE AdmissionApplications
                SET ApplicationSource = COALESCE(ApplicationSource, 'Administrative entry'),
                    SubmittedOn = COALESCE(SubmittedOn, DATEADD(day, DATEDIFF(day, 0, AppliedOn), 0)),
                    LastSavedOn = COALESCE(LastSavedOn, CreatedOn)
                WHERE Status <> 9;

                UPDATE AdmissionDocuments SET ReviewStatus = CASE WHEN IsVerified = 1 THEN 2 ELSE 1 END;

                INSERT INTO BranchAdmissionSettings
                    (Id, BranchId, ApplicationNumberFormat, AdmissionNumberFormat, NextApplicationSequence, NextAdmissionSequence,
                     OfferValidityDays, FeeDeadlineDays, AutoPromoteWaitlist, CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), b.Id, 'APP-{YYYY}-{SEQ:0000}', 'ADM-{YYYY}-{SEQ:0000}',
                       ISNULL((SELECT COUNT(1) + 1 FROM AdmissionApplications a WHERE a.TenantId = b.TenantId AND a.BranchId = b.Id), 1),
                       ISNULL((SELECT COUNT(1) + 1 FROM StudentProfiles s WHERE s.TenantId = b.TenantId AND s.BranchId = b.Id), 1),
                       7, 7, 1, SYSUTCDATETIME(), 0, NEWID(), b.TenantId
                FROM Branches b
                WHERE b.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM BranchAdmissionSettings x WHERE x.TenantId = b.TenantId AND x.BranchId = b.Id);
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionApplications_AdmissionApplicantAccounts_ApplicantAccountId",
                table: "AdmissionApplications",
                column: "ApplicantAccountId",
                principalTable: "AdmissionApplicantAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionApplications_StudentProfiles_ReAdmissionStudentProfileId",
                table: "AdmissionApplications",
                column: "ReAdmissionStudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionDocuments_AdmissionDocuments_ReplacesDocumentId",
                table: "AdmissionDocuments",
                column: "ReplacesDocumentId",
                principalTable: "AdmissionDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionApplications_AdmissionApplicantAccounts_ApplicantAccountId",
                table: "AdmissionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionApplications_StudentProfiles_ReAdmissionStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionDocuments_AdmissionDocuments_ReplacesDocumentId",
                table: "AdmissionDocuments");

            migrationBuilder.DropTable(
                name: "AdmissionApplicantAccounts");

            migrationBuilder.DropTable(
                name: "AdmissionAutomationLogs");

            migrationBuilder.DropTable(
                name: "AdmissionInterviewerAvailabilities");

            migrationBuilder.DropTable(
                name: "BranchAdmissionSettings");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionDocuments_ReplacesDocumentId",
                table: "AdmissionDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_ApplicantAccountId",
                table: "AdmissionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_ReAdmissionStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_TenantId_ApplicantAccountId_Status",
                table: "AdmissionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_TenantId_BranchId_CourseId_WaitlistPosition",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "CalendarProvider",
                table: "AdmissionInterviews");

            migrationBuilder.DropColumn(
                name: "ExternalCalendarEventId",
                table: "AdmissionInterviews");

            migrationBuilder.DropColumn(
                name: "ReplacedByDocumentId",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "ReplacesDocumentId",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "ReviewFeedback",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "ApplicantAccountId",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationSource",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "FeeDueOn",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "LastSavedOn",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "OfferExpiresOn",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "ReAdmissionStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "SubmittedOn",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "WaitlistPosition",
                table: "AdmissionApplications");
        }
    }
}
