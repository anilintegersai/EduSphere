BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [Circulars] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Title] nvarchar(180) NOT NULL,
        [ContentHtml] nvarchar(max) NOT NULL,
        [Summary] nvarchar(500) NULL,
        [Audience] int NOT NULL,
        [Status] int NOT NULL,
        [PublishOn] datetime2 NULL,
        [ExpireOn] datetime2 NULL,
        [AttachmentPath] nvarchar(500) NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ApprovedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Circulars] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [CommunicationCampaigns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(180) NOT NULL,
        [Audience] int NOT NULL,
        [Channel] int NOT NULL,
        [NotificationTemplateId] uniqueidentifier NULL,
        [Subject] nvarchar(250) NULL,
        [Body] nvarchar(4000) NOT NULL,
        [ScheduledOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [StartedOn] datetime2 NULL,
        [CompletedOn] datetime2 NULL,
        [RecipientCount] int NOT NULL,
        [SentCount] int NOT NULL,
        [FailedCount] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CommunicationCampaigns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommunicationCampaigns_NotificationTemplates_NotificationTemplateId] FOREIGN KEY ([NotificationTemplateId]) REFERENCES [NotificationTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [CommunicationConsents] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Channel] int NOT NULL,
        [Status] int NOT NULL,
        [Purpose] nvarchar(80) NOT NULL,
        [Source] nvarchar(80) NOT NULL,
        [CapturedOn] datetime2 NOT NULL,
        [WithdrawnOn] datetime2 NULL,
        [EvidenceHash] nvarchar(64) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CommunicationConsents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommunicationConsents_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [CommunicationUnsubscribes] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [DestinationAddress] nvarchar(250) NOT NULL,
        [Channel] int NOT NULL,
        [Purpose] nvarchar(80) NOT NULL,
        [TokenHash] nvarchar(64) NOT NULL,
        [UnsubscribedOn] datetime2 NOT NULL,
        [Source] nvarchar(80) NOT NULL,
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
        CONSTRAINT [PK_CommunicationUnsubscribes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [InAppNotifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [NotificationMessageId] uniqueidentifier NULL,
        [Title] nvarchar(250) NULL,
        [Body] nvarchar(4000) NOT NULL,
        [ActionUrl] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ReadOn] datetime2 NULL,
        [IsDismissed] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_InAppNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InAppNotifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InAppNotifications_NotificationMessages_NotificationMessageId] FOREIGN KEY ([NotificationMessageId]) REFERENCES [NotificationMessages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [NotificationDeadLetters] (
        [Id] uniqueidentifier NOT NULL,
        [NotificationMessageId] uniqueidentifier NOT NULL,
        [NotificationRecipientId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [AttemptCount] int NOT NULL,
        [LastError] nvarchar(1000) NOT NULL,
        [DeadLetteredOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [ResolvedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationDeadLetters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationDeadLetters_NotificationMessages_NotificationMessageId] FOREIGN KEY ([NotificationMessageId]) REFERENCES [NotificationMessages] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationDeadLetters_NotificationRecipients_NotificationRecipientId] FOREIGN KEY ([NotificationRecipientId]) REFERENCES [NotificationRecipients] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [NotificationProviderWebhookEvents] (
        [Id] uniqueidentifier NOT NULL,
        [ProviderKey] nvarchar(80) NOT NULL,
        [ProviderMessageId] nvarchar(160) NOT NULL,
        [EventType] int NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [PayloadJson] nvarchar(4000) NULL,
        [SignatureVerified] bit NOT NULL,
        [ProcessedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationProviderWebhookEvents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [NotificationTemplateLocalizations] (
        [Id] uniqueidentifier NOT NULL,
        [NotificationTemplateId] uniqueidentifier NOT NULL,
        [Locale] nvarchar(10) NOT NULL,
        [SubjectTemplate] nvarchar(250) NULL,
        [BodyTemplate] nvarchar(4000) NOT NULL,
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
        CONSTRAINT [PK_NotificationTemplateLocalizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationTemplateLocalizations_NotificationTemplates_NotificationTemplateId] FOREIGN KEY ([NotificationTemplateId]) REFERENCES [NotificationTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [NotificationTemplateVersions] (
        [Id] uniqueidentifier NOT NULL,
        [NotificationTemplateId] uniqueidentifier NOT NULL,
        [Version] int NOT NULL,
        [SubjectTemplate] nvarchar(250) NULL,
        [BodyTemplate] nvarchar(4000) NOT NULL,
        [VariablesJson] nvarchar(1000) NULL,
        [PublishedOn] datetime2 NOT NULL,
        [PublishedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationTemplateVersions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationTemplateVersions_NotificationTemplates_NotificationTemplateId] FOREIGN KEY ([NotificationTemplateId]) REFERENCES [NotificationTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [ParentTeacherMeetings] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [ParentUserId] uniqueidentifier NOT NULL,
        [StartsOn] datetime2 NOT NULL,
        [EndsOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Agenda] nvarchar(500) NULL,
        [MeetingUrl] nvarchar(500) NULL,
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
        CONSTRAINT [PK_ParentTeacherMeetings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ParentTeacherMeetings_AspNetUsers_ParentUserId] FOREIGN KEY ([ParentUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentTeacherMeetings_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentTeacherMeetings_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [PushSubscriptions] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Endpoint] nvarchar(500) NOT NULL,
        [DeviceToken] nvarchar(500) NULL,
        [Platform] nvarchar(80) NOT NULL,
        [DeviceName] nvarchar(200) NULL,
        [IsActive] bit NOT NULL,
        [LastSeenOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PushSubscriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PushSubscriptions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE TABLE [UserCommunicationPreferences] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Channel] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [Frequency] int NOT NULL,
        [Locale] nvarchar(10) NOT NULL,
        [QuietHoursStart] time NULL,
        [QuietHoursEnd] time NULL,
        [UpdatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserCommunicationPreferences] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserCommunicationPreferences_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_Circulars_TenantId_BranchId_Status_PublishOn] ON [Circulars] ([TenantId], [BranchId], [Status], [PublishOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_CommunicationCampaigns_NotificationTemplateId] ON [CommunicationCampaigns] ([NotificationTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_CommunicationCampaigns_TenantId_Status_ScheduledOn] ON [CommunicationCampaigns] ([TenantId], [Status], [ScheduledOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_CommunicationConsents_TenantId_UserId_Channel_Purpose_Status] ON [CommunicationConsents] ([TenantId], [UserId], [Channel], [Purpose], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_CommunicationConsents_UserId] ON [CommunicationConsents] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_CommunicationUnsubscribes_TenantId_Channel_DestinationAddress_Purpose_IsActive] ON [CommunicationUnsubscribes] ([TenantId], [Channel], [DestinationAddress], [Purpose], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_InAppNotifications_NotificationMessageId] ON [InAppNotifications] ([NotificationMessageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_InAppNotifications_TenantId_UserId_ReadOn_IsDismissed] ON [InAppNotifications] ([TenantId], [UserId], [ReadOn], [IsDismissed]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_InAppNotifications_UserId] ON [InAppNotifications] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_NotificationDeadLetters_NotificationMessageId] ON [NotificationDeadLetters] ([NotificationMessageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_NotificationDeadLetters_NotificationRecipientId] ON [NotificationDeadLetters] ([NotificationRecipientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_NotificationDeadLetters_TenantId_Status_DeadLetteredOn] ON [NotificationDeadLetters] ([TenantId], [Status], [DeadLetteredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificationProviderWebhookEvents_TenantId_ProviderKey_ProviderMessageId_EventType] ON [NotificationProviderWebhookEvents] ([TenantId], [ProviderKey], [ProviderMessageId], [EventType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificationTemplateLocalizations_NotificationTemplateId_Locale] ON [NotificationTemplateLocalizations] ([NotificationTemplateId], [Locale]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificationTemplateVersions_NotificationTemplateId_Version] ON [NotificationTemplateVersions] ([NotificationTemplateId], [Version]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_ParentTeacherMeetings_ParentUserId] ON [ParentTeacherMeetings] ([ParentUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_ParentTeacherMeetings_StudentProfileId] ON [ParentTeacherMeetings] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_ParentTeacherMeetings_TeacherProfileId] ON [ParentTeacherMeetings] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_ParentTeacherMeetings_TenantId_TeacherProfileId_StartsOn_EndsOn] ON [ParentTeacherMeetings] ([TenantId], [TeacherProfileId], [StartsOn], [EndsOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PushSubscriptions_TenantId_UserId_Endpoint] ON [PushSubscriptions] ([TenantId], [UserId], [Endpoint]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_PushSubscriptions_UserId] ON [PushSubscriptions] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserCommunicationPreferences_TenantId_UserId_Channel] ON [UserCommunicationPreferences] ([TenantId], [UserId], [Channel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    CREATE INDEX [IX_UserCommunicationPreferences_UserId] ON [UserCommunicationPreferences] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120233959_CommunicationNotificationDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261120233959_CommunicationNotificationDepth', N'9.0.0');
END;

COMMIT;
GO

