using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AIQuestionPaperGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIPromptTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SystemPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPromptTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputSchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AIPromptTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIPromptTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIProviderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModelOrDeployment = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProtectedApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiVersion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DataResidencyPolicy = table.Column<int>(type: "int", nullable: false),
                    RequiredRegion = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    InputCostPerMillionTokens = table.Column<decimal>(type: "decimal(12,6)", precision: 12, scale: 6, nullable: false),
                    OutputCostPerMillionTokens = table.Column<decimal>(type: "decimal(12,6)", precision: 12, scale: 6, nullable: false),
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
                    table.PrimaryKey("PK_AIProviderSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIGenerationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionPaperId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GeneratedVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromptTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegeneratedFromRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InputParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromptHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UsedManualFallback = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AIGenerationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_AIGenerationRequests_RegeneratedFromRequestId",
                        column: x => x.RegeneratedFromRequestId,
                        principalTable: "AIGenerationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_AIPromptTemplates_PromptTemplateId",
                        column: x => x.PromptTemplateId,
                        principalTable: "AIPromptTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_AIProviderSettings_ProviderSettingId",
                        column: x => x.ProviderSettingId,
                        principalTable: "AIProviderSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_QuestionPaperVersions_GeneratedVersionId",
                        column: x => x.GeneratedVersionId,
                        principalTable: "QuestionPaperVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_QuestionPapers_QuestionPaperId",
                        column: x => x.QuestionPaperId,
                        principalTable: "QuestionPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIGenerationRequests_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIGenerationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIGenerationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProviderRequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LatencyMilliseconds = table.Column<int>(type: "int", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AIGenerationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIGenerationLogs_AIGenerationRequests_AIGenerationRequestId",
                        column: x => x.AIGenerationRequestId,
                        principalTable: "AIGenerationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIGenerationResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIGenerationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentPaperJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarkingSchemeJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SchemaValidated = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AIGenerationResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIGenerationResponses_AIGenerationRequests_AIGenerationRequestId",
                        column: x => x.AIGenerationRequestId,
                        principalTable: "AIGenerationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIUsageRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIGenerationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InputTokens = table.Column<int>(type: "int", nullable: false),
                    OutputTokens = table.Column<int>(type: "int", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LatencyMilliseconds = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_AIUsageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIUsageRecords_AIGenerationRequests_AIGenerationRequestId",
                        column: x => x.AIGenerationRequestId,
                        principalTable: "AIGenerationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationLogs_AIGenerationRequestId_OccurredOn",
                table: "AIGenerationLogs",
                columns: new[] { "AIGenerationRequestId", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_BranchId",
                table: "AIGenerationRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_GeneratedVersionId",
                table: "AIGenerationRequests",
                column: "GeneratedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_PromptTemplateId",
                table: "AIGenerationRequests",
                column: "PromptTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_ProviderSettingId",
                table: "AIGenerationRequests",
                column: "ProviderSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_QuestionPaperId",
                table: "AIGenerationRequests",
                column: "QuestionPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_RegeneratedFromRequestId",
                table: "AIGenerationRequests",
                column: "RegeneratedFromRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_SubjectId",
                table: "AIGenerationRequests",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationRequests_TenantId_BranchId_Status_RequestedOn",
                table: "AIGenerationRequests",
                columns: new[] { "TenantId", "BranchId", "Status", "RequestedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AIGenerationResponses_AIGenerationRequestId",
                table: "AIGenerationResponses",
                column: "AIGenerationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIPromptTemplates_BranchId",
                table: "AIPromptTemplates",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AIPromptTemplates_TenantId_BranchId_Code_Version",
                table: "AIPromptTemplates",
                columns: new[] { "TenantId", "BranchId", "Code", "Version" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AIProviderSettings_TenantId_IsEnabled_IsDefault",
                table: "AIProviderSettings",
                columns: new[] { "TenantId", "IsEnabled", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_AIProviderSettings_TenantId_Name",
                table: "AIProviderSettings",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageRecords_AIGenerationRequestId",
                table: "AIUsageRecords",
                column: "AIGenerationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageRecords_TenantId_RecordedOn",
                table: "AIUsageRecords",
                columns: new[] { "TenantId", "RecordedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIGenerationLogs");

            migrationBuilder.DropTable(
                name: "AIGenerationResponses");

            migrationBuilder.DropTable(
                name: "AIUsageRecords");

            migrationBuilder.DropTable(
                name: "AIGenerationRequests");

            migrationBuilder.DropTable(
                name: "AIPromptTemplates");

            migrationBuilder.DropTable(
                name: "AIProviderSettings");
        }
    }
}
