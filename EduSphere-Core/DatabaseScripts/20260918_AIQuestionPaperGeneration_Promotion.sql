BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIPromptTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Code] nvarchar(80) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Version] int NOT NULL,
        [IsActive] bit NOT NULL,
        [SystemPrompt] nvarchar(max) NOT NULL,
        [UserPromptTemplate] nvarchar(max) NOT NULL,
        [OutputSchemaJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIPromptTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIPromptTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIProviderSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [ProviderType] int NOT NULL,
        [Endpoint] nvarchar(500) NOT NULL,
        [ModelOrDeployment] nvarchar(150) NOT NULL,
        [ProtectedApiKey] nvarchar(max) NOT NULL,
        [ApiVersion] nvarchar(40) NULL,
        [Region] nvarchar(80) NULL,
        [DataResidencyPolicy] int NOT NULL,
        [RequiredRegion] nvarchar(80) NULL,
        [IsEnabled] bit NOT NULL,
        [IsDefault] bit NOT NULL,
        [TimeoutSeconds] int NOT NULL,
        [InputCostPerMillionTokens] decimal(12,6) NOT NULL,
        [OutputCostPerMillionTokens] decimal(12,6) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIProviderSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIGenerationRequests] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [QuestionPaperId] uniqueidentifier NULL,
        [GeneratedVersionId] uniqueidentifier NULL,
        [ProviderSettingId] uniqueidentifier NULL,
        [PromptTemplateId] uniqueidentifier NOT NULL,
        [RegeneratedFromRequestId] uniqueidentifier NULL,
        [Status] int NOT NULL,
        [InputParametersJson] nvarchar(max) NOT NULL,
        [PromptHash] nvarchar(64) NOT NULL,
        [RequestedBy] nvarchar(450) NULL,
        [RequestedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [FailureMessage] nvarchar(2000) NULL,
        [UsedManualFallback] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIGenerationRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIGenerationRequests_AIGenerationRequests_RegeneratedFromRequestId] FOREIGN KEY ([RegeneratedFromRequestId]) REFERENCES [AIGenerationRequests] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_AIPromptTemplates_PromptTemplateId] FOREIGN KEY ([PromptTemplateId]) REFERENCES [AIPromptTemplates] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_AIProviderSettings_ProviderSettingId] FOREIGN KEY ([ProviderSettingId]) REFERENCES [AIProviderSettings] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_QuestionPaperVersions_GeneratedVersionId] FOREIGN KEY ([GeneratedVersionId]) REFERENCES [QuestionPaperVersions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_QuestionPapers_QuestionPaperId] FOREIGN KEY ([QuestionPaperId]) REFERENCES [QuestionPapers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AIGenerationRequests_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIGenerationLogs] (
        [Id] uniqueidentifier NOT NULL,
        [AIGenerationRequestId] uniqueidentifier NOT NULL,
        [EventType] nvarchar(50) NOT NULL,
        [Message] nvarchar(1000) NULL,
        [ProviderRequestId] nvarchar(100) NULL,
        [LatencyMilliseconds] int NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIGenerationLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIGenerationLogs_AIGenerationRequests_AIGenerationRequestId] FOREIGN KEY ([AIGenerationRequestId]) REFERENCES [AIGenerationRequests] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIGenerationResponses] (
        [Id] uniqueidentifier NOT NULL,
        [AIGenerationRequestId] uniqueidentifier NOT NULL,
        [StudentPaperJson] nvarchar(max) NOT NULL,
        [MarkingSchemeJson] nvarchar(max) NOT NULL,
        [ContentHash] nvarchar(64) NOT NULL,
        [SchemaValidated] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIGenerationResponses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIGenerationResponses_AIGenerationRequests_AIGenerationRequestId] FOREIGN KEY ([AIGenerationRequestId]) REFERENCES [AIGenerationRequests] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE TABLE [AIUsageRecords] (
        [Id] uniqueidentifier NOT NULL,
        [AIGenerationRequestId] uniqueidentifier NOT NULL,
        [InputTokens] int NOT NULL,
        [OutputTokens] int NOT NULL,
        [EstimatedCost] decimal(18,6) NOT NULL,
        [Currency] nvarchar(30) NOT NULL,
        [LatencyMilliseconds] int NOT NULL,
        [Model] nvarchar(150) NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AIUsageRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIUsageRecords_AIGenerationRequests_AIGenerationRequestId] FOREIGN KEY ([AIGenerationRequestId]) REFERENCES [AIGenerationRequests] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationLogs_AIGenerationRequestId_OccurredOn] ON [AIGenerationLogs] ([AIGenerationRequestId], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_BranchId] ON [AIGenerationRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_GeneratedVersionId] ON [AIGenerationRequests] ([GeneratedVersionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_PromptTemplateId] ON [AIGenerationRequests] ([PromptTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_ProviderSettingId] ON [AIGenerationRequests] ([ProviderSettingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_QuestionPaperId] ON [AIGenerationRequests] ([QuestionPaperId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_RegeneratedFromRequestId] ON [AIGenerationRequests] ([RegeneratedFromRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_SubjectId] ON [AIGenerationRequests] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIGenerationRequests_TenantId_BranchId_Status_RequestedOn] ON [AIGenerationRequests] ([TenantId], [BranchId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AIGenerationResponses_AIGenerationRequestId] ON [AIGenerationResponses] ([AIGenerationRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIPromptTemplates_BranchId] ON [AIPromptTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AIPromptTemplates_TenantId_BranchId_Code_Version] ON [AIPromptTemplates] ([TenantId], [BranchId], [Code], [Version]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIProviderSettings_TenantId_IsEnabled_IsDefault] ON [AIProviderSettings] ([TenantId], [IsEnabled], [IsDefault]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AIProviderSettings_TenantId_Name] ON [AIProviderSettings] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIUsageRecords_AIGenerationRequestId] ON [AIUsageRecords] ([AIGenerationRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    CREATE INDEX [IX_AIUsageRecords_TenantId_RecordedOn] ON [AIUsageRecords] ([TenantId], [RecordedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260918213000_AIQuestionPaperGeneration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260918213000_AIQuestionPaperGeneration', N'9.0.0');
END;

COMMIT;
GO

/* Safe reference-data seed. Provider credentials are intentionally never seeded. */
IF OBJECT_ID(N'[dbo].[AIPromptTemplates]', N'U') IS NOT NULL
BEGIN
    INSERT INTO [dbo].[AIPromptTemplates]
        ([Id], [BranchId], [Code], [Name], [Version], [IsActive], [SystemPrompt], [UserPromptTemplate], [OutputSchemaJson],
         [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy], [DeletedOn], [ConcurrencyToken], [TenantId])
    SELECT NEWID(), NULL, N'GENERAL-QUESTION-PAPER', N'General question paper', 1, 1,
           N'You are an expert assessment designer. Produce only valid JSON matching the requested schema. Do not include personal data, markdown fences, or commentary. Ensure exact total marks and unique questions. Every question must include a complete marking answer.',
           N'Create a question paper from this sanitized configuration: {{GenerationInputJson}}',
           N'{"title":"string","sections":[{"code":"string","title":"string","instructions":"string|null","questions":[{"text":"string","answer":"string","marks":1,"questionType":"ShortAnswer","difficulty":"Medium","bloomLevel":"Understand","syllabusUnit":"string|null"}]}]}',
           N'migration', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), t.[Id]
    FROM [dbo].[Tenants] t
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [dbo].[AIPromptTemplates] p
        WHERE p.[TenantId] = t.[Id] AND p.[Code] = N'GENERAL-QUESTION-PAPER' AND p.[Version] = 1
    );
END;
GO

