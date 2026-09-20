BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    ALTER TABLE [Results] ADD [IsPassed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    ALTER TABLE [Results] ADD [Percentile] decimal(5,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    ALTER TABLE [Results] ADD [Rank] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    EXEC(N'UPDATE [Results] SET [IsPassed] = CASE WHEN [Grade] IS NOT NULL AND UPPER([Grade]) <> N''F'' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [QuestionPaperModerations] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionPaperId] uniqueidentifier NOT NULL,
        [Decision] int NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NOT NULL,
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
        CONSTRAINT [PK_QuestionPaperModerations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPaperModerations_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPaperModerations_QuestionPapers_QuestionPaperId] FOREIGN KEY ([QuestionPaperId]) REFERENCES [QuestionPapers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [ResultPublicationBatches] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NULL,
        [Status] int NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ApprovedOn] datetime2 NULL,
        [PublishedByUserId] uniqueidentifier NULL,
        [PublishedOn] datetime2 NULL,
        [ResultCount] int NOT NULL,
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
        CONSTRAINT [PK_ResultPublicationBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultPublicationBatches_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPublicationBatches_AspNetUsers_PublishedByUserId] FOREIGN KEY ([PublishedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPublicationBatches_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPublicationBatches_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPublicationBatches_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPublicationBatches_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [ResultRankingRules] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NULL,
        [Method] int NOT NULL,
        [RankByPercentage] bit NOT NULL,
        [ExcludeFailedStudents] bit NOT NULL,
        [ExcludeWithheldResults] bit NOT NULL,
        [MinimumPercentageToRank] decimal(5,2) NOT NULL,
        [ShowRankOnReportCard] bit NOT NULL,
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
        CONSTRAINT [PK_ResultRankingRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultRankingRules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultRankingRules_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultRankingRules_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperModerations_QuestionPaperId] ON [QuestionPaperModerations] ([QuestionPaperId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperModerations_ReviewedByUserId] ON [QuestionPaperModerations] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperModerations_TenantId_QuestionPaperId_ReviewedOn] ON [QuestionPaperModerations] ([TenantId], [QuestionPaperId], [ReviewedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_ApprovedByUserId] ON [ResultPublicationBatches] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_BranchId] ON [ResultPublicationBatches] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_ExamId] ON [ResultPublicationBatches] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_PublishedByUserId] ON [ResultPublicationBatches] ([PublishedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_RequestedByUserId] ON [ResultPublicationBatches] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_SectionId] ON [ResultPublicationBatches] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultPublicationBatches_TenantId_BranchId_ExamId_SectionId_Status] ON [ResultPublicationBatches] ([TenantId], [BranchId], [ExamId], [SectionId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultRankingRules_BranchId] ON [ResultRankingRules] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultRankingRules_ExamId] ON [ResultRankingRules] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_ResultRankingRules_SectionId] ON [ResultRankingRules] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ResultRankingRules_TenantId_ExamId_SectionId] ON [ResultRankingRules] ([TenantId], [ExamId], [SectionId]) WHERE [SectionId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260916231939_ExamResultsQuestionBankWorkflowEnhancements', N'9.0.0');
END;

COMMIT;
GO

