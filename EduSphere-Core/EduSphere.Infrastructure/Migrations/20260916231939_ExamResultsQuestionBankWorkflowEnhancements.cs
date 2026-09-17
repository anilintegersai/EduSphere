using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExamResultsQuestionBankWorkflowEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "Results",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentile",
                table: "Results",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Results",
                type: "int",
                nullable: true);

            // Dynamic SQL forces SQL Server to compile this statement after the
            // preceding ALTER TABLE has introduced IsPassed.
            migrationBuilder.Sql("EXEC(N'UPDATE [Results] SET [IsPassed] = CASE WHEN [Grade] IS NOT NULL AND UPPER([Grade]) <> N''F'' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END')");

            migrationBuilder.CreateTable(
                name: "QuestionPaperModerations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionPaperId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_QuestionPaperModerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionPaperModerations_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionPaperModerations_QuestionPapers_QuestionPaperId",
                        column: x => x.QuestionPaperId,
                        principalTable: "QuestionPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultPublicationBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultCount = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ResultPublicationBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_AspNetUsers_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultPublicationBatches_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultRankingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Method = table.Column<int>(type: "int", nullable: false),
                    RankByPercentage = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeFailedStudents = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeWithheldResults = table.Column<bool>(type: "bit", nullable: false),
                    MinimumPercentageToRank = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ShowRankOnReportCard = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ResultRankingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultRankingRules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultRankingRules_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultRankingRules_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPaperModerations_QuestionPaperId",
                table: "QuestionPaperModerations",
                column: "QuestionPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPaperModerations_ReviewedByUserId",
                table: "QuestionPaperModerations",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPaperModerations_TenantId_QuestionPaperId_ReviewedOn",
                table: "QuestionPaperModerations",
                columns: new[] { "TenantId", "QuestionPaperId", "ReviewedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_ApprovedByUserId",
                table: "ResultPublicationBatches",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_BranchId",
                table: "ResultPublicationBatches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_ExamId",
                table: "ResultPublicationBatches",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_PublishedByUserId",
                table: "ResultPublicationBatches",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_RequestedByUserId",
                table: "ResultPublicationBatches",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_SectionId",
                table: "ResultPublicationBatches",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultPublicationBatches_TenantId_BranchId_ExamId_SectionId_Status",
                table: "ResultPublicationBatches",
                columns: new[] { "TenantId", "BranchId", "ExamId", "SectionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultRankingRules_BranchId",
                table: "ResultRankingRules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultRankingRules_ExamId",
                table: "ResultRankingRules",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultRankingRules_SectionId",
                table: "ResultRankingRules",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultRankingRules_TenantId_ExamId_SectionId",
                table: "ResultRankingRules",
                columns: new[] { "TenantId", "ExamId", "SectionId" },
                unique: true,
                filter: "[SectionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionPaperModerations");

            migrationBuilder.DropTable(
                name: "ResultPublicationBatches");

            migrationBuilder.DropTable(
                name: "ResultRankingRules");

            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Percentile",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Results");
        }
    }
}
