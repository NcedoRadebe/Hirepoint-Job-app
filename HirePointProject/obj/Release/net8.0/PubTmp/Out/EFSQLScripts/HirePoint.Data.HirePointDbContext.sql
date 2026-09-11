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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [Provinces] (
        [ProvinceID] int NOT NULL IDENTITY,
        [ProvinceName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Provinces] PRIMARY KEY ([ProvinceID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [UserID] int NOT NULL IDENTITY,
        [FullName] nvarchar(max) NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [ProvinceID] int NULL,
        [UserType] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [DateCreated] datetime2 NOT NULL,
        [Gender] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserID]),
        CONSTRAINT [FK_Users_Provinces_ProvinceID] FOREIGN KEY ([ProvinceID]) REFERENCES [Provinces] ([ProvinceID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [Jobs] (
        [JobID] int NOT NULL IDENTITY,
        [RecruiterID] int NOT NULL,
        [JobTitle] nvarchar(max) NOT NULL,
        [JobDescription] nvarchar(max) NOT NULL,
        [Requirements] nvarchar(max) NOT NULL,
        [MinimumSalary] decimal(18,2) NOT NULL,
        [MaximumSalary] decimal(18,2) NOT NULL,
        [SalaryType] nvarchar(max) NOT NULL,
        [Province] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [EmploymentType] nvarchar(max) NOT NULL,
        [ExperienceLevel] nvarchar(max) NOT NULL,
        [ClosingDate] datetime2 NOT NULL,
        [DatePosted] datetime2 NOT NULL,
        [JobStatus] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Jobs] PRIMARY KEY ([JobID]),
        CONSTRAINT [FK_Jobs_Users_RecruiterID] FOREIGN KEY ([RecruiterID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [JobseekerProfiles] (
        [UserID] int NOT NULL,
        [Skills] nvarchar(max) NOT NULL,
        [Education] nvarchar(max) NOT NULL,
        [Experience] nvarchar(max) NOT NULL,
        [CVFile] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_JobseekerProfiles] PRIMARY KEY ([UserID]),
        CONSTRAINT [FK_JobseekerProfiles_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [RecruiterProfiles] (
        [UserID] int NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [CompanyDescription] nvarchar(max) NOT NULL,
        [Website] nvarchar(max) NOT NULL,
        [Industry] nvarchar(max) NOT NULL,
        [CompanySize] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_RecruiterProfiles] PRIMARY KEY ([UserID]),
        CONSTRAINT [FK_RecruiterProfiles_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [UserActivity] (
        [ActivityID] int NOT NULL IDENTITY,
        [RecruiterProfileID] int NOT NULL,
        [ActivityType] nvarchar(max) NOT NULL,
        [ActivityDate] datetime2 NOT NULL,
        [UserID] int NULL,
        CONSTRAINT [PK_UserActivity] PRIMARY KEY ([ActivityID]),
        CONSTRAINT [FK_UserActivity_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE TABLE [Applications] (
        [ApplicationID] int NOT NULL IDENTITY,
        [JobID] int NOT NULL,
        [JobseekerID] int NOT NULL,
        [ApplicationDate] datetime2 NOT NULL,
        [ApplicationStatus] nvarchar(max) NOT NULL,
        [UserID] int NULL,
        CONSTRAINT [PK_Applications] PRIMARY KEY ([ApplicationID]),
        CONSTRAINT [FK_Applications_Jobs_JobID] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([JobID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Applications_Users_JobseekerID] FOREIGN KEY ([JobseekerID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Applications_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Applications_JobID] ON [Applications] ([JobID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Applications_JobseekerID] ON [Applications] ([JobseekerID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Applications_UserID] ON [Applications] ([UserID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Jobs_RecruiterID] ON [Jobs] ([RecruiterID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserActivity_UserID] ON [UserActivity] ([UserID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_ProvinceID] ON [Users] ([ProvinceID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806195003_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260806195003_InitialCreate', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806234857_AddSalaryColumnToJobs'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'MaximumSalary');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Jobs] DROP COLUMN [MaximumSalary];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806234857_AddSalaryColumnToJobs'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'MinimumSalary');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Jobs] DROP COLUMN [MinimumSalary];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806234857_AddSalaryColumnToJobs'
)
BEGIN
    EXEC sp_rename N'[Jobs].[SalaryType]', N'Salary', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806234857_AddSalaryColumnToJobs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260806234857_AddSalaryColumnToJobs', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260806235248_RemoveMinimumMaximumSalary'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260806235248_RemoveMinimumMaximumSalary', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'CVFile');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [CVFile] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [DecisionDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [DecisionMessage] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [InterviewDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [InterviewLocation] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [InterviewNotes] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [InterviewTime] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    ALTER TABLE [Applications] ADD [InterviewType] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    CREATE TABLE [Notifications] (
        [NotificationID] int NOT NULL IDENTITY,
        [UserID] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL,
        [DateCreated] datetime2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationID]),
        CONSTRAINT [FK_Notifications_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserID] ON [Notifications] ([UserID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808113106_AddApplicationDecisionAndInterview'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260808113106_AddApplicationDecisionAndInterview', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809152953_AddNotifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260809152953_AddNotifications', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260810010643_final'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260810010643_final', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RecruiterProfiles]') AND [c].[name] = N'CompanySize');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [RecruiterProfiles] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [RecruiterProfiles] DROP COLUMN [CompanySize];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RecruiterProfiles]') AND [c].[name] = N'Industry');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [RecruiterProfiles] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [RecruiterProfiles] DROP COLUMN [Industry];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'UserType');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Users] ALTER COLUMN [UserType] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Status');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Status] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'PhoneNumber');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Users] ALTER COLUMN [PhoneNumber] nvarchar(20) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Gender');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Gender] nvarchar(20) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'FullName');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Users] ALTER COLUMN [FullName] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DROP INDEX [IX_Users_Email] ON [Users];
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Email] nvarchar(254) NOT NULL;
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    ALTER TABLE [Users] ADD [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RecruiterProfiles]') AND [c].[name] = N'Website');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [RecruiterProfiles] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [RecruiterProfiles] ALTER COLUMN [Website] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RecruiterProfiles]') AND [c].[name] = N'CompanyName');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [RecruiterProfiles] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [RecruiterProfiles] ALTER COLUMN [CompanyName] nvarchar(150) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RecruiterProfiles]') AND [c].[name] = N'CompanyDescription');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [RecruiterProfiles] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [RecruiterProfiles] ALTER COLUMN [CompanyDescription] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Provinces]') AND [c].[name] = N'ProvinceName');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Provinces] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [Provinces] ALTER COLUMN [ProvinceName] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'Title');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [Notifications] ALTER COLUMN [Title] nvarchar(150) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'Message');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [Notifications] ALTER COLUMN [Message] nvarchar(2000) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Skills');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Skills] nvarchar(2000) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Experience');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Experience] nvarchar(3000) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Education');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Education] nvarchar(2000) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'CVFile');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [CVFile] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'Salary');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [Salary] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'Requirements');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [Requirements] nvarchar(3000) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'Province');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [Province] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'JobTitle');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [JobTitle] nvarchar(150) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'JobStatus');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [JobStatus] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'ExperienceLevel');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [ExperienceLevel] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'EmploymentType');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [EmploymentType] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'City');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [Jobs] ALTER COLUMN [City] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'InterviewType');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [InterviewType] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'InterviewTime');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [InterviewTime] nvarchar(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'InterviewNotes');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var31 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [InterviewNotes] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var32 sysname;
    SELECT @var32 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'InterviewLocation');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var32 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [InterviewLocation] nvarchar(250) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var33 sysname;
    SELECT @var33 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'DecisionMessage');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var33 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [DecisionMessage] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    DECLARE @var34 sysname;
    SELECT @var34 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Applications]') AND [c].[name] = N'ApplicationStatus');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Applications] DROP CONSTRAINT [' + @var34 + '];');
    ALTER TABLE [Applications] ALTER COLUMN [ApplicationStatus] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260813143310_updatemodel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260813143310_updatemodel', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818081554_UpdateDatabase'
)
BEGIN
    DECLARE @var35 sysname;
    SELECT @var35 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Provinces]') AND [c].[name] = N'ProvinceName');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Provinces] DROP CONSTRAINT [' + @var35 + '];');
    ALTER TABLE [Provinces] ALTER COLUMN [ProvinceName] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818081554_UpdateDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818081554_UpdateDatabase', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818085959_Jobseekerprofile'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818085959_Jobseekerprofile', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818092601_MakeJobseekerProfileFieldsNullable'
)
BEGIN
    DECLARE @var36 sysname;
    SELECT @var36 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Skills');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var36 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Skills] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818092601_MakeJobseekerProfileFieldsNullable'
)
BEGIN
    DECLARE @var37 sysname;
    SELECT @var37 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Experience');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var37 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Experience] nvarchar(3000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818092601_MakeJobseekerProfileFieldsNullable'
)
BEGIN
    DECLARE @var38 sysname;
    SELECT @var38 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Education');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var38 + '];');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Education] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818092601_MakeJobseekerProfileFieldsNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818092601_MakeJobseekerProfileFieldsNullable', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818170129_AddSystemAdministratorFlag'
)
BEGIN
    ALTER TABLE [Users] ADD [IsSystemAdministrator] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818170129_AddSystemAdministratorFlag'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818170129_AddSystemAdministratorFlag', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818185456_AddPasswordResetFields'
)
BEGIN
    ALTER TABLE [Users] ADD [PasswordResetToken] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818185456_AddPasswordResetFields'
)
BEGIN
    ALTER TABLE [Users] ADD [PasswordResetTokenExpiry] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818185456_AddPasswordResetFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818185456_AddPasswordResetFields', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    DECLARE @var39 sysname;
    SELECT @var39 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'PasswordResetToken');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var39 + '];');
    ALTER TABLE [Users] ALTER COLUMN [PasswordResetToken] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    ALTER TABLE [Users] ADD [CompanyID] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    ALTER TABLE [Jobs] ADD [CompanyID] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    CREATE TABLE [Companies] (
        [CompanyID] int NOT NULL IDENTITY,
        [CompanyName] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Email] nvarchar(254) NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [Address] nvarchar(500) NULL,
        [ProvinceID] int NULL,
        [DateCreated] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([CompanyID]),
        CONSTRAINT [FK_Companies_Provinces_ProvinceID] FOREIGN KEY ([ProvinceID]) REFERENCES [Provinces] ([ProvinceID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    CREATE INDEX [IX_Users_CompanyID] ON [Users] ([CompanyID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    CREATE INDEX [IX_Jobs_CompanyID] ON [Jobs] ([CompanyID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    CREATE INDEX [IX_Companies_ProvinceID] ON [Companies] ([ProvinceID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    ALTER TABLE [Jobs] ADD CONSTRAINT [FK_Jobs_Companies_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [Companies] ([CompanyID]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Companies_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [Companies] ([CompanyID]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819022250_CompanyJobRelationships'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819022250_CompanyJobRelationships', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819041208_AddRecruiterProfileColumns'
)
BEGIN
    EXEC sp_rename N'[Companies].[Description]', N'CompanyDescription', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819041208_AddRecruiterProfileColumns'
)
BEGIN
    ALTER TABLE [Companies] ADD [UserID] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819041208_AddRecruiterProfileColumns'
)
BEGIN
    ALTER TABLE [Companies] ADD [Website] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819041208_AddRecruiterProfileColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819041208_AddRecruiterProfileColumns', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Companies] DROP CONSTRAINT [FK_Companies_Provinces_ProvinceID];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Jobs] DROP CONSTRAINT [FK_Jobs_Companies_CompanyID];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Companies_CompanyID];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Companies] DROP CONSTRAINT [PK_Companies];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var40 sysname;
    SELECT @var40 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'IsSystemAdministrator');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var40 + '];');
    ALTER TABLE [Users] DROP COLUMN [IsSystemAdministrator];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var41 sysname;
    SELECT @var41 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'PasswordResetToken');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var41 + '];');
    ALTER TABLE [Users] DROP COLUMN [PasswordResetToken];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var42 sysname;
    SELECT @var42 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'PasswordResetTokenExpiry');
    IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var42 + '];');
    ALTER TABLE [Users] DROP COLUMN [PasswordResetTokenExpiry];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var43 sysname;
    SELECT @var43 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'UserID');
    IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var43 + '];');
    ALTER TABLE [Companies] DROP COLUMN [UserID];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    EXEC sp_rename N'[Companies]', N'Company';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    EXEC sp_rename N'[Company].[IX_Companies_ProvinceID]', N'IX_Company_ProvinceID', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var44 sysname;
    SELECT @var44 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Skills');
    IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var44 + '];');
    EXEC(N'UPDATE [JobseekerProfiles] SET [Skills] = N'''' WHERE [Skills] IS NULL');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Skills] nvarchar(2000) NOT NULL;
    ALTER TABLE [JobseekerProfiles] ADD DEFAULT N'' FOR [Skills];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var45 sysname;
    SELECT @var45 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Experience');
    IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var45 + '];');
    EXEC(N'UPDATE [JobseekerProfiles] SET [Experience] = N'''' WHERE [Experience] IS NULL');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Experience] nvarchar(3000) NOT NULL;
    ALTER TABLE [JobseekerProfiles] ADD DEFAULT N'' FOR [Experience];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DECLARE @var46 sysname;
    SELECT @var46 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[JobseekerProfiles]') AND [c].[name] = N'Education');
    IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [JobseekerProfiles] DROP CONSTRAINT [' + @var46 + '];');
    EXEC(N'UPDATE [JobseekerProfiles] SET [Education] = N'''' WHERE [Education] IS NULL');
    ALTER TABLE [JobseekerProfiles] ALTER COLUMN [Education] nvarchar(2000) NOT NULL;
    ALTER TABLE [JobseekerProfiles] ADD DEFAULT N'' FOR [Education];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    DROP INDEX [IX_Jobs_CompanyID] ON [Jobs];
    DECLARE @var47 sysname;
    SELECT @var47 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Jobs]') AND [c].[name] = N'CompanyID');
    IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Jobs] DROP CONSTRAINT [' + @var47 + '];');
    EXEC(N'UPDATE [Jobs] SET [CompanyID] = 0 WHERE [CompanyID] IS NULL');
    ALTER TABLE [Jobs] ALTER COLUMN [CompanyID] int NOT NULL;
    ALTER TABLE [Jobs] ADD DEFAULT 0 FOR [CompanyID];
    CREATE INDEX [IX_Jobs_CompanyID] ON [Jobs] ([CompanyID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [PK_Company] PRIMARY KEY ([CompanyID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Company] ADD CONSTRAINT [FK_Company_Provinces_ProvinceID] FOREIGN KEY ([ProvinceID]) REFERENCES [Provinces] ([ProvinceID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Jobs] ADD CONSTRAINT [FK_Jobs_Company_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([CompanyID]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Company_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([CompanyID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819200812_AddCompanyToJobs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819200812_AddCompanyToJobs', N'8.0.19');
END;
GO

COMMIT;
GO

