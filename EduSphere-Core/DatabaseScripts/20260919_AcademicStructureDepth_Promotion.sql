BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [AcademicTerms] (
        [Id] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Type] int NOT NULL,
        [Sequence] int NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [IsCurrent] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AcademicTerms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AcademicTerms_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [LearningOutcomes] (
        [Id] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [SyllabusUnitId] uniqueidentifier NULL,
        [Code] nvarchar(40) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [BloomLevel] nvarchar(80) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LearningOutcomes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LearningOutcomes_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LearningOutcomes_SyllabusUnits_SyllabusUnitId] FOREIGN KEY ([SyllabusUnitId]) REFERENCES [SyllabusUnits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [SubjectReferenceBooks] (
        [Id] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Authors] nvarchar(200) NULL,
        [Isbn] nvarchar(30) NULL,
        [Publisher] nvarchar(150) NULL,
        [EditionYear] int NULL,
        [ResourceUrl] nvarchar(500) NULL,
        [IsPrimary] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SubjectReferenceBooks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubjectReferenceBooks_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [SyllabusTopics] (
        [Id] uniqueidentifier NOT NULL,
        [SyllabusUnitId] uniqueidentifier NOT NULL,
        [Order] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EstimatedMinutes] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SyllabusTopics] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SyllabusTopics_SyllabusUnits_SyllabusUnitId] FOREIGN KEY ([SyllabusUnitId]) REFERENCES [SyllabusUnits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TeacherAvailabilityRules] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartsAt] time NOT NULL,
        [EndsAt] time NOT NULL,
        [RuleType] int NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [Reason] nvarchar(300) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherAvailabilityRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherAvailabilityRules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherAvailabilityRules_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [AcademicCalendarEvents] (
        [Id] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [AcademicTermId] uniqueidentifier NULL,
        [Title] nvarchar(160) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EventType] int NOT NULL,
        [StartsOn] date NOT NULL,
        [EndsOn] date NOT NULL,
        [IsInstructionalDay] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AcademicCalendarEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AcademicCalendarEvents_AcademicTerms_AcademicTermId] FOREIGN KEY ([AcademicTermId]) REFERENCES [AcademicTerms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AcademicCalendarEvents_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AcademicCalendarEvents_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TeacherLessonPlans] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [AcademicTermId] uniqueidentifier NOT NULL,
        [SyllabusTopicId] uniqueidentifier NULL,
        [PlannedDate] date NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Objectives] nvarchar(2000) NULL,
        [Activities] nvarchar(2000) NULL,
        [Resources] nvarchar(1000) NULL,
        [Assessment] nvarchar(1000) NULL,
        [Status] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherLessonPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherLessonPlans_AcademicTerms_AcademicTermId] FOREIGN KEY ([AcademicTermId]) REFERENCES [AcademicTerms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_SyllabusTopics_SyllabusTopicId] FOREIGN KEY ([SyllabusTopicId]) REFERENCES [SyllabusTopics] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableGenerationRuns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [TimetableTemplateId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [StartedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [ScheduledPeriods] int NOT NULL,
        [UnscheduledPeriods] int NOT NULL,
        [Diagnostics] nvarchar(4000) NULL,
        [GeneratedTimetableId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableGenerationRuns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableGenerationRuns_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_TimetableTemplates_TimetableTemplateId] FOREIGN KEY ([TimetableTemplateId]) REFERENCES [TimetableTemplates] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Timetables_GeneratedTimetableId] FOREIGN KEY ([GeneratedTimetableId]) REFERENCES [Timetables] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableTemplateItems] (
        [Id] uniqueidentifier NOT NULL,
        [TimetableTemplateId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [PeriodsPerWeek] int NOT NULL,
        [MaximumPeriodsPerDay] int NOT NULL,
        [PreferredRoomId] uniqueidentifier NULL,
        [RequireConsecutivePeriods] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableTemplateItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableTemplateItems_Rooms_PreferredRoomId] FOREIGN KEY ([PreferredRoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableTemplateItems_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableTemplateItems_TimetableTemplates_TimetableTemplateId] FOREIGN KEY ([TimetableTemplateId]) REFERENCES [TimetableTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_AcademicTermId] ON [AcademicCalendarEvents] ([AcademicTermId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_AcademicYearId] ON [AcademicCalendarEvents] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_BranchId] ON [AcademicCalendarEvents] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_TenantId_AcademicYearId_StartsOn] ON [AcademicCalendarEvents] ([TenantId], [AcademicYearId], [StartsOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicTerms_AcademicYearId] ON [AcademicTerms] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AcademicTerms_TenantId_AcademicYearId_Sequence] ON [AcademicTerms] ([TenantId], [AcademicYearId], [Sequence]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_LearningOutcomes_SubjectId] ON [LearningOutcomes] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_LearningOutcomes_SyllabusUnitId] ON [LearningOutcomes] ([SyllabusUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LearningOutcomes_TenantId_SubjectId_Code] ON [LearningOutcomes] ([TenantId], [SubjectId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_SubjectReferenceBooks_SubjectId] ON [SubjectReferenceBooks] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_SubjectReferenceBooks_TenantId_SubjectId_Title] ON [SubjectReferenceBooks] ([TenantId], [SubjectId], [Title]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SyllabusTopics_SyllabusUnitId_Order] ON [SyllabusTopics] ([SyllabusUnitId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_BranchId] ON [TeacherAvailabilityRules] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_TeacherProfileId] ON [TeacherAvailabilityRules] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_TenantId_TeacherProfileId_DayOfWeek_StartsAt_EndsAt] ON [TeacherAvailabilityRules] ([TenantId], [TeacherProfileId], [DayOfWeek], [StartsAt], [EndsAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_AcademicTermId] ON [TeacherLessonPlans] ([AcademicTermId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SectionId] ON [TeacherLessonPlans] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SubjectId] ON [TeacherLessonPlans] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SyllabusTopicId] ON [TeacherLessonPlans] ([SyllabusTopicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_TeacherProfileId] ON [TeacherLessonPlans] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_TenantId_TeacherProfileId_PlannedDate] ON [TeacherLessonPlans] ([TenantId], [TeacherProfileId], [PlannedDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_AcademicYearId] ON [TimetableGenerationRuns] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_BranchId] ON [TimetableGenerationRuns] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_GeneratedTimetableId] ON [TimetableGenerationRuns] ([GeneratedTimetableId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_SectionId] ON [TimetableGenerationRuns] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_TenantId_BranchId_StartedOn] ON [TimetableGenerationRuns] ([TenantId], [BranchId], [StartedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_TimetableTemplateId] ON [TimetableGenerationRuns] ([TimetableTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplateItems_PreferredRoomId] ON [TimetableTemplateItems] ([PreferredRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplateItems_SubjectId] ON [TimetableTemplateItems] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableTemplateItems_TimetableTemplateId_SubjectId] ON [TimetableTemplateItems] ([TimetableTemplateId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplates_BranchId] ON [TimetableTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableTemplates_TenantId_BranchId_Name] ON [TimetableTemplates] ([TenantId], [BranchId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260919110117_AcademicStructureDepth', N'9.0.0');
END;

COMMIT;
GO

