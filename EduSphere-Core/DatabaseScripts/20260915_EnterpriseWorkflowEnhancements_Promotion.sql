BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [FeeConcessionRequests] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NULL,
        [FeeStructureId] uniqueidentifier NULL,
        [DiscountType] int NOT NULL,
        [RequestedValue] decimal(12,2) NOT NULL,
        [ApprovedAmount] decimal(12,2) NULL,
        [Status] int NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [DecidedByUserId] uniqueidentifier NULL,
        [DecidedOn] datetime2 NULL,
        [Reason] nvarchar(500) NOT NULL,
        [DecisionNotes] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_FeeConcessionRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeConcessionRequests_AspNetUsers_DecidedByUserId] FOREIGN KEY ([DecidedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeConcessionRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeConcessionRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeConcessionRequests_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeConcessionRequests_FeeStructures_FeeStructureId] FOREIGN KEY ([FeeStructureId]) REFERENCES [FeeStructures] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeConcessionRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [FeeRefunds] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeePaymentId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Status] int NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ApprovedOn] datetime2 NULL,
        [ProcessedOn] datetime2 NULL,
        [GatewayRefundId] nvarchar(120) NULL,
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
        CONSTRAINT [PK_FeeRefunds] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeRefunds_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeRefunds_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeRefunds_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeRefunds_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeRefunds_FeePayments_FeePaymentId] FOREIGN KEY ([FeePaymentId]) REFERENCES [FeePayments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [FeeReminders] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [Recipient] nvarchar(250) NOT NULL,
        [ReminderOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [SentOn] datetime2 NULL,
        [AttemptCount] int NOT NULL,
        [LastError] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_FeeReminders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeReminders_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeReminders_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeReminders_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [HostelAllocationTransferRequests] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [FromRoomId] uniqueidentifier NOT NULL,
        [FromBedId] uniqueidentifier NULL,
        [ToRoomId] uniqueidentifier NOT NULL,
        [ToBedId] uniqueidentifier NULL,
        [Status] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [ApprovedOn] datetime2 NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [DecisionNotes] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelAllocationTransferRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelAllocationTransferRequests_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_HostelBeds_FromBedId] FOREIGN KEY ([FromBedId]) REFERENCES [HostelBeds] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_HostelBeds_ToBedId] FOREIGN KEY ([ToBedId]) REFERENCES [HostelBeds] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_HostelRooms_FromRoomId] FOREIGN KEY ([FromRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocationTransferRequests_HostelRooms_ToRoomId] FOREIGN KEY ([ToRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [HostelMaintenanceRequests] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelBlockId] uniqueidentifier NULL,
        [HostelRoomId] uniqueidentifier NULL,
        [HostelBedId] uniqueidentifier NULL,
        [ReportedByUserId] uniqueidentifier NULL,
        [Category] nvarchar(120) NOT NULL,
        [Priority] int NOT NULL,
        [Status] int NOT NULL,
        [ReportedOn] datetime2 NOT NULL,
        [DueOn] datetime2 NULL,
        [ResolvedOn] datetime2 NULL,
        [Description] nvarchar(1000) NOT NULL,
        [ResolutionNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelMaintenanceRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelMaintenanceRequests_AspNetUsers_ReportedByUserId] FOREIGN KEY ([ReportedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelMaintenanceRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelMaintenanceRequests_HostelBeds_HostelBedId] FOREIGN KEY ([HostelBedId]) REFERENCES [HostelBeds] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelMaintenanceRequests_HostelBlocks_HostelBlockId] FOREIGN KEY ([HostelBlockId]) REFERENCES [HostelBlocks] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelMaintenanceRequests_HostelRooms_HostelRoomId] FOREIGN KEY ([HostelRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [HostelVisitorLogs] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [VisitorName] nvarchar(120) NOT NULL,
        [Relationship] nvarchar(80) NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [Purpose] nvarchar(250) NULL,
        [CheckInOn] datetime2 NOT NULL,
        [CheckOutOn] datetime2 NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [IdDocumentNumber] nvarchar(80) NULL,
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
        CONSTRAINT [PK_HostelVisitorLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelVisitorLogs_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelVisitorLogs_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelVisitorLogs_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelVisitorLogs_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [LedgerExportBatches] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [ExportType] int NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [Format] int NOT NULL,
        [Status] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [GeneratedOn] datetime2 NULL,
        [GeneratedByUserId] uniqueidentifier NULL,
        [StoragePath] nvarchar(500) NULL,
        [RowCount] int NOT NULL,
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
        CONSTRAINT [PK_LedgerExportBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LedgerExportBatches_AspNetUsers_GeneratedByUserId] FOREIGN KEY ([GeneratedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LedgerExportBatches_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [LibraryBarcodeScans] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ScanCode] nvarchar(120) NOT NULL,
        [Purpose] int NOT NULL,
        [LibraryBookCopyId] uniqueidentifier NULL,
        [ScannedOn] datetime2 NOT NULL,
        [ResultMessage] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LibraryBarcodeScans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryBarcodeScans_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBarcodeScans_LibraryBookCopies_LibraryBookCopyId] FOREIGN KEY ([LibraryBookCopyId]) REFERENCES [LibraryBookCopies] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [LibraryOverdueNotifications] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookIssueId] uniqueidentifier NOT NULL,
        [LibraryMemberId] uniqueidentifier NOT NULL,
        [Channel] int NOT NULL,
        [Recipient] nvarchar(250) NOT NULL,
        [NotifiedOn] datetime2 NULL,
        [Status] int NOT NULL,
        [AttemptCount] int NOT NULL,
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
        CONSTRAINT [PK_LibraryOverdueNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryOverdueNotifications_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryOverdueNotifications_LibraryBookIssues_LibraryBookIssueId] FOREIGN KEY ([LibraryBookIssueId]) REFERENCES [LibraryBookIssues] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryOverdueNotifications_LibraryMembers_LibraryMemberId] FOREIGN KEY ([LibraryMemberId]) REFERENCES [LibraryMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [LibraryRenewals] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookIssueId] uniqueidentifier NOT NULL,
        [RenewedOn] datetime2 NOT NULL,
        [PreviousDueDate] date NOT NULL,
        [NewDueDate] date NOT NULL,
        [RenewedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_LibraryRenewals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryRenewals_AspNetUsers_RenewedByUserId] FOREIGN KEY ([RenewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryRenewals_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryRenewals_LibraryBookIssues_LibraryBookIssueId] FOREIGN KEY ([LibraryBookIssueId]) REFERENCES [LibraryBookIssues] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [LibraryReservations] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookId] uniqueidentifier NOT NULL,
        [LibraryBookCopyId] uniqueidentifier NULL,
        [LibraryMemberId] uniqueidentifier NOT NULL,
        [ReservedOn] datetime2 NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [FulfilledOn] datetime2 NULL,
        [CancelledOn] datetime2 NULL,
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
        CONSTRAINT [PK_LibraryReservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryReservations_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryReservations_LibraryBookCopies_LibraryBookCopyId] FOREIGN KEY ([LibraryBookCopyId]) REFERENCES [LibraryBookCopies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryReservations_LibraryBooks_LibraryBookId] FOREIGN KEY ([LibraryBookId]) REFERENCES [LibraryBooks] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryReservations_LibraryMembers_LibraryMemberId] FOREIGN KEY ([LibraryMemberId]) REFERENCES [LibraryMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [NotificationDeliveryAttempts] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [NotificationMessageId] uniqueidentifier NOT NULL,
        [NotificationRecipientId] uniqueidentifier NULL,
        [AttemptNumber] int NOT NULL,
        [Channel] int NOT NULL,
        [ProviderKey] nvarchar(80) NULL,
        [Status] int NOT NULL,
        [AttemptedOn] datetime2 NOT NULL,
        [NextRetryOn] datetime2 NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        [ProviderResponseJson] nvarchar(4000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationDeliveryAttempts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationDeliveryAttempts_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationDeliveryAttempts_NotificationMessages_NotificationMessageId] FOREIGN KEY ([NotificationMessageId]) REFERENCES [NotificationMessages] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationDeliveryAttempts_NotificationRecipients_NotificationRecipientId] FOREIGN KEY ([NotificationRecipientId]) REFERENCES [NotificationRecipients] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [OnlinePaymentTransactions] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NOT NULL,
        [GatewayProviderKey] nvarchar(80) NOT NULL,
        [GatewayOrderId] nvarchar(120) NOT NULL,
        [GatewayPaymentId] nvarchar(120) NULL,
        [Amount] decimal(12,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [Status] int NOT NULL,
        [InitiatedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [CallbackPayloadJson] nvarchar(4000) NULL,
        [FailureReason] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_OnlinePaymentTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OnlinePaymentTransactions_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OnlinePaymentTransactions_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [TransportDocumentReminders] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [ReminderType] int NOT NULL,
        [DueOn] date NOT NULL,
        [Status] int NOT NULL,
        [ReminderSentOn] datetime2 NULL,
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
        CONSTRAINT [PK_TransportDocumentReminders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportDocumentReminders_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportDocumentReminders_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [TransportMaintenanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [Category] int NOT NULL,
        [DueOn] date NOT NULL,
        [CompletedOn] date NULL,
        [OdometerReading] int NULL,
        [Status] int NOT NULL,
        [ReminderSentOn] datetime2 NULL,
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
        CONSTRAINT [PK_TransportMaintenanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportMaintenanceRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportMaintenanceRecords_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [VehicleGpsPings] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NULL,
        [Latitude] decimal(10,7) NOT NULL,
        [Longitude] decimal(10,7) NOT NULL,
        [SpeedKmph] decimal(8,2) NULL,
        [HeadingDegrees] decimal(6,2) NULL,
        [RecordedOn] datetime2 NOT NULL,
        [ProviderKey] nvarchar(80) NULL,
        [RawPayloadJson] nvarchar(4000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_VehicleGpsPings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleGpsPings_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_VehicleGpsPings_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_VehicleGpsPings_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryBookCopies_TenantId_BranchId_Barcode] ON [LibraryBookCopies] ([TenantId], [BranchId], [Barcode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_BranchId] ON [FeeConcessionRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_DecidedByUserId] ON [FeeConcessionRequests] ([DecidedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_FeeInvoiceId] ON [FeeConcessionRequests] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_FeeStructureId] ON [FeeConcessionRequests] ([FeeStructureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_RequestedByUserId] ON [FeeConcessionRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_StudentProfileId] ON [FeeConcessionRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeConcessionRequests_TenantId_BranchId_Status] ON [FeeConcessionRequests] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_ApprovedByUserId] ON [FeeRefunds] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_BranchId] ON [FeeRefunds] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_FeeInvoiceId] ON [FeeRefunds] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_FeePaymentId] ON [FeeRefunds] ([FeePaymentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_RequestedByUserId] ON [FeeRefunds] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeRefunds_TenantId_BranchId_Status] ON [FeeRefunds] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeReminders_BranchId] ON [FeeReminders] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeReminders_FeeInvoiceId] ON [FeeReminders] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeReminders_StudentProfileId] ON [FeeReminders] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_FeeReminders_TenantId_BranchId_Status_ReminderOn] ON [FeeReminders] ([TenantId], [BranchId], [Status], [ReminderOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_ApprovedByUserId] ON [HostelAllocationTransferRequests] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_BranchId] ON [HostelAllocationTransferRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_FromBedId] ON [HostelAllocationTransferRequests] ([FromBedId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_FromRoomId] ON [HostelAllocationTransferRequests] ([FromRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_HostelAllocationId] ON [HostelAllocationTransferRequests] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_RequestedByUserId] ON [HostelAllocationTransferRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_TenantId_BranchId_Status] ON [HostelAllocationTransferRequests] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_ToBedId] ON [HostelAllocationTransferRequests] ([ToBedId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelAllocationTransferRequests_ToRoomId] ON [HostelAllocationTransferRequests] ([ToRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_BranchId] ON [HostelMaintenanceRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_HostelBedId] ON [HostelMaintenanceRequests] ([HostelBedId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_HostelBlockId] ON [HostelMaintenanceRequests] ([HostelBlockId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_HostelRoomId] ON [HostelMaintenanceRequests] ([HostelRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_ReportedByUserId] ON [HostelMaintenanceRequests] ([ReportedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceRequests_TenantId_BranchId_Status_Priority] ON [HostelMaintenanceRequests] ([TenantId], [BranchId], [Status], [Priority]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelVisitorLogs_ApprovedByUserId] ON [HostelVisitorLogs] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelVisitorLogs_BranchId] ON [HostelVisitorLogs] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelVisitorLogs_HostelAllocationId] ON [HostelVisitorLogs] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelVisitorLogs_StudentProfileId] ON [HostelVisitorLogs] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_HostelVisitorLogs_TenantId_BranchId_CheckInOn] ON [HostelVisitorLogs] ([TenantId], [BranchId], [CheckInOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LedgerExportBatches_BranchId] ON [LedgerExportBatches] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LedgerExportBatches_GeneratedByUserId] ON [LedgerExportBatches] ([GeneratedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LedgerExportBatches_TenantId_BranchId_ExportType_RequestedOn] ON [LedgerExportBatches] ([TenantId], [BranchId], [ExportType], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryBarcodeScans_BranchId] ON [LibraryBarcodeScans] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryBarcodeScans_LibraryBookCopyId] ON [LibraryBarcodeScans] ([LibraryBookCopyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryBarcodeScans_TenantId_BranchId_ScanCode_ScannedOn] ON [LibraryBarcodeScans] ([TenantId], [BranchId], [ScanCode], [ScannedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryOverdueNotifications_BranchId] ON [LibraryOverdueNotifications] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryOverdueNotifications_LibraryBookIssueId] ON [LibraryOverdueNotifications] ([LibraryBookIssueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryOverdueNotifications_LibraryMemberId] ON [LibraryOverdueNotifications] ([LibraryMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryOverdueNotifications_TenantId_BranchId_Status] ON [LibraryOverdueNotifications] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryRenewals_BranchId] ON [LibraryRenewals] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryRenewals_LibraryBookIssueId] ON [LibraryRenewals] ([LibraryBookIssueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryRenewals_RenewedByUserId] ON [LibraryRenewals] ([RenewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryRenewals_TenantId_LibraryBookIssueId_RenewedOn] ON [LibraryRenewals] ([TenantId], [LibraryBookIssueId], [RenewedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryReservations_BranchId] ON [LibraryReservations] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryReservations_LibraryBookCopyId] ON [LibraryReservations] ([LibraryBookCopyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryReservations_LibraryBookId] ON [LibraryReservations] ([LibraryBookId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryReservations_LibraryMemberId] ON [LibraryReservations] ([LibraryMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_LibraryReservations_TenantId_BranchId_Status_ExpiresOn] ON [LibraryReservations] ([TenantId], [BranchId], [Status], [ExpiresOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_NotificationDeliveryAttempts_BranchId] ON [NotificationDeliveryAttempts] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_NotificationDeliveryAttempts_NotificationMessageId] ON [NotificationDeliveryAttempts] ([NotificationMessageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_NotificationDeliveryAttempts_NotificationRecipientId] ON [NotificationDeliveryAttempts] ([NotificationRecipientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_NotificationDeliveryAttempts_TenantId_BranchId_Status_NextRetryOn] ON [NotificationDeliveryAttempts] ([TenantId], [BranchId], [Status], [NextRetryOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_NotificationDeliveryAttempts_TenantId_NotificationMessageId_NotificationRecipientId_AttemptNumber] ON [NotificationDeliveryAttempts] ([TenantId], [NotificationMessageId], [NotificationRecipientId], [AttemptNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_OnlinePaymentTransactions_BranchId] ON [OnlinePaymentTransactions] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_OnlinePaymentTransactions_FeeInvoiceId] ON [OnlinePaymentTransactions] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_OnlinePaymentTransactions_TenantId_BranchId_Status] ON [OnlinePaymentTransactions] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OnlinePaymentTransactions_TenantId_GatewayProviderKey_GatewayOrderId] ON [OnlinePaymentTransactions] ([TenantId], [GatewayProviderKey], [GatewayOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportDocumentReminders_BranchId] ON [TransportDocumentReminders] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportDocumentReminders_TenantId_BranchId_Status_DueOn] ON [TransportDocumentReminders] ([TenantId], [BranchId], [Status], [DueOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportDocumentReminders_VehicleId] ON [TransportDocumentReminders] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportMaintenanceRecords_BranchId] ON [TransportMaintenanceRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportMaintenanceRecords_TenantId_BranchId_Status_DueOn] ON [TransportMaintenanceRecords] ([TenantId], [BranchId], [Status], [DueOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TransportMaintenanceRecords_VehicleId] ON [TransportMaintenanceRecords] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_VehicleGpsPings_BranchId] ON [VehicleGpsPings] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_VehicleGpsPings_TenantId_VehicleId_RecordedOn] ON [VehicleGpsPings] ([TenantId], [VehicleId], [RecordedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_VehicleGpsPings_TransportRouteId] ON [VehicleGpsPings] ([TransportRouteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_VehicleGpsPings_VehicleId] ON [VehicleGpsPings] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915223844_EnterpriseWorkflowEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260915223844_EnterpriseWorkflowEnhancements', N'9.0.0');
END;

COMMIT;
GO

