using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentTeacherLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "TeacherProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentLifecycleEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    FromBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromAcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToAcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_StudentLifecycleEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_AcademicYears_FromAcademicYearId",
                        column: x => x.FromAcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_AcademicYears_ToAcademicYearId",
                        column: x => x.ToAcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Batches_FromBatchId",
                        column: x => x.FromBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Batches_ToBatchId",
                        column: x => x.ToBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Branches_FromBranchId",
                        column: x => x.FromBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Courses_FromCourseId",
                        column: x => x.FromCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Courses_ToCourseId",
                        column: x => x.ToCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Sections_FromSectionId",
                        column: x => x.FromSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_Sections_ToSectionId",
                        column: x => x.ToSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleEvents_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherLifecycleEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    FromBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TeacherLifecycleEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_Branches_FromBranchId",
                        column: x => x.FromBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_Departments_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_Departments_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleEvents_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_DepartmentId",
                table: "TeacherProfiles",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_BranchId",
                table: "StudentLifecycleEvents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_FromAcademicYearId",
                table: "StudentLifecycleEvents",
                column: "FromAcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_FromBatchId",
                table: "StudentLifecycleEvents",
                column: "FromBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_FromBranchId",
                table: "StudentLifecycleEvents",
                column: "FromBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_FromCourseId",
                table: "StudentLifecycleEvents",
                column: "FromCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_FromSectionId",
                table: "StudentLifecycleEvents",
                column: "FromSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_RecordedByUserId",
                table: "StudentLifecycleEvents",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_StudentProfileId",
                table: "StudentLifecycleEvents",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn",
                table: "StudentLifecycleEvents",
                columns: new[] { "TenantId", "BranchId", "EventType", "EffectiveOn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_TenantId_StudentProfileId_EffectiveOn",
                table: "StudentLifecycleEvents",
                columns: new[] { "TenantId", "StudentProfileId", "EffectiveOn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_ToAcademicYearId",
                table: "StudentLifecycleEvents",
                column: "ToAcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_ToBatchId",
                table: "StudentLifecycleEvents",
                column: "ToBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_ToBranchId",
                table: "StudentLifecycleEvents",
                column: "ToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_ToCourseId",
                table: "StudentLifecycleEvents",
                column: "ToCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleEvents_ToSectionId",
                table: "StudentLifecycleEvents",
                column: "ToSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_BranchId",
                table: "TeacherLifecycleEvents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_FromBranchId",
                table: "TeacherLifecycleEvents",
                column: "FromBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_FromDepartmentId",
                table: "TeacherLifecycleEvents",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_RecordedByUserId",
                table: "TeacherLifecycleEvents",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_TeacherProfileId",
                table: "TeacherLifecycleEvents",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn",
                table: "TeacherLifecycleEvents",
                columns: new[] { "TenantId", "BranchId", "EventType", "EffectiveOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_TenantId_TeacherProfileId_EffectiveOn",
                table: "TeacherLifecycleEvents",
                columns: new[] { "TenantId", "TeacherProfileId", "EffectiveOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_ToBranchId",
                table: "TeacherLifecycleEvents",
                column: "ToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleEvents_ToDepartmentId",
                table: "TeacherLifecycleEvents",
                column: "ToDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherProfiles_Departments_DepartmentId",
                table: "TeacherProfiles",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherProfiles_Departments_DepartmentId",
                table: "TeacherProfiles");

            migrationBuilder.DropTable(
                name: "StudentLifecycleEvents");

            migrationBuilder.DropTable(
                name: "TeacherLifecycleEvents");

            migrationBuilder.DropIndex(
                name: "IX_TeacherProfiles_DepartmentId",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "TeacherProfiles");
        }
    }
}
