BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [RouteOptimizationRuns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [OriginalDistanceKm] decimal(12,2) NULL,
        [OptimizedDistanceKm] decimal(12,2) NULL,
        [ProposedStopOrderJson] nvarchar(max) NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RouteOptimizationRuns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RouteOptimizationRuns_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportAttendanceScans] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NULL,
        [Event] int NOT NULL,
        [ScanMethod] int NOT NULL,
        [Token] nvarchar(160) NOT NULL,
        [ScannedOn] datetime2 NOT NULL,
        [Latitude] decimal(18,2) NULL,
        [Longitude] decimal(18,2) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TransportAttendanceScans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportAttendanceScans_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportAttendanceScans_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportCrewProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [Role] int NOT NULL,
        [FullName] nvarchar(160) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [EmployeeNumber] nvarchar(60) NULL,
        [LicenseNumber] nvarchar(60) NULL,
        [LicenseValidUntil] date NULL,
        [RfidTag] nvarchar(80) NULL,
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
        CONSTRAINT [PK_TransportCrewProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportCrewProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportFeeCharges] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentTransportAssignmentId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NULL,
        [PeriodStart] date NOT NULL,
        [PeriodEnd] date NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [InvoiceNumber] nvarchar(80) NULL,
        [GeneratedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TransportFeeCharges] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportFeeCharges_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportFeeCharges_StudentTransportAssignments_StudentTransportAssignmentId] FOREIGN KEY ([StudentTransportAssignmentId]) REFERENCES [StudentTransportAssignments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportGeofences] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NULL,
        [TransportRouteStopId] uniqueidentifier NULL,
        [Name] nvarchar(160) NOT NULL,
        [Latitude] decimal(10,7) NOT NULL,
        [Longitude] decimal(10,7) NOT NULL,
        [RadiusMeters] int NOT NULL,
        [ArrivalLeadMinutes] int NOT NULL,
        [NotifyParents] bit NOT NULL,
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
        CONSTRAINT [PK_TransportGeofences] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportGpsProviderSettings] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [ProviderType] int NOT NULL,
        [EndpointUrl] nvarchar(500) NULL,
        [ProtectedApiKey] nvarchar(2000) NULL,
        [WebhookTokenHash] nvarchar(80) NOT NULL,
        [IsActive] bit NOT NULL,
        [LastSuccessfulSyncOn] datetime2 NULL,
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
        CONSTRAINT [PK_TransportGpsProviderSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [VehicleDocuments] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [DocumentType] int NOT NULL,
        [FileName] nvarchar(200) NOT NULL,
        [StoragePath] nvarchar(500) NOT NULL,
        [ContentType] nvarchar(120) NULL,
        [DocumentNumber] nvarchar(100) NULL,
        [IssuedOn] date NULL,
        [ExpiresOn] date NULL,
        [Status] int NOT NULL,
        [UploadedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_VehicleDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleDocuments_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [VehicleFuelLogs] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [FueledOn] date NOT NULL,
        [Litres] decimal(12,3) NOT NULL,
        [UnitPrice] decimal(12,2) NOT NULL,
        [TotalCost] decimal(12,2) NOT NULL,
        [OdometerReading] int NOT NULL,
        [Vendor] nvarchar(120) NULL,
        [ReceiptNumber] nvarchar(80) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_VehicleFuelLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleFuelLogs_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE TABLE [TransportGeofenceEvents] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TransportGeofenceId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [VehicleGpsPingId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [NotificationQueued] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TransportGeofenceEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportGeofenceEvents_TransportGeofences_TransportGeofenceId] FOREIGN KEY ([TransportGeofenceId]) REFERENCES [TransportGeofences] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportGeofenceEvents_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_RouteOptimizationRuns_TenantId_TransportRouteId_RequestedOn] ON [RouteOptimizationRuns] ([TenantId], [TransportRouteId], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_RouteOptimizationRuns_TransportRouteId] ON [RouteOptimizationRuns] ([TransportRouteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportAttendanceScans_StudentProfileId] ON [TransportAttendanceScans] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportAttendanceScans_TenantId_StudentProfileId_ScannedOn] ON [TransportAttendanceScans] ([TenantId], [StudentProfileId], [ScannedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportAttendanceScans_VehicleId] ON [TransportAttendanceScans] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportCrewProfiles_TenantId_BranchId_EmployeeNumber] ON [TransportCrewProfiles] ([TenantId], [BranchId], [EmployeeNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportCrewProfiles_UserId] ON [TransportCrewProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportFeeCharges_FeeInvoiceId] ON [TransportFeeCharges] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportFeeCharges_StudentTransportAssignmentId] ON [TransportFeeCharges] ([StudentTransportAssignmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportFeeCharges_TenantId_StudentTransportAssignmentId_PeriodStart_PeriodEnd] ON [TransportFeeCharges] ([TenantId], [StudentTransportAssignmentId], [PeriodStart], [PeriodEnd]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportGeofenceEvents_TenantId_VehicleId_OccurredOn] ON [TransportGeofenceEvents] ([TenantId], [VehicleId], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportGeofenceEvents_TransportGeofenceId] ON [TransportGeofenceEvents] ([TransportGeofenceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportGeofenceEvents_VehicleId] ON [TransportGeofenceEvents] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_TransportGeofences_TenantId_BranchId_IsActive] ON [TransportGeofences] ([TenantId], [BranchId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportGpsProviderSettings_TenantId_Name] ON [TransportGpsProviderSettings] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportGpsProviderSettings_WebhookTokenHash] ON [TransportGpsProviderSettings] ([WebhookTokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_VehicleDocuments_TenantId_VehicleId_DocumentType_Status] ON [VehicleDocuments] ([TenantId], [VehicleId], [DocumentType], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_VehicleDocuments_VehicleId] ON [VehicleDocuments] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_VehicleFuelLogs_TenantId_VehicleId_FueledOn] ON [VehicleFuelLogs] ([TenantId], [VehicleId], [FueledOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_VehicleFuelLogs_VehicleId] ON [VehicleFuelLogs] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261120223615_TransportOperationsDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261120223615_TransportOperationsDepth', N'9.0.0');
END;

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

