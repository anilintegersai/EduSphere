using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GuidKeysAuditSoftDelete : Migration
    {
        private const string EmptyGuid = "00000000-0000-0000-0000-000000000000";

        private static readonly string[] KeyTables =
        {
            "Tenants",
            "Branches",
            "AcademicYears",
            "Departments",
            "Courses",
            "Batches",
            "Sections",
            "Subjects",
            "SyllabusUnits"
        };

        private static readonly string[] TenantOwnedTables =
        {
            "Branches",
            "AcademicYears",
            "Departments",
            "Courses",
            "Batches",
            "Sections",
            "Subjects",
            "SyllabusUnits"
        };

        private static readonly string[] AcademicTables =
        {
            "AcademicYears",
            "Departments",
            "Courses",
            "Batches",
            "Sections",
            "Subjects",
            "SyllabusUnits"
        };

        private static readonly string[] AuditedTables =
        {
            "Tenants",
            "Branches",
            "AcademicYears",
            "Departments",
            "Courses",
            "Batches",
            "Sections",
            "Subjects",
            "SyllabusUnits",
            "AspNetUsers"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropRelationshipConstraints(migrationBuilder);

            foreach (var table in AuditedTables)
            {
                migrationBuilder.RenameColumn("CreatedAt", table, "CreatedOn");
                migrationBuilder.RenameColumn("UpdatedAt", table, "ModifiedOn");
                AddAuditColumns(migrationBuilder, table);
                AddConcurrencyToken(migrationBuilder, table);
            }

            AddSoftDeleteColumns(migrationBuilder);
            AddGuidMappingColumns(migrationBuilder);
            PopulateGuidMappings(migrationBuilder);
            ReplaceIntKeysWithGuidKeys(migrationBuilder);
            RecreateRelationshipConstraints(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "Rolling back GuidKeysAuditSoftDelete would require restoring the previous int key mapping. Restore from backup instead.");
        }

        private static void DropRelationshipConstraints(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_AspNetUsers_Branches_BranchId", "AspNetUsers");
            migrationBuilder.DropForeignKey("FK_Branches_Tenants_TenantId", "Branches");
            migrationBuilder.DropForeignKey("FK_Batches_AcademicYears_AcademicYearId", "Batches");
            migrationBuilder.DropForeignKey("FK_Batches_Courses_CourseId", "Batches");
            migrationBuilder.DropForeignKey("FK_Courses_Departments_DepartmentId", "Courses");
            migrationBuilder.DropForeignKey("FK_Sections_Batches_BatchId", "Sections");
            migrationBuilder.DropForeignKey("FK_Subjects_Courses_CourseId", "Subjects");
            migrationBuilder.DropForeignKey("FK_SyllabusUnits_Subjects_SubjectId", "SyllabusUnits");

            migrationBuilder.DropIndex("IX_AspNetUsers_BranchId", "AspNetUsers");
            migrationBuilder.DropIndex("IX_AspNetUsers_TenantId", "AspNetUsers");
            migrationBuilder.DropIndex("IX_Branches_TenantId", "Branches");
            migrationBuilder.DropIndex("IX_AcademicYears_TenantId_Name", "AcademicYears");
            migrationBuilder.DropIndex("IX_Batches_AcademicYearId", "Batches");
            migrationBuilder.DropIndex("IX_Batches_CourseId", "Batches");
            migrationBuilder.DropIndex("IX_Batches_TenantId", "Batches");
            migrationBuilder.DropIndex("IX_Courses_DepartmentId", "Courses");
            migrationBuilder.DropIndex("IX_Courses_TenantId_Code", "Courses");
            migrationBuilder.DropIndex("IX_Departments_TenantId_Code", "Departments");
            migrationBuilder.DropIndex("IX_Sections_BatchId", "Sections");
            migrationBuilder.DropIndex("IX_Subjects_CourseId", "Subjects");
            migrationBuilder.DropIndex("IX_Subjects_TenantId_Code", "Subjects");
            migrationBuilder.DropIndex("IX_SyllabusUnits_SubjectId", "SyllabusUnits");

            foreach (var table in KeyTables)
                migrationBuilder.DropPrimaryKey($"PK_{table}", table);
        }

        private static void AddAuditColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.Sql($"""
                ALTER TABLE [{table}] ADD [CreatedBy] nvarchar(128) NULL;
                ALTER TABLE [{table}] ADD [ModifiedBy] nvarchar(128) NULL;
                """);
        }

        private static void AddConcurrencyToken(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [ConcurrencyToken] uniqueidentifier NULL;");
            migrationBuilder.Sql($"UPDATE [{table}] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;");
            migrationBuilder.Sql($"ALTER TABLE [{table}] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;");
        }

        private static void AddSoftDeleteColumns(MigrationBuilder migrationBuilder)
        {
            AddSoftDeleteColumn(migrationBuilder, "Tenants", "CAST(0 AS bit)");
            AddSoftDeleteColumn(migrationBuilder, "Branches", "CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END");

            foreach (var table in AcademicTables)
            {
                AddSoftDeleteColumn(migrationBuilder, table, "CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END");
                migrationBuilder.DropColumn("IsActive", table);
            }
        }

        private static void AddSoftDeleteColumn(MigrationBuilder migrationBuilder, string table, string expression)
        {
            migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [IsDeleted] bit NULL;");
            migrationBuilder.Sql($"UPDATE [{table}] SET [IsDeleted] = {expression};");
            migrationBuilder.Sql($"ALTER TABLE [{table}] ALTER COLUMN [IsDeleted] bit NOT NULL;");
            migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [DeletedBy] nvarchar(max) NULL;");
            migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [DeletedOn] datetime2 NULL;");
        }

        private static void AddGuidMappingColumns(MigrationBuilder migrationBuilder)
        {
            foreach (var table in KeyTables)
                AddRequiredGuidColumn(migrationBuilder, table, "NewId", "NEWID()");

            foreach (var table in TenantOwnedTables)
                migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [NewTenantId] uniqueidentifier NULL;");

            migrationBuilder.Sql("""
                ALTER TABLE [AspNetUsers] ADD [NewTenantId] uniqueidentifier NULL;
                ALTER TABLE [AspNetUsers] ADD [NewBranchId] uniqueidentifier NULL;
                ALTER TABLE [Courses] ADD [NewDepartmentId] uniqueidentifier NULL;
                ALTER TABLE [Batches] ADD [NewCourseId] uniqueidentifier NULL;
                ALTER TABLE [Batches] ADD [NewAcademicYearId] uniqueidentifier NULL;
                ALTER TABLE [Sections] ADD [NewBatchId] uniqueidentifier NULL;
                ALTER TABLE [Subjects] ADD [NewCourseId] uniqueidentifier NULL;
                ALTER TABLE [SyllabusUnits] ADD [NewSubjectId] uniqueidentifier NULL;
                """);
        }

        private static void PopulateGuidMappings(MigrationBuilder migrationBuilder)
        {
            foreach (var table in TenantOwnedTables)
            {
                migrationBuilder.Sql($"""
                    UPDATE target
                    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '{EmptyGuid}'))
                    FROM [{table}] AS target
                    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

                    ALTER TABLE [{table}] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
                    """);
            }

            migrationBuilder.Sql($"""
                UPDATE target
                SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '{EmptyGuid}'))
                FROM [AspNetUsers] AS target
                LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

                ALTER TABLE [AspNetUsers] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;

                UPDATE target
                SET [NewBranchId] = branch.[NewId]
                FROM [AspNetUsers] AS target
                INNER JOIN [Branches] AS branch ON target.[BranchId] = branch.[Id]
                WHERE target.[BranchId] IS NOT NULL;
                """);

            PopulateOptionalReference(migrationBuilder, "Courses", "DepartmentId", "Departments", "NewDepartmentId");
            PopulateRequiredReference(migrationBuilder, "Batches", "CourseId", "Courses", "NewCourseId");
            PopulateRequiredReference(migrationBuilder, "Batches", "AcademicYearId", "AcademicYears", "NewAcademicYearId");
            PopulateRequiredReference(migrationBuilder, "Sections", "BatchId", "Batches", "NewBatchId");
            PopulateOptionalReference(migrationBuilder, "Subjects", "CourseId", "Courses", "NewCourseId");
            PopulateRequiredReference(migrationBuilder, "SyllabusUnits", "SubjectId", "Subjects", "NewSubjectId");
        }

        private static void PopulateOptionalReference(
            MigrationBuilder migrationBuilder,
            string table,
            string oldColumn,
            string principalTable,
            string newColumn)
        {
            migrationBuilder.Sql($"""
                UPDATE target
                SET [{newColumn}] = principal.[NewId]
                FROM [{table}] AS target
                INNER JOIN [{principalTable}] AS principal ON target.[{oldColumn}] = principal.[Id]
                WHERE target.[{oldColumn}] IS NOT NULL;
                """);
        }

        private static void PopulateRequiredReference(
            MigrationBuilder migrationBuilder,
            string table,
            string oldColumn,
            string principalTable,
            string newColumn)
        {
            PopulateOptionalReference(migrationBuilder, table, oldColumn, principalTable, newColumn);
            migrationBuilder.Sql($"ALTER TABLE [{table}] ALTER COLUMN [{newColumn}] uniqueidentifier NOT NULL;");
        }

        private static void ReplaceIntKeysWithGuidKeys(MigrationBuilder migrationBuilder)
        {
            foreach (var table in TenantOwnedTables)
                ReplaceColumn(migrationBuilder, table, "TenantId", "NewTenantId");

            ReplaceColumn(migrationBuilder, "AspNetUsers", "TenantId", "NewTenantId");
            ReplaceColumn(migrationBuilder, "AspNetUsers", "BranchId", "NewBranchId");
            ReplaceColumn(migrationBuilder, "Courses", "DepartmentId", "NewDepartmentId");
            ReplaceColumn(migrationBuilder, "Batches", "CourseId", "NewCourseId");
            ReplaceColumn(migrationBuilder, "Batches", "AcademicYearId", "NewAcademicYearId");
            ReplaceColumn(migrationBuilder, "Sections", "BatchId", "NewBatchId");
            ReplaceColumn(migrationBuilder, "Subjects", "CourseId", "NewCourseId");
            ReplaceColumn(migrationBuilder, "SyllabusUnits", "SubjectId", "NewSubjectId");

            foreach (var table in KeyTables)
                ReplaceColumn(migrationBuilder, table, "Id", "NewId");
        }

        private static void RecreateRelationshipConstraints(MigrationBuilder migrationBuilder)
        {
            foreach (var table in KeyTables)
                migrationBuilder.AddPrimaryKey($"PK_{table}", table, "Id");

            migrationBuilder.CreateIndex("IX_AspNetUsers_BranchId", "AspNetUsers", "BranchId");
            migrationBuilder.CreateIndex("IX_AspNetUsers_TenantId", "AspNetUsers", "TenantId");
            migrationBuilder.CreateIndex("IX_Branches_TenantId", "Branches", "TenantId");
            migrationBuilder.CreateIndex("IX_AcademicYears_TenantId_Name", "AcademicYears", new[] { "TenantId", "Name" }, unique: true);
            migrationBuilder.CreateIndex("IX_Batches_AcademicYearId", "Batches", "AcademicYearId");
            migrationBuilder.CreateIndex("IX_Batches_CourseId", "Batches", "CourseId");
            migrationBuilder.CreateIndex("IX_Batches_TenantId", "Batches", "TenantId");
            migrationBuilder.CreateIndex("IX_Courses_DepartmentId", "Courses", "DepartmentId");
            migrationBuilder.CreateIndex("IX_Courses_TenantId_Code", "Courses", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_Departments_TenantId_Code", "Departments", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_Sections_BatchId", "Sections", "BatchId");
            migrationBuilder.CreateIndex("IX_Subjects_CourseId", "Subjects", "CourseId");
            migrationBuilder.CreateIndex("IX_Subjects_TenantId_Code", "Subjects", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_SyllabusUnits_SubjectId", "SyllabusUnits", "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Tenants_TenantId",
                table: "Branches",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_AcademicYears_AcademicYearId",
                table: "Batches",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Courses_CourseId",
                table: "Batches",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Batches_BatchId",
                table: "Sections",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Courses_CourseId",
                table: "Subjects",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SyllabusUnits_Subjects_SubjectId",
                table: "SyllabusUnits",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        private static void AddRequiredGuidColumn(MigrationBuilder migrationBuilder, string table, string column, string expression)
        {
            migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [{column}] uniqueidentifier NULL;");
            migrationBuilder.Sql($"UPDATE [{table}] SET [{column}] = {expression};");
            migrationBuilder.Sql($"ALTER TABLE [{table}] ALTER COLUMN [{column}] uniqueidentifier NOT NULL;");
        }

        private static void ReplaceColumn(MigrationBuilder migrationBuilder, string table, string oldColumn, string newColumn)
        {
            migrationBuilder.DropColumn(oldColumn, table);
            migrationBuilder.RenameColumn(newColumn, table, oldColumn);
        }
    }
}
