using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AcademicStructureDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AcademicTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicTerms_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningOutcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BloomLevel = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
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
                    table.PrimaryKey("PK_LearningOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_SyllabusUnits_SyllabusUnitId",
                        column: x => x.SyllabusUnitId,
                        principalTable: "SyllabusUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectReferenceBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Authors = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Isbn = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Publisher = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EditionYear = table.Column<int>(type: "int", nullable: true),
                    ResourceUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SubjectReferenceBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectReferenceBooks_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SyllabusTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SyllabusTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyllabusTopics_SyllabusUnits_SyllabusUnitId",
                        column: x => x.SyllabusUnitId,
                        principalTable: "SyllabusUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAvailabilityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndsAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_TeacherAvailabilityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAvailabilityRules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherAvailabilityRules_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetableTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TimetableTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicCalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcademicTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInstructionalDay = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AcademicCalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherLessonPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusTopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlannedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Objectives = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Activities = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Resources = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Assessment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_TeacherLessonPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLessonPlans_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLessonPlans_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLessonPlans_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLessonPlans_SyllabusTopics_SyllabusTopicId",
                        column: x => x.SyllabusTopicId,
                        principalTable: "SyllabusTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLessonPlans_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetableGenerationRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimetableTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledPeriods = table.Column<int>(type: "int", nullable: false),
                    UnscheduledPeriods = table.Column<int>(type: "int", nullable: false),
                    Diagnostics = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    GeneratedTimetableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_TimetableGenerationRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableGenerationRuns_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableGenerationRuns_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableGenerationRuns_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableGenerationRuns_TimetableTemplates_TimetableTemplateId",
                        column: x => x.TimetableTemplateId,
                        principalTable: "TimetableTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableGenerationRuns_Timetables_GeneratedTimetableId",
                        column: x => x.GeneratedTimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetableTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimetableTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodsPerWeek = table.Column<int>(type: "int", nullable: false),
                    MaximumPeriodsPerDay = table.Column<int>(type: "int", nullable: false),
                    PreferredRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequireConsecutivePeriods = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_TimetableTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableTemplateItems_Rooms_PreferredRoomId",
                        column: x => x.PreferredRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableTemplateItems_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableTemplateItems_TimetableTemplates_TimetableTemplateId",
                        column: x => x.TimetableTemplateId,
                        principalTable: "TimetableTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_AcademicTermId",
                table: "AcademicCalendarEvents",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_AcademicYearId",
                table: "AcademicCalendarEvents",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_BranchId",
                table: "AcademicCalendarEvents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_TenantId_AcademicYearId_StartsOn",
                table: "AcademicCalendarEvents",
                columns: new[] { "TenantId", "AcademicYearId", "StartsOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_AcademicYearId",
                table: "AcademicTerms",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_TenantId_AcademicYearId_Sequence",
                table: "AcademicTerms",
                columns: new[] { "TenantId", "AcademicYearId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_SubjectId",
                table: "LearningOutcomes",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_SyllabusUnitId",
                table: "LearningOutcomes",
                column: "SyllabusUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_TenantId_SubjectId_Code",
                table: "LearningOutcomes",
                columns: new[] { "TenantId", "SubjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectReferenceBooks_SubjectId",
                table: "SubjectReferenceBooks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectReferenceBooks_TenantId_SubjectId_Title",
                table: "SubjectReferenceBooks",
                columns: new[] { "TenantId", "SubjectId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusTopics_SyllabusUnitId_Order",
                table: "SyllabusTopics",
                columns: new[] { "SyllabusUnitId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailabilityRules_BranchId",
                table: "TeacherAvailabilityRules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailabilityRules_TeacherProfileId",
                table: "TeacherAvailabilityRules",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailabilityRules_TenantId_TeacherProfileId_DayOfWeek_StartsAt_EndsAt",
                table: "TeacherAvailabilityRules",
                columns: new[] { "TenantId", "TeacherProfileId", "DayOfWeek", "StartsAt", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_AcademicTermId",
                table: "TeacherLessonPlans",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_SectionId",
                table: "TeacherLessonPlans",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_SubjectId",
                table: "TeacherLessonPlans",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_SyllabusTopicId",
                table: "TeacherLessonPlans",
                column: "SyllabusTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_TeacherProfileId",
                table: "TeacherLessonPlans",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessonPlans_TenantId_TeacherProfileId_PlannedDate",
                table: "TeacherLessonPlans",
                columns: new[] { "TenantId", "TeacherProfileId", "PlannedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_AcademicYearId",
                table: "TimetableGenerationRuns",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_BranchId",
                table: "TimetableGenerationRuns",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_GeneratedTimetableId",
                table: "TimetableGenerationRuns",
                column: "GeneratedTimetableId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_SectionId",
                table: "TimetableGenerationRuns",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_TenantId_BranchId_StartedOn",
                table: "TimetableGenerationRuns",
                columns: new[] { "TenantId", "BranchId", "StartedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableGenerationRuns_TimetableTemplateId",
                table: "TimetableGenerationRuns",
                column: "TimetableTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplateItems_PreferredRoomId",
                table: "TimetableTemplateItems",
                column: "PreferredRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplateItems_SubjectId",
                table: "TimetableTemplateItems",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplateItems_TimetableTemplateId_SubjectId",
                table: "TimetableTemplateItems",
                columns: new[] { "TimetableTemplateId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_BranchId",
                table: "TimetableTemplates",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_TenantId_BranchId_Name",
                table: "TimetableTemplates",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicCalendarEvents");

            migrationBuilder.DropTable(
                name: "LearningOutcomes");

            migrationBuilder.DropTable(
                name: "SubjectReferenceBooks");

            migrationBuilder.DropTable(
                name: "TeacherAvailabilityRules");

            migrationBuilder.DropTable(
                name: "TeacherLessonPlans");

            migrationBuilder.DropTable(
                name: "TimetableGenerationRuns");

            migrationBuilder.DropTable(
                name: "TimetableTemplateItems");

            migrationBuilder.DropTable(
                name: "AcademicTerms");

            migrationBuilder.DropTable(
                name: "SyllabusTopics");

            migrationBuilder.DropTable(
                name: "TimetableTemplates");
        }
    }
}
