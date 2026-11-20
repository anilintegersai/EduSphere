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

COMMIT;
GO

