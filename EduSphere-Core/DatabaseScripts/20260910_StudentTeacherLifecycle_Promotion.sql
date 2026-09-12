/*
    EduSphere Student/Teacher lifecycle promotion script
    Target: SQL Server
    Assumes prior migrations through 20260908204854_UserManagement are already applied.
*/

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'[dbo].[StudentProfiles]', N'U') IS NULL
        THROW 51000, 'StudentProfiles table was not found. Apply earlier EduSphere migrations first.', 1;

    IF OBJECT_ID(N'[dbo].[TeacherProfiles]', N'U') IS NULL
        THROW 51001, 'TeacherProfiles table was not found. Apply earlier EduSphere migrations first.', 1;

    IF COL_LENGTH(N'[dbo].[TeacherProfiles]', N'DepartmentId') IS NULL
    BEGIN
        ALTER TABLE [dbo].[TeacherProfiles]
            ADD [DepartmentId] uniqueidentifier NULL;
    END;

    IF OBJECT_ID(N'[dbo].[StudentLifecycleEvents]', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[StudentLifecycleEvents]
        (
            [Id] uniqueidentifier NOT NULL,
            [StudentProfileId] uniqueidentifier NOT NULL,
            [BranchId] uniqueidentifier NOT NULL,
            [EventType] int NOT NULL,
            [FromStatus] int NOT NULL,
            [ToStatus] int NOT NULL,
            [FromBranchId] uniqueidentifier NULL,
            [ToBranchId] uniqueidentifier NULL,
            [FromAcademicYearId] uniqueidentifier NULL,
            [ToAcademicYearId] uniqueidentifier NULL,
            [FromCourseId] uniqueidentifier NULL,
            [ToCourseId] uniqueidentifier NULL,
            [FromBatchId] uniqueidentifier NULL,
            [ToBatchId] uniqueidentifier NULL,
            [FromSectionId] uniqueidentifier NULL,
            [ToSectionId] uniqueidentifier NULL,
            [EffectiveOn] date NOT NULL,
            [RecordedOn] datetime2 NOT NULL,
            [RecordedByUserId] uniqueidentifier NULL,
            [Reason] nvarchar(500) NULL,
            [Notes] nvarchar(1000) NULL,
            [CreatedBy] nvarchar(128) NULL,
            [CreatedOn] datetime2 NOT NULL,
            [ModifiedBy] nvarchar(128) NULL,
            [ModifiedOn] datetime2 NULL,
            [IsDeleted] bit NOT NULL,
            [DeletedBy] nvarchar(max) NULL,
            [DeletedOn] datetime2 NULL,
            [ConcurrencyToken] uniqueidentifier NOT NULL,
            [TenantId] uniqueidentifier NOT NULL,
            CONSTRAINT [PK_StudentLifecycleEvents] PRIMARY KEY ([Id])
        );
    END;

    IF OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[TeacherLifecycleEvents]
        (
            [Id] uniqueidentifier NOT NULL,
            [TeacherProfileId] uniqueidentifier NOT NULL,
            [BranchId] uniqueidentifier NOT NULL,
            [EventType] int NOT NULL,
            [FromStatus] int NOT NULL,
            [ToStatus] int NOT NULL,
            [FromBranchId] uniqueidentifier NULL,
            [ToBranchId] uniqueidentifier NULL,
            [FromDepartmentId] uniqueidentifier NULL,
            [ToDepartmentId] uniqueidentifier NULL,
            [EffectiveOn] date NOT NULL,
            [RecordedOn] datetime2 NOT NULL,
            [RecordedByUserId] uniqueidentifier NULL,
            [Reason] nvarchar(500) NULL,
            [Notes] nvarchar(1000) NULL,
            [CreatedBy] nvarchar(128) NULL,
            [CreatedOn] datetime2 NOT NULL,
            [ModifiedBy] nvarchar(128) NULL,
            [ModifiedOn] datetime2 NULL,
            [IsDeleted] bit NOT NULL,
            [DeletedBy] nvarchar(max) NULL,
            [DeletedOn] datetime2 NULL,
            [ConcurrencyToken] uniqueidentifier NOT NULL,
            [TenantId] uniqueidentifier NOT NULL,
            CONSTRAINT [PK_TeacherLifecycleEvents] PRIMARY KEY ([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherProfiles_DepartmentId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherProfiles]'))
        CREATE INDEX [IX_TeacherProfiles_DepartmentId] ON [dbo].[TeacherProfiles] ([DepartmentId]);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherProfiles_Departments_DepartmentId')
        ALTER TABLE [dbo].[TeacherProfiles] ADD CONSTRAINT [FK_TeacherProfiles_Departments_DepartmentId]
            FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_StudentProfiles_StudentProfileId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_StudentProfiles_StudentProfileId]
            FOREIGN KEY ([StudentProfileId]) REFERENCES [dbo].[StudentProfiles] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Branches_BranchId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Branches_BranchId]
            FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Branches_FromBranchId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Branches_FromBranchId]
            FOREIGN KEY ([FromBranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Branches_ToBranchId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Branches_ToBranchId]
            FOREIGN KEY ([ToBranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_AcademicYears_FromAcademicYearId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_AcademicYears_FromAcademicYearId]
            FOREIGN KEY ([FromAcademicYearId]) REFERENCES [dbo].[AcademicYears] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_AcademicYears_ToAcademicYearId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_AcademicYears_ToAcademicYearId]
            FOREIGN KEY ([ToAcademicYearId]) REFERENCES [dbo].[AcademicYears] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Courses_FromCourseId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Courses_FromCourseId]
            FOREIGN KEY ([FromCourseId]) REFERENCES [dbo].[Courses] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Courses_ToCourseId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Courses_ToCourseId]
            FOREIGN KEY ([ToCourseId]) REFERENCES [dbo].[Courses] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Batches_FromBatchId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Batches_FromBatchId]
            FOREIGN KEY ([FromBatchId]) REFERENCES [dbo].[Batches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Batches_ToBatchId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Batches_ToBatchId]
            FOREIGN KEY ([ToBatchId]) REFERENCES [dbo].[Batches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Sections_FromSectionId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Sections_FromSectionId]
            FOREIGN KEY ([FromSectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_Sections_ToSectionId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_Sections_ToSectionId]
            FOREIGN KEY ([ToSectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_StudentLifecycleEvents_AspNetUsers_RecordedByUserId')
        ALTER TABLE [dbo].[StudentLifecycleEvents] ADD CONSTRAINT [FK_StudentLifecycleEvents_AspNetUsers_RecordedByUserId]
            FOREIGN KEY ([RecordedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_TeacherProfiles_TeacherProfileId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_TeacherProfiles_TeacherProfileId]
            FOREIGN KEY ([TeacherProfileId]) REFERENCES [dbo].[TeacherProfiles] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_Branches_BranchId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_Branches_BranchId]
            FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_Branches_FromBranchId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_Branches_FromBranchId]
            FOREIGN KEY ([FromBranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_Branches_ToBranchId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_Branches_ToBranchId]
            FOREIGN KEY ([ToBranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_Departments_FromDepartmentId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_Departments_FromDepartmentId]
            FOREIGN KEY ([FromDepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_Departments_ToDepartmentId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_Departments_ToDepartmentId]
            FOREIGN KEY ([ToDepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_TeacherLifecycleEvents_AspNetUsers_RecordedByUserId')
        ALTER TABLE [dbo].[TeacherLifecycleEvents] ADD CONSTRAINT [FK_TeacherLifecycleEvents_AspNetUsers_RecordedByUserId]
            FOREIGN KEY ([RecordedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_BranchId] ON [dbo].[StudentLifecycleEvents] ([BranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_FromAcademicYearId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_FromAcademicYearId] ON [dbo].[StudentLifecycleEvents] ([FromAcademicYearId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_FromBatchId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_FromBatchId] ON [dbo].[StudentLifecycleEvents] ([FromBatchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_FromBranchId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_FromBranchId] ON [dbo].[StudentLifecycleEvents] ([FromBranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_FromCourseId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_FromCourseId] ON [dbo].[StudentLifecycleEvents] ([FromCourseId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_FromSectionId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_FromSectionId] ON [dbo].[StudentLifecycleEvents] ([FromSectionId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_RecordedByUserId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_RecordedByUserId] ON [dbo].[StudentLifecycleEvents] ([RecordedByUserId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_StudentProfileId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_StudentProfileId] ON [dbo].[StudentLifecycleEvents] ([StudentProfileId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn] ON [dbo].[StudentLifecycleEvents] ([TenantId], [BranchId], [EventType], [EffectiveOn]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_TenantId_StudentProfileId_EffectiveOn' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_TenantId_StudentProfileId_EffectiveOn] ON [dbo].[StudentLifecycleEvents] ([TenantId], [StudentProfileId], [EffectiveOn]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_ToAcademicYearId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_ToAcademicYearId] ON [dbo].[StudentLifecycleEvents] ([ToAcademicYearId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_ToBatchId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_ToBatchId] ON [dbo].[StudentLifecycleEvents] ([ToBatchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_ToBranchId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_ToBranchId] ON [dbo].[StudentLifecycleEvents] ([ToBranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_ToCourseId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_ToCourseId] ON [dbo].[StudentLifecycleEvents] ([ToCourseId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StudentLifecycleEvents_ToSectionId' AND [object_id] = OBJECT_ID(N'[dbo].[StudentLifecycleEvents]'))
        CREATE INDEX [IX_StudentLifecycleEvents_ToSectionId] ON [dbo].[StudentLifecycleEvents] ([ToSectionId]);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_BranchId] ON [dbo].[TeacherLifecycleEvents] ([BranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_FromBranchId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_FromBranchId] ON [dbo].[TeacherLifecycleEvents] ([FromBranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_FromDepartmentId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_FromDepartmentId] ON [dbo].[TeacherLifecycleEvents] ([FromDepartmentId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_RecordedByUserId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_RecordedByUserId] ON [dbo].[TeacherLifecycleEvents] ([RecordedByUserId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_TeacherProfileId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_TeacherProfileId] ON [dbo].[TeacherLifecycleEvents] ([TeacherProfileId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn] ON [dbo].[TeacherLifecycleEvents] ([TenantId], [BranchId], [EventType], [EffectiveOn]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_TenantId_TeacherProfileId_EffectiveOn' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_TenantId_TeacherProfileId_EffectiveOn] ON [dbo].[TeacherLifecycleEvents] ([TenantId], [TeacherProfileId], [EffectiveOn]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_ToBranchId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_ToBranchId] ON [dbo].[TeacherLifecycleEvents] ([ToBranchId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_TeacherLifecycleEvents_ToDepartmentId' AND [object_id] = OBJECT_ID(N'[dbo].[TeacherLifecycleEvents]'))
        CREATE INDEX [IX_TeacherLifecycleEvents_ToDepartmentId] ON [dbo].[TeacherLifecycleEvents] ([ToDepartmentId]);

    DECLARE @now datetime2 = SYSUTCDATETIME();

    UPDATE tp
    SET DepartmentId = derived.DepartmentId,
        ModifiedOn = @now,
        ModifiedBy = N'lifecycle-seed',
        ConcurrencyToken = NEWID()
    FROM [dbo].[TeacherProfiles] tp
    OUTER APPLY
    (
        SELECT TOP (1) c.DepartmentId
        FROM [dbo].[TeacherSubjectAssignments] tsa
        INNER JOIN [dbo].[Subjects] s ON s.Id = tsa.SubjectId AND s.IsDeleted = 0
        INNER JOIN [dbo].[Courses] c ON c.Id = s.CourseId AND c.IsDeleted = 0
        WHERE tsa.TeacherProfileId = tp.Id
          AND tsa.IsDeleted = 0
          AND c.DepartmentId IS NOT NULL
        ORDER BY tsa.IsPrimary DESC, tsa.EffectiveFrom DESC
    ) derived
    WHERE tp.IsDeleted = 0
      AND tp.DepartmentId IS NULL
      AND derived.DepartmentId IS NOT NULL;

    INSERT INTO [dbo].[StudentLifecycleEvents]
    (
        Id, StudentProfileId, BranchId, EventType, FromStatus, ToStatus,
        FromBranchId, ToBranchId, FromAcademicYearId, ToAcademicYearId,
        FromCourseId, ToCourseId, FromBatchId, ToBatchId, FromSectionId, ToSectionId,
        EffectiveOn, RecordedOn, RecordedByUserId, Reason, Notes,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsDeleted, DeletedBy, DeletedOn,
        ConcurrencyToken, TenantId
    )
    SELECT NEWID(), s.Id, s.BranchId, 0, s.Status, s.Status,
           s.BranchId, s.BranchId, placement.AcademicYearId, placement.AcademicYearId,
           placement.CourseId, placement.CourseId, placement.BatchId, placement.BatchId,
           placement.SectionId, placement.SectionId,
           s.AdmissionDate, COALESCE(NULLIF(s.CreatedOn, CONVERT(datetime2, '0001-01-01T00:00:00')), @now), s.UserId,
           N'Initial student profile created.',
           N'Backfilled during Student/Teacher lifecycle implementation.',
           N'lifecycle-seed', @now, NULL, NULL, 0, NULL, NULL,
           NEWID(), s.TenantId
    FROM [dbo].[StudentProfiles] s
    OUTER APPLY
    (
        SELECT TOP (1) e.AcademicYearId, e.CourseId, e.BatchId, e.SectionId
        FROM [dbo].[Enrollments] e
        WHERE e.StudentProfileId = s.Id
          AND e.IsDeleted = 0
        ORDER BY CASE WHEN e.Status = 0 THEN 0 ELSE 1 END, e.EnrollmentDate DESC
    ) enrollment
    LEFT JOIN [dbo].[Sections] sectionPlacement ON sectionPlacement.Id = COALESCE(enrollment.SectionId, s.SectionId) AND sectionPlacement.IsDeleted = 0
    LEFT JOIN [dbo].[Batches] batchPlacement ON batchPlacement.Id = COALESCE(enrollment.BatchId, sectionPlacement.BatchId) AND batchPlacement.IsDeleted = 0
    OUTER APPLY
    (
        SELECT COALESCE(enrollment.AcademicYearId, batchPlacement.AcademicYearId) AS AcademicYearId,
               COALESCE(enrollment.CourseId, batchPlacement.CourseId) AS CourseId,
               COALESCE(enrollment.BatchId, batchPlacement.Id) AS BatchId,
               COALESCE(enrollment.SectionId, s.SectionId) AS SectionId
    ) placement
    WHERE s.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[StudentLifecycleEvents] existing
          WHERE existing.StudentProfileId = s.Id
            AND existing.EventType = 0
            AND existing.IsDeleted = 0
      );

    INSERT INTO [dbo].[StudentLifecycleEvents]
    (
        Id, StudentProfileId, BranchId, EventType, FromStatus, ToStatus,
        FromBranchId, ToBranchId, FromAcademicYearId, ToAcademicYearId,
        FromCourseId, ToCourseId, FromBatchId, ToBatchId, FromSectionId, ToSectionId,
        EffectiveOn, RecordedOn, RecordedByUserId, Reason, Notes,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsDeleted, DeletedBy, DeletedOn,
        ConcurrencyToken, TenantId
    )
    SELECT NEWID(), s.Id, e.BranchId, 2, s.Status, s.Status,
           e.BranchId, e.BranchId, e.AcademicYearId, e.AcademicYearId,
           e.CourseId, e.CourseId, e.BatchId, e.BatchId, e.SectionId, e.SectionId,
           e.EnrollmentDate, @now, s.UserId,
           N'Student enrolled for academic year.',
           COALESCE(e.Notes, N'Backfilled from active enrollment during lifecycle implementation.'),
           N'lifecycle-seed', @now, NULL, NULL, 0, NULL, NULL,
           NEWID(), s.TenantId
    FROM [dbo].[Enrollments] e
    INNER JOIN [dbo].[StudentProfiles] s ON s.Id = e.StudentProfileId AND s.IsDeleted = 0
    WHERE e.IsDeleted = 0
      AND e.Status = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[StudentLifecycleEvents] existing
          WHERE existing.StudentProfileId = e.StudentProfileId
            AND existing.EventType = 2
            AND existing.ToAcademicYearId = e.AcademicYearId
            AND existing.IsDeleted = 0
      );

    INSERT INTO [dbo].[TeacherLifecycleEvents]
    (
        Id, TeacherProfileId, BranchId, EventType, FromStatus, ToStatus,
        FromBranchId, ToBranchId, FromDepartmentId, ToDepartmentId,
        EffectiveOn, RecordedOn, RecordedByUserId, Reason, Notes,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsDeleted, DeletedBy, DeletedOn,
        ConcurrencyToken, TenantId
    )
    SELECT NEWID(), tp.Id, tp.BranchId, 0, tp.Status, tp.Status,
           tp.BranchId, tp.BranchId, tp.DepartmentId, tp.DepartmentId,
           tp.JoiningDate, COALESCE(NULLIF(tp.CreatedOn, CONVERT(datetime2, '0001-01-01T00:00:00')), @now), tp.UserId,
           N'Initial teacher profile created.',
           N'Backfilled during Student/Teacher lifecycle implementation.',
           N'lifecycle-seed', @now, NULL, NULL, 0, NULL, NULL,
           NEWID(), tp.TenantId
    FROM [dbo].[TeacherProfiles] tp
    WHERE tp.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[TeacherLifecycleEvents] existing
          WHERE existing.TeacherProfileId = tp.Id
            AND existing.EventType = 0
            AND existing.IsDeleted = 0
      );

    INSERT INTO [dbo].[TeacherLifecycleEvents]
    (
        Id, TeacherProfileId, BranchId, EventType, FromStatus, ToStatus,
        FromBranchId, ToBranchId, FromDepartmentId, ToDepartmentId,
        EffectiveOn, RecordedOn, RecordedByUserId, Reason, Notes,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsDeleted, DeletedBy, DeletedOn,
        ConcurrencyToken, TenantId
    )
    SELECT NEWID(), tp.Id, tp.BranchId, 1, tp.Status, tp.Status,
           tp.BranchId, tp.BranchId, tp.DepartmentId, tp.DepartmentId,
           tp.JoiningDate, @now, tp.UserId,
           N'Teacher onboarded to branch.',
           N'Backfilled from teacher joining date during lifecycle implementation.',
           N'lifecycle-seed', @now, NULL, NULL, 0, NULL, NULL,
           NEWID(), tp.TenantId
    FROM [dbo].[TeacherProfiles] tp
    WHERE tp.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[TeacherLifecycleEvents] existing
          WHERE existing.TeacherProfileId = tp.Id
            AND existing.EventType = 1
            AND existing.IsDeleted = 0
      );

    INSERT INTO [dbo].[TeacherLifecycleEvents]
    (
        Id, TeacherProfileId, BranchId, EventType, FromStatus, ToStatus,
        FromBranchId, ToBranchId, FromDepartmentId, ToDepartmentId,
        EffectiveOn, RecordedOn, RecordedByUserId, Reason, Notes,
        CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsDeleted, DeletedBy, DeletedOn,
        ConcurrencyToken, TenantId
    )
    SELECT NEWID(), tp.Id, tp.BranchId, 5, tp.Status, tp.Status,
           tp.BranchId, tp.BranchId, tp.DepartmentId, tp.DepartmentId,
           COALESCE(firstAssignment.EffectiveFrom, tp.JoiningDate), @now, tp.UserId,
           N'Teaching assignment confirmed.',
           N'Backfilled from teacher subject assignments during lifecycle implementation.',
           N'lifecycle-seed', @now, NULL, NULL, 0, NULL, NULL,
           NEWID(), tp.TenantId
    FROM [dbo].[TeacherProfiles] tp
    CROSS APPLY
    (
        SELECT TOP (1) tsa.EffectiveFrom
        FROM [dbo].[TeacherSubjectAssignments] tsa
        WHERE tsa.TeacherProfileId = tp.Id
          AND tsa.IsDeleted = 0
        ORDER BY tsa.IsPrimary DESC, tsa.EffectiveFrom DESC
    ) firstAssignment
    WHERE tp.IsDeleted = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[TeacherLifecycleEvents] existing
          WHERE existing.TeacherProfileId = tp.Id
            AND existing.EventType = 5
            AND existing.IsDeleted = 0
      );

    IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle')
    BEGIN
        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260909231741_StudentTeacherLifecycle', N'9.0.0');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

SELECT N'TeacherProfilesWithDepartment' AS [Item], COUNT(*) AS [CountValue]
FROM [dbo].[TeacherProfiles]
WHERE IsDeleted = 0 AND DepartmentId IS NOT NULL
UNION ALL
SELECT N'StudentLifecycleEvents', COUNT(*)
FROM [dbo].[StudentLifecycleEvents]
WHERE IsDeleted = 0
UNION ALL
SELECT N'TeacherLifecycleEvents', COUNT(*)
FROM [dbo].[TeacherLifecycleEvents]
WHERE IsDeleted = 0;
