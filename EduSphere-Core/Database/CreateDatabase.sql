IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(255) NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [Tenants] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [TenantIdentifier] nvarchar(100) NOT NULL,
        [CustomDomain] nvarchar(255) NULL,
        [Description] nvarchar(255) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [Branches] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NULL,
        [BoardType] nvarchar(50) NULL,
        [Address] nvarchar(255) NULL,
        [City] nvarchar(50) NULL,
        [State] nvarchar(50) NULL,
        [Country] nvarchar(50) NULL,
        [Pincode] nvarchar(20) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [TenantId] int NOT NULL,
        CONSTRAINT [PK_Branches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Branches_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] int NOT NULL,
        [BranchId] int NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [UserType] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUsers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Description], [Name], [NormalizedName])
    VALUES (''11111111-1111-1111-1111-111111111111'', N''11111111-1111-1111-1111-111111111111'', N''SuperAdmin platform role'', N''SuperAdmin'', N''SUPERADMIN''),
    (''22222222-2222-2222-2222-222222222222'', N''22222222-2222-2222-2222-222222222222'', N''TenantAdmin platform role'', N''TenantAdmin'', N''TENANTADMIN''),
    (''33333333-3333-3333-3333-333333333333'', N''33333333-3333-3333-3333-333333333333'', N''Principal platform role'', N''Principal'', N''PRINCIPAL''),
    (''44444444-4444-4444-4444-444444444444'', N''44444444-4444-4444-4444-444444444444'', N''Teacher platform role'', N''Teacher'', N''TEACHER''),
    (''55555555-5555-5555-5555-555555555555'', N''55555555-5555-5555-5555-555555555555'', N''Student platform role'', N''Student'', N''STUDENT''),
    (''66666666-6666-6666-6666-666666666666'', N''66666666-6666-6666-6666-666666666666'', N''Parent platform role'', N''Parent'', N''PARENT'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_BranchId] ON [AspNetUsers] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_TenantId] ON [AspNetUsers] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Branches_TenantId] ON [Branches] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Tenants_CustomDomain] ON [Tenants] ([CustomDomain]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Tenants_TenantIdentifier] ON [Tenants] ([TenantIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902200639_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260902200639_InitialCreate', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [AcademicYears] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [IsCurrent] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_AcademicYears] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Description] nvarchar(255) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [Courses] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [DurationMonths] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [DepartmentId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Courses_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [Batches] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Capacity] int NOT NULL,
        [CourseId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Batches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Batches_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Batches_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [Credits] int NOT NULL,
        [CourseId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Subjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [Sections] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Capacity] int NOT NULL,
        [BatchId] int NOT NULL,
        [ClassTeacherUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sections_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE TABLE [SyllabusUnits] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] int NOT NULL,
        [SubjectId] int NOT NULL,
        [Order] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EstimatedHours] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SyllabusUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SyllabusUnits_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AcademicYears_TenantId_Name] ON [AcademicYears] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Batches_AcademicYearId] ON [Batches] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Batches_CourseId] ON [Batches] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Batches_TenantId] ON [Batches] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Courses_DepartmentId] ON [Courses] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Courses_TenantId_Code] ON [Courses] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departments_TenantId_Code] ON [Departments] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Sections_BatchId] ON [Sections] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_Subjects_CourseId] ON [Subjects] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Subjects_TenantId_Code] ON [Subjects] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    CREATE INDEX [IX_SyllabusUnits_SubjectId] ON [SyllabusUnits] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902205713_AcademicStructure'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260902205713_AcademicStructure', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Branches_BranchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] DROP CONSTRAINT [FK_Branches_Tenants_TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] DROP CONSTRAINT [FK_Batches_AcademicYears_AcademicYearId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] DROP CONSTRAINT [FK_Batches_Courses_CourseId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] DROP CONSTRAINT [FK_Courses_Departments_DepartmentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] DROP CONSTRAINT [FK_Sections_Batches_BatchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] DROP CONSTRAINT [FK_Subjects_Courses_CourseId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [FK_SyllabusUnits_Subjects_SubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_AspNetUsers_BranchId] ON [AspNetUsers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_AspNetUsers_TenantId] ON [AspNetUsers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Branches_TenantId] ON [Branches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_AcademicYears_TenantId_Name] ON [AcademicYears];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Batches_AcademicYearId] ON [Batches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Batches_CourseId] ON [Batches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Batches_TenantId] ON [Batches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Courses_DepartmentId] ON [Courses];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Courses_TenantId_Code] ON [Courses];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Departments_TenantId_Code] ON [Departments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Sections_BatchId] ON [Sections];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Subjects_CourseId] ON [Subjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_Subjects_TenantId_Code] ON [Subjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DROP INDEX [IX_SyllabusUnits_SubjectId] ON [SyllabusUnits];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] DROP CONSTRAINT [PK_Tenants];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] DROP CONSTRAINT [PK_Branches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] DROP CONSTRAINT [PK_AcademicYears];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] DROP CONSTRAINT [PK_Departments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] DROP CONSTRAINT [PK_Courses];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] DROP CONSTRAINT [PK_Batches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] DROP CONSTRAINT [PK_Sections];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] DROP CONSTRAINT [PK_Subjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [PK_SyllabusUnits];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Tenants].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Tenants].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Tenants] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Tenants] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Branches].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Branches].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Branches] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Branches] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AcademicYears].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AcademicYears].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [AcademicYears] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [AcademicYears] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Departments].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Departments].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Departments] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Departments] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Courses].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Courses].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Courses] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Courses] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Batches] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Batches] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Sections].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Sections].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Sections] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Sections] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Subjects].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Subjects].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [Subjects] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Subjects] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[SyllabusUnits].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[SyllabusUnits].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [SyllabusUnits] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [SyllabusUnits] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers].[CreatedAt]', N'CreatedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers].[UpdatedAt]', N'ModifiedOn', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedBy] nvarchar(128) NULL;
    ALTER TABLE [AspNetUsers] ADD [ModifiedBy] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ConcurrencyToken] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [AspNetUsers] SET [ConcurrencyToken] = NEWID() WHERE [ConcurrencyToken] IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] ALTER COLUMN [ConcurrencyToken] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Tenants] SET [IsDeleted] = CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Branches] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [AcademicYears] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AcademicYears]') AND [c].[name] = N'IsActive');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AcademicYears] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [AcademicYears] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Departments] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Departments]') AND [c].[name] = N'IsActive');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Departments] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Departments] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Courses] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'IsActive');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Courses] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Batches] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Batches]') AND [c].[name] = N'IsActive');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Batches] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Batches] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Sections] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sections]') AND [c].[name] = N'IsActive');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Sections] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Sections] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Subjects] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'IsActive');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Subjects] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [IsDeleted] bit NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [SyllabusUnits] SET [IsDeleted] = CASE WHEN [IsActive] = 1 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ALTER COLUMN [IsDeleted] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [DeletedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SyllabusUnits]') AND [c].[name] = N'IsActive');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [SyllabusUnits] DROP COLUMN [IsActive];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Tenants] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Branches] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [AcademicYears] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Departments] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Courses] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Batches] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Sections] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [Subjects] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [NewId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE [SyllabusUnits] SET [NewId] = NEWID();
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD [NewTenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [NewTenantId] uniqueidentifier NULL;
    ALTER TABLE [AspNetUsers] ADD [NewBranchId] uniqueidentifier NULL;
    ALTER TABLE [Courses] ADD [NewDepartmentId] uniqueidentifier NULL;
    ALTER TABLE [Batches] ADD [NewCourseId] uniqueidentifier NULL;
    ALTER TABLE [Batches] ADD [NewAcademicYearId] uniqueidentifier NULL;
    ALTER TABLE [Sections] ADD [NewBatchId] uniqueidentifier NULL;
    ALTER TABLE [Subjects] ADD [NewCourseId] uniqueidentifier NULL;
    ALTER TABLE [SyllabusUnits] ADD [NewSubjectId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Branches] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Branches] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [AcademicYears] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [AcademicYears] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Departments] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Departments] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Courses] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Courses] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Batches] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Batches] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Sections] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Sections] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [Subjects] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [Subjects] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [SyllabusUnits] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [SyllabusUnits] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewTenantId] = COALESCE(tenant.[NewId], CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'))
    FROM [AspNetUsers] AS target
    LEFT JOIN [Tenants] AS tenant ON target.[TenantId] = tenant.[Id];

    ALTER TABLE [AspNetUsers] ALTER COLUMN [NewTenantId] uniqueidentifier NOT NULL;

    UPDATE target
    SET [NewBranchId] = branch.[NewId]
    FROM [AspNetUsers] AS target
    INNER JOIN [Branches] AS branch ON target.[BranchId] = branch.[Id]
    WHERE target.[BranchId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewDepartmentId] = principal.[NewId]
    FROM [Courses] AS target
    INNER JOIN [Departments] AS principal ON target.[DepartmentId] = principal.[Id]
    WHERE target.[DepartmentId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewCourseId] = principal.[NewId]
    FROM [Batches] AS target
    INNER JOIN [Courses] AS principal ON target.[CourseId] = principal.[Id]
    WHERE target.[CourseId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ALTER COLUMN [NewCourseId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewAcademicYearId] = principal.[NewId]
    FROM [Batches] AS target
    INNER JOIN [AcademicYears] AS principal ON target.[AcademicYearId] = principal.[Id]
    WHERE target.[AcademicYearId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ALTER COLUMN [NewAcademicYearId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewBatchId] = principal.[NewId]
    FROM [Sections] AS target
    INNER JOIN [Batches] AS principal ON target.[BatchId] = principal.[Id]
    WHERE target.[BatchId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ALTER COLUMN [NewBatchId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewCourseId] = principal.[NewId]
    FROM [Subjects] AS target
    INNER JOIN [Courses] AS principal ON target.[CourseId] = principal.[Id]
    WHERE target.[CourseId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    UPDATE target
    SET [NewSubjectId] = principal.[NewId]
    FROM [SyllabusUnits] AS target
    INNER JOIN [Subjects] AS principal ON target.[SubjectId] = principal.[Id]
    WHERE target.[SubjectId] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ALTER COLUMN [NewSubjectId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'TenantId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Branches] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Branches].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AcademicYears]') AND [c].[name] = N'TenantId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [AcademicYears] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [AcademicYears] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AcademicYears].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Departments]') AND [c].[name] = N'TenantId');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Departments] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Departments] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Departments].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'TenantId');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Courses] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Courses].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Batches]') AND [c].[name] = N'TenantId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Batches] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Batches] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sections]') AND [c].[name] = N'TenantId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Sections] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Sections] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Sections].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'TenantId');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Subjects] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Subjects].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SyllabusUnits]') AND [c].[name] = N'TenantId');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [SyllabusUnits] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[SyllabusUnits].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'TenantId');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [AspNetUsers] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers].[NewTenantId]', N'TenantId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'BranchId');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [AspNetUsers] DROP COLUMN [BranchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers].[NewBranchId]', N'BranchId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'DepartmentId');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [Courses] DROP COLUMN [DepartmentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Courses].[NewDepartmentId]', N'DepartmentId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Batches]') AND [c].[name] = N'CourseId');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Batches] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [Batches] DROP COLUMN [CourseId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[NewCourseId]', N'CourseId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Batches]') AND [c].[name] = N'AcademicYearId');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Batches] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [Batches] DROP COLUMN [AcademicYearId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[NewAcademicYearId]', N'AcademicYearId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sections]') AND [c].[name] = N'BatchId');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Sections] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [Sections] DROP COLUMN [BatchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Sections].[NewBatchId]', N'BatchId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'CourseId');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [Subjects] DROP COLUMN [CourseId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Subjects].[NewCourseId]', N'CourseId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SyllabusUnits]') AND [c].[name] = N'SubjectId');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [SyllabusUnits] DROP COLUMN [SubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[SyllabusUnits].[NewSubjectId]', N'SubjectId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tenants]') AND [c].[name] = N'Id');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Tenants] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Tenants] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Tenants].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'Id');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [Branches] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Branches].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AcademicYears]') AND [c].[name] = N'Id');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [AcademicYears] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [AcademicYears] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[AcademicYears].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Departments]') AND [c].[name] = N'Id');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Departments] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [Departments] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Departments].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'Id');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [Courses] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Courses].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Batches]') AND [c].[name] = N'Id');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Batches] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [Batches] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Batches].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sections]') AND [c].[name] = N'Id');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Sections] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [Sections] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Sections].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'Id');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [Subjects] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[Subjects].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SyllabusUnits]') AND [c].[name] = N'Id');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [SyllabusUnits] DROP CONSTRAINT [' + @var31 + '];');
    ALTER TABLE [SyllabusUnits] DROP COLUMN [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    EXEC sp_rename N'[SyllabusUnits].[NewId]', N'Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Tenants] ADD CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD CONSTRAINT [PK_Branches] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AcademicYears] ADD CONSTRAINT [PK_AcademicYears] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Departments] ADD CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD CONSTRAINT [PK_Batches] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD CONSTRAINT [PK_SyllabusUnits] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_BranchId] ON [AspNetUsers] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_TenantId] ON [AspNetUsers] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Branches_TenantId] ON [Branches] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AcademicYears_TenantId_Name] ON [AcademicYears] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Batches_AcademicYearId] ON [Batches] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Batches_CourseId] ON [Batches] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Batches_TenantId] ON [Batches] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Courses_DepartmentId] ON [Courses] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Courses_TenantId_Code] ON [Courses] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departments_TenantId_Code] ON [Departments] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Sections_BatchId] ON [Sections] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Subjects_CourseId] ON [Subjects] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Subjects_TenantId_Code] ON [Subjects] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    CREATE INDEX [IX_SyllabusUnits_SubjectId] ON [SyllabusUnits] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Branches] ADD CONSTRAINT [FK_Branches_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD CONSTRAINT [FK_Batches_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Batches] ADD CONSTRAINT [FK_Batches_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Sections] ADD CONSTRAINT [FK_Sections_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [FK_Subjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    ALTER TABLE [SyllabusUnits] ADD CONSTRAINT [FK_SyllabusUnits_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902230123_GuidKeysAuditSoftDelete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260902230123_GuidKeysAuditSoftDelete', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE TABLE [ProfileDocuments] (
        [Id] uniqueidentifier NOT NULL,
        [OwnerType] int NOT NULL,
        [OwnerId] uniqueidentifier NOT NULL,
        [DocumentType] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [ContentType] nvarchar(120) NULL,
        [StoragePath] nvarchar(1000) NOT NULL,
        [SizeBytes] bigint NULL,
        [UploadedOn] datetime2 NOT NULL,
        [IsVerified] bit NOT NULL,
        [VerifiedBy] nvarchar(128) NULL,
        [VerifiedOn] datetime2 NULL,
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
        CONSTRAINT [PK_ProfileDocuments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE TABLE [StudentProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NULL,
        [AdmissionNumber] nvarchar(50) NOT NULL,
        [RollNumber] nvarchar(50) NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DateOfBirth] date NOT NULL,
        [Gender] int NOT NULL,
        [BloodGroup] int NOT NULL,
        [AdmissionDate] date NOT NULL,
        [Email] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [EmergencyContactName] nvarchar(150) NULL,
        [EmergencyContactPhone] nvarchar(30) NULL,
        [Address] nvarchar(500) NULL,
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
        CONSTRAINT [PK_StudentProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentProfiles_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE TABLE [TeacherProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EmployeeNumber] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DateOfBirth] date NULL,
        [Gender] int NOT NULL,
        [Email] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Designation] nvarchar(120) NULL,
        [Qualifications] nvarchar(500) NULL,
        [Specializations] nvarchar(500) NULL,
        [ExperienceYears] decimal(18,2) NOT NULL,
        [JoiningDate] date NOT NULL,
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
        CONSTRAINT [PK_TeacherProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE TABLE [StudentGuardians] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [ParentUserId] uniqueidentifier NULL,
        [Relationship] int NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [Email] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Occupation] nvarchar(120) NULL,
        [IsPrimary] bit NOT NULL,
        [HasPortalAccess] bit NOT NULL,
        [CanPickup] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentGuardians] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentGuardians_AspNetUsers_ParentUserId] FOREIGN KEY ([ParentUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentGuardians_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE TABLE [TeacherSubjectAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NULL,
        [IsPrimary] bit NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveUntil] date NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherSubjectAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherSubjectAssignments_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherSubjectAssignments_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherSubjectAssignments_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_ProfileDocuments_OwnerType_OwnerId] ON [ProfileDocuments] ([OwnerType], [OwnerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_StudentGuardians_ParentUserId] ON [StudentGuardians] ([ParentUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_StudentGuardians_StudentProfileId] ON [StudentGuardians] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_StudentProfiles_BranchId] ON [StudentProfiles] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_StudentProfiles_SectionId] ON [StudentProfiles] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentProfiles_TenantId_AdmissionNumber] ON [StudentProfiles] ([TenantId], [AdmissionNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_StudentProfiles_UserId] ON [StudentProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_TeacherProfiles_BranchId] ON [TeacherProfiles] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TeacherProfiles_TenantId_EmployeeNumber] ON [TeacherProfiles] ([TenantId], [EmployeeNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_TeacherProfiles_UserId] ON [TeacherProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_TeacherSubjectAssignments_SectionId] ON [TeacherSubjectAssignments] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_TeacherSubjectAssignments_SubjectId] ON [TeacherSubjectAssignments] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    CREATE INDEX [IX_TeacherSubjectAssignments_TeacherProfileId] ON [TeacherSubjectAssignments] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903232745_StudentsTeachers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260903232745_StudentsTeachers', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AdmissionApplications] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NOT NULL,
        [BatchId] uniqueidentifier NULL,
        [SectionId] uniqueidentifier NULL,
        [ApplicationNumber] nvarchar(50) NOT NULL,
        [ApplicantFirstName] nvarchar(100) NOT NULL,
        [ApplicantMiddleName] nvarchar(100) NULL,
        [ApplicantLastName] nvarchar(100) NOT NULL,
        [DateOfBirth] date NOT NULL,
        [Gender] int NOT NULL,
        [Email] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [GuardianName] nvarchar(150) NULL,
        [GuardianPhone] nvarchar(30) NULL,
        [Address] nvarchar(500) NULL,
        [AppliedOn] date NOT NULL,
        [Status] int NOT NULL,
        [ReviewNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AdmissionApplications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionApplications_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionApplications_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionApplications_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionApplications_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionApplications_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AttendancePolicies] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [MinimumPercentage] decimal(5,2) NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendancePolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendancePolicies_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [LeaveApplications] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [LeaveType] int NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [Reason] nvarchar(500) NULL,
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
        CONSTRAINT [PK_LeaveApplications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveApplications_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveApplications_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [PromotionRecords] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [FromAcademicYearId] uniqueidentifier NOT NULL,
        [ToAcademicYearId] uniqueidentifier NOT NULL,
        [FromSectionId] uniqueidentifier NULL,
        [ToSectionId] uniqueidentifier NOT NULL,
        [PromotionDate] date NOT NULL,
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
        CONSTRAINT [PK_PromotionRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionRecords_AcademicYears_FromAcademicYearId] FOREIGN KEY ([FromAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionRecords_AcademicYears_ToAcademicYearId] FOREIGN KEY ([ToAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionRecords_Sections_FromSectionId] FOREIGN KEY ([FromSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionRecords_Sections_ToSectionId] FOREIGN KEY ([ToSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [Rooms] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Type] int NOT NULL,
        [Capacity] int NOT NULL,
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
        CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rooms_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [TimeSlots] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(80) NOT NULL,
        [DayOfWeek] int NOT NULL,
        [PeriodNumber] int NOT NULL,
        [StartsAt] time NOT NULL,
        [EndsAt] time NOT NULL,
        [IsBreak] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimeSlots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimeSlots_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [Timetables] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
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
        CONSTRAINT [PK_Timetables] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Timetables_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Timetables_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Timetables_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AdmissionDocuments] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionApplicationId] uniqueidentifier NOT NULL,
        [DocumentType] int NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [ContentType] nvarchar(100) NULL,
        [StoragePath] nvarchar(500) NULL,
        [IsVerified] bit NOT NULL,
        [VerifiedByUserId] uniqueidentifier NULL,
        [VerifiedOn] datetime2 NULL,
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
        CONSTRAINT [PK_AdmissionDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionDocuments_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionDocuments_AspNetUsers_VerifiedByUserId] FOREIGN KEY ([VerifiedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AdmissionReviews] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionApplicationId] uniqueidentifier NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [FromStatus] int NOT NULL,
        [ToStatus] int NOT NULL,
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
        CONSTRAINT [PK_AdmissionReviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionReviews_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionReviews_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [Enrollments] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [AdmissionApplicationId] uniqueidentifier NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [CourseId] uniqueidentifier NOT NULL,
        [BatchId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NULL,
        [EnrollmentNumber] nvarchar(50) NOT NULL,
        [EnrollmentDate] date NOT NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Enrollments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AttendanceAlerts] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [AttendancePolicyId] uniqueidentifier NULL,
        [PeriodStart] date NOT NULL,
        [PeriodEnd] date NOT NULL,
        [AttendancePercentage] decimal(5,2) NOT NULL,
        [ThresholdPercentage] decimal(5,2) NOT NULL,
        [GeneratedOn] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Message] nvarchar(500) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceAlerts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceAlerts_AttendancePolicies_AttendancePolicyId] FOREIGN KEY ([AttendancePolicyId]) REFERENCES [AttendancePolicies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceAlerts_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [TimetableEntries] (
        [Id] uniqueidentifier NOT NULL,
        [TimetableId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [TimeSlotId] uniqueidentifier NOT NULL,
        [RoomId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_TimetableEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableEntries_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableEntries_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableEntries_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableEntries_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableEntries_TimeSlots_TimeSlotId] FOREIGN KEY ([TimeSlotId]) REFERENCES [TimeSlots] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableEntries_Timetables_TimetableId] FOREIGN KEY ([TimetableId]) REFERENCES [Timetables] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AttendanceSessions] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NULL,
        [TimetableEntryId] uniqueidentifier NULL,
        [AttendanceDate] date NOT NULL,
        [SessionType] int NOT NULL,
        [PeriodNumber] int NULL,
        [StartsAt] time NULL,
        [EndsAt] time NULL,
        [MarkedByUserId] uniqueidentifier NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_AttendanceSessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceSessions_AspNetUsers_MarkedByUserId] FOREIGN KEY ([MarkedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceSessions_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceSessions_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceSessions_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceSessions_TimetableEntries_TimetableEntryId] FOREIGN KEY ([TimetableEntryId]) REFERENCES [TimetableEntries] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE TABLE [AttendanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [AttendanceSessionId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [MarkedByUserId] uniqueidentifier NULL,
        [MarkedOn] datetime2 NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceRecords_AspNetUsers_MarkedByUserId] FOREIGN KEY ([MarkedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId] FOREIGN KEY ([AttendanceSessionId]) REFERENCES [AttendanceSessions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_AcademicYearId] ON [AdmissionApplications] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_BatchId] ON [AdmissionApplications] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_BranchId] ON [AdmissionApplications] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_CourseId] ON [AdmissionApplications] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_SectionId] ON [AdmissionApplications] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_Status] ON [AdmissionApplications] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdmissionApplications_TenantId_ApplicationNumber] ON [AdmissionApplications] ([TenantId], [ApplicationNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionDocuments_AdmissionApplicationId] ON [AdmissionDocuments] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionDocuments_VerifiedByUserId] ON [AdmissionDocuments] ([VerifiedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionReviews_AdmissionApplicationId] ON [AdmissionReviews] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AdmissionReviews_ReviewedByUserId] ON [AdmissionReviews] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceAlerts_AttendancePolicyId] ON [AttendanceAlerts] ([AttendancePolicyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceAlerts_StudentProfileId] ON [AttendanceAlerts] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceAlerts_TenantId_StudentProfileId_Status] ON [AttendanceAlerts] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendancePolicies_BranchId] ON [AttendancePolicies] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttendancePolicies_TenantId_Name] ON [AttendancePolicies] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecords_AttendanceSessionId] ON [AttendanceRecords] ([AttendanceSessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecords_MarkedByUserId] ON [AttendanceRecords] ([MarkedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecords_StudentProfileId] ON [AttendanceRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttendanceRecords_TenantId_AttendanceSessionId_StudentProfileId] ON [AttendanceRecords] ([TenantId], [AttendanceSessionId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_BranchId] ON [AttendanceSessions] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_MarkedByUserId] ON [AttendanceSessions] ([MarkedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_SectionId] ON [AttendanceSessions] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_SubjectId] ON [AttendanceSessions] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_TenantId_SectionId_AttendanceDate] ON [AttendanceSessions] ([TenantId], [SectionId], [AttendanceDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_AttendanceSessions_TimetableEntryId] ON [AttendanceSessions] ([TimetableEntryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_AcademicYearId] ON [Enrollments] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_AdmissionApplicationId] ON [Enrollments] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_BatchId] ON [Enrollments] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_BranchId] ON [Enrollments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_CourseId] ON [Enrollments] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_SectionId] ON [Enrollments] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Enrollments_StudentProfileId] ON [Enrollments] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Enrollments_TenantId_EnrollmentNumber] ON [Enrollments] ([TenantId], [EnrollmentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Enrollments_TenantId_StudentProfileId_AcademicYearId] ON [Enrollments] ([TenantId], [StudentProfileId], [AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_LeaveApplications_ReviewedByUserId] ON [LeaveApplications] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_LeaveApplications_StudentProfileId] ON [LeaveApplications] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_LeaveApplications_TenantId_StudentProfileId_FromDate] ON [LeaveApplications] ([TenantId], [StudentProfileId], [FromDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_PromotionRecords_FromAcademicYearId] ON [PromotionRecords] ([FromAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_PromotionRecords_FromSectionId] ON [PromotionRecords] ([FromSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_PromotionRecords_StudentProfileId] ON [PromotionRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_PromotionRecords_ToAcademicYearId] ON [PromotionRecords] ([ToAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_PromotionRecords_ToSectionId] ON [PromotionRecords] ([ToSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Rooms_BranchId] ON [Rooms] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rooms_TenantId_BranchId_Code] ON [Rooms] ([TenantId], [BranchId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimeSlots_BranchId] ON [TimeSlots] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimeSlots_TenantId_BranchId_DayOfWeek_PeriodNumber] ON [TimeSlots] ([TenantId], [BranchId], [DayOfWeek], [PeriodNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_RoomId] ON [TimetableEntries] ([RoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_SectionId] ON [TimetableEntries] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_SubjectId] ON [TimetableEntries] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_TeacherProfileId] ON [TimetableEntries] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_TimeSlotId] ON [TimetableEntries] ([TimeSlotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_TimetableEntries_TimetableId_RoomId_TimeSlotId] ON [TimetableEntries] ([TimetableId], [RoomId], [TimeSlotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableEntries_TimetableId_SectionId_TimeSlotId] ON [TimetableEntries] ([TimetableId], [SectionId], [TimeSlotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableEntries_TimetableId_TeacherProfileId_TimeSlotId] ON [TimetableEntries] ([TimetableId], [TeacherProfileId], [TimeSlotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Timetables_AcademicYearId] ON [Timetables] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Timetables_BranchId] ON [Timetables] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Timetables_SectionId] ON [Timetables] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    CREATE INDEX [IX_Timetables_TenantId_SectionId_EffectiveFrom] ON [Timetables] ([TenantId], [SectionId], [EffectiveFrom]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904201728_AdmissionsAttendanceTimetable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260904201728_AdmissionsAttendanceTimetable', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224438_BranchAdminRole'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Description], [Name], [NormalizedName])
    VALUES (''77777777-7777-7777-7777-777777777777'', N''77777777-7777-7777-7777-777777777777'', N''BranchAdmin platform role'', N''BranchAdmin'', N''BRANCHADMIN'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904224438_BranchAdminRole'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260904224438_BranchAdminRole', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [Exams] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [WeightagePercentage] decimal(5,2) NOT NULL,
        [Status] int NOT NULL,
        [Instructions] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Exams_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Exams_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [GradingSchemes] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Type] int NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [IsDefault] bit NOT NULL,
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
        CONSTRAINT [PK_GradingSchemes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GradingSchemes_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GradingSchemes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [QuestionBankItems] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [SyllabusUnitId] uniqueidentifier NULL,
        [AuthorTeacherProfileId] uniqueidentifier NULL,
        [QuestionType] int NOT NULL,
        [Difficulty] int NOT NULL,
        [BloomLevel] int NOT NULL,
        [Source] int NOT NULL,
        [Marks] decimal(7,2) NOT NULL,
        [QuestionText] nvarchar(4000) NOT NULL,
        [ExpectedAnswer] nvarchar(4000) NULL,
        [Tags] nvarchar(500) NULL,
        [ApprovalStatus] int NOT NULL,
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
        CONSTRAINT [PK_QuestionBankItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionBankItems_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionBankItems_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionBankItems_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionBankItems_SyllabusUnits_SyllabusUnitId] FOREIGN KEY ([SyllabusUnitId]) REFERENCES [SyllabusUnits] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionBankItems_TeacherProfiles_AuthorTeacherProfileId] FOREIGN KEY ([AuthorTeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [ExamSchedules] (
        [Id] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NULL,
        [RoomId] uniqueidentifier NULL,
        [ExamDate] date NOT NULL,
        [StartsAt] time NOT NULL,
        [EndsAt] time NOT NULL,
        [DurationMinutes] int NOT NULL,
        [MaximumMarks] decimal(7,2) NOT NULL,
        [PassingMarks] decimal(7,2) NOT NULL,
        [Status] int NOT NULL,
        [SeatingPlanNotes] nvarchar(1000) NULL,
        [Instructions] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExamSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamSchedules_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSchedules_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSchedules_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSchedules_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSchedules_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [Results] (
        [Id] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [CourseId] uniqueidentifier NULL,
        [BatchId] uniqueidentifier NULL,
        [SectionId] uniqueidentifier NULL,
        [TotalMarks] decimal(9,2) NOT NULL,
        [MarksObtained] decimal(9,2) NOT NULL,
        [Percentage] decimal(5,2) NOT NULL,
        [Grade] nvarchar(20) NULL,
        [GradePoint] decimal(4,2) NULL,
        [Status] int NOT NULL,
        [ComputedOn] datetime2 NOT NULL,
        [PublishedByUserId] uniqueidentifier NULL,
        [PublishedOn] datetime2 NULL,
        [Remarks] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Results] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Results_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_AspNetUsers_PublishedByUserId] FOREIGN KEY ([PublishedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Results_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [GradingSchemeBands] (
        [Id] uniqueidentifier NOT NULL,
        [GradingSchemeId] uniqueidentifier NOT NULL,
        [Grade] nvarchar(20) NOT NULL,
        [MinimumPercentage] decimal(5,2) NOT NULL,
        [MaximumPercentage] decimal(5,2) NOT NULL,
        [GradePoint] decimal(4,2) NOT NULL,
        [SortOrder] int NOT NULL,
        [Remarks] nvarchar(200) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_GradingSchemeBands] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GradingSchemeBands_GradingSchemes_GradingSchemeId] FOREIGN KEY ([GradingSchemeId]) REFERENCES [GradingSchemes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [MarkEntries] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [MarksObtained] decimal(7,2) NOT NULL,
        [IsAbsent] bit NOT NULL,
        [Grade] nvarchar(20) NULL,
        [GradePoint] decimal(4,2) NULL,
        [EnteredByUserId] uniqueidentifier NULL,
        [EnteredOn] datetime2 NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MarkEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarkEntries_AspNetUsers_EnteredByUserId] FOREIGN KEY ([EnteredByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarkEntries_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarkEntries_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [QuestionPapers] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NULL,
        [ExamScheduleId] uniqueidentifier NULL,
        [Title] nvarchar(150) NOT NULL,
        [TotalMarks] decimal(7,2) NOT NULL,
        [DurationMinutes] int NOT NULL,
        [Instructions] nvarchar(1000) NULL,
        [Source] int NOT NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_QuestionPapers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPapers_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPapers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPapers_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPapers_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPapers_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [QuestionPaperSections] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionPaperId] uniqueidentifier NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [Title] nvarchar(120) NOT NULL,
        [SortOrder] int NOT NULL,
        [Marks] decimal(7,2) NOT NULL,
        [Instructions] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_QuestionPaperSections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPaperSections_QuestionPapers_QuestionPaperId] FOREIGN KEY ([QuestionPaperId]) REFERENCES [QuestionPapers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [QuestionPaperVersions] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionPaperId] uniqueidentifier NOT NULL,
        [VersionNumber] int NOT NULL,
        [Source] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [StudentContentJson] nvarchar(max) NOT NULL,
        [SolutionContentJson] nvarchar(max) NOT NULL,
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
        CONSTRAINT [PK_QuestionPaperVersions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPaperVersions_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPaperVersions_QuestionPapers_QuestionPaperId] FOREIGN KEY ([QuestionPaperId]) REFERENCES [QuestionPapers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE TABLE [QuestionPaperQuestions] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionPaperSectionId] uniqueidentifier NOT NULL,
        [QuestionBankItemId] uniqueidentifier NULL,
        [SortOrder] int NOT NULL,
        [QuestionType] int NOT NULL,
        [Difficulty] int NOT NULL,
        [BloomLevel] int NOT NULL,
        [Marks] decimal(7,2) NOT NULL,
        [QuestionText] nvarchar(4000) NOT NULL,
        [SolutionText] nvarchar(4000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_QuestionPaperQuestions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPaperQuestions_QuestionBankItems_QuestionBankItemId] FOREIGN KEY ([QuestionBankItemId]) REFERENCES [QuestionBankItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionPaperQuestions_QuestionPaperSections_QuestionPaperSectionId] FOREIGN KEY ([QuestionPaperSectionId]) REFERENCES [QuestionPaperSections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Exams_AcademicYearId] ON [Exams] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Exams_BranchId] ON [Exams] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Exams_Status] ON [Exams] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Exams_TenantId_BranchId_AcademicYearId_Name] ON [Exams] ([TenantId], [BranchId], [AcademicYearId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_ExamDate_StartsAt] ON [ExamSchedules] ([ExamDate], [StartsAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_ExamId] ON [ExamSchedules] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_RoomId] ON [ExamSchedules] ([RoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_SectionId] ON [ExamSchedules] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_SubjectId] ON [ExamSchedules] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_ExamSchedules_TeacherProfileId] ON [ExamSchedules] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamSchedules_TenantId_ExamId_SectionId_SubjectId] ON [ExamSchedules] ([TenantId], [ExamId], [SectionId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GradingSchemeBands_GradingSchemeId_Grade] ON [GradingSchemeBands] ([GradingSchemeId], [Grade]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_GradingSchemeBands_GradingSchemeId_SortOrder] ON [GradingSchemeBands] ([GradingSchemeId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_GradingSchemes_BranchId] ON [GradingSchemes] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_GradingSchemes_CourseId] ON [GradingSchemes] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_GradingSchemes_TenantId_BranchId_Name] ON [GradingSchemes] ([TenantId], [BranchId], [Name]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_MarkEntries_EnteredByUserId] ON [MarkEntries] ([EnteredByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_MarkEntries_ExamScheduleId] ON [MarkEntries] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_MarkEntries_StudentProfileId] ON [MarkEntries] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarkEntries_TenantId_ExamScheduleId_StudentProfileId] ON [MarkEntries] ([TenantId], [ExamScheduleId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_ApprovedByUserId] ON [QuestionBankItems] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_AuthorTeacherProfileId] ON [QuestionBankItems] ([AuthorTeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_BranchId] ON [QuestionBankItems] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_SubjectId_SyllabusUnitId] ON [QuestionBankItems] ([SubjectId], [SyllabusUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_SyllabusUnitId] ON [QuestionBankItems] ([SyllabusUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionBankItems_TenantId_BranchId_SubjectId] ON [QuestionBankItems] ([TenantId], [BranchId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperQuestions_QuestionBankItemId] ON [QuestionPaperQuestions] ([QuestionBankItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperQuestions_QuestionPaperSectionId_SortOrder] ON [QuestionPaperQuestions] ([QuestionPaperSectionId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_ApprovedByUserId] ON [QuestionPapers] ([ApprovedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_BranchId] ON [QuestionPapers] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_ExamId] ON [QuestionPapers] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_ExamScheduleId] ON [QuestionPapers] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_SubjectId] ON [QuestionPapers] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPapers_TenantId_BranchId_SubjectId] ON [QuestionPapers] ([TenantId], [BranchId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QuestionPaperSections_QuestionPaperId_Code] ON [QuestionPaperSections] ([QuestionPaperId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperSections_QuestionPaperId_SortOrder] ON [QuestionPaperSections] ([QuestionPaperId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperVersions_CreatedByUserId] ON [QuestionPaperVersions] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QuestionPaperVersions_QuestionPaperId_VersionNumber] ON [QuestionPaperVersions] ([QuestionPaperId], [VersionNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_AcademicYearId] ON [Results] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_BatchId] ON [Results] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_BranchId] ON [Results] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_CourseId] ON [Results] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_ExamId] ON [Results] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_PublishedByUserId] ON [Results] ([PublishedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_SectionId] ON [Results] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_StudentProfileId] ON [Results] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE INDEX [IX_Results_TenantId_BranchId_Status] ON [Results] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Results_TenantId_ExamId_StudentProfileId] ON [Results] ([TenantId], [ExamId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260904235702_ExamsResultsQuestionBank'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260904235702_ExamsResultsQuestionBank', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [Announcements] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Title] nvarchar(180) NOT NULL,
        [Message] nvarchar(4000) NOT NULL,
        [Audience] int NOT NULL,
        [PublishOn] datetime2 NOT NULL,
        [ExpireOn] datetime2 NULL,
        [IsPinned] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Announcements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Announcements_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Announcements_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [CommunicationLogs] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [Direction] int NOT NULL,
        [Recipient] nvarchar(250) NOT NULL,
        [Subject] nvarchar(250) NULL,
        [Status] int NOT NULL,
        [ProviderKey] nvarchar(80) NULL,
        [ProviderMessageId] nvarchar(160) NULL,
        [PayloadSummary] nvarchar(1000) NULL,
        [ErrorMessage] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_CommunicationLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommunicationLogs_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [DiscountRules] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Type] int NOT NULL,
        [Value] decimal(12,2) NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
        [IsActive] bit NOT NULL,
        [EligibilityCriteria] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DiscountRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DiscountRules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DiscountRules_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeeStructures] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [CourseId] uniqueidentifier NULL,
        [BatchId] uniqueidentifier NULL,
        [Name] nvarchar(150) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Frequency] int NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
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
        CONSTRAINT [PK_FeeStructures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeStructures_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeStructures_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeStructures_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeStructures_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [HostelBlocks] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Code] nvarchar(40) NOT NULL,
        [Gender] int NOT NULL,
        [WardenName] nvarchar(120) NULL,
        [WardenPhone] nvarchar(30) NULL,
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
        CONSTRAINT [PK_HostelBlocks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelBlocks_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryBooks] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NULL,
        [Isbn] nvarchar(30) NULL,
        [Title] nvarchar(250) NOT NULL,
        [Authors] nvarchar(250) NOT NULL,
        [Publisher] nvarchar(150) NULL,
        [Category] nvarchar(120) NULL,
        [Edition] nvarchar(80) NULL,
        [PublicationYear] int NULL,
        [Language] nvarchar(50) NULL,
        [Keywords] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LibraryBooks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryBooks_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBooks_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryMembers] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [TeacherProfileId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NULL,
        [MemberNumber] nvarchar(60) NOT NULL,
        [MemberType] int NOT NULL,
        [JoinedOn] date NOT NULL,
        [ExpiresOn] date NULL,
        [MaxBooksAllowed] int NOT NULL,
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
        CONSTRAINT [PK_LibraryMembers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryMembers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryMembers_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryMembers_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [NotificationProviderSettings] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [ProviderKey] nvarchar(80) NOT NULL,
        [DisplayName] nvarchar(120) NOT NULL,
        [FromAddress] nvarchar(150) NULL,
        [FromDisplayName] nvarchar(150) NULL,
        [ConfigurationJson] nvarchar(2000) NULL,
        [SecretReference] nvarchar(200) NULL,
        [IsEnabled] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationProviderSettings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationProviderSettings_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [NotificationTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Code] nvarchar(80) NOT NULL,
        [Channel] int NOT NULL,
        [SubjectTemplate] nvarchar(250) NULL,
        [BodyTemplate] nvarchar(4000) NOT NULL,
        [VariablesJson] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [Scholarships] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Type] int NOT NULL,
        [Value] decimal(12,2) NOT NULL,
        [AwardedOn] date NOT NULL,
        [ValidUntil] date NULL,
        [IsActive] bit NOT NULL,
        [SponsorName] nvarchar(150) NULL,
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
        CONSTRAINT [PK_Scholarships] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Scholarships_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Scholarships_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [TransportDrivers] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [LicenseNumber] nvarchar(60) NOT NULL,
        [LicenseValidUntil] date NULL,
        [Status] int NOT NULL,
        [Address] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TransportDrivers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportDrivers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [TransportRoutes] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [RouteCode] nvarchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [ShiftName] nvarchar(80) NULL,
        [StartsAt] time NULL,
        [EndsAt] time NULL,
        [DistanceKm] decimal(8,2) NOT NULL,
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
        CONSTRAINT [PK_TransportRoutes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportRoutes_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [RegistrationNumber] nvarchar(30) NOT NULL,
        [DisplayName] nvarchar(80) NULL,
        [Type] int NOT NULL,
        [SeatCapacity] int NOT NULL,
        [Status] int NOT NULL,
        [InsuranceValidUntil] date NULL,
        [FitnessValidUntil] date NULL,
        [GpsDeviceId] nvarchar(80) NULL,
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
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vehicles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeeComponents] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeeStructureId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Type] int NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [IsOptional] bit NOT NULL,
        [DueDaysFromStart] int NOT NULL,
        [SortOrder] int NOT NULL,
        [LedgerCode] nvarchar(50) NULL,
        [Notes] nvarchar(300) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_FeeComponents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeComponents_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeComponents_FeeStructures_FeeStructureId] FOREIGN KEY ([FeeStructureId]) REFERENCES [FeeStructures] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [StudentFeeAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [FeeStructureId] uniqueidentifier NOT NULL,
        [DiscountRuleId] uniqueidentifier NULL,
        [AssignedOn] date NOT NULL,
        [Status] int NOT NULL,
        [CustomDiscountAmount] decimal(12,2) NOT NULL,
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
        CONSTRAINT [PK_StudentFeeAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentFeeAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentFeeAssignments_DiscountRules_DiscountRuleId] FOREIGN KEY ([DiscountRuleId]) REFERENCES [DiscountRules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentFeeAssignments_FeeStructures_FeeStructureId] FOREIGN KEY ([FeeStructureId]) REFERENCES [FeeStructures] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentFeeAssignments_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [HostelRooms] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelBlockId] uniqueidentifier NOT NULL,
        [RoomNumber] nvarchar(40) NOT NULL,
        [Floor] nvarchar(40) NULL,
        [Capacity] int NOT NULL,
        [MonthlyFee] decimal(12,2) NOT NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_HostelRooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelRooms_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelRooms_HostelBlocks_HostelBlockId] FOREIGN KEY ([HostelBlockId]) REFERENCES [HostelBlocks] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryBookCopies] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookId] uniqueidentifier NOT NULL,
        [AccessionNumber] nvarchar(60) NOT NULL,
        [Barcode] nvarchar(80) NULL,
        [ShelfLocation] nvarchar(120) NULL,
        [Status] int NOT NULL,
        [AcquisitionDate] date NULL,
        [Price] decimal(12,2) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LibraryBookCopies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryBookCopies_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookCopies_LibraryBooks_LibraryBookId] FOREIGN KEY ([LibraryBookId]) REFERENCES [LibraryBooks] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [NotificationMessages] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [NotificationTemplateId] uniqueidentifier NULL,
        [Channel] int NOT NULL,
        [Subject] nvarchar(250) NULL,
        [Body] nvarchar(4000) NOT NULL,
        [Status] int NOT NULL,
        [ScheduledOn] datetime2 NULL,
        [SentOn] datetime2 NULL,
        [ProviderKey] nvarchar(80) NULL,
        [ProviderMessageId] nvarchar(160) NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        [CreatedForUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NotificationMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationMessages_AspNetUsers_CreatedForUserId] FOREIGN KEY ([CreatedForUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationMessages_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationMessages_NotificationTemplates_NotificationTemplateId] FOREIGN KEY ([NotificationTemplateId]) REFERENCES [NotificationTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [TransportRouteStops] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NOT NULL,
        [StopName] nvarchar(150) NOT NULL,
        [StopOrder] int NOT NULL,
        [PickupTime] time NULL,
        [DropTime] time NULL,
        [MonthlyFee] decimal(12,2) NOT NULL,
        [Landmark] nvarchar(200) NULL,
        [Latitude] decimal(10,7) NULL,
        [Longitude] decimal(10,7) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TransportRouteStops] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportRouteStops_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportRouteStops_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [TransportRouteAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [DriverId] uniqueidentifier NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
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
        CONSTRAINT [PK_TransportRouteAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportRouteAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportRouteAssignments_TransportDrivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [TransportDrivers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportRouteAssignments_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransportRouteAssignments_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeeInvoices] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentFeeAssignmentId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [InvoiceNumber] nvarchar(60) NOT NULL,
        [InvoiceDate] date NOT NULL,
        [DueDate] date NOT NULL,
        [Status] int NOT NULL,
        [SubTotal] decimal(12,2) NOT NULL,
        [DiscountAmount] decimal(12,2) NOT NULL,
        [FineAmount] decimal(12,2) NOT NULL,
        [TotalAmount] decimal(12,2) NOT NULL,
        [PaidAmount] decimal(12,2) NOT NULL,
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
        CONSTRAINT [PK_FeeInvoices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeInvoices_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeInvoices_StudentFeeAssignments_StudentFeeAssignmentId] FOREIGN KEY ([StudentFeeAssignmentId]) REFERENCES [StudentFeeAssignments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeInvoices_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [HostelBeds] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelRoomId] uniqueidentifier NOT NULL,
        [BedNumber] nvarchar(40) NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(300) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelBeds] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelBeds_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelBeds_HostelRooms_HostelRoomId] FOREIGN KEY ([HostelRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryBookIssues] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookCopyId] uniqueidentifier NOT NULL,
        [LibraryMemberId] uniqueidentifier NOT NULL,
        [IssueDate] date NOT NULL,
        [DueDate] date NOT NULL,
        [ReturnedOn] date NULL,
        [Status] int NOT NULL,
        [FineAmount] decimal(12,2) NOT NULL,
        [IssuedByUserId] uniqueidentifier NULL,
        [ReturnReceivedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_LibraryBookIssues] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryBookIssues_AspNetUsers_IssuedByUserId] FOREIGN KEY ([IssuedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookIssues_AspNetUsers_ReturnReceivedByUserId] FOREIGN KEY ([ReturnReceivedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookIssues_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookIssues_LibraryBookCopies_LibraryBookCopyId] FOREIGN KEY ([LibraryBookCopyId]) REFERENCES [LibraryBookCopies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookIssues_LibraryMembers_LibraryMemberId] FOREIGN KEY ([LibraryMemberId]) REFERENCES [LibraryMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [NotificationRecipients] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [NotificationMessageId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [DestinationAddress] nvarchar(250) NOT NULL,
        [Status] int NOT NULL,
        [SentOn] datetime2 NULL,
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
        CONSTRAINT [PK_NotificationRecipients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationRecipients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationRecipients_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationRecipients_NotificationMessages_NotificationMessageId] FOREIGN KEY ([NotificationMessageId]) REFERENCES [NotificationMessages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [StudentTransportAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NOT NULL,
        [TransportRouteStopId] uniqueidentifier NULL,
        [VehicleId] uniqueidentifier NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NULL,
        [MonthlyFee] decimal(12,2) NOT NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_StudentTransportAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentTransportAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTransportAssignments_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTransportAssignments_TransportRouteStops_TransportRouteStopId] FOREIGN KEY ([TransportRouteStopId]) REFERENCES [TransportRouteStops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTransportAssignments_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTransportAssignments_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeeInvoiceLines] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NOT NULL,
        [FeeComponentId] uniqueidentifier NULL,
        [Description] nvarchar(180) NOT NULL,
        [ComponentType] int NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [SortOrder] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_FeeInvoiceLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeInvoiceLines_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeInvoiceLines_FeeComponents_FeeComponentId] FOREIGN KEY ([FeeComponentId]) REFERENCES [FeeComponents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeInvoiceLines_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeePayments] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeeInvoiceId] uniqueidentifier NOT NULL,
        [PaymentNumber] nvarchar(60) NOT NULL,
        [PaidOn] datetime2 NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [Mode] int NOT NULL,
        [Status] int NOT NULL,
        [TransactionReference] nvarchar(120) NULL,
        [ReceivedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_FeePayments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeePayments_AspNetUsers_ReceivedByUserId] FOREIGN KEY ([ReceivedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeePayments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeePayments_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [HostelAllocations] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [HostelRoomId] uniqueidentifier NOT NULL,
        [HostelBedId] uniqueidentifier NULL,
        [AllocatedOn] date NOT NULL,
        [ExpectedCheckoutOn] date NULL,
        [CheckedOutOn] date NULL,
        [Status] int NOT NULL,
        [MonthlyFee] decimal(12,2) NOT NULL,
        [SecurityDeposit] decimal(12,2) NOT NULL,
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
        CONSTRAINT [PK_HostelAllocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelAllocations_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocations_HostelBeds_HostelBedId] FOREIGN KEY ([HostelBedId]) REFERENCES [HostelBeds] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocations_HostelRooms_HostelRoomId] FOREIGN KEY ([HostelRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelAllocations_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryBookReturns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookIssueId] uniqueidentifier NOT NULL,
        [ReturnedOn] datetime2 NOT NULL,
        [FineAssessed] decimal(12,2) NOT NULL,
        [FinePaid] decimal(12,2) NOT NULL,
        [ReceivedByUserId] uniqueidentifier NULL,
        [ConditionNotes] nvarchar(500) NULL,
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
        CONSTRAINT [PK_LibraryBookReturns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryBookReturns_AspNetUsers_ReceivedByUserId] FOREIGN KEY ([ReceivedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookReturns_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryBookReturns_LibraryBookIssues_LibraryBookIssueId] FOREIGN KEY ([LibraryBookIssueId]) REFERENCES [LibraryBookIssues] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [LibraryFineRecords] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryMemberId] uniqueidentifier NOT NULL,
        [LibraryBookIssueId] uniqueidentifier NULL,
        [Amount] decimal(12,2) NOT NULL,
        [Reason] nvarchar(250) NOT NULL,
        [Status] int NOT NULL,
        [AssessedOn] datetime2 NOT NULL,
        [PaidOn] datetime2 NULL,
        [WaivedOn] datetime2 NULL,
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
        CONSTRAINT [PK_LibraryFineRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryFineRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryFineRecords_LibraryBookIssues_LibraryBookIssueId] FOREIGN KEY ([LibraryBookIssueId]) REFERENCES [LibraryBookIssues] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryFineRecords_LibraryMembers_LibraryMemberId] FOREIGN KEY ([LibraryMemberId]) REFERENCES [LibraryMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [FeeReceipts] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FeePaymentId] uniqueidentifier NOT NULL,
        [ReceiptNumber] nvarchar(60) NOT NULL,
        [IssuedOn] datetime2 NOT NULL,
        [PdfStoragePath] nvarchar(500) NULL,
        [IssuedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_FeeReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeReceipts_AspNetUsers_IssuedByUserId] FOREIGN KEY ([IssuedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeReceipts_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeReceipts_FeePayments_FeePaymentId] FOREIGN KEY ([FeePaymentId]) REFERENCES [FeePayments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE TABLE [HostelFees] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [InvoiceNumber] nvarchar(60) NOT NULL,
        [BillingMonth] date NOT NULL,
        [DueDate] date NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [PaidAmount] decimal(12,2) NOT NULL,
        [Status] int NOT NULL,
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
        CONSTRAINT [PK_HostelFees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelFees_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelFees_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Description], [Name], [NormalizedName])
    VALUES (''88888888-8888-8888-8888-888888888888'', N''88888888-8888-8888-8888-888888888888'', N''DepartmentAdmin platform role'', N''DepartmentAdmin'', N''DEPARTMENTADMIN''),
    (''99999999-9999-9999-9999-999999999999'', N''99999999-9999-9999-9999-999999999999'', N''Accountant platform role'', N''Accountant'', N''ACCOUNTANT''),
    (''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'', N''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'', N''Librarian platform role'', N''Librarian'', N''LIBRARIAN''),
    (''bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'', N''bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'', N''TransportManager platform role'', N''TransportManager'', N''TRANSPORTMANAGER''),
    (''cccccccc-cccc-cccc-cccc-cccccccccccc'', N''cccccccc-cccc-cccc-cccc-cccccccccccc'', N''HostelManager platform role'', N''HostelManager'', N''HOSTELMANAGER''),
    (''dddddddd-dddd-dddd-dddd-dddddddddddd'', N''dddddddd-dddd-dddd-dddd-dddddddddddd'', N''ExamController platform role'', N''ExamController'', N''EXAMCONTROLLER''),
    (''eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'', N''eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'', N''StaffAdmin platform role'', N''StaffAdmin'', N''STAFFADMIN'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Announcements_BranchId] ON [Announcements] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Announcements_CreatedByUserId] ON [Announcements] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Announcements_TenantId_BranchId_Audience_PublishOn] ON [Announcements] ([TenantId], [BranchId], [Audience], [PublishOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_CommunicationLogs_BranchId] ON [CommunicationLogs] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_CommunicationLogs_TenantId_BranchId_Channel_OccurredOn] ON [CommunicationLogs] ([TenantId], [BranchId], [Channel], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_DiscountRules_BranchId] ON [DiscountRules] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_DiscountRules_CourseId] ON [DiscountRules] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DiscountRules_TenantId_Code] ON [DiscountRules] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeComponents_BranchId] ON [FeeComponents] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeComponents_FeeStructureId_SortOrder] ON [FeeComponents] ([FeeStructureId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoiceLines_BranchId] ON [FeeInvoiceLines] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoiceLines_FeeComponentId] ON [FeeInvoiceLines] ([FeeComponentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoiceLines_FeeInvoiceId_SortOrder] ON [FeeInvoiceLines] ([FeeInvoiceId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoices_BranchId] ON [FeeInvoices] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoices_StudentFeeAssignmentId] ON [FeeInvoices] ([StudentFeeAssignmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoices_StudentProfileId] ON [FeeInvoices] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeInvoices_TenantId_BranchId_Status] ON [FeeInvoices] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeeInvoices_TenantId_InvoiceNumber] ON [FeeInvoices] ([TenantId], [InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeePayments_BranchId] ON [FeePayments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeePayments_FeeInvoiceId] ON [FeePayments] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeePayments_ReceivedByUserId] ON [FeePayments] ([ReceivedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeePayments_TenantId_PaymentNumber] ON [FeePayments] ([TenantId], [PaymentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeReceipts_BranchId] ON [FeeReceipts] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeeReceipts_FeePaymentId] ON [FeeReceipts] ([FeePaymentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeReceipts_IssuedByUserId] ON [FeeReceipts] ([IssuedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeeReceipts_TenantId_ReceiptNumber] ON [FeeReceipts] ([TenantId], [ReceiptNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeStructures_AcademicYearId] ON [FeeStructures] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeStructures_BatchId] ON [FeeStructures] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeStructures_BranchId] ON [FeeStructures] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeStructures_CourseId] ON [FeeStructures] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_FeeStructures_TenantId_BranchId_AcademicYearId_CourseId_BatchId] ON [FeeStructures] ([TenantId], [BranchId], [AcademicYearId], [CourseId], [BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeeStructures_TenantId_BranchId_Code] ON [FeeStructures] ([TenantId], [BranchId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_BranchId] ON [HostelAllocations] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_HostelBedId] ON [HostelAllocations] ([HostelBedId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_HostelRoomId] ON [HostelAllocations] ([HostelRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_StudentProfileId] ON [HostelAllocations] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_TenantId_HostelBedId_Status] ON [HostelAllocations] ([TenantId], [HostelBedId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelAllocations_TenantId_StudentProfileId_Status] ON [HostelAllocations] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelBeds_BranchId] ON [HostelBeds] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelBeds_HostelRoomId] ON [HostelBeds] ([HostelRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelBeds_TenantId_HostelRoomId_BedNumber] ON [HostelBeds] ([TenantId], [HostelRoomId], [BedNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelBlocks_BranchId] ON [HostelBlocks] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelBlocks_TenantId_BranchId_Code] ON [HostelBlocks] ([TenantId], [BranchId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelFees_BranchId] ON [HostelFees] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelFees_HostelAllocationId] ON [HostelFees] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelFees_TenantId_BranchId_Status] ON [HostelFees] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelFees_TenantId_InvoiceNumber] ON [HostelFees] ([TenantId], [InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelRooms_BranchId] ON [HostelRooms] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_HostelRooms_HostelBlockId] ON [HostelRooms] ([HostelBlockId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelRooms_TenantId_HostelBlockId_RoomNumber] ON [HostelRooms] ([TenantId], [HostelBlockId], [RoomNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookCopies_BranchId] ON [LibraryBookCopies] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookCopies_LibraryBookId] ON [LibraryBookCopies] ([LibraryBookId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LibraryBookCopies_TenantId_BranchId_AccessionNumber] ON [LibraryBookCopies] ([TenantId], [BranchId], [AccessionNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_BranchId] ON [LibraryBookIssues] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_IssuedByUserId] ON [LibraryBookIssues] ([IssuedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_LibraryBookCopyId_Status] ON [LibraryBookIssues] ([LibraryBookCopyId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_LibraryMemberId] ON [LibraryBookIssues] ([LibraryMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_ReturnReceivedByUserId] ON [LibraryBookIssues] ([ReturnReceivedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookIssues_TenantId_BranchId_Status] ON [LibraryBookIssues] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookReturns_BranchId] ON [LibraryBookReturns] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookReturns_LibraryBookIssueId] ON [LibraryBookReturns] ([LibraryBookIssueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBookReturns_ReceivedByUserId] ON [LibraryBookReturns] ([ReceivedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBooks_BranchId] ON [LibraryBooks] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBooks_SubjectId] ON [LibraryBooks] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBooks_TenantId_BranchId_Isbn] ON [LibraryBooks] ([TenantId], [BranchId], [Isbn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryBooks_TenantId_BranchId_Title] ON [LibraryBooks] ([TenantId], [BranchId], [Title]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryFineRecords_BranchId] ON [LibraryFineRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryFineRecords_LibraryBookIssueId] ON [LibraryFineRecords] ([LibraryBookIssueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryFineRecords_LibraryMemberId] ON [LibraryFineRecords] ([LibraryMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryFineRecords_TenantId_LibraryMemberId_Status] ON [LibraryFineRecords] ([TenantId], [LibraryMemberId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryMembers_BranchId] ON [LibraryMembers] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryMembers_StudentProfileId] ON [LibraryMembers] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryMembers_TeacherProfileId] ON [LibraryMembers] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LibraryMembers_TenantId_BranchId_MemberNumber] ON [LibraryMembers] ([TenantId], [BranchId], [MemberNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryMembers_TenantId_StudentProfileId] ON [LibraryMembers] ([TenantId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_LibraryMembers_UserId] ON [LibraryMembers] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationMessages_BranchId] ON [NotificationMessages] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationMessages_CreatedForUserId] ON [NotificationMessages] ([CreatedForUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationMessages_NotificationTemplateId] ON [NotificationMessages] ([NotificationTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationMessages_ScheduledOn] ON [NotificationMessages] ([ScheduledOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationMessages_TenantId_BranchId_Status] ON [NotificationMessages] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationProviderSettings_BranchId] ON [NotificationProviderSettings] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_NotificationProviderSettings_TenantId_BranchId_Channel_ProviderKey] ON [NotificationProviderSettings] ([TenantId], [BranchId], [Channel], [ProviderKey]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationRecipients_BranchId] ON [NotificationRecipients] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationRecipients_NotificationMessageId_DestinationAddress] ON [NotificationRecipients] ([NotificationMessageId], [DestinationAddress]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationRecipients_UserId] ON [NotificationRecipients] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_NotificationTemplates_BranchId] ON [NotificationTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_NotificationTemplates_TenantId_BranchId_Code] ON [NotificationTemplates] ([TenantId], [BranchId], [Code]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Scholarships_BranchId] ON [Scholarships] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Scholarships_StudentProfileId] ON [Scholarships] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Scholarships_TenantId_StudentProfileId_Code] ON [Scholarships] ([TenantId], [StudentProfileId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentFeeAssignments_BranchId] ON [StudentFeeAssignments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentFeeAssignments_DiscountRuleId] ON [StudentFeeAssignments] ([DiscountRuleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentFeeAssignments_FeeStructureId] ON [StudentFeeAssignments] ([FeeStructureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentFeeAssignments_StudentProfileId] ON [StudentFeeAssignments] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentFeeAssignments_TenantId_StudentProfileId_FeeStructureId] ON [StudentFeeAssignments] ([TenantId], [StudentProfileId], [FeeStructureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_BranchId] ON [StudentTransportAssignments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_StudentProfileId] ON [StudentTransportAssignments] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_TenantId_StudentProfileId_Status] ON [StudentTransportAssignments] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_TransportRouteId] ON [StudentTransportAssignments] ([TransportRouteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_TransportRouteStopId] ON [StudentTransportAssignments] ([TransportRouteStopId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_VehicleId] ON [StudentTransportAssignments] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportDrivers_BranchId] ON [TransportDrivers] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportDrivers_TenantId_BranchId_LicenseNumber] ON [TransportDrivers] ([TenantId], [BranchId], [LicenseNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteAssignments_BranchId] ON [TransportRouteAssignments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteAssignments_DriverId] ON [TransportRouteAssignments] ([DriverId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteAssignments_TenantId_BranchId_TransportRouteId_EffectiveFrom] ON [TransportRouteAssignments] ([TenantId], [BranchId], [TransportRouteId], [EffectiveFrom]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteAssignments_TransportRouteId] ON [TransportRouteAssignments] ([TransportRouteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteAssignments_VehicleId] ON [TransportRouteAssignments] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRoutes_BranchId] ON [TransportRoutes] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportRoutes_TenantId_BranchId_RouteCode] ON [TransportRoutes] ([TenantId], [BranchId], [RouteCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_TransportRouteStops_BranchId] ON [TransportRouteStops] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransportRouteStops_TransportRouteId_StopOrder] ON [TransportRouteStops] ([TransportRouteId], [StopOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE INDEX [IX_Vehicles_BranchId] ON [Vehicles] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicles_TenantId_BranchId_RegistrationNumber] ON [Vehicles] ([TenantId], [BranchId], [RegistrationNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906224752_EnterpriseModules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260906224752_EnterpriseModules', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ActivatedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastPasswordChangedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastPasswordResetRequestedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [RequiresActivation] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE TABLE [ParentProfiles] (
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
        CONSTRAINT [FK_ParentProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE TABLE [StaffProfiles] (
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
        CONSTRAINT [FK_StaffProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffProfiles_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StaffProfiles_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE TABLE [UserBranchAssignments] (
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
        CONSTRAINT [FK_UserBranchAssignments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserBranchAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE TABLE [UserInvitations] (
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
        CONSTRAINT [FK_UserInvitations_AspNetUsers_InvitedByUserId] FOREIGN KEY ([InvitedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserInvitations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserInvitations_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE TABLE [UserRoleAssignments] (
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
        CONSTRAINT [FK_UserRoleAssignments_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_AspNetUsers_AssignedByUserId] FOREIGN KEY ([AssignedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoleAssignments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_RequiresActivation] ON [AspNetUsers] ([RequiresActivation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_TenantId_BranchId] ON [AspNetUsers] ([TenantId], [BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_ParentProfiles_BranchId] ON [ParentProfiles] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_ParentProfiles_TenantId_Email] ON [ParentProfiles] ([TenantId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ParentProfiles_TenantId_UserId] ON [ParentProfiles] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_ParentProfiles_UserId] ON [ParentProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_StaffProfiles_BranchId] ON [StaffProfiles] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_StaffProfiles_DepartmentId] ON [StaffProfiles] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffProfiles_TenantId_EmployeeNumber] ON [StaffProfiles] ([TenantId], [EmployeeNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffProfiles_TenantId_UserId] ON [StaffProfiles] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_StaffProfiles_UserId] ON [StaffProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserBranchAssignments_BranchId] ON [UserBranchAssignments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserBranchAssignments_TenantId_BranchId_IsActive] ON [UserBranchAssignments] ([TenantId], [BranchId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserBranchAssignments_TenantId_UserId_BranchId] ON [UserBranchAssignments] ([TenantId], [UserId], [BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserBranchAssignments_UserId] ON [UserBranchAssignments] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserInvitations_BranchId] ON [UserInvitations] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserInvitations_InvitedByUserId] ON [UserInvitations] ([InvitedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserInvitations_TenantId_Email_Status] ON [UserInvitations] ([TenantId], [Email], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserInvitations_TenantId_UserId_Status] ON [UserInvitations] ([TenantId], [UserId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserInvitations_UserId] ON [UserInvitations] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserRoleAssignments_AssignedByUserId] ON [UserRoleAssignments] ([AssignedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserRoleAssignments_BranchId] ON [UserRoleAssignments] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserRoleAssignments_RoleId] ON [UserRoleAssignments] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserRoleAssignments_TenantId_RoleName_IsActive] ON [UserRoleAssignments] ([TenantId], [RoleName], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_UserRoleAssignments_TenantId_UserId_RoleName_BranchId] ON [UserRoleAssignments] ([TenantId], [UserId], [RoleName], [BranchId]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    CREATE INDEX [IX_UserRoleAssignments_UserId] ON [UserRoleAssignments] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908204854_UserManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908204854_UserManagement', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    ALTER TABLE [TeacherProfiles] ADD [DepartmentId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE TABLE [StudentLifecycleEvents] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [FromStatus] int NOT NULL,
        [ToStatus] int NOT NULL,
        [FromBranchId] uniqueidentifier NULL,
        [ToBranchId] uniqueidentifier NULL,
        [FromAcademicYearId] uniqueidentifier NULL,
        [ToAcademicYearId] uniqueidentifier NULL,
        [FromCourseId] uniqueidentifier NULL,
        [ToCourseId] uniqueidentifier NULL,
        [FromBatchId] uniqueidentifier NULL,
        [ToBatchId] uniqueidentifier NULL,
        [FromSectionId] uniqueidentifier NULL,
        [ToSectionId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [RecordedByUserId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
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
        CONSTRAINT [PK_StudentLifecycleEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentLifecycleEvents_AcademicYears_FromAcademicYearId] FOREIGN KEY ([FromAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_AcademicYears_ToAcademicYearId] FOREIGN KEY ([ToAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_AspNetUsers_RecordedByUserId] FOREIGN KEY ([RecordedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Batches_FromBatchId] FOREIGN KEY ([FromBatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Batches_ToBatchId] FOREIGN KEY ([ToBatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Branches_FromBranchId] FOREIGN KEY ([FromBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Courses_FromCourseId] FOREIGN KEY ([FromCourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Courses_ToCourseId] FOREIGN KEY ([ToCourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Sections_FromSectionId] FOREIGN KEY ([FromSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_Sections_ToSectionId] FOREIGN KEY ([ToSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleEvents_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE TABLE [TeacherLifecycleEvents] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [FromStatus] int NOT NULL,
        [ToStatus] int NOT NULL,
        [FromBranchId] uniqueidentifier NULL,
        [ToBranchId] uniqueidentifier NULL,
        [FromDepartmentId] uniqueidentifier NULL,
        [ToDepartmentId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [RecordedByUserId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
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
        CONSTRAINT [PK_TeacherLifecycleEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherLifecycleEvents_AspNetUsers_RecordedByUserId] FOREIGN KEY ([RecordedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_Branches_FromBranchId] FOREIGN KEY ([FromBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_Departments_FromDepartmentId] FOREIGN KEY ([FromDepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_Departments_ToDepartmentId] FOREIGN KEY ([ToDepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleEvents_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherProfiles_DepartmentId] ON [TeacherProfiles] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_BranchId] ON [StudentLifecycleEvents] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_FromAcademicYearId] ON [StudentLifecycleEvents] ([FromAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_FromBatchId] ON [StudentLifecycleEvents] ([FromBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_FromBranchId] ON [StudentLifecycleEvents] ([FromBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_FromCourseId] ON [StudentLifecycleEvents] ([FromCourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_FromSectionId] ON [StudentLifecycleEvents] ([FromSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_RecordedByUserId] ON [StudentLifecycleEvents] ([RecordedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_StudentProfileId] ON [StudentLifecycleEvents] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn] ON [StudentLifecycleEvents] ([TenantId], [BranchId], [EventType], [EffectiveOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_TenantId_StudentProfileId_EffectiveOn] ON [StudentLifecycleEvents] ([TenantId], [StudentProfileId], [EffectiveOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_ToAcademicYearId] ON [StudentLifecycleEvents] ([ToAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_ToBatchId] ON [StudentLifecycleEvents] ([ToBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_ToBranchId] ON [StudentLifecycleEvents] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_ToCourseId] ON [StudentLifecycleEvents] ([ToCourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleEvents_ToSectionId] ON [StudentLifecycleEvents] ([ToSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_BranchId] ON [TeacherLifecycleEvents] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_FromBranchId] ON [TeacherLifecycleEvents] ([FromBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_FromDepartmentId] ON [TeacherLifecycleEvents] ([FromDepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_RecordedByUserId] ON [TeacherLifecycleEvents] ([RecordedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_TeacherProfileId] ON [TeacherLifecycleEvents] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_TenantId_BranchId_EventType_EffectiveOn] ON [TeacherLifecycleEvents] ([TenantId], [BranchId], [EventType], [EffectiveOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_TenantId_TeacherProfileId_EffectiveOn] ON [TeacherLifecycleEvents] ([TenantId], [TeacherProfileId], [EffectiveOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_ToBranchId] ON [TeacherLifecycleEvents] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleEvents_ToDepartmentId] ON [TeacherLifecycleEvents] ([ToDepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    ALTER TABLE [TeacherProfiles] ADD CONSTRAINT [FK_TeacherProfiles_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909231741_StudentTeacherLifecycle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909231741_StudentTeacherLifecycle', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [SizeBytes] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [UploadedByUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [UploadedOn] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [AdmissionFormTemplateId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [EnrolledStudentProfileId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [FormResponseJson] nvarchar(4000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE TABLE [AdmissionFormTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [AcademicYearId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Instructions] nvarchar(1000) NULL,
        [IsDefault] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AdmissionFormTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionFormTemplates_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionFormTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionFormTemplates_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE TABLE [AdmissionDocumentRequirements] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionFormTemplateId] uniqueidentifier NOT NULL,
        [DocumentType] int NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [IsRequired] bit NOT NULL,
        [SortOrder] int NOT NULL,
        [Notes] nvarchar(500) NULL,
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
        CONSTRAINT [PK_AdmissionDocumentRequirements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionDocumentRequirements_AdmissionFormTemplates_AdmissionFormTemplateId] FOREIGN KEY ([AdmissionFormTemplateId]) REFERENCES [AdmissionFormTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE TABLE [AdmissionFormFields] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionFormTemplateId] uniqueidentifier NOT NULL,
        [FieldKey] nvarchar(80) NOT NULL,
        [Label] nvarchar(150) NOT NULL,
        [FieldType] int NOT NULL,
        [IsRequired] bit NOT NULL,
        [SortOrder] int NOT NULL,
        [Placeholder] nvarchar(200) NULL,
        [HelpText] nvarchar(500) NULL,
        [OptionsJson] nvarchar(2000) NULL,
        [ValidationRegex] nvarchar(200) NULL,
        [MaxLength] int NULL,
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
        CONSTRAINT [PK_AdmissionFormFields] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionFormFields_AdmissionFormTemplates_AdmissionFormTemplateId] FOREIGN KEY ([AdmissionFormTemplateId]) REFERENCES [AdmissionFormTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionDocuments_UploadedByUserId] ON [AdmissionDocuments] ([UploadedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_AdmissionFormTemplateId] ON [AdmissionApplications] ([AdmissionFormTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_EnrolledStudentProfileId] ON [AdmissionApplications] ([EnrolledStudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdmissionDocumentRequirements_AdmissionFormTemplateId_DocumentType] ON [AdmissionDocumentRequirements] ([AdmissionFormTemplateId], [DocumentType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionDocumentRequirements_AdmissionFormTemplateId_SortOrder] ON [AdmissionDocumentRequirements] ([AdmissionFormTemplateId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdmissionFormFields_AdmissionFormTemplateId_FieldKey] ON [AdmissionFormFields] ([AdmissionFormTemplateId], [FieldKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionFormFields_AdmissionFormTemplateId_SortOrder] ON [AdmissionFormFields] ([AdmissionFormTemplateId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionFormTemplates_AcademicYearId] ON [AdmissionFormTemplates] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionFormTemplates_BranchId] ON [AdmissionFormTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionFormTemplates_CourseId] ON [AdmissionFormTemplates] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE INDEX [IX_AdmissionFormTemplates_TenantId_BranchId_CourseId_AcademicYearId_IsActive] ON [AdmissionFormTemplates] ([TenantId], [BranchId], [CourseId], [AcademicYearId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdmissionFormTemplates_TenantId_Name] ON [AdmissionFormTemplates] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD CONSTRAINT [FK_AdmissionApplications_AdmissionFormTemplates_AdmissionFormTemplateId] FOREIGN KEY ([AdmissionFormTemplateId]) REFERENCES [AdmissionFormTemplates] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD CONSTRAINT [FK_AdmissionApplications_StudentProfiles_EnrolledStudentProfileId] FOREIGN KEY ([EnrolledStudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD CONSTRAINT [FK_AdmissionDocuments_AspNetUsers_UploadedByUserId] FOREIGN KEY ([UploadedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912235710_AdmissionsWorkflowPolish'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260912235710_AdmissionsWorkflowPolish', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [AttendanceCorrectionRequests] (
        [Id] uniqueidentifier NOT NULL,
        [AttendanceRecordId] uniqueidentifier NOT NULL,
        [AttendanceSessionId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [CurrentStatus] int NOT NULL,
        [RequestedStatus] int NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [Status] int NOT NULL,
        [Reason] nvarchar(500) NULL,
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
        CONSTRAINT [PK_AttendanceCorrectionRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceCorrectionRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCorrectionRequests_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCorrectionRequests_AttendanceRecords_AttendanceRecordId] FOREIGN KEY ([AttendanceRecordId]) REFERENCES [AttendanceRecords] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCorrectionRequests_AttendanceSessions_AttendanceSessionId] FOREIGN KEY ([AttendanceSessionId]) REFERENCES [AttendanceSessions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCorrectionRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE TABLE [TimetableSubstitutions] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [TimetableEntryId] uniqueidentifier NOT NULL,
        [SubstitutionDate] date NOT NULL,
        [OriginalTeacherProfileId] uniqueidentifier NOT NULL,
        [SubstituteTeacherProfileId] uniqueidentifier NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [Status] int NOT NULL,
        [Reason] nvarchar(500) NULL,
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
        CONSTRAINT [PK_TimetableSubstitutions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableSubstitutions_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableSubstitutions_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableSubstitutions_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableSubstitutions_TeacherProfiles_OriginalTeacherProfileId] FOREIGN KEY ([OriginalTeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableSubstitutions_TeacherProfiles_SubstituteTeacherProfileId] FOREIGN KEY ([SubstituteTeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableSubstitutions_TimetableEntries_TimetableEntryId] FOREIGN KEY ([TimetableEntryId]) REFERENCES [TimetableEntries] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_AttendanceRecordId] ON [AttendanceCorrectionRequests] ([AttendanceRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_AttendanceSessionId] ON [AttendanceCorrectionRequests] ([AttendanceSessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_RequestedByUserId] ON [AttendanceCorrectionRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_ReviewedByUserId] ON [AttendanceCorrectionRequests] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_StudentProfileId] ON [AttendanceCorrectionRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_TenantId_AttendanceRecordId_Status] ON [AttendanceCorrectionRequests] ([TenantId], [AttendanceRecordId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_AttendanceCorrectionRequests_TenantId_AttendanceSessionId_Status] ON [AttendanceCorrectionRequests] ([TenantId], [AttendanceSessionId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_BranchId] ON [TimetableSubstitutions] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_OriginalTeacherProfileId] ON [TimetableSubstitutions] ([OriginalTeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_RequestedByUserId] ON [TimetableSubstitutions] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_ReviewedByUserId] ON [TimetableSubstitutions] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_SubstituteTeacherProfileId] ON [TimetableSubstitutions] ([SubstituteTeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_TenantId_BranchId_SubstitutionDate_Status] ON [TimetableSubstitutions] ([TenantId], [BranchId], [SubstitutionDate], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_TenantId_TimetableEntryId_SubstitutionDate] ON [TimetableSubstitutions] ([TenantId], [TimetableEntryId], [SubstitutionDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    CREATE INDEX [IX_TimetableSubstitutions_TimetableEntryId] ON [TimetableSubstitutions] ([TimetableEntryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913201046_AttendanceTimetableWorkflowEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913201046_AttendanceTimetableWorkflowEnhancements', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    DECLARE @var32 sysname;
    SELECT @var32 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FeeInvoices]') AND [c].[name] = N'StudentProfileId');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [FeeInvoices] DROP CONSTRAINT [' + @var32 + '];');
    ALTER TABLE [FeeInvoices] ALTER COLUMN [StudentProfileId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913235447_AdmissionsWorkflowCompletion'
)
BEGIN
    DECLARE @var33 sysname;
    SELECT @var33 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FeeInvoices]') AND [c].[name] = N'StudentFeeAssignmentId');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [FeeInvoices] DROP CONSTRAINT [' + @var33 + '];');
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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [StudentAlumniRecords] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [BatchId] uniqueidentifier NULL,
        [AlumniNumber] nvarchar(50) NOT NULL,
        [GraduationDate] date NOT NULL,
        [ContactEmail] nvarchar(150) NULL,
        [ContactPhone] nvarchar(30) NULL,
        [HigherEducation] nvarchar(250) NULL,
        [EmployerOrInstitution] nvarchar(250) NULL,
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
        CONSTRAINT [PK_StudentAlumniRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentAlumniRecords_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Batches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAlumniRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [StudentLifecycleRequests] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [ToStatus] int NOT NULL,
        [Status] int NOT NULL,
        [ToBranchId] uniqueidentifier NULL,
        [ToAcademicYearId] uniqueidentifier NULL,
        [ToCourseId] uniqueidentifier NULL,
        [ToBatchId] uniqueidentifier NULL,
        [ToSectionId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [DecidedOn] datetime2 NULL,
        [DecidedByUserId] uniqueidentifier NULL,
        [AppliedStudentLifecycleEventId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
        [Notes] nvarchar(1000) NULL,
        [DecisionNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentLifecycleRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentLifecycleRequests_AcademicYears_ToAcademicYearId] FOREIGN KEY ([ToAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_AspNetUsers_DecidedByUserId] FOREIGN KEY ([DecidedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Batches_ToBatchId] FOREIGN KEY ([ToBatchId]) REFERENCES [Batches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Courses_ToCourseId] FOREIGN KEY ([ToCourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_Sections_ToSectionId] FOREIGN KEY ([ToSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_StudentLifecycleEvents_AppliedStudentLifecycleEventId] FOREIGN KEY ([AppliedStudentLifecycleEventId]) REFERENCES [StudentLifecycleEvents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentLifecycleRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE TABLE [TeacherLifecycleRequests] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [EventType] int NOT NULL,
        [ToStatus] int NOT NULL,
        [Status] int NOT NULL,
        [ToBranchId] uniqueidentifier NULL,
        [ToDepartmentId] uniqueidentifier NULL,
        [EffectiveOn] date NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [DecidedOn] datetime2 NULL,
        [DecidedByUserId] uniqueidentifier NULL,
        [AppliedTeacherLifecycleEventId] uniqueidentifier NULL,
        [Reason] nvarchar(500) NULL,
        [Notes] nvarchar(1000) NULL,
        [DecisionNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherLifecycleRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherLifecycleRequests_AspNetUsers_DecidedByUserId] FOREIGN KEY ([DecidedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Branches_ToBranchId] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_Departments_ToDepartmentId] FOREIGN KEY ([ToDepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_TeacherLifecycleEvents_AppliedTeacherLifecycleEventId] FOREIGN KEY ([AppliedTeacherLifecycleEventId]) REFERENCES [TeacherLifecycleEvents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLifecycleRequests_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_AcademicYearId] ON [StudentAlumniRecords] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_BatchId] ON [StudentAlumniRecords] ([BatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_BranchId] ON [StudentAlumniRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_CourseId] ON [StudentAlumniRecords] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_StudentProfileId] ON [StudentAlumniRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentAlumniRecords_TenantId_AlumniNumber] ON [StudentAlumniRecords] ([TenantId], [AlumniNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentAlumniRecords_TenantId_BranchId_GraduationDate] ON [StudentAlumniRecords] ([TenantId], [BranchId], [GraduationDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentAlumniRecords_TenantId_StudentProfileId] ON [StudentAlumniRecords] ([TenantId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_AppliedStudentLifecycleEventId] ON [StudentLifecycleRequests] ([AppliedStudentLifecycleEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_BranchId] ON [StudentLifecycleRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_DecidedByUserId] ON [StudentLifecycleRequests] ([DecidedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_RequestedByUserId] ON [StudentLifecycleRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_StudentProfileId] ON [StudentLifecycleRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_TenantId_BranchId_Status_RequestedOn] ON [StudentLifecycleRequests] ([TenantId], [BranchId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_TenantId_StudentProfileId_Status] ON [StudentLifecycleRequests] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToAcademicYearId] ON [StudentLifecycleRequests] ([ToAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToBatchId] ON [StudentLifecycleRequests] ([ToBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToBranchId] ON [StudentLifecycleRequests] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToCourseId] ON [StudentLifecycleRequests] ([ToCourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_StudentLifecycleRequests_ToSectionId] ON [StudentLifecycleRequests] ([ToSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_AppliedTeacherLifecycleEventId] ON [TeacherLifecycleRequests] ([AppliedTeacherLifecycleEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_BranchId] ON [TeacherLifecycleRequests] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_DecidedByUserId] ON [TeacherLifecycleRequests] ([DecidedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_RequestedByUserId] ON [TeacherLifecycleRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TeacherProfileId] ON [TeacherLifecycleRequests] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TenantId_BranchId_Status_RequestedOn] ON [TeacherLifecycleRequests] ([TenantId], [BranchId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_TenantId_TeacherProfileId_Status] ON [TeacherLifecycleRequests] ([TenantId], [TeacherProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_ToBranchId] ON [TeacherLifecycleRequests] ([ToBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    CREATE INDEX [IX_TeacherLifecycleRequests_ToDepartmentId] ON [TeacherLifecycleRequests] ([ToDepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260914221820_StudentTeacherLifecycleWorkflowCompletion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260914221820_StudentTeacherLifecycleWorkflowCompletion', N'9.0.0');
END;

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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [AcademicTerms] (
        [Id] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Type] int NOT NULL,
        [Sequence] int NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
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
        CONSTRAINT [PK_AcademicTerms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AcademicTerms_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [LearningOutcomes] (
        [Id] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [SyllabusUnitId] uniqueidentifier NULL,
        [Code] nvarchar(40) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [BloomLevel] nvarchar(80) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LearningOutcomes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LearningOutcomes_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LearningOutcomes_SyllabusUnits_SyllabusUnitId] FOREIGN KEY ([SyllabusUnitId]) REFERENCES [SyllabusUnits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [SubjectReferenceBooks] (
        [Id] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Authors] nvarchar(200) NULL,
        [Isbn] nvarchar(30) NULL,
        [Publisher] nvarchar(150) NULL,
        [EditionYear] int NULL,
        [ResourceUrl] nvarchar(500) NULL,
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
        CONSTRAINT [PK_SubjectReferenceBooks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubjectReferenceBooks_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [SyllabusTopics] (
        [Id] uniqueidentifier NOT NULL,
        [SyllabusUnitId] uniqueidentifier NOT NULL,
        [Order] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EstimatedMinutes] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SyllabusTopics] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SyllabusTopics_SyllabusUnits_SyllabusUnitId] FOREIGN KEY ([SyllabusUnitId]) REFERENCES [SyllabusUnits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TeacherAvailabilityRules] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartsAt] time NOT NULL,
        [EndsAt] time NOT NULL,
        [RuleType] int NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [Reason] nvarchar(300) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TeacherAvailabilityRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherAvailabilityRules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherAvailabilityRules_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
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
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [AcademicCalendarEvents] (
        [Id] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [AcademicTermId] uniqueidentifier NULL,
        [Title] nvarchar(160) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EventType] int NOT NULL,
        [StartsOn] date NOT NULL,
        [EndsOn] date NOT NULL,
        [IsInstructionalDay] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AcademicCalendarEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AcademicCalendarEvents_AcademicTerms_AcademicTermId] FOREIGN KEY ([AcademicTermId]) REFERENCES [AcademicTerms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AcademicCalendarEvents_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AcademicCalendarEvents_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TeacherLessonPlans] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [AcademicTermId] uniqueidentifier NOT NULL,
        [SyllabusTopicId] uniqueidentifier NULL,
        [PlannedDate] date NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Objectives] nvarchar(2000) NULL,
        [Activities] nvarchar(2000) NULL,
        [Resources] nvarchar(1000) NULL,
        [Assessment] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_TeacherLessonPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherLessonPlans_AcademicTerms_AcademicTermId] FOREIGN KEY ([AcademicTermId]) REFERENCES [AcademicTerms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_SyllabusTopics_SyllabusTopicId] FOREIGN KEY ([SyllabusTopicId]) REFERENCES [SyllabusTopics] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherLessonPlans_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableGenerationRuns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [TimetableTemplateId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [StartedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [ScheduledPeriods] int NOT NULL,
        [UnscheduledPeriods] int NOT NULL,
        [Diagnostics] nvarchar(4000) NULL,
        [GeneratedTimetableId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableGenerationRuns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableGenerationRuns_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_TimetableTemplates_TimetableTemplateId] FOREIGN KEY ([TimetableTemplateId]) REFERENCES [TimetableTemplates] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableGenerationRuns_Timetables_GeneratedTimetableId] FOREIGN KEY ([GeneratedTimetableId]) REFERENCES [Timetables] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE TABLE [TimetableTemplateItems] (
        [Id] uniqueidentifier NOT NULL,
        [TimetableTemplateId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [PeriodsPerWeek] int NOT NULL,
        [MaximumPeriodsPerDay] int NOT NULL,
        [PreferredRoomId] uniqueidentifier NULL,
        [RequireConsecutivePeriods] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TimetableTemplateItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimetableTemplateItems_Rooms_PreferredRoomId] FOREIGN KEY ([PreferredRoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableTemplateItems_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TimetableTemplateItems_TimetableTemplates_TimetableTemplateId] FOREIGN KEY ([TimetableTemplateId]) REFERENCES [TimetableTemplates] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_AcademicTermId] ON [AcademicCalendarEvents] ([AcademicTermId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_AcademicYearId] ON [AcademicCalendarEvents] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_BranchId] ON [AcademicCalendarEvents] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicCalendarEvents_TenantId_AcademicYearId_StartsOn] ON [AcademicCalendarEvents] ([TenantId], [AcademicYearId], [StartsOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_AcademicTerms_AcademicYearId] ON [AcademicTerms] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AcademicTerms_TenantId_AcademicYearId_Sequence] ON [AcademicTerms] ([TenantId], [AcademicYearId], [Sequence]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_LearningOutcomes_SubjectId] ON [LearningOutcomes] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_LearningOutcomes_SyllabusUnitId] ON [LearningOutcomes] ([SyllabusUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LearningOutcomes_TenantId_SubjectId_Code] ON [LearningOutcomes] ([TenantId], [SubjectId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_SubjectReferenceBooks_SubjectId] ON [SubjectReferenceBooks] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_SubjectReferenceBooks_TenantId_SubjectId_Title] ON [SubjectReferenceBooks] ([TenantId], [SubjectId], [Title]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SyllabusTopics_SyllabusUnitId_Order] ON [SyllabusTopics] ([SyllabusUnitId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_BranchId] ON [TeacherAvailabilityRules] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_TeacherProfileId] ON [TeacherAvailabilityRules] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilityRules_TenantId_TeacherProfileId_DayOfWeek_StartsAt_EndsAt] ON [TeacherAvailabilityRules] ([TenantId], [TeacherProfileId], [DayOfWeek], [StartsAt], [EndsAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_AcademicTermId] ON [TeacherLessonPlans] ([AcademicTermId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SectionId] ON [TeacherLessonPlans] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SubjectId] ON [TeacherLessonPlans] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_SyllabusTopicId] ON [TeacherLessonPlans] ([SyllabusTopicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_TeacherProfileId] ON [TeacherLessonPlans] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherLessonPlans_TenantId_TeacherProfileId_PlannedDate] ON [TeacherLessonPlans] ([TenantId], [TeacherProfileId], [PlannedDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_AcademicYearId] ON [TimetableGenerationRuns] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_BranchId] ON [TimetableGenerationRuns] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_GeneratedTimetableId] ON [TimetableGenerationRuns] ([GeneratedTimetableId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_SectionId] ON [TimetableGenerationRuns] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_TenantId_BranchId_StartedOn] ON [TimetableGenerationRuns] ([TenantId], [BranchId], [StartedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableGenerationRuns_TimetableTemplateId] ON [TimetableGenerationRuns] ([TimetableTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplateItems_PreferredRoomId] ON [TimetableTemplateItems] ([PreferredRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplateItems_SubjectId] ON [TimetableTemplateItems] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableTemplateItems_TimetableTemplateId_SubjectId] ON [TimetableTemplateItems] ([TimetableTemplateId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE INDEX [IX_TimetableTemplates_BranchId] ON [TimetableTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TimetableTemplates_TenantId_BranchId_Name] ON [TimetableTemplates] ([TenantId], [BranchId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260919110117_AcademicStructureDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260919110117_AcademicStructureDepth', N'9.0.0');
END;

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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [ExpiresOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [ProfileDocumentPolicyId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [RetainUntil] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [ScanMessage] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [ScanStatus] int NOT NULL DEFAULT 5;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [ScannedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [Sha256] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [StorageProvider] nvarchar(30) NOT NULL DEFAULT N'Local';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD [VerificationStatus] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [AlumniEngagements] (
        [Id] uniqueidentifier NOT NULL,
        [StudentAlumniRecordId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [ContributionAmount] decimal(18,2) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AlumniEngagements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AlumniEngagements_StudentAlumniRecords_StudentAlumniRecordId] FOREIGN KEY ([StudentAlumniRecordId]) REFERENCES [StudentAlumniRecords] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [BulkStudentLifecycleRuns] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [FromAcademicYearId] uniqueidentifier NOT NULL,
        [ToAcademicYearId] uniqueidentifier NOT NULL,
        [FromSectionId] uniqueidentifier NULL,
        [ToSectionId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [TotalCount] int NOT NULL,
        [SucceededCount] int NOT NULL,
        [FailedCount] int NOT NULL,
        [StartedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [RunBy] nvarchar(128) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BulkStudentLifecycleRuns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BulkStudentLifecycleRuns_AcademicYears_FromAcademicYearId] FOREIGN KEY ([FromAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BulkStudentLifecycleRuns_AcademicYears_ToAcademicYearId] FOREIGN KEY ([ToAcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BulkStudentLifecycleRuns_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BulkStudentLifecycleRuns_Sections_FromSectionId] FOREIGN KEY ([FromSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BulkStudentLifecycleRuns_Sections_ToSectionId] FOREIGN KEY ([ToSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [LifecycleDocumentTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(60) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [OwnerType] int NOT NULL,
        [DocumentType] nvarchar(100) NOT NULL,
        [HtmlTemplate] nvarchar(max) NOT NULL,
        [Version] int NOT NULL,
        [IsActive] bit NOT NULL,
        [RequiresSignature] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LifecycleDocumentTemplates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [ProfileDocumentPolicies] (
        [Id] uniqueidentifier NOT NULL,
        [OwnerType] int NOT NULL,
        [DocumentType] nvarchar(100) NOT NULL,
        [AllowedExtensions] nvarchar(300) NOT NULL,
        [MaximumBytes] bigint NOT NULL,
        [RequiresVirusScan] bit NOT NULL,
        [RequiresVerification] bit NOT NULL,
        [RetentionDays] int NULL,
        [ExpiryDays] int NULL,
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
        CONSTRAINT [PK_ProfileDocumentPolicies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [ProfileStatusHistories] (
        [Id] uniqueidentifier NOT NULL,
        [OwnerType] int NOT NULL,
        [OwnerId] uniqueidentifier NOT NULL,
        [PreviousStatus] nvarchar(50) NULL,
        [Status] nvarchar(50) NOT NULL,
        [EffectiveOn] date NOT NULL,
        [Source] nvarchar(80) NOT NULL,
        [SourceEntityId] uniqueidentifier NULL,
        [Notes] nvarchar(500) NULL,
        [RecordedBy] nvarchar(128) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProfileStatusHistories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [StudentMedicalProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [BloodGroup] nvarchar(20) NULL,
        [Allergies] nvarchar(2000) NULL,
        [CurrentMedications] nvarchar(2000) NULL,
        [EmergencyInstructions] nvarchar(2000) NULL,
        [PhysicianName] nvarchar(150) NULL,
        [PhysicianPhone] nvarchar(30) NULL,
        [InsuranceProvider] nvarchar(100) NULL,
        [InsurancePolicyNumber] nvarchar(100) NULL,
        [EmergencyCareConsent] bit NOT NULL,
        [LastReviewedOn] date NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentMedicalProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentMedicalProfiles_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [StudentTranscripts] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [AcademicYearId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [TranscriptNumber] nvarchar(50) NOT NULL,
        [IssuedOn] date NOT NULL,
        [AggregatePercentage] decimal(18,2) NULL,
        [FinalGrade] nvarchar(20) NULL,
        [IsFinal] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentTranscripts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentTranscripts_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTranscripts_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentTranscripts_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [BulkStudentLifecycleRows] (
        [Id] uniqueidentifier NOT NULL,
        [BulkStudentLifecycleRunId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Succeeded] bit NOT NULL,
        [Message] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BulkStudentLifecycleRows] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BulkStudentLifecycleRows_BulkStudentLifecycleRuns_BulkStudentLifecycleRunId] FOREIGN KEY ([BulkStudentLifecycleRunId]) REFERENCES [BulkStudentLifecycleRuns] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BulkStudentLifecycleRows_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [DocumentApprovalSignatures] (
        [Id] uniqueidentifier NOT NULL,
        [ProfileDocumentId] uniqueidentifier NULL,
        [LifecycleDocumentTemplateId] uniqueidentifier NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [SignerName] nvarchar(150) NOT NULL,
        [SignerTitle] nvarchar(150) NULL,
        [SignedOn] datetime2 NOT NULL,
        [SignatureHash] nvarchar(128) NOT NULL,
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
        CONSTRAINT [PK_DocumentApprovalSignatures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DocumentApprovalSignatures_LifecycleDocumentTemplates_LifecycleDocumentTemplateId] FOREIGN KEY ([LifecycleDocumentTemplateId]) REFERENCES [LifecycleDocumentTemplates] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DocumentApprovalSignatures_ProfileDocuments_ProfileDocumentId] FOREIGN KEY ([ProfileDocumentId]) REFERENCES [ProfileDocuments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [StudentMedicalConditions] (
        [Id] uniqueidentifier NOT NULL,
        [StudentMedicalProfileId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Severity] nvarchar(50) NULL,
        [DiagnosedOn] date NULL,
        [ResolvedOn] date NULL,
        [CareInstructions] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentMedicalConditions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentMedicalConditions_StudentMedicalProfiles_StudentMedicalProfileId] FOREIGN KEY ([StudentMedicalProfileId]) REFERENCES [StudentMedicalProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE TABLE [StudentTranscriptLines] (
        [Id] uniqueidentifier NOT NULL,
        [StudentTranscriptId] uniqueidentifier NOT NULL,
        [AssessmentName] nvarchar(150) NOT NULL,
        [MaximumMarks] decimal(18,2) NOT NULL,
        [MarksObtained] decimal(18,2) NOT NULL,
        [Percentage] decimal(18,2) NOT NULL,
        [Grade] nvarchar(20) NULL,
        [IsPassed] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StudentTranscriptLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentTranscriptLines_StudentTranscripts_StudentTranscriptId] FOREIGN KEY ([StudentTranscriptId]) REFERENCES [StudentTranscripts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_ProfileDocuments_ProfileDocumentPolicyId] ON [ProfileDocuments] ([ProfileDocumentPolicyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_AlumniEngagements_StudentAlumniRecordId] ON [AlumniEngagements] ([StudentAlumniRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_AlumniEngagements_TenantId_StudentAlumniRecordId_OccurredOn] ON [AlumniEngagements] ([TenantId], [StudentAlumniRecordId], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BulkStudentLifecycleRows_BulkStudentLifecycleRunId_StudentProfileId] ON [BulkStudentLifecycleRows] ([BulkStudentLifecycleRunId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRows_StudentProfileId] ON [BulkStudentLifecycleRows] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_BranchId] ON [BulkStudentLifecycleRuns] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_FromAcademicYearId] ON [BulkStudentLifecycleRuns] ([FromAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_FromSectionId] ON [BulkStudentLifecycleRuns] ([FromSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_TenantId_BranchId_StartedOn] ON [BulkStudentLifecycleRuns] ([TenantId], [BranchId], [StartedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_ToAcademicYearId] ON [BulkStudentLifecycleRuns] ([ToAcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_BulkStudentLifecycleRuns_ToSectionId] ON [BulkStudentLifecycleRuns] ([ToSectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_DocumentApprovalSignatures_LifecycleDocumentTemplateId] ON [DocumentApprovalSignatures] ([LifecycleDocumentTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_DocumentApprovalSignatures_ProfileDocumentId] ON [DocumentApprovalSignatures] ([ProfileDocumentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_DocumentApprovalSignatures_TenantId_SubjectId_SignedOn] ON [DocumentApprovalSignatures] ([TenantId], [SubjectId], [SignedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LifecycleDocumentTemplates_TenantId_Code_Version] ON [LifecycleDocumentTemplates] ([TenantId], [Code], [Version]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProfileDocumentPolicies_TenantId_OwnerType_DocumentType] ON [ProfileDocumentPolicies] ([TenantId], [OwnerType], [DocumentType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_ProfileStatusHistories_TenantId_OwnerType_OwnerId_EffectiveOn] ON [ProfileStatusHistories] ([TenantId], [OwnerType], [OwnerId], [EffectiveOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentMedicalConditions_StudentMedicalProfileId] ON [StudentMedicalConditions] ([StudentMedicalProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentMedicalProfiles_StudentProfileId] ON [StudentMedicalProfiles] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentMedicalProfiles_TenantId_StudentProfileId] ON [StudentMedicalProfiles] ([TenantId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentTranscriptLines_StudentTranscriptId] ON [StudentTranscriptLines] ([StudentTranscriptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentTranscripts_AcademicYearId] ON [StudentTranscripts] ([AcademicYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentTranscripts_CourseId] ON [StudentTranscripts] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentTranscripts_StudentProfileId] ON [StudentTranscripts] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE INDEX [IX_StudentTranscripts_TenantId_StudentProfileId_IsFinal] ON [StudentTranscripts] ([TenantId], [StudentProfileId], [IsFinal]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentTranscripts_TenantId_TranscriptNumber] ON [StudentTranscripts] ([TenantId], [TranscriptNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    INSERT INTO ProfileStatusHistories
        (Id, OwnerType, OwnerId, PreviousStatus, Status, EffectiveOn, Source, SourceEntityId, Notes, RecordedBy,
         CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), 1, StudentProfileId,
           CASE FromStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'Inactive' WHEN 3 THEN 'Transferred' WHEN 4 THEN 'Graduated' WHEN 5 THEN 'Withdrawn' END,
           CASE ToStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'Inactive' WHEN 3 THEN 'Transferred' WHEN 4 THEN 'Graduated' WHEN 5 THEN 'Withdrawn' ELSE CAST(ToStatus AS nvarchar(50)) END, EffectiveOn,
           CONCAT('StudentLifecycle:', CAST(EventType AS nvarchar(20))), Id, COALESCE(Notes, Reason), NULL,
           SYSUTCDATETIME(), 0, NEWID(), TenantId
    FROM StudentLifecycleEvents e
    WHERE e.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM ProfileStatusHistories h WHERE h.SourceEntityId = e.Id AND h.OwnerType = 1);

    INSERT INTO ProfileStatusHistories
        (Id, OwnerType, OwnerId, PreviousStatus, Status, EffectiveOn, Source, SourceEntityId, Notes, RecordedBy,
         CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), 2, TeacherProfileId,
           CASE FromStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'OnLeave' WHEN 3 THEN 'Inactive' WHEN 4 THEN 'Relieved' END,
           CASE ToStatus WHEN 1 THEN 'Active' WHEN 2 THEN 'OnLeave' WHEN 3 THEN 'Inactive' WHEN 4 THEN 'Relieved' ELSE CAST(ToStatus AS nvarchar(50)) END, EffectiveOn,
           CONCAT('TeacherLifecycle:', CAST(EventType AS nvarchar(20))), Id, COALESCE(Notes, Reason), NULL,
           SYSUTCDATETIME(), 0, NEWID(), TenantId
    FROM TeacherLifecycleEvents e
    WHERE e.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM ProfileStatusHistories h WHERE h.SourceEntityId = e.Id AND h.OwnerType = 2);

    INSERT INTO LifecycleDocumentTemplates
        (Id, Code, Name, OwnerType, DocumentType, HtmlTemplate, Version, IsActive, RequiresSignature,
         CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), 'TRANSFER_CERTIFICATE', 'Transfer Certificate', 1, 'Transfer Certificate',
           '<h1>Transfer Certificate</h1><p>This certifies that <strong>{{Name}}</strong> ({{Number}}) studied at {{Branch}}.</p><p>Date: {{Date}}</p>',
           1, 1, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
    FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM LifecycleDocumentTemplates d WHERE d.TenantId = t.Id AND d.Code = 'TRANSFER_CERTIFICATE');

    INSERT INTO LifecycleDocumentTemplates
        (Id, Code, Name, OwnerType, DocumentType, HtmlTemplate, Version, IsActive, RequiresSignature,
         CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), 'GRADUATION_CERTIFICATE', 'Graduation Certificate', 1, 'Graduation Certificate',
           '<h1>Graduation Certificate</h1><p><strong>{{Name}}</strong> ({{Number}}) has fulfilled the graduation requirements of {{Branch}}.</p><p>Date: {{Date}}</p>',
           1, 1, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
    FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM LifecycleDocumentTemplates d WHERE d.TenantId = t.Id AND d.Code = 'GRADUATION_CERTIFICATE');

    INSERT INTO ProfileDocumentPolicies
        (Id, OwnerType, DocumentType, AllowedExtensions, MaximumBytes, RequiresVirusScan, RequiresVerification,
         RetentionDays, ExpiryDays, IsActive, CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), 1, 'Lifecycle', '.pdf,.png,.jpg,.jpeg,.doc,.docx', 10485760, 1, 1,
           2555, NULL, 1, SYSUTCDATETIME(), 0, NEWID(), t.Id
    FROM Tenants t WHERE t.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM ProfileDocumentPolicies p WHERE p.TenantId = t.Id AND p.OwnerType = 1 AND p.DocumentType = 'Lifecycle');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    ALTER TABLE [ProfileDocuments] ADD CONSTRAINT [FK_ProfileDocuments_ProfileDocumentPolicies_ProfileDocumentPolicyId] FOREIGN KEY ([ProfileDocumentPolicyId]) REFERENCES [ProfileDocumentPolicies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260930000851_StudentTeacherLifecycleDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260930000851_StudentTeacherLifecycleDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionInterviews] ADD [CalendarProvider] nvarchar(80) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionInterviews] ADD [ExternalCalendarEventId] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [ReplacedByDocumentId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [ReplacesDocumentId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [ReviewFeedback] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD [ReviewStatus] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [ApplicantAccountId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [ApplicationSource] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [FeeDueOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [LastSavedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [OfferExpiresOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [ReAdmissionStudentProfileId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [SubmittedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD [WaitlistPosition] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE TABLE [AdmissionApplicantAccounts] (
        [Id] uniqueidentifier NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [DisplayName] nvarchar(150) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Status] int NOT NULL,
        [FailedLoginCount] int NOT NULL,
        [LockoutEnd] datetime2 NULL,
        [LastLoginOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AdmissionApplicantAccounts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE TABLE [AdmissionAutomationLogs] (
        [Id] uniqueidentifier NOT NULL,
        [AdmissionApplicationId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        [Message] nvarchar(1000) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AdmissionAutomationLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionAutomationLogs_AdmissionApplications_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [AdmissionApplications] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE TABLE [AdmissionInterviewerAvailabilities] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [InterviewerUserId] uniqueidentifier NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartsAt] time NOT NULL,
        [EndsAt] time NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveUntil] date NULL,
        [Location] nvarchar(180) NULL,
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
        CONSTRAINT [PK_AdmissionInterviewerAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdmissionInterviewerAvailabilities_AspNetUsers_InterviewerUserId] FOREIGN KEY ([InterviewerUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AdmissionInterviewerAvailabilities_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE TABLE [BranchAdmissionSettings] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ApplicationNumberFormat] nvarchar(100) NOT NULL,
        [AdmissionNumberFormat] nvarchar(100) NOT NULL,
        [NextApplicationSequence] int NOT NULL,
        [NextAdmissionSequence] int NOT NULL,
        [OfferValidityDays] int NOT NULL,
        [FeeDeadlineDays] int NOT NULL,
        [AutoPromoteWaitlist] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BranchAdmissionSettings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchAdmissionSettings_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AdmissionDocuments_ReplacesDocumentId] ON [AdmissionDocuments] ([ReplacesDocumentId]) WHERE [ReplacesDocumentId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_ApplicantAccountId] ON [AdmissionApplications] ([ApplicantAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_ReAdmissionStudentProfileId] ON [AdmissionApplications] ([ReAdmissionStudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_TenantId_ApplicantAccountId_Status] ON [AdmissionApplications] ([TenantId], [ApplicantAccountId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionApplications_TenantId_BranchId_CourseId_WaitlistPosition] ON [AdmissionApplications] ([TenantId], [BranchId], [CourseId], [WaitlistPosition]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdmissionApplicantAccounts_TenantId_Email] ON [AdmissionApplicantAccounts] ([TenantId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionAutomationLogs_AdmissionApplicationId] ON [AdmissionAutomationLogs] ([AdmissionApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionAutomationLogs_TenantId_AdmissionApplicationId_OccurredOn] ON [AdmissionAutomationLogs] ([TenantId], [AdmissionApplicationId], [OccurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviewerAvailabilities_BranchId] ON [AdmissionInterviewerAvailabilities] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviewerAvailabilities_InterviewerUserId] ON [AdmissionInterviewerAvailabilities] ([InterviewerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_AdmissionInterviewerAvailabilities_TenantId_InterviewerUserId_DayOfWeek_StartsAt_EndsAt] ON [AdmissionInterviewerAvailabilities] ([TenantId], [InterviewerUserId], [DayOfWeek], [StartsAt], [EndsAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE INDEX [IX_BranchAdmissionSettings_BranchId] ON [BranchAdmissionSettings] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BranchAdmissionSettings_TenantId_BranchId] ON [BranchAdmissionSettings] ([TenantId], [BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    UPDATE AdmissionApplications
    SET ApplicationSource = COALESCE(ApplicationSource, 'Administrative entry'),
        SubmittedOn = COALESCE(SubmittedOn, DATEADD(day, DATEDIFF(day, 0, AppliedOn), 0)),
        LastSavedOn = COALESCE(LastSavedOn, CreatedOn)
    WHERE Status <> 9;

    UPDATE AdmissionDocuments SET ReviewStatus = CASE WHEN IsVerified = 1 THEN 2 ELSE 1 END;

    INSERT INTO BranchAdmissionSettings
        (Id, BranchId, ApplicationNumberFormat, AdmissionNumberFormat, NextApplicationSequence, NextAdmissionSequence,
         OfferValidityDays, FeeDeadlineDays, AutoPromoteWaitlist, CreatedOn, IsDeleted, ConcurrencyToken, TenantId)
    SELECT NEWID(), b.Id, 'APP-{YYYY}-{SEQ:0000}', 'ADM-{YYYY}-{SEQ:0000}',
           ISNULL((SELECT COUNT(1) + 1 FROM AdmissionApplications a WHERE a.TenantId = b.TenantId AND a.BranchId = b.Id), 1),
           ISNULL((SELECT COUNT(1) + 1 FROM StudentProfiles s WHERE s.TenantId = b.TenantId AND s.BranchId = b.Id), 1),
           7, 7, 1, SYSUTCDATETIME(), 0, NEWID(), b.TenantId
    FROM Branches b
    WHERE b.IsDeleted = 0 AND NOT EXISTS
        (SELECT 1 FROM BranchAdmissionSettings x WHERE x.TenantId = b.TenantId AND x.BranchId = b.Id);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD CONSTRAINT [FK_AdmissionApplications_AdmissionApplicantAccounts_ApplicantAccountId] FOREIGN KEY ([ApplicantAccountId]) REFERENCES [AdmissionApplicantAccounts] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionApplications] ADD CONSTRAINT [FK_AdmissionApplications_StudentProfiles_ReAdmissionStudentProfileId] FOREIGN KEY ([ReAdmissionStudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    ALTER TABLE [AdmissionDocuments] ADD CONSTRAINT [FK_AdmissionDocuments_AdmissionDocuments_ReplacesDocumentId] FOREIGN KEY ([ReplacesDocumentId]) REFERENCES [AdmissionDocuments] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261009213756_AdmissionsRefinements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261009213756_AdmissionsRefinements', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceAlertSchedules] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [ThresholdPercentage] decimal(5,2) NOT NULL,
        [LookbackDays] int NOT NULL,
        [Channel] int NOT NULL,
        [IsActive] bit NOT NULL,
        [NextRunOn] datetime2 NOT NULL,
        [LastRunOn] datetime2 NULL,
        [RunEveryHours] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceAlertSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceAlertSchedules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceCondonationRequests] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [EvidenceFileName] nvarchar(255) NULL,
        [EvidenceStoragePath] nvarchar(500) NULL,
        [EvidenceContentType] nvarchar(120) NULL,
        [Status] int NOT NULL,
        [RequestedByUserId] uniqueidentifier NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [ReviewNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceCondonationRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceCondonationRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCondonationRequests_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceCondonationRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceDevices] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [DeviceIdentifier] nvarchar(120) NOT NULL,
        [DeviceType] int NOT NULL,
        [EndpointUrl] nvarchar(500) NULL,
        [ApiKeyProtected] nvarchar(500) NULL,
        [LastSynchronizedOn] datetime2 NULL,
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
        CONSTRAINT [PK_AttendanceDevices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceDevices_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceLeaveReconciliations] (
        [Id] uniqueidentifier NOT NULL,
        [LeaveApplicationId] uniqueidentifier NOT NULL,
        [AttendanceRecordId] uniqueidentifier NOT NULL,
        [PreviousStatus] int NOT NULL,
        [AppliedStatus] int NOT NULL,
        [ReconciledOn] datetime2 NOT NULL,
        [ReconciledByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceLeaveReconciliations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceLeaveReconciliations_AspNetUsers_ReconciledByUserId] FOREIGN KEY ([ReconciledByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceLeaveReconciliations_AttendanceRecords_AttendanceRecordId] FOREIGN KEY ([AttendanceRecordId]) REFERENCES [AttendanceRecords] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceLeaveReconciliations_LeaveApplications_LeaveApplicationId] FOREIGN KEY ([LeaveApplicationId]) REFERENCES [LeaveApplications] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [BusAttendanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [TransportRouteId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NULL,
        [TransportRouteStopId] uniqueidentifier NULL,
        [AttendanceDate] date NOT NULL,
        [Status] int NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [RecordedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_BusAttendanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BusAttendanceRecords_AspNetUsers_RecordedByUserId] FOREIGN KEY ([RecordedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusAttendanceRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusAttendanceRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusAttendanceRecords_TransportRouteStops_TransportRouteStopId] FOREIGN KEY ([TransportRouteStopId]) REFERENCES [TransportRouteStops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusAttendanceRecords_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BusAttendanceRecords_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [ParentAbsenceExplanations] (
        [Id] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [AttendanceRecordId] uniqueidentifier NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [Explanation] nvarchar(1000) NOT NULL,
        [Status] int NOT NULL,
        [SubmittedByUserId] uniqueidentifier NOT NULL,
        [SubmittedOn] datetime2 NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [ReviewNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ParentAbsenceExplanations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ParentAbsenceExplanations_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentAbsenceExplanations_AspNetUsers_SubmittedByUserId] FOREIGN KEY ([SubmittedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentAbsenceExplanations_AttendanceRecords_AttendanceRecordId] FOREIGN KEY ([AttendanceRecordId]) REFERENCES [AttendanceRecords] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ParentAbsenceExplanations_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [TeacherAttendanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherProfileId] uniqueidentifier NOT NULL,
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
        CONSTRAINT [PK_TeacherAttendanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherAttendanceRecords_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherAttendanceRecords_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceDeviceImportBatches] (
        [Id] uniqueidentifier NOT NULL,
        [AttendanceDeviceId] uniqueidentifier NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [Status] int NOT NULL,
        [TotalRows] int NOT NULL,
        [ImportedRows] int NOT NULL,
        [RejectedRows] int NOT NULL,
        [ErrorSummary] nvarchar(4000) NULL,
        [UploadedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceDeviceImportBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceDeviceImportBatches_AttendanceDevices_AttendanceDeviceId] FOREIGN KEY ([AttendanceDeviceId]) REFERENCES [AttendanceDevices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE TABLE [AttendanceDevicePunches] (
        [Id] uniqueidentifier NOT NULL,
        [AttendanceDeviceId] uniqueidentifier NOT NULL,
        [AttendanceDeviceImportBatchId] uniqueidentifier NOT NULL,
        [ExternalRecordId] nvarchar(160) NOT NULL,
        [PersonCode] nvarchar(80) NOT NULL,
        [PunchedOn] datetime2 NOT NULL,
        [Direction] int NOT NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [StaffProfileId] uniqueidentifier NULL,
        [TeacherProfileId] uniqueidentifier NULL,
        [IsProcessed] bit NOT NULL,
        [ProcessingNotes] nvarchar(1000) NULL,
        [RawPayload] nvarchar(4000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttendanceDevicePunches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttendanceDevicePunches_AttendanceDeviceImportBatches_AttendanceDeviceImportBatchId] FOREIGN KEY ([AttendanceDeviceImportBatchId]) REFERENCES [AttendanceDeviceImportBatches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceDevicePunches_AttendanceDevices_AttendanceDeviceId] FOREIGN KEY ([AttendanceDeviceId]) REFERENCES [AttendanceDevices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceDevicePunches_StaffProfiles_StaffProfileId] FOREIGN KEY ([StaffProfileId]) REFERENCES [StaffProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceDevicePunches_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceDevicePunches_TeacherProfiles_TeacherProfileId] FOREIGN KEY ([TeacherProfileId]) REFERENCES [TeacherProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceAlertSchedules_BranchId] ON [AttendanceAlertSchedules] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceAlertSchedules_IsActive_NextRunOn] ON [AttendanceAlertSchedules] ([IsActive], [NextRunOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceCondonationRequests_RequestedByUserId] ON [AttendanceCondonationRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceCondonationRequests_ReviewedByUserId] ON [AttendanceCondonationRequests] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceCondonationRequests_StudentProfileId] ON [AttendanceCondonationRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceCondonationRequests_TenantId_StudentProfileId_Status] ON [AttendanceCondonationRequests] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDeviceImportBatches_AttendanceDeviceId] ON [AttendanceDeviceImportBatches] ([AttendanceDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDeviceImportBatches_TenantId_AttendanceDeviceId_UploadedOn] ON [AttendanceDeviceImportBatches] ([TenantId], [AttendanceDeviceId], [UploadedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_AttendanceDeviceId] ON [AttendanceDevicePunches] ([AttendanceDeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_AttendanceDeviceImportBatchId] ON [AttendanceDevicePunches] ([AttendanceDeviceImportBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_StaffProfileId] ON [AttendanceDevicePunches] ([StaffProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_StudentProfileId] ON [AttendanceDevicePunches] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_TeacherProfileId] ON [AttendanceDevicePunches] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttendanceDevicePunches_TenantId_AttendanceDeviceId_ExternalRecordId] ON [AttendanceDevicePunches] ([TenantId], [AttendanceDeviceId], [ExternalRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevicePunches_TenantId_PersonCode_PunchedOn] ON [AttendanceDevicePunches] ([TenantId], [PersonCode], [PunchedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceDevices_BranchId] ON [AttendanceDevices] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttendanceDevices_TenantId_DeviceIdentifier] ON [AttendanceDevices] ([TenantId], [DeviceIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceLeaveReconciliations_AttendanceRecordId] ON [AttendanceLeaveReconciliations] ([AttendanceRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceLeaveReconciliations_LeaveApplicationId] ON [AttendanceLeaveReconciliations] ([LeaveApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_AttendanceLeaveReconciliations_ReconciledByUserId] ON [AttendanceLeaveReconciliations] ([ReconciledByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttendanceLeaveReconciliations_TenantId_LeaveApplicationId_AttendanceRecordId] ON [AttendanceLeaveReconciliations] ([TenantId], [LeaveApplicationId], [AttendanceRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_BranchId] ON [BusAttendanceRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_RecordedByUserId] ON [BusAttendanceRecords] ([RecordedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_StudentProfileId] ON [BusAttendanceRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_TenantId_StudentProfileId_AttendanceDate_Status] ON [BusAttendanceRecords] ([TenantId], [StudentProfileId], [AttendanceDate], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_TransportRouteId] ON [BusAttendanceRecords] ([TransportRouteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_TransportRouteStopId] ON [BusAttendanceRecords] ([TransportRouteStopId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_BusAttendanceRecords_VehicleId] ON [BusAttendanceRecords] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_ParentAbsenceExplanations_AttendanceRecordId] ON [ParentAbsenceExplanations] ([AttendanceRecordId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_ParentAbsenceExplanations_ReviewedByUserId] ON [ParentAbsenceExplanations] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_ParentAbsenceExplanations_StudentProfileId] ON [ParentAbsenceExplanations] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_ParentAbsenceExplanations_SubmittedByUserId] ON [ParentAbsenceExplanations] ([SubmittedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_ParentAbsenceExplanations_TenantId_StudentProfileId_Status] ON [ParentAbsenceExplanations] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAttendanceRecords_BranchId] ON [TeacherAttendanceRecords] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE INDEX [IX_TeacherAttendanceRecords_TeacherProfileId] ON [TeacherAttendanceRecords] ([TeacherProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TeacherAttendanceRecords_TenantId_TeacherProfileId_AttendanceDate] ON [TeacherAttendanceRecords] ([TenantId], [TeacherProfileId], [AttendanceDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016001247_AttendanceTimetableDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016001247_AttendanceTimetableDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExamAttendanceRecords] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [RecordedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_ExamAttendanceRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamAttendanceRecords_AspNetUsers_RecordedByUserId] FOREIGN KEY ([RecordedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamAttendanceRecords_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamAttendanceRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExamHallTickets] (
        [Id] uniqueidentifier NOT NULL,
        [ExamId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [TicketNumber] nvarchar(60) NOT NULL,
        [GeneratedOn] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExamHallTickets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamHallTickets_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamHallTickets_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExamMalpracticeRecords] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Severity] int NOT NULL,
        [Status] int NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [ActionTaken] nvarchar(1000) NULL,
        [ReportedOn] datetime2 NOT NULL,
        [ReportedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExamMalpracticeRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamMalpracticeRecords_AspNetUsers_ReportedByUserId] FOREIGN KEY ([ReportedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamMalpracticeRecords_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamMalpracticeRecords_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExamSeatingPlans] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [GeneratedOn] datetime2 NOT NULL,
        [GeneratedByUserId] uniqueidentifier NULL,
        [CandidateCount] int NOT NULL,
        [Instructions] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExamSeatingPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamSeatingPlans_AspNetUsers_GeneratedByUserId] FOREIGN KEY ([GeneratedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSeatingPlans_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExternalGradingRules] (
        [Id] uniqueidentifier NOT NULL,
        [GradingSchemeId] uniqueidentifier NOT NULL,
        [AuthorityName] nvarchar(120) NOT NULL,
        [AuthorityCode] nvarchar(50) NULL,
        [MaximumGraceMarks] decimal(9,2) NOT NULL,
        [MaximumGracePercentage] decimal(5,2) NOT NULL,
        [GraceOnlyForPassing] bit NOT NULL,
        [ApplyBestOfSubjects] bit NOT NULL,
        [BestOfSubjectCount] int NULL,
        [RulesJson] nvarchar(2000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExternalGradingRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExternalGradingRules_GradingSchemes_GradingSchemeId] FOREIGN KEY ([GradingSchemeId]) REFERENCES [GradingSchemes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarkingSchemeReleases] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionPaperVersionId] uniqueidentifier NOT NULL,
        [ReleasedToUserId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [ReleasedOn] datetime2 NULL,
        [ExpiresOn] datetime2 NULL,
        [ReleasedByUserId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_MarkingSchemeReleases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarkingSchemeReleases_AspNetUsers_ReleasedByUserId] FOREIGN KEY ([ReleasedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarkingSchemeReleases_AspNetUsers_ReleasedToUserId] FOREIGN KEY ([ReleasedToUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarkingSchemeReleases_QuestionPaperVersions_QuestionPaperVersionId] FOREIGN KEY ([QuestionPaperVersionId]) REFERENCES [QuestionPaperVersions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarkRubrics] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
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
        CONSTRAINT [PK_MarkRubrics] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarkRubrics_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarksImportBatches] (
        [Id] uniqueidentifier NOT NULL,
        [ExamScheduleId] uniqueidentifier NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [Status] int NOT NULL,
        [TotalRows] int NOT NULL,
        [ValidRows] int NOT NULL,
        [ImportedRows] int NOT NULL,
        [ErrorRows] int NOT NULL,
        [UploadedByUserId] uniqueidentifier NOT NULL,
        [UploadedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MarksImportBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarksImportBatches_AspNetUsers_UploadedByUserId] FOREIGN KEY ([UploadedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarksImportBatches_ExamSchedules_ExamScheduleId] FOREIGN KEY ([ExamScheduleId]) REFERENCES [ExamSchedules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [QuestionPaperReviewPolicies] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [RequiredReviewerCount] int NOT NULL,
        [RequiredApprovalCount] int NOT NULL,
        [RejectOnAnyRejection] bit NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_QuestionPaperReviewPolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionPaperReviewPolicies_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ReportCardTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [PrimaryColor] nvarchar(20) NOT NULL,
        [HeaderText] nvarchar(250) NULL,
        [FooterText] nvarchar(500) NULL,
        [ShowRank] bit NOT NULL,
        [ShowGradePoint] bit NOT NULL,
        [ShowAttendance] bit NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ReportCardTemplates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReportCardTemplates_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ReportCardTemplates_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ResultReviewRequests] (
        [Id] uniqueidentifier NOT NULL,
        [MarkEntryId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [Status] int NOT NULL,
        [OriginalMarks] decimal(9,2) NOT NULL,
        [RevisedMarks] decimal(9,2) NULL,
        [RequestedByUserId] uniqueidentifier NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [ReviewedByUserId] uniqueidentifier NULL,
        [ReviewedOn] datetime2 NULL,
        [ReviewNotes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ResultReviewRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultReviewRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultReviewRequests_AspNetUsers_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultReviewRequests_MarkEntries_MarkEntryId] FOREIGN KEY ([MarkEntryId]) REFERENCES [MarkEntries] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultReviewRequests_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [ExamSeatAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [ExamSeatingPlanId] uniqueidentifier NOT NULL,
        [StudentProfileId] uniqueidentifier NOT NULL,
        [RoomId] uniqueidentifier NOT NULL,
        [SeatNumber] nvarchar(30) NOT NULL,
        [CandidateStatus] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExamSeatAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExamSeatAssignments_ExamSeatingPlans_ExamSeatingPlanId] FOREIGN KEY ([ExamSeatingPlanId]) REFERENCES [ExamSeatingPlans] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSeatAssignments_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamSeatAssignments_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarkRubricCriteria] (
        [Id] uniqueidentifier NOT NULL,
        [MarkRubricId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [MaximumMarks] decimal(9,2) NOT NULL,
        [SortOrder] int NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MarkRubricCriteria] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarkRubricCriteria_MarkRubrics_MarkRubricId] FOREIGN KEY ([MarkRubricId]) REFERENCES [MarkRubrics] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarksImportRows] (
        [Id] uniqueidentifier NOT NULL,
        [MarksImportBatchId] uniqueidentifier NOT NULL,
        [RowNumber] int NOT NULL,
        [StudentIdentifier] nvarchar(50) NOT NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [MarksObtained] decimal(9,2) NULL,
        [IsAbsent] bit NOT NULL,
        [IsValid] bit NOT NULL,
        [IsImported] bit NOT NULL,
        [ValidationMessage] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MarksImportRows] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarksImportRows_MarksImportBatches_MarksImportBatchId] FOREIGN KEY ([MarksImportBatchId]) REFERENCES [MarksImportBatches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarksImportRows_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE TABLE [MarkRubricScores] (
        [Id] uniqueidentifier NOT NULL,
        [MarkEntryId] uniqueidentifier NOT NULL,
        [MarkRubricCriterionId] uniqueidentifier NOT NULL,
        [MarksAwarded] decimal(9,2) NOT NULL,
        [Feedback] nvarchar(500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MarkRubricScores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarkRubricScores_MarkEntries_MarkEntryId] FOREIGN KEY ([MarkEntryId]) REFERENCES [MarkEntries] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarkRubricScores_MarkRubricCriteria_MarkRubricCriterionId] FOREIGN KEY ([MarkRubricCriterionId]) REFERENCES [MarkRubricCriteria] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamAttendanceRecords_ExamScheduleId] ON [ExamAttendanceRecords] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamAttendanceRecords_RecordedByUserId] ON [ExamAttendanceRecords] ([RecordedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamAttendanceRecords_StudentProfileId] ON [ExamAttendanceRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamAttendanceRecords_TenantId_ExamScheduleId_StudentProfileId] ON [ExamAttendanceRecords] ([TenantId], [ExamScheduleId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamHallTickets_ExamId] ON [ExamHallTickets] ([ExamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamHallTickets_StudentProfileId] ON [ExamHallTickets] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamHallTickets_TenantId_ExamId_StudentProfileId] ON [ExamHallTickets] ([TenantId], [ExamId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamHallTickets_TenantId_TicketNumber] ON [ExamHallTickets] ([TenantId], [TicketNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamMalpracticeRecords_ExamScheduleId] ON [ExamMalpracticeRecords] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamMalpracticeRecords_ReportedByUserId] ON [ExamMalpracticeRecords] ([ReportedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamMalpracticeRecords_StudentProfileId] ON [ExamMalpracticeRecords] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamMalpracticeRecords_TenantId_ExamScheduleId_StudentProfileId_ReportedOn] ON [ExamMalpracticeRecords] ([TenantId], [ExamScheduleId], [StudentProfileId], [ReportedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamSeatAssignments_ExamSeatingPlanId_RoomId_SeatNumber] ON [ExamSeatAssignments] ([ExamSeatingPlanId], [RoomId], [SeatNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamSeatAssignments_ExamSeatingPlanId_StudentProfileId] ON [ExamSeatAssignments] ([ExamSeatingPlanId], [StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamSeatAssignments_RoomId] ON [ExamSeatAssignments] ([RoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamSeatAssignments_StudentProfileId] ON [ExamSeatAssignments] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamSeatingPlans_ExamScheduleId] ON [ExamSeatingPlans] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamSeatingPlans_GeneratedByUserId] ON [ExamSeatingPlans] ([GeneratedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExamSeatingPlans_TenantId_ExamScheduleId_GeneratedOn] ON [ExamSeatingPlans] ([TenantId], [ExamScheduleId], [GeneratedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ExternalGradingRules_GradingSchemeId] ON [ExternalGradingRules] ([GradingSchemeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ExternalGradingRules_TenantId_GradingSchemeId_AuthorityCode] ON [ExternalGradingRules] ([TenantId], [GradingSchemeId], [AuthorityCode]) WHERE [AuthorityCode] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarkingSchemeReleases_QuestionPaperVersionId] ON [MarkingSchemeReleases] ([QuestionPaperVersionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarkingSchemeReleases_ReleasedByUserId] ON [MarkingSchemeReleases] ([ReleasedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarkingSchemeReleases_ReleasedToUserId] ON [MarkingSchemeReleases] ([ReleasedToUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarkingSchemeReleases_TenantId_QuestionPaperVersionId_ReleasedToUserId] ON [MarkingSchemeReleases] ([TenantId], [QuestionPaperVersionId], [ReleasedToUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarkRubricCriteria_MarkRubricId_SortOrder] ON [MarkRubricCriteria] ([MarkRubricId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarkRubrics_ExamScheduleId] ON [MarkRubrics] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarkRubrics_TenantId_ExamScheduleId_Name] ON [MarkRubrics] ([TenantId], [ExamScheduleId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarkRubricScores_MarkEntryId_MarkRubricCriterionId] ON [MarkRubricScores] ([MarkEntryId], [MarkRubricCriterionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarkRubricScores_MarkRubricCriterionId] ON [MarkRubricScores] ([MarkRubricCriterionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarksImportBatches_ExamScheduleId] ON [MarksImportBatches] ([ExamScheduleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarksImportBatches_TenantId_ExamScheduleId_UploadedOn] ON [MarksImportBatches] ([TenantId], [ExamScheduleId], [UploadedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarksImportBatches_UploadedByUserId] ON [MarksImportBatches] ([UploadedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MarksImportRows_MarksImportBatchId_RowNumber] ON [MarksImportRows] ([MarksImportBatchId], [RowNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_MarksImportRows_StudentProfileId] ON [MarksImportRows] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_QuestionPaperReviewPolicies_BranchId] ON [QuestionPaperReviewPolicies] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_QuestionPaperReviewPolicies_TenantId_BranchId_Name] ON [QuestionPaperReviewPolicies] ([TenantId], [BranchId], [Name]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ReportCardTemplates_BranchId] ON [ReportCardTemplates] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ReportCardTemplates_CourseId] ON [ReportCardTemplates] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ReportCardTemplates_TenantId_BranchId_Name] ON [ReportCardTemplates] ([TenantId], [BranchId], [Name]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ResultReviewRequests_MarkEntryId] ON [ResultReviewRequests] ([MarkEntryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ResultReviewRequests_RequestedByUserId] ON [ResultReviewRequests] ([RequestedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ResultReviewRequests_ReviewedByUserId] ON [ResultReviewRequests] ([ReviewedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ResultReviewRequests_StudentProfileId] ON [ResultReviewRequests] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    CREATE INDEX [IX_ResultReviewRequests_TenantId_StudentProfileId_Status] ON [ResultReviewRequests] ([TenantId], [StudentProfileId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016233800_ExamsResultsQuestionBankDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016233800_ExamsResultsQuestionBankDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [AccountingPeriods] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(80) NOT NULL,
        [StartsOn] date NOT NULL,
        [EndsOn] date NOT NULL,
        [Status] int NOT NULL,
        [ClosedOn] datetime2 NULL,
        [ClosedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AccountingPeriods] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [ChartOfAccounts] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [ParentAccountId] uniqueidentifier NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [IsControlAccount] bit NOT NULL,
        [AllowPosting] bit NOT NULL,
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
        CONSTRAINT [PK_ChartOfAccounts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChartOfAccounts_ChartOfAccounts_ParentAccountId] FOREIGN KEY ([ParentAccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [ConcessionRules] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [CourseId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Type] int NOT NULL,
        [Value] decimal(12,2) NOT NULL,
        [MaximumAmount] decimal(12,2) NULL,
        [RequiresApproval] bit NOT NULL,
        [Priority] int NOT NULL,
        [ConditionsJson] nvarchar(max) NOT NULL,
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
        CONSTRAINT [PK_ConcessionRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [FeeInstallmentPlans] (
        [Id] uniqueidentifier NOT NULL,
        [FeeStructureId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
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
        CONSTRAINT [PK_FeeInstallmentPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeInstallmentPlans_FeeStructures_FeeStructureId] FOREIGN KEY ([FeeStructureId]) REFERENCES [FeeStructures] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [FinanceNumberSequences] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [DocumentType] nvarchar(40) NOT NULL,
        [Format] nvarchar(80) NOT NULL,
        [NextNumber] bigint NOT NULL,
        [FinancialYearStartMonth] int NOT NULL,
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
        CONSTRAINT [PK_FinanceNumberSequences] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [JournalEntries] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [JournalNumber] nvarchar(60) NOT NULL,
        [PostingDate] date NOT NULL,
        [VoucherType] int NOT NULL,
        [Narration] nvarchar(250) NOT NULL,
        [Status] int NOT NULL,
        [SourceEntityId] uniqueidentifier NULL,
        [SourceType] nvarchar(80) NULL,
        [ReversesJournalEntryId] uniqueidentifier NULL,
        [PostedOn] datetime2 NULL,
        [PostedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_JournalEntries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [LateFeeRules] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [FeeStructureId] uniqueidentifier NULL,
        [Name] nvarchar(120) NOT NULL,
        [Method] int NOT NULL,
        [Value] decimal(12,2) NOT NULL,
        [MaximumAmount] decimal(12,2) NULL,
        [GraceDays] int NOT NULL,
        [Compound] bit NOT NULL,
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
        CONSTRAINT [PK_LateFeeRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [PaymentGatewayWebhookEvents] (
        [Id] uniqueidentifier NOT NULL,
        [Provider] nvarchar(40) NOT NULL,
        [EventId] nvarchar(160) NOT NULL,
        [EventType] nvarchar(80) NOT NULL,
        [Status] int NOT NULL,
        [ReceivedOn] datetime2 NOT NULL,
        [ProcessedOn] datetime2 NULL,
        [PayloadHash] nvarchar(4000) NOT NULL,
        [Error] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PaymentGatewayWebhookEvents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [TaxCodes] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Type] int NOT NULL,
        [Rate] decimal(7,4) NOT NULL,
        [HsnSacCode] nvarchar(30) NULL,
        [Inclusive] bit NOT NULL,
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
        CONSTRAINT [PK_TaxCodes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [BankAccounts] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [ChartOfAccountId] uniqueidentifier NOT NULL,
        [BankName] nvarchar(120) NOT NULL,
        [AccountNumberMasked] nvarchar(40) NOT NULL,
        [IfscCode] nvarchar(20) NULL,
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
        CONSTRAINT [PK_BankAccounts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BankAccounts_ChartOfAccounts_ChartOfAccountId] FOREIGN KEY ([ChartOfAccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [FeeInstallments] (
        [Id] uniqueidentifier NOT NULL,
        [FeeInstallmentPlanId] uniqueidentifier NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Sequence] int NOT NULL,
        [DueDate] date NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [Percentage] decimal(7,4) NOT NULL,
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
        CONSTRAINT [PK_FeeInstallments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeInstallments_FeeInstallmentPlans_FeeInstallmentPlanId] FOREIGN KEY ([FeeInstallmentPlanId]) REFERENCES [FeeInstallmentPlans] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [JournalLines] (
        [Id] uniqueidentifier NOT NULL,
        [JournalEntryId] uniqueidentifier NOT NULL,
        [ChartOfAccountId] uniqueidentifier NOT NULL,
        [Debit] decimal(14,2) NOT NULL,
        [Credit] decimal(14,2) NOT NULL,
        [Description] nvarchar(250) NULL,
        [StudentProfileId] uniqueidentifier NULL,
        [TaxCodeId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_JournalLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JournalLines_ChartOfAccounts_ChartOfAccountId] FOREIGN KEY ([ChartOfAccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JournalLines_JournalEntries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [JournalEntries] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JournalLines_TaxCodes_TaxCodeId] FOREIGN KEY ([TaxCodeId]) REFERENCES [TaxCodes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE TABLE [BankStatementLines] (
        [Id] uniqueidentifier NOT NULL,
        [BankAccountId] uniqueidentifier NOT NULL,
        [TransactionDate] date NOT NULL,
        [Description] nvarchar(180) NOT NULL,
        [Reference] nvarchar(120) NULL,
        [Debit] decimal(14,2) NOT NULL,
        [Credit] decimal(14,2) NOT NULL,
        [Status] int NOT NULL,
        [MatchedJournalLineId] uniqueidentifier NULL,
        [MatchedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BankStatementLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BankStatementLines_BankAccounts_BankAccountId] FOREIGN KEY ([BankAccountId]) REFERENCES [BankAccounts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AccountingPeriods_TenantId_BranchId_StartsOn_EndsOn] ON [AccountingPeriods] ([TenantId], [BranchId], [StartsOn], [EndsOn]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_BankAccounts_ChartOfAccountId] ON [BankAccounts] ([ChartOfAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BankAccounts_TenantId_BranchId_AccountNumberMasked] ON [BankAccounts] ([TenantId], [BranchId], [AccountNumberMasked]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_BankStatementLines_BankAccountId] ON [BankStatementLines] ([BankAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_ChartOfAccounts_ParentAccountId] ON [ChartOfAccounts] ([ParentAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ChartOfAccounts_TenantId_Code] ON [ChartOfAccounts] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ConcessionRules_TenantId_Code] ON [ConcessionRules] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_FeeInstallmentPlans_FeeStructureId] ON [FeeInstallmentPlans] ([FeeStructureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeeInstallmentPlans_TenantId_FeeStructureId_Name] ON [FeeInstallmentPlans] ([TenantId], [FeeStructureId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_FeeInstallments_FeeInstallmentPlanId] ON [FeeInstallments] ([FeeInstallmentPlanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_FinanceNumberSequences_TenantId_BranchId_DocumentType] ON [FinanceNumberSequences] ([TenantId], [BranchId], [DocumentType]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JournalEntries_TenantId_JournalNumber] ON [JournalEntries] ([TenantId], [JournalNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_JournalLines_ChartOfAccountId] ON [JournalLines] ([ChartOfAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_JournalLines_JournalEntryId] ON [JournalLines] ([JournalEntryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE INDEX [IX_JournalLines_TaxCodeId] ON [JournalLines] ([TaxCodeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PaymentGatewayWebhookEvents_TenantId_Provider_EventId] ON [PaymentGatewayWebhookEvents] ([TenantId], [Provider], [EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TaxCodes_TenantId_Code] ON [TaxCodes] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017203536_FinanceAccountingDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017203536_FinanceAccountingDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelVisitorLogs] ADD [ApprovedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelVisitorLogs] ADD [CheckedInByUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelVisitorLogs] ADD [CheckedOutByUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelVisitorLogs] ADD [RequestedVisitOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelVisitorLogs] ADD [Status] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelMaintenanceRequests] ADD [AssignedToUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelMaintenanceRequests] ADD [EscalationLevel] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelMaintenanceRequests] ADD [FirstResponseOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelMaintenanceRequests] ADD [SlaMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelFees] ADD [FeeInvoiceId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelClearanceRequests] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [VacationFrom] date NOT NULL,
        [ExpectedReturnOn] date NULL,
        [IsFinalCheckout] bit NOT NULL,
        [Status] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
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
        CONSTRAINT [PK_HostelClearanceRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelClearanceRequests_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelGatePasses] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [PlannedOutOn] datetime2 NOT NULL,
        [PlannedReturnOn] datetime2 NOT NULL,
        [ActualOutOn] datetime2 NULL,
        [ActualReturnOn] datetime2 NULL,
        [Status] int NOT NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ApprovedOn] datetime2 NULL,
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
        CONSTRAINT [PK_HostelGatePasses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelGatePasses_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelMaintenanceEscalations] (
        [Id] uniqueidentifier NOT NULL,
        [HostelMaintenanceRequestId] uniqueidentifier NOT NULL,
        [Level] int NOT NULL,
        [EscalatedOn] datetime2 NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [EscalatedToUserId] uniqueidentifier NULL,
        [AcknowledgedOn] datetime2 NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelMaintenanceEscalations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelMaintenanceEscalations_HostelMaintenanceRequests_HostelMaintenanceRequestId] FOREIGN KEY ([HostelMaintenanceRequestId]) REFERENCES [HostelMaintenanceRequests] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelMessAttendances] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [AttendanceDate] date NOT NULL,
        [MealType] int NOT NULL,
        [Status] int NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [RecordedByUserId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelMessAttendances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelMessAttendances_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelMessBills] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NOT NULL,
        [PeriodStart] date NOT NULL,
        [PeriodEnd] date NOT NULL,
        [ChargeableMeals] int NOT NULL,
        [RatePerMeal] decimal(12,2) NOT NULL,
        [Amount] decimal(12,2) NOT NULL,
        [FeeInvoiceId] uniqueidentifier NULL,
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
        CONSTRAINT [PK_HostelMessBills] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelMessBills_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelMessBills_HostelAllocations_HostelAllocationId] FOREIGN KEY ([HostelAllocationId]) REFERENCES [HostelAllocations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelMessMenus] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelBlockId] uniqueidentifier NULL,
        [MenuDate] date NOT NULL,
        [MealType] int NOT NULL,
        [MenuItems] nvarchar(1000) NOT NULL,
        [IsVegetarian] bit NOT NULL,
        [AllergenNotes] nvarchar(500) NULL,
        [GuestPrice] decimal(12,2) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelMessMenus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelRoomInspections] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [HostelRoomId] uniqueidentifier NOT NULL,
        [ScheduledOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
        [Status] int NOT NULL,
        [InspectorUserId] uniqueidentifier NULL,
        [CleanlinessScore] int NOT NULL,
        [SafetyScore] int NOT NULL,
        [Findings] nvarchar(1500) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelRoomInspections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelRoomInspections_HostelRooms_HostelRoomId] FOREIGN KEY ([HostelRoomId]) REFERENCES [HostelRooms] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelClearanceItems] (
        [Id] uniqueidentifier NOT NULL,
        [HostelClearanceRequestId] uniqueidentifier NOT NULL,
        [Area] int NOT NULL,
        [IsCleared] bit NOT NULL,
        [ClearedByUserId] uniqueidentifier NULL,
        [ClearedOn] datetime2 NULL,
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
        CONSTRAINT [PK_HostelClearanceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelClearanceItems_HostelClearanceRequests_HostelClearanceRequestId] FOREIGN KEY ([HostelClearanceRequestId]) REFERENCES [HostelClearanceRequests] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE TABLE [HostelDamageAssessments] (
        [Id] uniqueidentifier NOT NULL,
        [HostelRoomInspectionId] uniqueidentifier NOT NULL,
        [HostelAllocationId] uniqueidentifier NULL,
        [Description] nvarchar(500) NOT NULL,
        [EstimatedCost] decimal(12,2) NOT NULL,
        [ChargedAmount] decimal(12,2) NOT NULL,
        [StudentLiable] bit NOT NULL,
        [FeeInvoiceId] uniqueidentifier NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HostelDamageAssessments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HostelDamageAssessments_HostelRoomInspections_HostelRoomInspectionId] FOREIGN KEY ([HostelRoomInspectionId]) REFERENCES [HostelRoomInspections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Description], [Name], [NormalizedName])
    VALUES (''f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1'', N''f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1'', N''Warden platform role'', N''Warden'', N''WARDEN'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Description', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelFees_FeeInvoiceId] ON [HostelFees] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelClearanceItems_HostelClearanceRequestId] ON [HostelClearanceItems] ([HostelClearanceRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelClearanceItems_TenantId_HostelClearanceRequestId_Area] ON [HostelClearanceItems] ([TenantId], [HostelClearanceRequestId], [Area]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelClearanceRequests_HostelAllocationId] ON [HostelClearanceRequests] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelClearanceRequests_TenantId_BranchId_Status] ON [HostelClearanceRequests] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelDamageAssessments_HostelRoomInspectionId] ON [HostelDamageAssessments] ([HostelRoomInspectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelGatePasses_HostelAllocationId] ON [HostelGatePasses] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelGatePasses_TenantId_BranchId_Status_PlannedReturnOn] ON [HostelGatePasses] ([TenantId], [BranchId], [Status], [PlannedReturnOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelMaintenanceEscalations_HostelMaintenanceRequestId] ON [HostelMaintenanceEscalations] ([HostelMaintenanceRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelMessAttendances_HostelAllocationId] ON [HostelMessAttendances] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelMessAttendances_TenantId_HostelAllocationId_AttendanceDate_MealType] ON [HostelMessAttendances] ([TenantId], [HostelAllocationId], [AttendanceDate], [MealType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelMessBills_FeeInvoiceId] ON [HostelMessBills] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelMessBills_HostelAllocationId] ON [HostelMessBills] ([HostelAllocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HostelMessBills_TenantId_HostelAllocationId_PeriodStart_PeriodEnd] ON [HostelMessBills] ([TenantId], [HostelAllocationId], [PeriodStart], [PeriodEnd]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HostelMessMenus_TenantId_BranchId_HostelBlockId_MenuDate_MealType] ON [HostelMessMenus] ([TenantId], [BranchId], [HostelBlockId], [MenuDate], [MealType]) WHERE [HostelBlockId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelRoomInspections_HostelRoomId] ON [HostelRoomInspections] ([HostelRoomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_HostelRoomInspections_TenantId_BranchId_Status_ScheduledOn] ON [HostelRoomInspections] ([TenantId], [BranchId], [Status], [ScheduledOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    ALTER TABLE [HostelFees] ADD CONSTRAINT [FK_HostelFees_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018222855_HostelOperationsDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018222855_HostelOperationsDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    ALTER TABLE [LibraryReservations] ADD [AvailabilityNotifiedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    ALTER TABLE [LibraryReservations] ADD [QueuePosition] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    ALTER TABLE [LibraryFineRecords] ADD [FeeInvoiceId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    ALTER TABLE [LibraryBookCopies] ADD [LastVerifiedOn] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryCirculationPolicies] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [MemberType] int NOT NULL,
        [MaxIssues] int NOT NULL,
        [LoanDays] int NOT NULL,
        [MaxRenewals] int NOT NULL,
        [GraceDays] int NOT NULL,
        [FinePerDay] decimal(12,2) NOT NULL,
        [MaximumFine] decimal(12,2) NOT NULL,
        [LostBookMultiplier] decimal(8,2) NOT NULL,
        [DamageChargePercent] decimal(8,2) NOT NULL,
        [ReservationHoldDays] int NOT NULL,
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
        CONSTRAINT [PK_LibraryCirculationPolicies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryDigitalResources] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookId] uniqueidentifier NULL,
        [Title] nvarchar(250) NOT NULL,
        [ResourceType] int NOT NULL,
        [ResourceUrl] nvarchar(1000) NOT NULL,
        [LicenseName] nvarchar(250) NULL,
        [LicenseExpiresOn] date NULL,
        [AccessLevel] int NOT NULL,
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
        CONSTRAINT [PK_LibraryDigitalResources] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryDigitalResources_LibraryBooks_LibraryBookId] FOREIGN KEY ([LibraryBookId]) REFERENCES [LibraryBooks] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryItemIncidents] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryBookCopyId] uniqueidentifier NOT NULL,
        [LibraryMemberId] uniqueidentifier NOT NULL,
        [LibraryBookIssueId] uniqueidentifier NULL,
        [Condition] int NOT NULL,
        [Details] nvarchar(1000) NOT NULL,
        [AssessedAmount] decimal(12,2) NOT NULL,
        [Status] int NOT NULL,
        [FeeInvoiceId] uniqueidentifier NULL,
        [ReportedOn] datetime2 NOT NULL,
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
        CONSTRAINT [PK_LibraryItemIncidents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryItemIncidents_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryItemIncidents_LibraryBookCopies_LibraryBookCopyId] FOREIGN KEY ([LibraryBookCopyId]) REFERENCES [LibraryBookCopies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryItemIncidents_LibraryBookIssues_LibraryBookIssueId] FOREIGN KEY ([LibraryBookIssueId]) REFERENCES [LibraryBookIssues] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryItemIncidents_LibraryMembers_LibraryMemberId] FOREIGN KEY ([LibraryMemberId]) REFERENCES [LibraryMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryStockVerifications] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [StartedOn] datetime2 NOT NULL,
        [CompletedOn] datetime2 NULL,
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
        CONSTRAINT [PK_LibraryStockVerifications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryVendors] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [Name] nvarchar(160) NOT NULL,
        [ContactPerson] nvarchar(120) NULL,
        [Email] nvarchar(254) NULL,
        [Phone] nvarchar(30) NULL,
        [Address] nvarchar(500) NULL,
        [TaxNumber] nvarchar(40) NULL,
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
        CONSTRAINT [PK_LibraryVendors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryStockVerificationItems] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryStockVerificationId] uniqueidentifier NOT NULL,
        [LibraryBookCopyId] uniqueidentifier NULL,
        [ScanCode] nvarchar(120) NOT NULL,
        [Result] int NOT NULL,
        [ScannedOn] datetime2 NOT NULL,
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
        CONSTRAINT [PK_LibraryStockVerificationItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryStockVerificationItems_LibraryBookCopies_LibraryBookCopyId] FOREIGN KEY ([LibraryBookCopyId]) REFERENCES [LibraryBookCopies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryStockVerificationItems_LibraryStockVerifications_LibraryStockVerificationId] FOREIGN KEY ([LibraryStockVerificationId]) REFERENCES [LibraryStockVerifications] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryPurchaseOrders] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryVendorId] uniqueidentifier NOT NULL,
        [OrderNumber] nvarchar(60) NOT NULL,
        [OrderDate] date NOT NULL,
        [ExpectedOn] date NULL,
        [Status] int NOT NULL,
        [TotalAmount] decimal(12,2) NOT NULL,
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
        CONSTRAINT [PK_LibraryPurchaseOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryPurchaseOrders_LibraryVendors_LibraryVendorId] FOREIGN KEY ([LibraryVendorId]) REFERENCES [LibraryVendors] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE TABLE [LibraryPurchaseOrderLines] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [LibraryPurchaseOrderId] uniqueidentifier NOT NULL,
        [LibraryBookId] uniqueidentifier NULL,
        [Title] nvarchar(250) NOT NULL,
        [Isbn] nvarchar(30) NULL,
        [Quantity] int NOT NULL,
        [ReceivedQuantity] int NOT NULL,
        [UnitPrice] decimal(12,2) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LibraryPurchaseOrderLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryPurchaseOrderLines_LibraryBooks_LibraryBookId] FOREIGN KEY ([LibraryBookId]) REFERENCES [LibraryBooks] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LibraryPurchaseOrderLines_LibraryPurchaseOrders_LibraryPurchaseOrderId] FOREIGN KEY ([LibraryPurchaseOrderId]) REFERENCES [LibraryPurchaseOrders] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryFineRecords_FeeInvoiceId] ON [LibraryFineRecords] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LibraryCirculationPolicies_TenantId_BranchId_MemberType] ON [LibraryCirculationPolicies] ([TenantId], [BranchId], [MemberType]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryDigitalResources_LibraryBookId] ON [LibraryDigitalResources] ([LibraryBookId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryDigitalResources_TenantId_BranchId_IsActive] ON [LibraryDigitalResources] ([TenantId], [BranchId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryItemIncidents_FeeInvoiceId] ON [LibraryItemIncidents] ([FeeInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryItemIncidents_LibraryBookCopyId] ON [LibraryItemIncidents] ([LibraryBookCopyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryItemIncidents_LibraryBookIssueId] ON [LibraryItemIncidents] ([LibraryBookIssueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryItemIncidents_LibraryMemberId] ON [LibraryItemIncidents] ([LibraryMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryItemIncidents_TenantId_BranchId_Status] ON [LibraryItemIncidents] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryPurchaseOrderLines_LibraryBookId] ON [LibraryPurchaseOrderLines] ([LibraryBookId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryPurchaseOrderLines_LibraryPurchaseOrderId] ON [LibraryPurchaseOrderLines] ([LibraryPurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryPurchaseOrders_LibraryVendorId] ON [LibraryPurchaseOrders] ([LibraryVendorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LibraryPurchaseOrders_TenantId_BranchId_OrderNumber] ON [LibraryPurchaseOrders] ([TenantId], [BranchId], [OrderNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryStockVerificationItems_LibraryBookCopyId] ON [LibraryStockVerificationItems] ([LibraryBookCopyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryStockVerificationItems_LibraryStockVerificationId] ON [LibraryStockVerificationItems] ([LibraryStockVerificationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LibraryStockVerificationItems_TenantId_LibraryStockVerificationId_ScanCode] ON [LibraryStockVerificationItems] ([TenantId], [LibraryStockVerificationId], [ScanCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE INDEX [IX_LibraryStockVerifications_TenantId_BranchId_Status] ON [LibraryStockVerifications] ([TenantId], [BranchId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LibraryVendors_TenantId_BranchId_Name] ON [LibraryVendors] ([TenantId], [BranchId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    ALTER TABLE [LibraryFineRecords] ADD CONSTRAINT [FK_LibraryFineRecords_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021231624_LibraryOperationsDepth'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021231624_LibraryOperationsDepth', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [AnalyticsSnapshots] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [SnapshotDate] date NOT NULL,
        [MetricKey] nvarchar(120) NOT NULL,
        [MetricValue] decimal(18,4) NOT NULL,
        [DimensionsJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AnalyticsSnapshots] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [BiDashboards] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Name] nvarchar(160) NOT NULL,
        [Provider] int NOT NULL,
        [EmbedUrl] nvarchar(1000) NOT NULL,
        [ExternalReportId] nvarchar(160) NULL,
        [TenantIntegrationSettingId] uniqueidentifier NULL,
        [AllowedRolesCsv] nvarchar(1000) NULL,
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
        CONSTRAINT [PK_BiDashboards] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BiDashboards_TenantIntegrationSettings_TenantIntegrationSettingId] FOREIGN KEY ([TenantIntegrationSettingId]) REFERENCES [TenantIntegrationSettings] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [DashboardWidgetConfigurations] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [RoleName] nvarchar(80) NOT NULL,
        [WidgetKey] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(150) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [Size] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [SettingsJson] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DashboardWidgetConfigurations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [ReportDefinitions] (
        [Id] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Code] nvarchar(80) NOT NULL,
        [Name] nvarchar(180) NOT NULL,
        [Description] nvarchar(500) NULL,
        [DataSource] int NOT NULL,
        [SelectedFieldsJson] nvarchar(max) NOT NULL,
        [FiltersJson] nvarchar(max) NOT NULL,
        [GroupingJson] nvarchar(max) NOT NULL,
        [SortJson] nvarchar(max) NOT NULL,
        [IsSystem] bit NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ReportDefinitions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [ReportExecutions] (
        [Id] uniqueidentifier NOT NULL,
        [ReportDefinitionId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Format] int NOT NULL,
        [Status] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [StartedOn] datetime2 NULL,
        [CompletedOn] datetime2 NULL,
        [RowCount] int NOT NULL,
        [FileName] nvarchar(500) NULL,
        [ContentType] nvarchar(120) NULL,
        [OutputContent] varbinary(max) NULL,
        [ErrorMessage] nvarchar(2000) NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ReportExecutions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReportExecutions_ReportDefinitions_ReportDefinitionId] FOREIGN KEY ([ReportDefinitionId]) REFERENCES [ReportDefinitions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE TABLE [ReportSchedules] (
        [Id] uniqueidentifier NOT NULL,
        [ReportDefinitionId] uniqueidentifier NOT NULL,
        [BranchId] uniqueidentifier NULL,
        [Frequency] int NOT NULL,
        [DayOfWeek] int NULL,
        [DayOfMonth] int NULL,
        [DeliveryTime] time NOT NULL,
        [TimeZoneId] nvarchar(100) NOT NULL,
        [Format] int NOT NULL,
        [Recipients] nvarchar(2000) NOT NULL,
        [NextRunOn] datetime2 NOT NULL,
        [LastRunOn] datetime2 NULL,
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
        CONSTRAINT [PK_ReportSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReportSchedules_ReportDefinitions_ReportDefinitionId] FOREIGN KEY ([ReportDefinitionId]) REFERENCES [ReportDefinitions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_AnalyticsSnapshots_TenantId_BranchId_SnapshotDate_MetricKey] ON [AnalyticsSnapshots] ([TenantId], [BranchId], [SnapshotDate], [MetricKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_BiDashboards_TenantId_BranchId_Name] ON [BiDashboards] ([TenantId], [BranchId], [Name]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_BiDashboards_TenantIntegrationSettingId] ON [BiDashboards] ([TenantIntegrationSettingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_DashboardWidgetConfigurations_TenantId_BranchId_RoleName_WidgetKey] ON [DashboardWidgetConfigurations] ([TenantId], [BranchId], [RoleName], [WidgetKey]) WHERE [BranchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ReportDefinitions_TenantId_Code] ON [ReportDefinitions] ([TenantId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_ReportExecutions_ReportDefinitionId] ON [ReportExecutions] ([ReportDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_ReportExecutions_TenantId_Status_RequestedOn] ON [ReportExecutions] ([TenantId], [Status], [RequestedOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_ReportSchedules_ReportDefinitionId] ON [ReportSchedules] ([ReportDefinitionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    CREATE INDEX [IX_ReportSchedules_TenantId_IsActive_NextRunOn] ON [ReportSchedules] ([TenantId], [IsActive], [NextRunOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103215821_ReportingAnalytics'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103215821_ReportingAnalytics', N'9.0.0');
END;

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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123222441_ApiCompleteness'
)
BEGIN
    CREATE TABLE [ApiIdempotencyRecords] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [IdempotencyKey] nvarchar(100) NOT NULL,
        [HttpMethod] nvarchar(10) NOT NULL,
        [RequestPath] nvarchar(500) NOT NULL,
        [RequestHash] nvarchar(64) NOT NULL,
        [StatusCode] int NOT NULL,
        [ContentType] nvarchar(150) NOT NULL,
        [ResponseBody] varbinary(max) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(128) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] nvarchar(128) NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedOn] datetime2 NULL,
        [ConcurrencyToken] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ApiIdempotencyRecords] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123222441_ApiCompleteness'
)
BEGIN
    CREATE INDEX [IX_ApiIdempotencyRecords_ExpiresOn] ON [ApiIdempotencyRecords] ([ExpiresOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123222441_ApiCompleteness'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ApiIdempotencyRecords_TenantId_UserId_HttpMethod_RequestPath_IdempotencyKey] ON [ApiIdempotencyRecords] ([TenantId], [UserId], [HttpMethod], [RequestPath], [IdempotencyKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123222441_ApiCompleteness'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261123222441_ApiCompleteness', N'9.0.0');
END;

COMMIT;
GO

