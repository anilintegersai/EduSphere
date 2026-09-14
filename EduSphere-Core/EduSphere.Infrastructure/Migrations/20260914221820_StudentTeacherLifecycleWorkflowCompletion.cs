using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentTeacherLifecycleWorkflowCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAlumniRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlumniNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GraduationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HigherEducation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EmployerOrInstitution = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_StudentAlumniRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAlumniRecords_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAlumniRecords_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAlumniRecords_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAlumniRecords_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAlumniRecords_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentLifecycleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ToBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToAcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppliedStudentLifecycleEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_StudentLifecycleRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_AcademicYears_ToAcademicYearId",
                        column: x => x.ToAcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_AspNetUsers_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_Batches_ToBatchId",
                        column: x => x.ToBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_Courses_ToCourseId",
                        column: x => x.ToCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_Sections_ToSectionId",
                        column: x => x.ToSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_StudentLifecycleEvents_AppliedStudentLifecycleEventId",
                        column: x => x.AppliedStudentLifecycleEventId,
                        principalTable: "StudentLifecycleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentLifecycleRequests_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherLifecycleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ToBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppliedTeacherLifecycleEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_TeacherLifecycleRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_AspNetUsers_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_Departments_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_TeacherLifecycleEvents_AppliedTeacherLifecycleEventId",
                        column: x => x.AppliedTeacherLifecycleEventId,
                        principalTable: "TeacherLifecycleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherLifecycleRequests_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_AcademicYearId",
                table: "StudentAlumniRecords",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_BatchId",
                table: "StudentAlumniRecords",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_BranchId",
                table: "StudentAlumniRecords",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_CourseId",
                table: "StudentAlumniRecords",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_StudentProfileId",
                table: "StudentAlumniRecords",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_TenantId_AlumniNumber",
                table: "StudentAlumniRecords",
                columns: new[] { "TenantId", "AlumniNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_TenantId_BranchId_GraduationDate",
                table: "StudentAlumniRecords",
                columns: new[] { "TenantId", "BranchId", "GraduationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniRecords_TenantId_StudentProfileId",
                table: "StudentAlumniRecords",
                columns: new[] { "TenantId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_AppliedStudentLifecycleEventId",
                table: "StudentLifecycleRequests",
                column: "AppliedStudentLifecycleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_BranchId",
                table: "StudentLifecycleRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_DecidedByUserId",
                table: "StudentLifecycleRequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_RequestedByUserId",
                table: "StudentLifecycleRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_StudentProfileId",
                table: "StudentLifecycleRequests",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_TenantId_BranchId_Status_RequestedOn",
                table: "StudentLifecycleRequests",
                columns: new[] { "TenantId", "BranchId", "Status", "RequestedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_TenantId_StudentProfileId_Status",
                table: "StudentLifecycleRequests",
                columns: new[] { "TenantId", "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_ToAcademicYearId",
                table: "StudentLifecycleRequests",
                column: "ToAcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_ToBatchId",
                table: "StudentLifecycleRequests",
                column: "ToBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_ToBranchId",
                table: "StudentLifecycleRequests",
                column: "ToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_ToCourseId",
                table: "StudentLifecycleRequests",
                column: "ToCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLifecycleRequests_ToSectionId",
                table: "StudentLifecycleRequests",
                column: "ToSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_AppliedTeacherLifecycleEventId",
                table: "TeacherLifecycleRequests",
                column: "AppliedTeacherLifecycleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_BranchId",
                table: "TeacherLifecycleRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_DecidedByUserId",
                table: "TeacherLifecycleRequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_RequestedByUserId",
                table: "TeacherLifecycleRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_TeacherProfileId",
                table: "TeacherLifecycleRequests",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_TenantId_BranchId_Status_RequestedOn",
                table: "TeacherLifecycleRequests",
                columns: new[] { "TenantId", "BranchId", "Status", "RequestedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_TenantId_TeacherProfileId_Status",
                table: "TeacherLifecycleRequests",
                columns: new[] { "TenantId", "TeacherProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_ToBranchId",
                table: "TeacherLifecycleRequests",
                column: "ToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLifecycleRequests_ToDepartmentId",
                table: "TeacherLifecycleRequests",
                column: "ToDepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAlumniRecords");

            migrationBuilder.DropTable(
                name: "StudentLifecycleRequests");

            migrationBuilder.DropTable(
                name: "TeacherLifecycleRequests");
        }
    }
}
