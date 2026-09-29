BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [AccountEmailJobs] (
        [Id] uniqueidentifier NOT NULL,
        [NotificationMessageId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [JobType] nvarchar(80) NOT NULL,
        [Status] int NOT NULL,
        [AttemptCount] int NOT NULL,
        [MaximumAttempts] int NOT NULL,
        [AvailableOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [LastError] nvarchar(2000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AccountEmailJobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AccountEmailJobs_NotificationMessages_NotificationMessageId] FOREIGN KEY ([NotificationMessageId]) REFERENCES [NotificationMessages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [EmailBounceEvents] (
        [Id] uniqueidentifier NOT NULL,
        [NotificationMessageId] uniqueidentifier NULL,
        [Recipient] nvarchar(254) NOT NULL,
        [BounceType] nvarchar(80) NOT NULL,
        [ProviderEventId] nvarchar(200) NULL,
        [Reason] nvarchar(2000) NULL,
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
        CONSTRAINT [PK_EmailBounceEvents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [PayrollRuns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [Status] int NOT NULL,
        [ApprovedOn] datetime2 NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ExportFileName] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PayrollRuns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PayrollRuns_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [SecurityEvents] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [EventType] int NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [Email] nvarchar(254) NULL,
        [IpAddress] nvarchar(64) NULL,
        [UserAgent] nvarchar(500) NULL,
        [Succeeded] bit NOT NULL,
        [Details] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SecurityEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SecurityEvents_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [StaffAttendanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [StaffProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AttendanceDate] date NOT NULL,
        [Status] int NOT NULL,
        [CheckIn] time NULL,
        [CheckOut] time NULL,
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
        CONSTRAINT [PK_StaffAttendanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffAttendanceRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffAttendanceRecords_StaffProfiles_StaffProfileId] FOREIGN KEY ([StaffProfileId]) REFERENCES [StaffProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [StaffLeaveRequests] (
        [Id] uniqueidentifier NOT NULL,
        [StaffProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LeaveType] nvarchar(80) NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [Status] int NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [ReviewNotes] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StaffLeaveRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffLeaveRequests_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffLeaveRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffLeaveRequests_StaffProfiles_StaffProfileId] FOREIGN KEY ([StaffProfileId]) REFERENCES [StaffProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [StaffPayrollProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [StaffProfileId] uniqueidentifier NOT NULL,
        [BasicSalary] decimal(18,2) NOT NULL,
        [Allowances] decimal(18,2) NOT NULL,
        [Deductions] decimal(18,2) NOT NULL,
        [BankAccountReference] nvarchar(100) NULL,
        [TaxIdentifier] nvarchar(80) NULL,
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
        CONSTRAINT [PK_StaffPayrollProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffPayrollProfiles_StaffProfiles_StaffProfileId] FOREIGN KEY ([StaffProfileId]) REFERENCES [StaffProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [TeacherExperienceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [Organization] nvarchar(180) NOT NULL,
        [Designation] nvarchar(120) NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NULL,
        [Responsibilities] nvarchar(500) NULL,
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
        CONSTRAINT [PK_TeacherExperienceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherExperienceRecords_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [TeacherQualificationRecords] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [Qualification] nvarchar(150) NOT NULL,
        [Institution] nvarchar(200) NULL,
        [Specialization] nvarchar(120) NULL,
        [CompletionYear] int NULL,
        [CertificateNumber] nvarchar(100) NULL,
        [IsVerified] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherQualificationRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherQualificationRecords_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [UserImportBatches] (
        [Id] uniqueidentifier NOT NULL,
        [RequestedByUserId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [RoleName] nvarchar(80) NOT NULL,
        [Status] int NOT NULL,
        [TotalRows] int NOT NULL,
        [ProcessedRows] int NOT NULL,
        [SucceededRows] int NOT NULL,
        [FailedRows] int NOT NULL,
        [SendActivationEmail] bit NOT NULL,
        [LastError] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserImportBatches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [PayrollEntries] (
        [Id] uniqueidentifier NOT NULL,
        [PayrollRunId] uniqueidentifier NOT NULL,
        [StaffProfileId] uniqueidentifier NOT NULL,
        [BasicSalary] decimal(18,2) NOT NULL,
        [Allowances] decimal(18,2) NOT NULL,
        [Deductions] decimal(18,2) NOT NULL,
        [NetPay] decimal(18,2) NOT NULL,
        [ExternalPayrollReference] nvarchar(100) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PayrollEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PayrollEntries_PayrollRuns_PayrollRunId] FOREIGN KEY ([PayrollRunId]) REFERENCES [PayrollRuns] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PayrollEntries_StaffProfiles_StaffProfileId] FOREIGN KEY ([StaffProfileId]) REFERENCES [StaffProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE TABLE [UserImportRows] (
        [Id] uniqueidentifier NOT NULL,
        [UserImportBatchId] uniqueidentifier NOT NULL,
        [RowNumber] int NOT NULL,
        [PayloadJson] nvarchar(4000) NOT NULL,
        [IsValid] bit NOT NULL,
        [IsProcessed] bit NOT NULL,
        [Succeeded] bit NOT NULL,
        [CreatedUserId] uniqueidentifier NULL,
        [Error] nvarchar(2000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserImportRows] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserImportRows_UserImportBatches_UserImportBatchId] FOREIGN KEY ([UserImportBatchId]) REFERENCES [UserImportBatches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_StudentGuardians_TenantId_ParentUserId_StudentProfileId] ON [StudentGuardians] ([TenantId], [ParentUserId], [StudentProfileId]) WHERE [ParentUserId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_AccountEmailJobs_NotificationMessageId] ON [AccountEmailJobs] ([NotificationMessageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_AccountEmailJobs_Status_AvailableOn] ON [AccountEmailJobs] ([Status], [AvailableOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_EmailBounceEvents_TenantId_Recipient_OccurredOn] ON [EmailBounceEvents] ([TenantId], [Recipient], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PayrollEntries_PayrollRunId_StaffProfileId] ON [PayrollEntries] ([PayrollRunId], [StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_PayrollEntries_StaffProfileId] ON [PayrollEntries] ([StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_PayrollRuns_BranchId] ON [PayrollRuns] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PayrollRuns_TenantId_BranchId_Year_Month] ON [PayrollRuns] ([TenantId], [BranchId], [Year], [Month]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_SecurityEvents_TenantId_OccurredOn_EventType] ON [SecurityEvents] ([TenantId], [OccurredOn], [EventType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_SecurityEvents_UserId] ON [SecurityEvents] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffAttendanceRecords_BranchId] ON [StaffAttendanceRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffAttendanceRecords_StaffProfileId] ON [StaffAttendanceRecords] ([StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffAttendanceRecords_TenantId_StaffProfileId_AttendanceDate] ON [StaffAttendanceRecords] ([TenantId], [StaffProfileId], [AttendanceDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffLeaveRequests_BranchId] ON [StaffLeaveRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffLeaveRequests_ReviewedByUserId] ON [StaffLeaveRequests] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffLeaveRequests_StaffProfileId] ON [StaffLeaveRequests] ([StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffLeaveRequests_TenantId_BranchId_Status_FromDate] ON [StaffLeaveRequests] ([TenantId], [BranchId], [Status], [FromDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_StaffPayrollProfiles_StaffProfileId] ON [StaffPayrollProfiles] ([StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffPayrollProfiles_TenantId_StaffProfileId] ON [StaffPayrollProfiles] ([TenantId], [StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherExperienceRecords_TeacherProfileId] ON [TeacherExperienceRecords] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherExperienceRecords_TenantId_TeacherProfileId_FromDate] ON [TeacherExperienceRecords] ([TenantId], [TeacherProfileId], [FromDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherQualificationRecords_TeacherProfileId] ON [TeacherQualificationRecords] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherQualificationRecords_TenantId_TeacherProfileId_Qualification] ON [TeacherQualificationRecords] ([TenantId], [TeacherProfileId], [Qualification]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE INDEX [IX_UserImportBatches_TenantId_Status_CreatedOn] ON [UserImportBatches] ([TenantId], [Status], [CreatedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserImportRows_UserImportBatchId_RowNumber] ON [UserImportRows] ([UserImportBatchId], [RowNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928230739_UserParentStaffManagementCompletion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928230739_UserParentStaffManagementCompletion', N'9.0.0');
END;

COMMIT;
GO

