BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [AppSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(150) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [ValueType] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsSensitive] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AppSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [BranchId] uniqueidentifier NULL,
        [EntityType] nvarchar(200) NOT NULL,
        [EntityId] nvarchar(100) NOT NULL,
        [Action] int NOT NULL,
        [ActorId] nvarchar(450) NULL,
        [ActorEmail] nvarchar(254) NULL,
        [OccurredOn] datetime2 NOT NULL,
        [OldValuesJson] nvarchar(max) NULL,
        [NewValuesJson] nvarchar(max) NULL,
        [CorrelationId] nvarchar(100) NULL,
        [IpAddress] nvarchar(64) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [BackgroundJobLogs] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [JobType] nvarchar(120) NOT NULL,
        [JobKey] nvarchar(120) NOT NULL,
        [Status] int NOT NULL,
        [AttemptCount] int NOT NULL,
        [MaxAttempts] int NOT NULL,
        [ScheduledOn] datetime2 NULL,
        [StartedOn] datetime2 NULL,
        [CompletedOn] datetime2 NULL,
        [NextRetryOn] datetime2 NULL,
        [PayloadJson] nvarchar(max) NOT NULL,
        [LastError] nvarchar(2000) NULL,
        [CorrelationId] nvarchar(100) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BackgroundJobLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [BranchAcademicConfigs] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TimeZoneId] nvarchar(100) NOT NULL,
        [WeekStartsOn] nvarchar(10) NOT NULL,
        [WeekendDays] nvarchar(10) NULL,
        [DefaultPeriodMinutes] int NOT NULL,
        [WorkingDaysPerWeek] int NOT NULL,
        [DateFormat] nvarchar(30) NOT NULL,
        [DefaultLanguage] nvarchar(10) NOT NULL,
        [AdmissionNumberPrefix] nvarchar(20) NOT NULL,
        [StudentNumberPrefix] nvarchar(20) NOT NULL,
        [PreferencesJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BranchAcademicConfigs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchAcademicConfigs_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [BranchContacts] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ContactType] nvarchar(50) NOT NULL,
        [ContactName] nvarchar(150) NOT NULL,
        [Designation] nvarchar(150) NULL,
        [Email] nvarchar(254) NULL,
        [Phone] nvarchar(30) NULL,
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
        CONSTRAINT [PK_BranchContacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchContacts_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [LookupItems] (
        [Id] uniqueidentifier NOT NULL,
        [LookupType] nvarchar(80) NOT NULL,
        [Code] nvarchar(80) NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [MetadataJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LookupItems] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [PermissionDefinitions] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(120) NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [Module] nvarchar(80) NOT NULL,
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
        CONSTRAINT [PK_PermissionDefinitions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [SubscriptionPlans] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(500) NULL,
        [MonthlyPrice] decimal(18,2) NOT NULL,
        [AnnualPrice] decimal(18,2) NOT NULL,
        [MaxBranches] int NOT NULL,
        [MaxStudents] int NOT NULL,
        [MaxStaff] int NOT NULL,
        [IsActive] bit NOT NULL,
        [IncludedFeaturesJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SubscriptionPlans] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantBrandings] (
        [Id] uniqueidentifier NOT NULL,
        [LogoUrl] nvarchar(250) NULL,
        [FaviconUrl] nvarchar(250) NULL,
        [PrimaryColor] nvarchar(7) NOT NULL,
        [SecondaryColor] nvarchar(7) NOT NULL,
        [AccentColor] nvarchar(7) NOT NULL,
        [FontFamily] nvarchar(100) NULL,
        [EmailFromName] nvarchar(150) NULL,
        [EmailHeaderHtml] nvarchar(250) NULL,
        [EmailFooterHtml] nvarchar(1000) NULL,
        [LoginWelcomeText] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantBrandings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantDomains] (
        [Id] uniqueidentifier NOT NULL,
        [DomainName] nvarchar(253) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [IsVerified] bit NOT NULL,
        [VerificationToken] nvarchar(100) NOT NULL,
        [VerifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantDomains] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantFeatureFlags] (
        [Id] uniqueidentifier NOT NULL,
        [FeatureKey] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsEnabled] bit NOT NULL,
        [EnabledFrom] datetime2 NULL,
        [EnabledUntil] datetime2 NULL,
        [ConditionsJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantFeatureFlags] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantIntegrationSettings] (
        [Id] uniqueidentifier NOT NULL,
        [IntegrationKey] nvarchar(80) NOT NULL,
        [DisplayName] nvarchar(120) NOT NULL,
        [Category] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [ConfigurationJson] nvarchar(max) NOT NULL,
        [ProtectedSecret] nvarchar(max) NULL,
        [SecretHint] nvarchar(120) NULL,
        [LastTestedOn] datetime2 NULL,
        [LastTestSucceeded] bit NULL,
        [LastTestMessage] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantIntegrationSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(150) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [ValueType] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsSensitive] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [RolePermissionGrants] (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [PermissionDefinitionId] uniqueidentifier NOT NULL,
        [IsAllowed] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RolePermissionGrants] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RolePermissionGrants_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RolePermissionGrants_PermissionDefinitions_PermissionDefinitionId] FOREIGN KEY ([PermissionDefinitionId]) REFERENCES [PermissionDefinitions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [UserPermissionGrants] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [PermissionDefinitionId] uniqueidentifier NOT NULL,
        [IsAllowed] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserPermissionGrants] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserPermissionGrants_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserPermissionGrants_PermissionDefinitions_PermissionDefinitionId] FOREIGN KEY ([PermissionDefinitionId]) REFERENCES [PermissionDefinitions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE TABLE [TenantSubscriptions] (
        [Id] uniqueidentifier NOT NULL,
        [SubscriptionPlanId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [StartsOn] date NOT NULL,
        [EndsOn] date NULL,
        [AutoRenew] bit NOT NULL,
        [ExternalSubscriptionId] nvarchar(100) NULL,
        [AgreedMonthlyPrice] decimal(18,2) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TenantSubscriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY ([SubscriptionPlanId]) REFERENCES [SubscriptionPlans] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppSettings_Key] ON [AppSettings] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityType_EntityId] ON [AuditLogs] ([EntityType], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TenantId_OccurredOn] ON [AuditLogs] ([TenantId], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_BackgroundJobLogs_TenantId_JobKey] ON [BackgroundJobLogs] ([TenantId], [JobKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_BackgroundJobLogs_TenantId_Status_ScheduledOn] ON [BackgroundJobLogs] ([TenantId], [Status], [ScheduledOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BranchAcademicConfigs_BranchId] ON [BranchAcademicConfigs] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_BranchContacts_BranchId] ON [BranchContacts] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_BranchContacts_TenantId_BranchId_ContactType_ContactName] ON [BranchContacts] ([TenantId], [BranchId], [ContactType], [ContactName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LookupItems_TenantId_LookupType_Code] ON [LookupItems] ([TenantId], [LookupType], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PermissionDefinitions_Key] ON [PermissionDefinitions] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_RolePermissionGrants_PermissionDefinitionId] ON [RolePermissionGrants] ([PermissionDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_RolePermissionGrants_RoleId] ON [RolePermissionGrants] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RolePermissionGrants_TenantId_RoleId_PermissionDefinitionId] ON [RolePermissionGrants] ([TenantId], [RoleId], [PermissionDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SubscriptionPlans_Code] ON [SubscriptionPlans] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantBrandings_TenantId] ON [TenantBrandings] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantDomains_DomainName] ON [TenantDomains] ([DomainName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_TenantDomains_TenantId_IsPrimary] ON [TenantDomains] ([TenantId], [IsPrimary]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantFeatureFlags_TenantId_FeatureKey] ON [TenantFeatureFlags] ([TenantId], [FeatureKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantIntegrationSettings_TenantId_IntegrationKey] ON [TenantIntegrationSettings] ([TenantId], [IntegrationKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantSettings_TenantId_Key] ON [TenantSettings] ([TenantId], [Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_TenantSubscriptions_SubscriptionPlanId] ON [TenantSubscriptions] ([SubscriptionPlanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_TenantSubscriptions_TenantId_Status] ON [TenantSubscriptions] ([TenantId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_UserPermissionGrants_PermissionDefinitionId] ON [UserPermissionGrants] ([PermissionDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserPermissionGrants_TenantId_UserId_PermissionDefinitionId] ON [UserPermissionGrants] ([TenantId], [UserId], [PermissionDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    CREATE INDEX [IX_UserPermissionGrants_UserId] ON [UserPermissionGrants] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919101602_TenantSettingsAdministration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260919101602_TenantSettingsAdministration', N'9.0.0');
END;

COMMIT;
GO

/* Idempotent administration reference data. No secrets or environment values are inserted. */
IF OBJECT_ID(N'[dbo].[SubscriptionPlans]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[SubscriptionPlans] WHERE [Code] = N'STARTER')
        INSERT INTO [dbo].[SubscriptionPlans] ([Id],[Code],[Name],[Description],[MonthlyPrice],[AnnualPrice],[MaxBranches],[MaxStudents],[MaxStaff],[IsActive],[IncludedFeaturesJson],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[DeletedBy],[DeletedOn],[ConcurrencyToken])
        VALUES (NEWID(),N'STARTER',N'Starter',N'Core operations for smaller institutions.',4999,49990,2,1000,100,1,N'["Academics","Attendance","Examinations"]',N'migration',SYSUTCDATETIME(),NULL,NULL,0,NULL,NULL,NEWID());
    IF NOT EXISTS (SELECT 1 FROM [dbo].[SubscriptionPlans] WHERE [Code] = N'PROFESSIONAL')
        INSERT INTO [dbo].[SubscriptionPlans] ([Id],[Code],[Name],[Description],[MonthlyPrice],[AnnualPrice],[MaxBranches],[MaxStudents],[MaxStaff],[IsActive],[IncludedFeaturesJson],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[DeletedBy],[DeletedOn],[ConcurrencyToken])
        VALUES (NEWID(),N'PROFESSIONAL',N'Professional',N'Complete academic and enterprise operations.',12999,129990,10,10000,1000,1,N'["All core modules","Enterprise modules","AI question papers"]',N'migration',SYSUTCDATETIME(),NULL,NULL,0,NULL,NULL,NEWID());
    IF NOT EXISTS (SELECT 1 FROM [dbo].[SubscriptionPlans] WHERE [Code] = N'ENTERPRISE')
        INSERT INTO [dbo].[SubscriptionPlans] ([Id],[Code],[Name],[Description],[MonthlyPrice],[AnnualPrice],[MaxBranches],[MaxStudents],[MaxStaff],[IsActive],[IncludedFeaturesJson],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[DeletedBy],[DeletedOn],[ConcurrencyToken])
        VALUES (NEWID(),N'ENTERPRISE',N'Enterprise',N'Unlimited scale with custom integrations.',0,0,0,0,0,1,N'["Unlimited scale","Custom integrations","Priority support"]',N'migration',SYSUTCDATETIME(),NULL,NULL,0,NULL,NULL,NEWID());
END;
GO

IF OBJECT_ID(N'[dbo].[PermissionDefinitions]', N'U') IS NOT NULL
BEGIN
    DECLARE @Permissions TABLE ([Key] nvarchar(120), [DisplayName] nvarchar(150), [Module] nvarchar(80), [Description] nvarchar(500));
    INSERT INTO @Permissions VALUES
      (N'administration.view',N'View administration',N'Administration',N'View settings, audit history and background jobs.'),
      (N'administration.manage-tenant',N'Manage tenant settings',N'Administration',N'Manage branding, domains, integrations, flags, branch configuration and lookups.'),
      (N'administration.manage-global',N'Manage global settings',N'Administration',N'Manage subscription plans and platform-wide settings.'),
      (N'administration.manage-permissions',N'Manage permission grants',N'Security',N'Assign tenant-scoped role and user permission overrides.'),
      (N'tenants.manage',N'Manage tenants',N'Tenants',N'Create, update, activate and suspend tenants.'),
      (N'tenant-workspace.manage',N'Manage tenant workspace',N'Tenants',N'Manage branches and tenant-wide academic configuration.'),
      (N'branch-workspace.manage',N'Manage branch workspace',N'Branches',N'Manage branch academic and operational records.'),
      (N'attendance.mark',N'Mark attendance',N'Attendance',N'Create and correct attendance records.'),
      (N'finance.manage',N'Manage finance',N'Finance',N'Manage fees, payments and finance workflows.'),
      (N'transport.manage',N'Manage transport',N'Transport',N'Manage transport operations.'),
      (N'library.manage',N'Manage library',N'Library',N'Manage library operations.'),
      (N'hostel.manage',N'Manage hostel',N'Hostel',N'Manage hostel operations.'),
      (N'communications.manage',N'Manage communications',N'Communications',N'Manage announcements and outbound communications.'),
      (N'users.manage',N'Manage users',N'Users',N'User creation and role assignments.'),
      (N'ai-question-papers.manage',N'Manage AI question papers',N'AI',N'Generate and govern AI-assisted question papers.');
    INSERT INTO [dbo].[PermissionDefinitions] ([Id],[Key],[DisplayName],[Module],[Description],[IsActive],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[DeletedBy],[DeletedOn],[ConcurrencyToken])
    SELECT NEWID(), p.[Key], p.[DisplayName], p.[Module], p.[Description], 1, N'migration', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID()
    FROM @Permissions p WHERE NOT EXISTS (SELECT 1 FROM [dbo].[PermissionDefinitions] d WHERE d.[Key] = p.[Key]);
END;
GO

