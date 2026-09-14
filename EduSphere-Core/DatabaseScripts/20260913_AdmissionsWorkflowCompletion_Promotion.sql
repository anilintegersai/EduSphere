BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FeeInvoices]') AND [c].[name] = N'StudentProfileId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [FeeInvoices] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [FeeInvoices] ALTER COLUMN [StudentProfileId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FeeInvoices]') AND [c].[name] = N'StudentFeeAssignmentId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [FeeInvoices] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [FeeInvoices] ALTER COLUMN [StudentFeeAssignmentId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    ALTER TABLE [FeeInvoices] ADD [AdmissionApplicationId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE TABLE [AdmissionInterviews] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionApplicationId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [InterviewerUserId] uniqueidentifier NULL,
        [StartsOn] datetime2 NOT NULL,
        [EndsOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Location] nvarchar(180) NULL,
        [MeetingLink] nvarchar(500) NULL,
        [Notes] nvarchar(1000) NULL,
        [OutcomeNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AdmissionInterviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionInterviews_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionInterviews_AspNetUsers_InterviewerUserId] FOREIGN KEY ([InterviewerUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionInterviews_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_FeeInvoices_AdmissionApplicationId] ON [FeeInvoices] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviews_AdmissionApplicationId] ON [AdmissionInterviews] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviews_BranchId] ON [AdmissionInterviews] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviews_InterviewerUserId] ON [AdmissionInterviews] ([InterviewerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviews_TenantId_BranchId_StartsOn_Status] ON [AdmissionInterviews] ([TenantId], [BranchId], [StartsOn], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviews_TenantId_InterviewerUserId_StartsOn_EndsOn] ON [AdmissionInterviews] ([TenantId], [InterviewerUserId], [StartsOn], [EndsOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    ALTER TABLE [FeeInvoices] ADD CONSTRAINT [FK_FeeInvoices_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913235447_AdmissionsWorkflowCompletion', N'9.0.0');
END;

COMMIT;
GO

