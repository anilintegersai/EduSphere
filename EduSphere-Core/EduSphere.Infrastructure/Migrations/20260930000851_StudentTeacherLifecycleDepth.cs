using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentTeacherLifecycleDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresOn",
                table: "ProfileDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileDocumentPolicyId",
                table: "ProfileDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RetainUntil",
                table: "ProfileDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScanMessage",
                table: "ProfileDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScanStatus",
                table: "ProfileDocuments",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScannedOn",
                table: "ProfileDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sha256",
                table: "ProfileDocuments",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageProvider",
                table: "ProfileDocuments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Local");

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "ProfileDocuments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "AlumniEngagements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentAlumniRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContributionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                    table.PrimaryKey("PK_AlumniEngagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlumniEngagements_StudentAlumniRecords_StudentAlumniRecordId",
                        column: x => x.StudentAlumniRecordId,
                        principalTable: "StudentAlumniRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BulkStudentLifecycleRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromAcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalCount = table.Column<int>(type: "int", nullable: false),
                    SucceededCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RunBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_BulkStudentLifecycleRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRuns_AcademicYears_FromAcademicYearId",
                        column: x => x.FromAcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRuns_AcademicYears_ToAcademicYearId",
                        column: x => x.ToAcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRuns_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRuns_Sections_FromSectionId",
                        column: x => x.FromSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRuns_Sections_ToSectionId",
                        column: x => x.ToSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LifecycleDocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OwnerType = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HtmlTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequiresSignature = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LifecycleDocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileDocumentPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerType = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AllowedExtensions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MaximumBytes = table.Column<long>(type: "bigint", nullable: false),
                    RequiresVirusScan = table.Column<bool>(type: "bit", nullable: false),
                    RequiresVerification = table.Column<bool>(type: "bit", nullable: false),
                    RetentionDays = table.Column<int>(type: "int", nullable: true),
                    ExpiryDays = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ProfileDocumentPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerType = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_ProfileStatusHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentMedicalProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CurrentMedications = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EmergencyInstructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhysicianName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PhysicianPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    InsuranceProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmergencyCareConsent = table.Column<bool>(type: "bit", nullable: false),
                    LastReviewedOn = table.Column<DateOnly>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_StudentMedicalProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentMedicalProfiles_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentTranscripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TranscriptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    AggregatePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalGrade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_StudentTranscripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentTranscripts_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTranscripts_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTranscripts_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BulkStudentLifecycleRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BulkStudentLifecycleRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_BulkStudentLifecycleRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRows_BulkStudentLifecycleRuns_BulkStudentLifecycleRunId",
                        column: x => x.BulkStudentLifecycleRunId,
                        principalTable: "BulkStudentLifecycleRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BulkStudentLifecycleRows_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentApprovalSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LifecycleDocumentTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SignerTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SignedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    table.PrimaryKey("PK_DocumentApprovalSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentApprovalSignatures_LifecycleDocumentTemplates_LifecycleDocumentTemplateId",
                        column: x => x.LifecycleDocumentTemplateId,
                        principalTable: "LifecycleDocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentApprovalSignatures_ProfileDocuments_ProfileDocumentId",
                        column: x => x.ProfileDocumentId,
                        principalTable: "ProfileDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentMedicalConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentMedicalProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiagnosedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    ResolvedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    CareInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_StudentMedicalConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentMedicalConditions_StudentMedicalProfiles_StudentMedicalProfileId",
                        column: x => x.StudentMedicalProfileId,
                        principalTable: "StudentMedicalProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentTranscriptLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentTranscriptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaximumMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarksObtained = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_StudentTranscriptLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentTranscriptLines_StudentTranscripts_StudentTranscriptId",
                        column: x => x.StudentTranscriptId,
                        principalTable: "StudentTranscripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileDocuments_ProfileDocumentPolicyId",
                table: "ProfileDocuments",
                column: "ProfileDocumentPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_AlumniEngagements_StudentAlumniRecordId",
                table: "AlumniEngagements",
                column: "StudentAlumniRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AlumniEngagements_TenantId_StudentAlumniRecordId_OccurredOn",
                table: "AlumniEngagements",
                columns: new[] { "TenantId", "StudentAlumniRecordId", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRows_BulkStudentLifecycleRunId_StudentProfileId",
                table: "BulkStudentLifecycleRows",
                columns: new[] { "BulkStudentLifecycleRunId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRows_StudentProfileId",
                table: "BulkStudentLifecycleRows",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_BranchId",
                table: "BulkStudentLifecycleRuns",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_FromAcademicYearId",
                table: "BulkStudentLifecycleRuns",
                column: "FromAcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_FromSectionId",
                table: "BulkStudentLifecycleRuns",
                column: "FromSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_TenantId_BranchId_StartedOn",
                table: "BulkStudentLifecycleRuns",
                columns: new[] { "TenantId", "BranchId", "StartedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_ToAcademicYearId",
                table: "BulkStudentLifecycleRuns",
                column: "ToAcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkStudentLifecycleRuns_ToSectionId",
                table: "BulkStudentLifecycleRuns",
                column: "ToSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentApprovalSignatures_LifecycleDocumentTemplateId",
                table: "DocumentApprovalSignatures",
                column: "LifecycleDocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentApprovalSignatures_ProfileDocumentId",
                table: "DocumentApprovalSignatures",
                column: "ProfileDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentApprovalSignatures_TenantId_SubjectId_SignedOn",
                table: "DocumentApprovalSignatures",
                columns: new[] { "TenantId", "SubjectId", "SignedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleDocumentTemplates_TenantId_Code_Version",
                table: "LifecycleDocumentTemplates",
                columns: new[] { "TenantId", "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileDocumentPolicies_TenantId_OwnerType_DocumentType",
                table: "ProfileDocumentPolicies",
                columns: new[] { "TenantId", "OwnerType", "DocumentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileStatusHistories_TenantId_OwnerType_OwnerId_EffectiveOn",
                table: "ProfileStatusHistories",
                columns: new[] { "TenantId", "OwnerType", "OwnerId", "EffectiveOn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentMedicalConditions_StudentMedicalProfileId",
                table: "StudentMedicalConditions",
                column: "StudentMedicalProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMedicalProfiles_StudentProfileId",
                table: "StudentMedicalProfiles",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMedicalProfiles_TenantId_StudentProfileId",
                table: "StudentMedicalProfiles",
                columns: new[] { "TenantId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscriptLines_StudentTranscriptId",
                table: "StudentTranscriptLines",
                column: "StudentTranscriptId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscripts_AcademicYearId",
                table: "StudentTranscripts",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscripts_CourseId",
                table: "StudentTranscripts",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscripts_StudentProfileId",
                table: "StudentTranscripts",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscripts_TenantId_StudentProfileId_IsFinal",
                table: "StudentTranscripts",
                columns: new[] { "TenantId", "StudentProfileId", "IsFinal" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTranscripts_TenantId_TranscriptNumber",
                table: "StudentTranscripts",
                columns: new[] { "TenantId", "TranscriptNumber" },
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO ProfileStatusHistories
                    (Id, OwnerType, OwnerId, PreviousStatus, Status, EffectiveOn, Source, SourceEntityId, Notes, RecordedBy,
                     CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), 1, StudentProfileId,
                       CASE FromStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'Inactive' WHEN 3 THEN 'Transferred' WHEN 4 THEN 'Graduated' WHEN 5 THEN 'Withdrawn' END,
                       CASE ToStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'Inactive' WHEN 3 THEN 'Transferred' WHEN 4 THEN 'Graduated' WHEN 5 THEN 'Withdrawn' ELSE CAST(ToStatus AS nvarchar(50)) END, EffectiveOn,
                       CONCAT('StudentLifecycle:', CAST(EventType AS nvarchar(20))), Id, COALESCE(Notes, Reason), NULL,
                       SYSUTCDATETIME(), 0, NEWID(), TenantId
                FROM StudentLifecycleEvents e
                WHERE e.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM ProfileStatusHistories h WHERE h.SourceEntityId = e.Id AND h.OwnerType = 1);

                INSERT INTO ProfileStatusHistories
                    (Id, OwnerType, OwnerId, PreviousStatus, Status, EffectiveOn, Source, SourceEntityId, Notes, RecordedBy,
                     CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), 2, TeacherProfileId,
                       CASE FromStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'OnLeave' WHEN 3 THEN 'Inactive' WHEN 4 THEN 'Relieved' END,
                       CASE ToStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'OnLeave' WHEN 3 THEN 'Inactive' WHEN 4 THEN 'Relieved' ELSE CAST(ToStatus AS nvarchar(50)) END, EffectiveOn,
                       CONCAT('TeacherLifecycle:', CAST(EventType AS nvarchar(20))), Id, COALESCE(Notes, Reason), NULL,
                       SYSUTCDATETIME(), 0, NEWID(), TenantId
                FROM TeacherLifecycleEvents e
                WHERE e.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM ProfileStatusHistories h WHERE h.SourceEntityId = e.Id AND h.OwnerType = 2);

                INSERT INTO LifecycleDocumentTemplates
                    (Id, Code, Name, OwnerType, DocumentType, HtmlTemplate, Version, IsActive, RequiresSignature,
                     CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), 'TRANSFER_CERTIFICATE', 'Transfer Certificate', 1, 'Transfer Certificate',
                       '<h1>Transfer Certificate</h1><p>This certifies that <strong>{{Name}}</strong> ({{Number}}) studied at {{Branch}}.</p><p>Date: {{Date}}</p>',
                       1, 1, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
                FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM LifecycleDocumentTemplates d WHERE d.TenantId = t.Id AND d.Code = 'TRANSFER_CERTIFICATE');

                INSERT INTO LifecycleDocumentTemplates
                    (Id, Code, Name, OwnerType, DocumentType, HtmlTemplate, Version, IsActive, RequiresSignature,
                     CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), 'GRADUATION_CERTIFICATE', 'Graduation Certificate', 1, 'Graduation Certificate',
                       '<h1>Graduation Certificate</h1><p><strong>{{Name}}</strong> ({{Number}}) has fulfilled the graduation requirements of {{Branch}}.</p><p>Date: {{Date}}</p>',
                       1, 1, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
                FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM LifecycleDocumentTemplates d WHERE d.TenantId = t.Id AND d.Code = 'GRADUATION_CERTIFICATE');

                INSERT INTO ProfileDocumentPolicies
                    (Id, OwnerType, DocumentType, AllowedExtensions, MaximumBytes, RequiresVirusScan, RequiresVerification,
                     RetentionDays, ExpiryDays, IsActive, CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
                SELECT NEWID(), 1, 'Lifecycle', '.pdf,.png,.jpg,.jpeg,.doc,.docx', 10485760, 1, 1,
                       2555, NULL, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
                FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
                    (SELECT 1 FROM ProfileDocumentPolicies p WHERE p.TenantId = t.Id AND p.OwnerType = 1 AND p.DocumentType = 'Lifecycle');
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileDocuments_ProfileDocumentPolicies_ProfileDocumentPolicyId",
                table: "ProfileDocuments",
                column: "ProfileDocumentPolicyId",
                principalTable: "ProfileDocumentPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileDocuments_ProfileDocumentPolicies_ProfileDocumentPolicyId",
                table: "ProfileDocuments");

            migrationBuilder.DropTable(
                name: "AlumniEngagements");

            migrationBuilder.DropTable(
                name: "BulkStudentLifecycleRows");

            migrationBuilder.DropTable(
                name: "DocumentApprovalSignatures");

            migrationBuilder.DropTable(
                name: "ProfileDocumentPolicies");

            migrationBuilder.DropTable(
                name: "ProfileStatusHistories");

            migrationBuilder.DropTable(
                name: "StudentMedicalConditions");

            migrationBuilder.DropTable(
                name: "StudentTranscriptLines");

            migrationBuilder.DropTable(
                name: "BulkStudentLifecycleRuns");

            migrationBuilder.DropTable(
                name: "LifecycleDocumentTemplates");

            migrationBuilder.DropTable(
                name: "StudentMedicalProfiles");

            migrationBuilder.DropTable(
                name: "StudentTranscripts");

            migrationBuilder.DropIndex(
                name: "IX_ProfileDocuments_ProfileDocumentPolicyId",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "ExpiresOn",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "ProfileDocumentPolicyId",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "RetainUntil",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "ScanMessage",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "ScanStatus",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "ScannedOn",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "Sha256",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "StorageProvider",
                table: "ProfileDocuments");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "ProfileDocuments");
        }
    }
}
