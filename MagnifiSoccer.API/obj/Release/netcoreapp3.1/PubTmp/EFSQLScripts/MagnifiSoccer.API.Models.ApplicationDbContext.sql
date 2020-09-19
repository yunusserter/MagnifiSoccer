IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [Group] (
        [Id] nvarchar(450) NOT NULL,
        [GroupName] nvarchar(max) NOT NULL,
        [PhotoUrl] nvarchar(max) NULL,
        CONSTRAINT [PK_Group] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [User] (
        [Id] nvarchar(450) NOT NULL,
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
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Remainder] decimal(18,2) NOT NULL,
        [OverAllRating] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_User] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [Game] (
        [Id] int NOT NULL IDENTITY,
        [GroupId] nvarchar(450) NULL,
        [HomeTeam] nvarchar(max) NULL,
        [AwayTeam] nvarchar(max) NULL,
        [WinnerTeam] nvarchar(max) NULL,
        [GameDate] DateTime2 NULL,
        [Location] nvarchar(max) NULL,
        [Price] Decimal(16,2) NULL,
        CONSTRAINT [PK_Game] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Game_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [GroupMember] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [FirstName] nvarchar(max) NULL,
        [LastName] nvarchar(max) NULL,
        [GroupId] nvarchar(450) NULL,
        [Role] nvarchar(max) NULL,
        CONSTRAINT [PK_GroupMember] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GroupMember_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GroupMember_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [GamePlayer] (
        [Id] int NOT NULL IDENTITY,
        [GameId] int NULL,
        [UserId] nvarchar(450) NULL,
        [Position] int NOT NULL,
        [Team] nvarchar(max) NULL,
        [Attended] bit NULL,
        [GameRating] decimal(18,2) NOT NULL,
        [InviteResponse] bit NULL,
        CONSTRAINT [PK_GamePlayer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GamePlayer_Game_GameId] FOREIGN KEY ([GameId]) REFERENCES [Game] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GamePlayer_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE TABLE [Rating] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [VoterUserId] nvarchar(450) NOT NULL,
        [RatingValue] decimal(18,2) NOT NULL,
        [GameId] int NOT NULL,
        CONSTRAINT [PK_Rating] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rating_Game_GameId] FOREIGN KEY ([GameId]) REFERENCES [Game] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Rating_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_Game_GroupId] ON [Game] ([GroupId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_GamePlayer_GameId] ON [GamePlayer] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_GamePlayer_UserId] ON [GamePlayer] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_GroupMember_GroupId] ON [GroupMember] ([GroupId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_GroupMember_UserId] ON [GroupMember] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_Rating_GameId] ON [Rating] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [IX_Rating_UserId] ON [Rating] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE INDEX [EmailIndex] ON [User] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [User] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418102255_Update-Database')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200418102255_Update-Database', N'3.1.2');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200418185019_update')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200418185019_update', N'3.1.2');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502180228_Update2')
BEGIN
    ALTER TABLE [GamePlayer] DROP CONSTRAINT [FK_GamePlayer_Game_GameId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502180228_Update2')
BEGIN
    DROP INDEX [IX_GamePlayer_GameId] ON [GamePlayer];
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GamePlayer]') AND [c].[name] = N'GameId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [GamePlayer] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [GamePlayer] ALTER COLUMN [GameId] int NOT NULL;
    CREATE INDEX [IX_GamePlayer_GameId] ON [GamePlayer] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502180228_Update2')
BEGIN
    ALTER TABLE [GamePlayer] ADD CONSTRAINT [FK_GamePlayer_Game_GameId] FOREIGN KEY ([GameId]) REFERENCES [Game] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502180228_Update2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200502180228_Update2', N'3.1.2');
END;

GO

