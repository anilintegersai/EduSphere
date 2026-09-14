BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [StudentAlumniRecords] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [BatchId] uniqueidentifier NULL,
        [AlumniNumber] nvarchar(50) NOT NULL,
        [GraduationDate] date NOT NULL,
        [ContactEmail] nvarchar(150) NULL,
        [ContactPhone] nvarchar(30) NULL,
        [HigherEducation] nvarchar(250) NULL,
        [EmployerOrInstitution] nvarchar(250) NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentAlumniRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentAlumniRecords_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [StudentLifecycleRequests] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [ToStatus] int NOT NULL,
        [Status] int NOT NULL,
        [ToBranchId] uniqueidentifier NULL,
        [ToAcademicYearId] uniqueidentifier NULL,
        [ToCourseId] uniqueidentifier NULL,
        [ToBatchId] uniqueidentifier NULL,
        [ToSectionId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [DecidedOn] datetime2 NULL,
        [DecidedByUserId] uniqueidentifier NULL,
        [AppliedStudentLifecycleEventId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
        [Notes] nvarchar(1000) NULL,
        [DecisionNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentLifecycleRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentLifecycleRequests_AcademicYears_ToAcademicYearId] FOREIGN KEY ([ToAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_AspNetUsers_DecidedByUserId] FOREIGN KEY ([DecidedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Batches_ToBatchId] FOREIGN KEY ([ToBatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Courses_ToCourseId] FOREIGN KEY ([ToCourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Sections_ToSectionId] FOREIGN KEY ([ToSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_StudentLifecycleEvents_AppliedStudentLifecycleEventId] FOREIGN KEY ([AppliedStudentLifecycleEventId]) REFERENCES [StudentLifecycleEvents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [TeacherLifecycleRequests] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [ToStatus] int NOT NULL,
        [Status] int NOT NULL,
        [ToBranchId] uniqueidentifier NULL,
        [ToDepartmentId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [DecidedOn] datetime2 NULL,
        [DecidedByUserId] uniqueidentifier NULL,
        [AppliedTeacherLifecycleEventId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
        [Notes] nvarchar(1000) NULL,
        [DecisionNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherLifecycleRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherLifecycleRequests_AspNetUsers_DecidedByUserId] FOREIGN KEY ([DecidedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Departments_ToDepartmentId] FOREIGN KEY ([ToDepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_TeacherLifecycleEvents_AppliedTeacherLifecycleEventId] FOREIGN KEY ([AppliedTeacherLifecycleEventId]) REFERENCES [TeacherLifecycleEvents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_AcademicYearId] ON [StudentAlumniRecords] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_BatchId] ON [StudentAlumniRecords] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_BranchId] ON [StudentAlumniRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_CourseId] ON [StudentAlumniRecords] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_StudentProfileId] ON [StudentAlumniRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentAlumniRecords_TenantId_AlumniNumber] ON [StudentAlumniRecords] ([TenantId], [AlumniNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_TenantId_BranchId_GraduationDate] ON [StudentAlumniRecords] ([TenantId], [BranchId], [GraduationDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentAlumniRecords_TenantId_StudentProfileId] ON [StudentAlumniRecords] ([TenantId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_AppliedStudentLifecycleEventId] ON [StudentLifecycleRequests] ([AppliedStudentLifecycleEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_BranchId] ON [StudentLifecycleRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_DecidedByUserId] ON [StudentLifecycleRequests] ([DecidedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_RequestedByUserId] ON [StudentLifecycleRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_StudentProfileId] ON [StudentLifecycleRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_TenantId_BranchId_Status_RequestedOn] ON [StudentLifecycleRequests] ([TenantId], [BranchId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_TenantId_StudentProfileId_Status] ON [StudentLifecycleRequests] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToAcademicYearId] ON [StudentLifecycleRequests] ([ToAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToBatchId] ON [StudentLifecycleRequests] ([ToBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToBranchId] ON [StudentLifecycleRequests] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToCourseId] ON [StudentLifecycleRequests] ([ToCourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToSectionId] ON [StudentLifecycleRequests] ([ToSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_AppliedTeacherLifecycleEventId] ON [TeacherLifecycleRequests] ([AppliedTeacherLifecycleEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_BranchId] ON [TeacherLifecycleRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_DecidedByUserId] ON [TeacherLifecycleRequests] ([DecidedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_RequestedByUserId] ON [TeacherLifecycleRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TeacherProfileId] ON [TeacherLifecycleRequests] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TenantId_BranchId_Status_RequestedOn] ON [TeacherLifecycleRequests] ([TenantId], [BranchId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TenantId_TeacherProfileId_Status] ON [TeacherLifecycleRequests] ([TenantId], [TeacherProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_ToBranchId] ON [TeacherLifecycleRequests] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_ToDepartmentId] ON [TeacherLifecycleRequests] ([ToDepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260914221820_StudentTeacherLifecycleWorkflowCompletion', N'9.0.0');
END;

COMMIT;
GO

