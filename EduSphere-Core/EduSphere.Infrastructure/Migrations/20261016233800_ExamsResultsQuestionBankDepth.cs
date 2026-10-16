using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExamsResultsQuestionBankDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ExamAttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamAttendanceRecords_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAttendanceRecords_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAttendanceRecords_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamHallTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ExamHallTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamHallTickets_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamHallTickets_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamMalpracticeRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReportedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_ExamMalpracticeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamMalpracticeRecords_AspNetUsers_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamMalpracticeRecords_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamMalpracticeRecords_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSeatingPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CandidateCount = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ExamSeatingPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSeatingPlans_AspNetUsers_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSeatingPlans_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalGradingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradingSchemeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorityName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AuthorityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaximumGraceMarks = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    MaximumGracePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    GraceOnlyForPassing = table.Column<bool>(type: "bit", nullable: false),
                    ApplyBestOfSubjects = table.Column<bool>(type: "bit", nullable: false),
                    BestOfSubjectCount = table.Column<int>(type: "int", nullable: true),
                    RulesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_ExternalGradingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalGradingRules_GradingSchemes_GradingSchemeId",
                        column: x => x.GradingSchemeId,
                        principalTable: "GradingSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkingSchemeReleases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionPaperVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleasedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReleasedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_MarkingSchemeReleases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkingSchemeReleases_AspNetUsers_ReleasedByUserId",
                        column: x => x.ReleasedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkingSchemeReleases_AspNetUsers_ReleasedToUserId",
                        column: x => x.ReleasedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkingSchemeReleases_QuestionPaperVersions_QuestionPaperVersionId",
                        column: x => x.QuestionPaperVersionId,
                        principalTable: "QuestionPaperVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkRubrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_MarkRubrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkRubrics_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarksImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ValidRows = table.Column<int>(type: "int", nullable: false),
                    ImportedRows = table.Column<int>(type: "int", nullable: false),
                    ErrorRows = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_MarksImportBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarksImportBatches_AspNetUsers_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarksImportBatches_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionPaperReviewPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RequiredReviewerCount = table.Column<int>(type: "int", nullable: false),
                    RequiredApprovalCount = table.Column<int>(type: "int", nullable: false),
                    RejectOnAnyRejection = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_QuestionPaperReviewPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionPaperReviewPolicies_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportCardTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeaderText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ShowRank = table.Column<bool>(type: "bit", nullable: false),
                    ShowGradePoint = table.Column<bool>(type: "bit", nullable: false),
                    ShowAttendance = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ReportCardTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportCardTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportCardTemplates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultReviewRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OriginalMarks = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    RevisedMarks = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ResultReviewRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultReviewRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultReviewRequests_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultReviewRequests_MarkEntries_MarkEntryId",
                        column: x => x.MarkEntryId,
                        principalTable: "MarkEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultReviewRequests_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSeatAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamSeatingPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CandidateStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ExamSeatAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSeatAssignments_ExamSeatingPlans_ExamSeatingPlanId",
                        column: x => x.ExamSeatingPlanId,
                        principalTable: "ExamSeatingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSeatAssignments_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSeatAssignments_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkRubricCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkRubricId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaximumMarks = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_MarkRubricCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkRubricCriteria_MarkRubrics_MarkRubricId",
                        column: x => x.MarkRubricId,
                        principalTable: "MarkRubrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarksImportRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarksImportBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MarksObtained = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    IsAbsent = table.Column<bool>(type: "bit", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IsImported = table.Column<bool>(type: "bit", nullable: false),
                    ValidationMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_MarksImportRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarksImportRows_MarksImportBatches_MarksImportBatchId",
                        column: x => x.MarksImportBatchId,
                        principalTable: "MarksImportBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarksImportRows_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkRubricScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkRubricCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarksAwarded = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_MarkRubricScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkRubricScores_MarkEntries_MarkEntryId",
                        column: x => x.MarkEntryId,
                        principalTable: "MarkEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarkRubricScores_MarkRubricCriteria_MarkRubricCriterionId",
                        column: x => x.MarkRubricCriterionId,
                        principalTable: "MarkRubricCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttendanceRecords_ExamScheduleId",
                table: "ExamAttendanceRecords",
                column: "ExamScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttendanceRecords_RecordedByUserId",
                table: "ExamAttendanceRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttendanceRecords_StudentProfileId",
                table: "ExamAttendanceRecords",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttendanceRecords_TenantId_ExamScheduleId_StudentProfileId",
                table: "ExamAttendanceRecords",
                columns: new[] { "TenantId", "ExamScheduleId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamHallTickets_ExamId",
                table: "ExamHallTickets",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamHallTickets_StudentProfileId",
                table: "ExamHallTickets",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamHallTickets_TenantId_ExamId_StudentProfileId",
                table: "ExamHallTickets",
                columns: new[] { "TenantId", "ExamId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamHallTickets_TenantId_TicketNumber",
                table: "ExamHallTickets",
                columns: new[] { "TenantId", "TicketNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamMalpracticeRecords_ExamScheduleId",
                table: "ExamMalpracticeRecords",
                column: "ExamScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMalpracticeRecords_ReportedByUserId",
                table: "ExamMalpracticeRecords",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMalpracticeRecords_StudentProfileId",
                table: "ExamMalpracticeRecords",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMalpracticeRecords_TenantId_ExamScheduleId_StudentProfileId_ReportedOn",
                table: "ExamMalpracticeRecords",
                columns: new[] { "TenantId", "ExamScheduleId", "StudentProfileId", "ReportedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatAssignments_ExamSeatingPlanId_RoomId_SeatNumber",
                table: "ExamSeatAssignments",
                columns: new[] { "ExamSeatingPlanId", "RoomId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatAssignments_ExamSeatingPlanId_StudentProfileId",
                table: "ExamSeatAssignments",
                columns: new[] { "ExamSeatingPlanId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatAssignments_RoomId",
                table: "ExamSeatAssignments",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatAssignments_StudentProfileId",
                table: "ExamSeatAssignments",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatingPlans_ExamScheduleId",
                table: "ExamSeatingPlans",
                column: "ExamScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatingPlans_GeneratedByUserId",
                table: "ExamSeatingPlans",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSeatingPlans_TenantId_ExamScheduleId_GeneratedOn",
                table: "ExamSeatingPlans",
                columns: new[] { "TenantId", "ExamScheduleId", "GeneratedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalGradingRules_GradingSchemeId",
                table: "ExternalGradingRules",
                column: "GradingSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalGradingRules_TenantId_GradingSchemeId_AuthorityCode",
                table: "ExternalGradingRules",
                columns: new[] { "TenantId", "GradingSchemeId", "AuthorityCode" },
                unique: true,
                filter: "[AuthorityCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarkingSchemeReleases_QuestionPaperVersionId",
                table: "MarkingSchemeReleases",
                column: "QuestionPaperVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkingSchemeReleases_ReleasedByUserId",
                table: "MarkingSchemeReleases",
                column: "ReleasedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkingSchemeReleases_ReleasedToUserId",
                table: "MarkingSchemeReleases",
                column: "ReleasedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkingSchemeReleases_TenantId_QuestionPaperVersionId_ReleasedToUserId",
                table: "MarkingSchemeReleases",
                columns: new[] { "TenantId", "QuestionPaperVersionId", "ReleasedToUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkRubricCriteria_MarkRubricId_SortOrder",
                table: "MarkRubricCriteria",
                columns: new[] { "MarkRubricId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkRubrics_ExamScheduleId",
                table: "MarkRubrics",
                column: "ExamScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkRubrics_TenantId_ExamScheduleId_Name",
                table: "MarkRubrics",
                columns: new[] { "TenantId", "ExamScheduleId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkRubricScores_MarkEntryId_MarkRubricCriterionId",
                table: "MarkRubricScores",
                columns: new[] { "MarkEntryId", "MarkRubricCriterionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkRubricScores_MarkRubricCriterionId",
                table: "MarkRubricScores",
                column: "MarkRubricCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarksImportBatches_ExamScheduleId",
                table: "MarksImportBatches",
                column: "ExamScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_MarksImportBatches_TenantId_ExamScheduleId_UploadedOn",
                table: "MarksImportBatches",
                columns: new[] { "TenantId", "ExamScheduleId", "UploadedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_MarksImportBatches_UploadedByUserId",
                table: "MarksImportBatches",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarksImportRows_MarksImportBatchId_RowNumber",
                table: "MarksImportRows",
                columns: new[] { "MarksImportBatchId", "RowNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarksImportRows_StudentProfileId",
                table: "MarksImportRows",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPaperReviewPolicies_BranchId",
                table: "QuestionPaperReviewPolicies",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPaperReviewPolicies_TenantId_BranchId_Name",
                table: "QuestionPaperReviewPolicies",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportCardTemplates_BranchId",
                table: "ReportCardTemplates",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportCardTemplates_CourseId",
                table: "ReportCardTemplates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportCardTemplates_TenantId_BranchId_Name",
                table: "ReportCardTemplates",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReviewRequests_MarkEntryId",
                table: "ResultReviewRequests",
                column: "MarkEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReviewRequests_RequestedByUserId",
                table: "ResultReviewRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReviewRequests_ReviewedByUserId",
                table: "ResultReviewRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReviewRequests_StudentProfileId",
                table: "ResultReviewRequests",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReviewRequests_TenantId_StudentProfileId_Status",
                table: "ResultReviewRequests",
                columns: new[] { "TenantId", "StudentProfileId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamAttendanceRecords");

            migrationBuilder.DropTable(
                name: "ExamHallTickets");

            migrationBuilder.DropTable(
                name: "ExamMalpracticeRecords");

            migrationBuilder.DropTable(
                name: "ExamSeatAssignments");

            migrationBuilder.DropTable(
                name: "ExternalGradingRules");

            migrationBuilder.DropTable(
                name: "MarkingSchemeReleases");

            migrationBuilder.DropTable(
                name: "MarkRubricScores");

            migrationBuilder.DropTable(
                name: "MarksImportRows");

            migrationBuilder.DropTable(
                name: "QuestionPaperReviewPolicies");

            migrationBuilder.DropTable(
                name: "ReportCardTemplates");

            migrationBuilder.DropTable(
                name: "ResultReviewRequests");

            migrationBuilder.DropTable(
                name: "ExamSeatingPlans");

            migrationBuilder.DropTable(
                name: "MarkRubricCriteria");

            migrationBuilder.DropTable(
                name: "MarksImportBatches");

            migrationBuilder.DropTable(
                name: "MarkRubrics");
        }
    }
}
