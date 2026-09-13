using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdmissionsWorkflowPolish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "AdmissionDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedByUserId",
                table: "AdmissionDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedOn",
                table: "AdmissionDocuments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<Guid>(
                name: "AdmissionFormTemplateId",
                table: "AdmissionApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnrolledStudentProfileId",
                table: "AdmissionApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormResponseJson",
                table: "AdmissionApplications",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdmissionFormTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_AdmissionFormTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionFormTemplates_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionFormTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionFormTemplates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionDocumentRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionFormTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_AdmissionDocumentRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionDocumentRequirements_AdmissionFormTemplates_AdmissionFormTemplateId",
                        column: x => x.AdmissionFormTemplateId,
                        principalTable: "AdmissionFormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionFormFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionFormTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Placeholder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HelpText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OptionsJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValidationRegex = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_AdmissionFormFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionFormFields_AdmissionFormTemplates_AdmissionFormTemplateId",
                        column: x => x.AdmissionFormTemplateId,
                        principalTable: "AdmissionFormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocuments_UploadedByUserId",
                table: "AdmissionDocuments",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_AdmissionFormTemplateId",
                table: "AdmissionApplications",
                column: "AdmissionFormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_EnrolledStudentProfileId",
                table: "AdmissionApplications",
                column: "EnrolledStudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocumentRequirements_AdmissionFormTemplateId_DocumentType",
                table: "AdmissionDocumentRequirements",
                columns: new[] { "AdmissionFormTemplateId", "DocumentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionDocumentRequirements_AdmissionFormTemplateId_SortOrder",
                table: "AdmissionDocumentRequirements",
                columns: new[] { "AdmissionFormTemplateId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormFields_AdmissionFormTemplateId_FieldKey",
                table: "AdmissionFormFields",
                columns: new[] { "AdmissionFormTemplateId", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormFields_AdmissionFormTemplateId_SortOrder",
                table: "AdmissionFormFields",
                columns: new[] { "AdmissionFormTemplateId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormTemplates_AcademicYearId",
                table: "AdmissionFormTemplates",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormTemplates_BranchId",
                table: "AdmissionFormTemplates",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormTemplates_CourseId",
                table: "AdmissionFormTemplates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormTemplates_TenantId_BranchId_CourseId_AcademicYearId_IsActive",
                table: "AdmissionFormTemplates",
                columns: new[] { "TenantId", "BranchId", "CourseId", "AcademicYearId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionFormTemplates_TenantId_Name",
                table: "AdmissionFormTemplates",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionApplications_AdmissionFormTemplates_AdmissionFormTemplateId",
                table: "AdmissionApplications",
                column: "AdmissionFormTemplateId",
                principalTable: "AdmissionFormTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionApplications_StudentProfiles_EnrolledStudentProfileId",
                table: "AdmissionApplications",
                column: "EnrolledStudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionDocuments_AspNetUsers_UploadedByUserId",
                table: "AdmissionDocuments",
                column: "UploadedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionApplications_AdmissionFormTemplates_AdmissionFormTemplateId",
                table: "AdmissionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionApplications_StudentProfiles_EnrolledStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionDocuments_AspNetUsers_UploadedByUserId",
                table: "AdmissionDocuments");

            migrationBuilder.DropTable(
                name: "AdmissionDocumentRequirements");

            migrationBuilder.DropTable(
                name: "AdmissionFormFields");

            migrationBuilder.DropTable(
                name: "AdmissionFormTemplates");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionDocuments_UploadedByUserId",
                table: "AdmissionDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_AdmissionFormTemplateId",
                table: "AdmissionApplications");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionApplications_EnrolledStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "UploadedOn",
                table: "AdmissionDocuments");

            migrationBuilder.DropColumn(
                name: "AdmissionFormTemplateId",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "EnrolledStudentProfileId",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "FormResponseJson",
                table: "AdmissionApplications");
        }
    }
}
