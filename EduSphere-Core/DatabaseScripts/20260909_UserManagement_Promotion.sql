/*
EduSphere User Management promotion script
Target database: SQL Server
Migration: 20260908204854_UserManagement

Run this script on the test/UAT database after taking a backup.
It is written to be re-runnable: schema objects, indexes, migration history,
and data backfill rows are guarded against duplicates.
*/

SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory]
    (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

IF COL_LENGTH(N'dbo.AspNetUsers', N'ActivatedOn') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] ADD [ActivatedOn] datetime2 NULL;
END;
GO

IF COL_LENGTH(N'dbo.AspNetUsers', N'LastPasswordChangedOn') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] ADD [LastPasswordChangedOn] datetime2 NULL;
END;
GO

IF COL_LENGTH(N'dbo.AspNetUsers', N'LastPasswordResetRequestedOn') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] ADD [LastPasswordResetRequestedOn] datetime2 NULL;
END;
GO

IF COL_LENGTH(N'dbo.AspNetUsers', N'RequiresActivation') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
        ADD [RequiresActivation] bit NOT NULL
            CONSTRAINT [DF_AspNetUsers_RequiresActivation] DEFAULT CAST(0 AS bit);
END;
GO

IF OBJECT_ID(N'[dbo].[ParentProfiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ParentProfiles]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Occupation] nvarchar(120) NULL,
        [Address] nvarchar(500) NULL,
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
        CONSTRAINT [PK_ParentProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ParentProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[dbo].[StaffProfiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[StaffProfiles]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [DepartmentId] uniqueidentifier NULL,
        [EmployeeNumber] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Designation] nvarchar(120) NULL,
        [EmploymentType] int NOT NULL,
        [DateOfBirth] date NULL,
        [Gender] int NOT NULL,
        [JoiningDate] date NOT NULL,
        [Qualifications] nvarchar(500) NULL,
        [ExperienceYears] decimal(18,2) NOT NULL,
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
        CONSTRAINT [PK_StaffProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffProfiles_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[dbo].[UserBranchAssignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserBranchAssignments]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [IsPrimary] bit NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveUntil] date NULL,
        [IsActive] bit NOT NULL,
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
        CONSTRAINT [PK_UserBranchAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserBranchAssignments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserBranchAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[dbo].[UserInvitations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserInvitations]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Email] nvarchar(150) NOT NULL,
        [RoleName] nvarchar(80) NOT NULL,
        [ActivationTokenHash] nvarchar(128) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [AcceptedOn] datetime2 NULL,
        [LastSentOn] datetime2 NULL,
        [SendAttempts] int NOT NULL,
        [Status] int NOT NULL,
        [InvitedByUserId] uniqueidentifier NULL,
        [LastSendError] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserInvitations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserInvitations_AspNetUsers_InvitedByUserId] FOREIGN KEY ([InvitedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserInvitations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserInvitations_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[dbo].[UserRoleAssignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserRoleAssignments]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [RoleName] nvarchar(80) NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [AssignedByUserId] uniqueidentifier NULL,
        [AssignedOn] datetime2 NOT NULL,
        [EffectiveUntil] datetime2 NULL,
        [IsActive] bit NOT NULL,
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
        CONSTRAINT [PK_UserRoleAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRoleAssignments_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_AspNetUsers_AssignedByUserId] FOREIGN KEY ([AssignedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[Branches] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_AspNetUsers_RequiresActivation' AND [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]'))
    CREATE INDEX [IX_AspNetUsers_RequiresActivation] ON [dbo].[AspNetUsers] ([RequiresActivation]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_AspNetUsers_TenantId_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]'))
    CREATE INDEX [IX_AspNetUsers_TenantId_BranchId] ON [dbo].[AspNetUsers] ([TenantId], [BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ParentProfiles_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[ParentProfiles]'))
    CREATE INDEX [IX_ParentProfiles_BranchId] ON [dbo].[ParentProfiles] ([BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ParentProfiles_TenantId_Email' AND [object_id] = OBJECT_ID(N'[dbo].[ParentProfiles]'))
    CREATE INDEX [IX_ParentProfiles_TenantId_Email] ON [dbo].[ParentProfiles] ([TenantId], [Email]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ParentProfiles_TenantId_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[ParentProfiles]'))
    CREATE UNIQUE INDEX [IX_ParentProfiles_TenantId_UserId] ON [dbo].[ParentProfiles] ([TenantId], [UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ParentProfiles_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[ParentProfiles]'))
    CREATE INDEX [IX_ParentProfiles_UserId] ON [dbo].[ParentProfiles] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StaffProfiles_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[StaffProfiles]'))
    CREATE INDEX [IX_StaffProfiles_BranchId] ON [dbo].[StaffProfiles] ([BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StaffProfiles_DepartmentId' AND [object_id] = OBJECT_ID(N'[dbo].[StaffProfiles]'))
    CREATE INDEX [IX_StaffProfiles_DepartmentId] ON [dbo].[StaffProfiles] ([DepartmentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StaffProfiles_TenantId_EmployeeNumber' AND [object_id] = OBJECT_ID(N'[dbo].[StaffProfiles]'))
    CREATE UNIQUE INDEX [IX_StaffProfiles_TenantId_EmployeeNumber] ON [dbo].[StaffProfiles] ([TenantId], [EmployeeNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StaffProfiles_TenantId_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[StaffProfiles]'))
    CREATE UNIQUE INDEX [IX_StaffProfiles_TenantId_UserId] ON [dbo].[StaffProfiles] ([TenantId], [UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_StaffProfiles_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[StaffProfiles]'))
    CREATE INDEX [IX_StaffProfiles_UserId] ON [dbo].[StaffProfiles] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserBranchAssignments_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[UserBranchAssignments]'))
    CREATE INDEX [IX_UserBranchAssignments_BranchId] ON [dbo].[UserBranchAssignments] ([BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserBranchAssignments_TenantId_BranchId_IsActive' AND [object_id] = OBJECT_ID(N'[dbo].[UserBranchAssignments]'))
    CREATE INDEX [IX_UserBranchAssignments_TenantId_BranchId_IsActive] ON [dbo].[UserBranchAssignments] ([TenantId], [BranchId], [IsActive]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserBranchAssignments_TenantId_UserId_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[UserBranchAssignments]'))
    CREATE UNIQUE INDEX [IX_UserBranchAssignments_TenantId_UserId_BranchId] ON [dbo].[UserBranchAssignments] ([TenantId], [UserId], [BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserBranchAssignments_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[UserBranchAssignments]'))
    CREATE INDEX [IX_UserBranchAssignments_UserId] ON [dbo].[UserBranchAssignments] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserInvitations_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[UserInvitations]'))
    CREATE INDEX [IX_UserInvitations_BranchId] ON [dbo].[UserInvitations] ([BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserInvitations_InvitedByUserId' AND [object_id] = OBJECT_ID(N'[dbo].[UserInvitations]'))
    CREATE INDEX [IX_UserInvitations_InvitedByUserId] ON [dbo].[UserInvitations] ([InvitedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserInvitations_TenantId_Email_Status' AND [object_id] = OBJECT_ID(N'[dbo].[UserInvitations]'))
    CREATE INDEX [IX_UserInvitations_TenantId_Email_Status] ON [dbo].[UserInvitations] ([TenantId], [Email], [Status]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserInvitations_TenantId_UserId_Status' AND [object_id] = OBJECT_ID(N'[dbo].[UserInvitations]'))
    CREATE INDEX [IX_UserInvitations_TenantId_UserId_Status] ON [dbo].[UserInvitations] ([TenantId], [UserId], [Status]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserInvitations_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[UserInvitations]'))
    CREATE INDEX [IX_UserInvitations_UserId] ON [dbo].[UserInvitations] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_AssignedByUserId' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE INDEX [IX_UserRoleAssignments_AssignedByUserId] ON [dbo].[UserRoleAssignments] ([AssignedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE INDEX [IX_UserRoleAssignments_BranchId] ON [dbo].[UserRoleAssignments] ([BranchId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_RoleId' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE INDEX [IX_UserRoleAssignments_RoleId] ON [dbo].[UserRoleAssignments] ([RoleId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_TenantId_RoleName_IsActive' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE INDEX [IX_UserRoleAssignments_TenantId_RoleName_IsActive] ON [dbo].[UserRoleAssignments] ([TenantId], [RoleName], [IsActive]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_TenantId_UserId_RoleName_BranchId' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE UNIQUE INDEX [IX_UserRoleAssignments_TenantId_UserId_RoleName_BranchId]
        ON [dbo].[UserRoleAssignments] ([TenantId], [UserId], [RoleName], [BranchId])
        WHERE [BranchId] IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_UserRoleAssignments_UserId' AND [object_id] = OBJECT_ID(N'[dbo].[UserRoleAssignments]'))
    CREATE INDEX [IX_UserRoleAssignments_UserId] ON [dbo].[UserRoleAssignments] ([UserId]);
GO

BEGIN TRY
    BEGIN TRANSACTION;

    /* Existing table data update: keep legacy password users sign-in compatible. */
    UPDATE [dbo].[AspNetUsers]
    SET [EmailConfirmed] = 1,
        [ActivatedOn] = COALESCE([ActivatedOn], [CreatedOn], SYSUTCDATETIME()),
        [LastPasswordChangedOn] = COALESCE([LastPasswordChangedOn], [CreatedOn], SYSUTCDATETIME())
    WHERE [RequiresActivation] = 0
      AND [PasswordHash] IS NOT NULL
      AND [EmailConfirmed] = 0;

    /* Backfill new branch assignment table from the existing AspNetUsers.BranchId column. */
    INSERT INTO [dbo].[UserBranchAssignments]
    (
        [Id], [UserId], [BranchId], [IsPrimary], [EffectiveFrom], [EffectiveUntil], [IsActive], [Notes],
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy], [DeletedOn],
        [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], u.[BranchId], 1, CAST(COALESCE(u.[CreatedOn], SYSUTCDATETIME()) AS date), NULL, u.[IsActive],
        N'Seeded from AspNetUsers.BranchId during user-management rollout.',
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL,
        NEWID(), u.[TenantId]
    FROM [dbo].[AspNetUsers] u
    WHERE u.[BranchId] IS NOT NULL
      AND EXISTS (SELECT 1 FROM [dbo].[Branches] b WHERE b.[Id] = u.[BranchId])
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[UserBranchAssignments] a
          WHERE a.[TenantId] = u.[TenantId]
            AND a.[UserId] = u.[Id]
            AND a.[BranchId] = u.[BranchId]
            AND a.[IsDeleted] = 0
      );

    /* Backfill new role assignment audit table from existing Identity role membership. */
    INSERT INTO [dbo].[UserRoleAssignments]
    (
        [Id], [UserId], [RoleId], [RoleName], [BranchId], [AssignedByUserId], [AssignedOn], [EffectiveUntil],
        [IsActive], [Notes], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy],
        [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], r.[Id], r.[Name], u.[BranchId], NULL, COALESCE(u.[CreatedOn], SYSUTCDATETIME()), NULL,
        u.[IsActive], N'Seeded from AspNetUserRoles during user-management rollout.',
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL,
        NEWID(), u.[TenantId]
    FROM [dbo].[AspNetUserRoles] ur
    JOIN [dbo].[AspNetUsers] u ON u.[Id] = ur.[UserId]
    JOIN [dbo].[AspNetRoles] r ON r.[Id] = ur.[RoleId]
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[UserRoleAssignments] a
        WHERE a.[TenantId] = u.[TenantId]
          AND a.[UserId] = u.[Id]
          AND a.[RoleName] = r.[Name]
          AND ((a.[BranchId] IS NULL AND u.[BranchId] IS NULL) OR a.[BranchId] = u.[BranchId])
          AND a.[IsDeleted] = 0
    );

    /* Backfill staff profiles for existing non-teacher operational/admin staff users. */
    WITH StaffUsers AS
    (
        SELECT
            u.[Id] AS [UserId],
            u.[TenantId],
            u.[BranchId],
            u.[FirstName],
            u.[MiddleName],
            u.[LastName],
            u.[Email],
            u.[PhoneNumber],
            COALESCE(MIN(r.[Name]), N'Staff') AS [RoleName]
        FROM [dbo].[AspNetUsers] u
        JOIN [dbo].[AspNetUserRoles] ur ON ur.[UserId] = u.[Id]
        JOIN [dbo].[AspNetRoles] r ON r.[Id] = ur.[RoleId]
        WHERE r.[Name] IN
        (
            N'BranchAdmin', N'Principal', N'DepartmentAdmin', N'StaffAdmin',
            N'Accountant', N'Librarian', N'TransportManager', N'HostelManager', N'ExamController'
        )
        GROUP BY u.[Id], u.[TenantId], u.[BranchId], u.[FirstName], u.[MiddleName], u.[LastName], u.[Email], u.[PhoneNumber]
    )
    INSERT INTO [dbo].[StaffProfiles]
    (
        [Id], [UserId], [BranchId], [DepartmentId], [EmployeeNumber], [FirstName], [MiddleName], [LastName],
        [Email], [PhoneNumber], [Designation], [EmploymentType], [DateOfBirth], [Gender], [JoiningDate],
        [Qualifications], [ExperienceYears], [Status], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
        [IsDeleted], [DeletedBy], [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), su.[UserId], su.[BranchId], d.[Id],
        CONCAT(N'STF-SEED-', LEFT(REPLACE(CONVERT(varchar(36), su.[UserId]), '-', ''), 12)),
        COALESCE(NULLIF(su.[FirstName], N''), N'Staff'),
        su.[MiddleName],
        COALESCE(NULLIF(su.[LastName], N''), N'Member'),
        su.[Email],
        su.[PhoneNumber],
        CASE su.[RoleName]
            WHEN N'BranchAdmin' THEN N'Branch Administrator'
            WHEN N'DepartmentAdmin' THEN N'Department Administrator'
            WHEN N'StaffAdmin' THEN N'Staff Administrator'
            WHEN N'TransportManager' THEN N'Transport Manager'
            WHEN N'HostelManager' THEN N'Hostel Manager'
            WHEN N'ExamController' THEN N'Exam Controller'
            ELSE su.[RoleName]
        END,
        1, NULL, 0, CAST(SYSUTCDATETIME() AS date), NULL, 0, 1,
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), su.[TenantId]
    FROM StaffUsers su
    OUTER APPLY
    (
        SELECT TOP (1) dep.[Id]
        FROM [dbo].[Departments] dep
        WHERE dep.[TenantId] = su.[TenantId]
          AND dep.[IsDeleted] = 0
        ORDER BY dep.[Name]
    ) d
    WHERE su.[TenantId] <> '00000000-0000-0000-0000-000000000000'
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[StaffProfiles] sp
          WHERE sp.[TenantId] = su.[TenantId]
            AND sp.[UserId] = su.[UserId]
            AND sp.[IsDeleted] = 0
      );

    /* Backfill parent profiles for existing Parent role users. */
    WITH ParentUsers AS
    (
        SELECT DISTINCT
            u.[Id] AS [UserId],
            u.[TenantId],
            u.[BranchId],
            u.[FirstName],
            u.[MiddleName],
            u.[LastName],
            u.[Email],
            u.[PhoneNumber]
        FROM [dbo].[AspNetUsers] u
        JOIN [dbo].[AspNetUserRoles] ur ON ur.[UserId] = u.[Id]
        JOIN [dbo].[AspNetRoles] r ON r.[Id] = ur.[RoleId]
        WHERE r.[Name] = N'Parent'
    )
    INSERT INTO [dbo].[ParentProfiles]
    (
        [Id], [UserId], [BranchId], [FirstName], [MiddleName], [LastName], [Email], [PhoneNumber], [Occupation],
        [Address], [IsActive], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy],
        [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), pu.[UserId], pu.[BranchId],
        COALESCE(NULLIF(pu.[FirstName], N''), N'Parent'),
        pu.[MiddleName],
        COALESCE(NULLIF(pu.[LastName], N''), N'Guardian'),
        COALESCE(pu.[Email], CONCAT(N'parent-', LEFT(REPLACE(CONVERT(varchar(36), pu.[UserId]), '-', ''), 8), N'@edusphere.local')),
        pu.[PhoneNumber], N'Guardian', NULL, 1,
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), pu.[TenantId]
    FROM ParentUsers pu
    WHERE pu.[TenantId] <> '00000000-0000-0000-0000-000000000000'
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[ParentProfiles] pp
          WHERE pp.[TenantId] = pu.[TenantId]
            AND pp.[UserId] = pu.[UserId]
            AND pp.[IsDeleted] = 0
      );

    /* Legacy accepted invitation audit rows for existing active users. */
    INSERT INTO [dbo].[UserInvitations]
    (
        [Id], [UserId], [BranchId], [Email], [RoleName], [ActivationTokenHash], [ExpiresOn], [AcceptedOn],
        [LastSentOn], [SendAttempts], [Status], [InvitedByUserId], [LastSendError], [CreatedBy], [CreatedOn],
        [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy], [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], u.[BranchId], u.[Email], COALESCE(r.[Name], N'User'), N'LEGACY-ACTIVATED',
        DATEADD(day, 7, COALESCE(u.[CreatedOn], SYSUTCDATETIME())),
        COALESCE(u.[ActivatedOn], u.[CreatedOn], SYSUTCDATETIME()),
        COALESCE(u.[CreatedOn], SYSUTCDATETIME()), 0, 2, NULL, NULL,
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), u.[TenantId]
    FROM [dbo].[AspNetUsers] u
    OUTER APPLY
    (
        SELECT TOP (1) ar.[Name]
        FROM [dbo].[AspNetUserRoles] ur
        JOIN [dbo].[AspNetRoles] ar ON ar.[Id] = ur.[RoleId]
        WHERE ur.[UserId] = u.[Id]
        ORDER BY ar.[Name]
    ) r
    WHERE u.[TenantId] <> '00000000-0000-0000-0000-000000000000'
      AND u.[Email] IS NOT NULL
      AND u.[RequiresActivation] = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[UserInvitations] i
          WHERE i.[TenantId] = u.[TenantId]
            AND i.[UserId] = u.[Id]
            AND i.[ActivationTokenHash] = N'LEGACY-ACTIVATED'
            AND i.[IsDeleted] = 0
      );

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20260908204854_UserManagement'
    )
    BEGIN
        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260908204854_UserManagement', N'9.0.0');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

/*
Seed data for the parent portal path.
This mirrors the local development seed by adding four Parent users tied to the
first four existing students by deterministic ordering. Default password:
Parent@12345
*/
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ParentRoleId uniqueidentifier =
        (SELECT TOP (1) [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = N'PARENT' OR [Name] = N'Parent');

    IF @ParentRoleId IS NULL
    BEGIN
        SET @ParentRoleId = '66666666-6666-6666-6666-666666666666';
        INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
        VALUES (@ParentRoleId, N'Parent', N'PARENT', CONVERT(nvarchar(36), NEWID()), N'Parent platform role');
    END;

    DECLARE @ParentSeed TABLE
    (
        [RowNumber] int NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [Occupation] nvarchar(120) NOT NULL,
        [Relationship] int NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL
    );

    INSERT INTO @ParentSeed
        ([RowNumber], [FirstName], [LastName], [PhoneNumber], [Occupation], [Relationship], [PasswordHash])
    VALUES
        (1, N'Ananya', N'Sharma', N'+91-98765-44011', N'Product Manager', 2, N'AQAAAAIAAYagAAAAEEtkqT7cbHGFKBIT3RQjJYJyR0wuzivqK3k3rq8SdVW9+X31lAxrK2dK8vkY2i21Rw=='),
        (2, N'Rohan', N'Mehta', N'+91-98765-44012', N'Chartered Accountant', 1, N'AQAAAAIAAYagAAAAEO4yuxI24eVNYOx5zHtJp3SY4Rk5Ce7oeKDwdXBHev5Kl70IDIEgrQlbkn1J2NkcTA=='),
        (3, N'Priya', N'Nair', N'+91-98765-44013', N'Doctor', 2, N'AQAAAAIAAYagAAAAECOqLykz2CE2kUsh1iR2lnzoY8xQsY2Mq7AYy/wEFuCh7hiEsvR7uVZe8QJD+SXdug=='),
        (4, N'Vikram', N'Rao', N'+91-98765-44014', N'Civil Engineer', 1, N'AQAAAAIAAYagAAAAEKXR/mDChC5nQMnMBTohXFamJgqbWqeZ5wPFzYzBL2jH1zy8y3oaTNmwGTCL1+cu0w==');

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0
          AND b.[IsDeleted] = 0
          AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId],
            ps.[TenantId],
            ps.[BranchId],
            seed.[FirstName],
            seed.[LastName],
            seed.[PhoneNumber],
            seed.[Occupation],
            seed.[Relationship],
            seed.[PasswordHash],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[AspNetUsers]
    (
        [Id], [FirstName], [MiddleName], [LastName], [UserType], [IsActive], [CreatedOn], [LastLoginAt], [ModifiedOn],
        [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp],
        [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled],
        [AccessFailedCount], [CreatedBy], [ModifiedBy], [ConcurrencyToken], [TenantId], [BranchId], [ActivatedOn],
        [LastPasswordChangedOn], [LastPasswordResetRequestedOn], [RequiresActivation]
    )
    SELECT
        NEWID(), p.[FirstName], NULL, p.[LastName], 6, 1, SYSUTCDATETIME(), NULL, NULL,
        p.[Email], UPPER(p.[Email]), p.[Email], UPPER(p.[Email]), 1, p.[PasswordHash], CONVERT(nvarchar(36), NEWID()),
        CONVERT(nvarchar(36), NEWID()), p.[PhoneNumber], 0, 0, NULL, 1,
        0, N'system', NULL, NEWID(), p.[TenantId], p.[BranchId], SYSUTCDATETIME(),
        SYSUTCDATETIME(), NULL, 0
    FROM ParentRows p
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] u WHERE u.[NormalizedEmail] = UPPER(p.[Email]));

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
    SELECT u.[Id], @ParentRoleId
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AspNetUserRoles] ur
        WHERE ur.[UserId] = u.[Id]
          AND ur.[RoleId] = @ParentRoleId
    );

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[ParentProfiles]
    (
        [Id], [UserId], [BranchId], [FirstName], [MiddleName], [LastName], [Email], [PhoneNumber], [Occupation],
        [Address], [IsActive], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy],
        [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], p.[BranchId], p.[FirstName], NULL, p.[LastName], p.[Email], p.[PhoneNumber], p.[Occupation],
        N'Seeded parent portal contact for test/UAT.', 1,
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), p.[TenantId]
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[ParentProfiles] pp
        WHERE pp.[TenantId] = p.[TenantId]
          AND pp.[UserId] = u.[Id]
          AND pp.[IsDeleted] = 0
    );

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[UserBranchAssignments]
    (
        [Id], [UserId], [BranchId], [IsPrimary], [EffectiveFrom], [EffectiveUntil], [IsActive], [Notes],
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy], [DeletedOn],
        [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], p.[BranchId], 1, CAST(SYSUTCDATETIME() AS date), NULL, 1,
        N'Seeded parent primary branch assignment.', N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL,
        NULL, NEWID(), p.[TenantId]
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[UserBranchAssignments] a
        WHERE a.[TenantId] = p.[TenantId]
          AND a.[UserId] = u.[Id]
          AND a.[BranchId] = p.[BranchId]
          AND a.[IsDeleted] = 0
    );

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[UserRoleAssignments]
    (
        [Id], [UserId], [RoleId], [RoleName], [BranchId], [AssignedByUserId], [AssignedOn], [EffectiveUntil],
        [IsActive], [Notes], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy],
        [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], @ParentRoleId, N'Parent', p.[BranchId], NULL, SYSUTCDATETIME(), NULL,
        1, N'Seeded parent role assignment.', N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL,
        NULL, NEWID(), p.[TenantId]
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[UserRoleAssignments] a
        WHERE a.[TenantId] = p.[TenantId]
          AND a.[UserId] = u.[Id]
          AND a.[RoleName] = N'Parent'
          AND a.[BranchId] = p.[BranchId]
          AND a.[IsDeleted] = 0
    );

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[StudentGuardians]
    (
        [Id], [StudentProfileId], [ParentUserId], [Relationship], [FullName], [Email], [PhoneNumber], [Occupation],
        [IsPrimary], [HasPortalAccess], [CanPickup], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
        [IsDeleted], [DeletedBy], [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), p.[StudentProfileId], u.[Id], p.[Relationship], CONCAT(p.[FirstName], N' ', p.[LastName]), p.[Email],
        p.[PhoneNumber], p.[Occupation], 1, 1, 1, N'system', SYSUTCDATETIME(), NULL, NULL,
        0, NULL, NULL, NEWID(), p.[TenantId]
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[StudentGuardians] g
        WHERE g.[TenantId] = p.[TenantId]
          AND g.[StudentProfileId] = p.[StudentProfileId]
          AND g.[Email] = p.[Email]
          AND g.[IsDeleted] = 0
    );

    WITH PickedStudents AS
    (
        SELECT TOP (4)
            ROW_NUMBER() OVER (ORDER BY t.[Name], b.[Name], s.[FirstName], s.[LastName], s.[Id]) AS [RowNumber],
            s.[Id] AS [StudentProfileId],
            s.[TenantId],
            s.[BranchId]
        FROM [dbo].[StudentProfiles] s
        JOIN [dbo].[Branches] b ON b.[Id] = s.[BranchId]
        JOIN [dbo].[Tenants] t ON t.[Id] = s.[TenantId]
        WHERE s.[IsDeleted] = 0 AND b.[IsDeleted] = 0 AND t.[IsDeleted] = 0
    ),
    ParentRows AS
    (
        SELECT
            ps.[StudentProfileId], ps.[TenantId], ps.[BranchId],
            seed.[FirstName], seed.[LastName], seed.[PhoneNumber], seed.[Occupation], seed.[Relationship],
            CONCAT(N'parent.', LOWER(seed.[FirstName]), N'.', LOWER(seed.[LastName]), seed.[RowNumber], N'@edusphere.local') AS [Email]
        FROM PickedStudents ps
        JOIN @ParentSeed seed ON seed.[RowNumber] = ps.[RowNumber]
    )
    INSERT INTO [dbo].[UserInvitations]
    (
        [Id], [UserId], [BranchId], [Email], [RoleName], [ActivationTokenHash], [ExpiresOn], [AcceptedOn],
        [LastSentOn], [SendAttempts], [Status], [InvitedByUserId], [LastSendError], [CreatedBy], [CreatedOn],
        [ModifiedBy], [ModifiedOn], [IsDeleted], [DeletedBy], [DeletedOn], [ConcurrencyToken], [TenantId]
    )
    SELECT
        NEWID(), u.[Id], p.[BranchId], p.[Email], N'Parent', N'SEEDED-PARENT-ACTIVATED',
        DATEADD(day, 7, SYSUTCDATETIME()), SYSUTCDATETIME(), SYSUTCDATETIME(), 0, 2, NULL, NULL,
        N'system', SYSUTCDATETIME(), NULL, NULL, 0, NULL, NULL, NEWID(), p.[TenantId]
    FROM ParentRows p
    JOIN [dbo].[AspNetUsers] u ON u.[NormalizedEmail] = UPPER(p.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[UserInvitations] i
        WHERE i.[TenantId] = p.[TenantId]
          AND i.[UserId] = u.[Id]
          AND i.[ActivationTokenHash] = N'SEEDED-PARENT-ACTIVATED'
          AND i.[IsDeleted] = 0
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

SELECT [MigrationId], [ProductVersion]
FROM [dbo].[__EFMigrationsHistory]
WHERE [MigrationId] = N'20260908204854_UserManagement';

SELECT N'UserBranchAssignments' AS [TableName], COUNT(*) AS [TotalRows] FROM [dbo].[UserBranchAssignments]
UNION ALL SELECT N'UserRoleAssignments', COUNT(*) FROM [dbo].[UserRoleAssignments]
UNION ALL SELECT N'StaffProfiles', COUNT(*) FROM [dbo].[StaffProfiles]
UNION ALL SELECT N'ParentProfiles', COUNT(*) FROM [dbo].[ParentProfiles]
UNION ALL SELECT N'UserInvitations', COUNT(*) FROM [dbo].[UserInvitations];
GO
