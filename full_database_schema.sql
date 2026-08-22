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
CREATE TABLE [Foods] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(max) NULL,
    [ImageUrl] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Foods] PRIMARY KEY ([Id])
);

CREATE TABLE [PlaceTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_PlaceTypes] PRIMARY KEY ([Id])
);

CREATE TABLE [Regions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Regions] PRIMARY KEY ([Id])
);

CREATE TABLE [ReportReasons] (
    [Id] int NOT NULL IDENTITY,
    [Content] nvarchar(150) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_ReportReasons] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [AvatarUrl] nvarchar(500) NULL,
    [GoogleId] nvarchar(100) NULL,
    [Role] nvarchar(30) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Bio] nvarchar(500) NULL,
    [CoverUrl] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [PlaceTypeId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_PlaceTypes_PlaceTypeId] FOREIGN KEY ([PlaceTypeId]) REFERENCES [PlaceTypes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Provinces] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [RegionId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Provinces] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Provinces_Regions_RegionId] FOREIGN KEY ([RegionId]) REFERENCES [Regions] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Appeals] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [TargetType] nvarchar(30) NOT NULL,
    [TargetId] bigint NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [Status] nvarchar(40) NOT NULL,
    [CategoryAdminId] bigint NULL,
    [CategoryAdminResult] nvarchar(max) NULL,
    [CategoryAdminAt] datetime2 NULL,
    [SystemAdminId] bigint NULL,
    [FinalResult] nvarchar(max) NULL,
    [SystemAdminAt] datetime2 NULL,
    [SubmittedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Appeals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appeals_Users_CategoryAdminId] FOREIGN KEY ([CategoryAdminId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Appeals_Users_SystemAdminId] FOREIGN KEY ([SystemAdminId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Appeals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Action] nvarchar(100) NOT NULL,
    [TargetType] nvarchar(50) NULL,
    [TargetId] bigint NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Content] nvarchar(500) NOT NULL,
    [Type] nvarchar(40) NOT NULL,
    [TargetType] nvarchar(30) NULL,
    [TargetId] bigint NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reports] (
    [Id] bigint NOT NULL IDENTITY,
    [ReporterId] bigint NOT NULL,
    [TargetType] nvarchar(20) NOT NULL,
    [TargetId] bigint NOT NULL,
    [ReasonId] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [Status] nvarchar(20) NOT NULL,
    [Result] nvarchar(30) NULL,
    [HandledBy] bigint NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [HandledAt] datetime2 NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reports_ReportReasons_ReasonId] FOREIGN KEY ([ReasonId]) REFERENCES [ReportReasons] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reports_Users_HandledBy] FOREIGN KEY ([HandledBy]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Reports_Users_ReporterId] FOREIGN KEY ([ReporterId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [SystemConfigs] (
    [Id] int NOT NULL IDENTITY,
    [ConfigKey] nvarchar(100) NOT NULL,
    [ConfigValue] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UpdatedBy] bigint NULL,
    CONSTRAINT [PK_SystemConfigs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SystemConfigs_Users_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [AdminCategoryAssignments] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [CategoryId] int NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AdminCategoryAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AdminCategoryAssignments_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AdminCategoryAssignments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [FoodProvinces] (
    [FoodId] bigint NOT NULL,
    [ProvinceId] int NOT NULL,
    CONSTRAINT [PK_FoodProvinces] PRIMARY KEY ([FoodId], [ProvinceId]),
    CONSTRAINT [FK_FoodProvinces_Foods_FoodId] FOREIGN KEY ([FoodId]) REFERENCES [Foods] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FoodProvinces_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) REFERENCES [Provinces] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Places] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Address] nvarchar(255) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [Website] nvarchar(500) NULL,
    [MinPrice] decimal(12,0) NULL,
    [MaxPrice] decimal(12,0) NULL,
    [OpeningHours] nvarchar(255) NULL,
    [Latitude] decimal(10,7) NULL,
    [Longitude] decimal(10,7) NULL,
    [ProvinceId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [AvgRating] decimal(2,1) NOT NULL,
    [ReviewCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Places] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Places_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Places_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) REFERENCES [Provinces] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [AccessHistories] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [PlaceId] bigint NOT NULL,
    [ViewedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AccessHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AccessHistories_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccessHistories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Favorites] (
    [UserId] bigint NOT NULL,
    [PlaceId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Favorites] PRIMARY KEY ([UserId], [PlaceId]),
    CONSTRAINT [FK_Favorites_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Favorites_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [FoodPlaces] (
    [FoodId] bigint NOT NULL,
    [PlaceId] bigint NOT NULL,
    CONSTRAINT [PK_FoodPlaces] PRIMARY KEY ([FoodId], [PlaceId]),
    CONSTRAINT [FK_FoodPlaces_Foods_FoodId] FOREIGN KEY ([FoodId]) REFERENCES [Foods] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FoodPlaces_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PlaceMedia] (
    [Id] bigint NOT NULL IDENTITY,
    [PlaceId] bigint NOT NULL,
    [UploadedBy] bigint NULL,
    [MediaType] nvarchar(10) NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PlaceMedia] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaceMedia_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlaceMedia_Users_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [Users] ([Id])
);

CREATE TABLE [PlaceProposals] (
    [Id] bigint NOT NULL IDENTITY,
    [ProposalType] nvarchar(20) NOT NULL,
    [TargetPlaceId] bigint NULL,
    [ProposedBy] bigint NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Address] nvarchar(255) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [Website] nvarchar(500) NULL,
    [MinPrice] decimal(12,0) NULL,
    [MaxPrice] decimal(12,0) NULL,
    [OpeningHours] nvarchar(255) NULL,
    [Latitude] decimal(10,7) NULL,
    [Longitude] decimal(10,7) NULL,
    [ProvinceId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RejectReason] nvarchar(500) NULL,
    [ReviewedBy] bigint NULL,
    [ApprovedPlaceId] bigint NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [ReviewedAt] datetime2 NULL,
    CONSTRAINT [PK_PlaceProposals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaceProposals_Places_TargetPlaceId] FOREIGN KEY ([TargetPlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlaceProposals_Users_ProposedBy] FOREIGN KEY ([ProposedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PlaceProposals_Users_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Reviews] (
    [Id] bigint NOT NULL IDENTITY,
    [PlaceId] bigint NOT NULL,
    [UserId] bigint NOT NULL,
    [Rating] tinyint NOT NULL,
    [Content] nvarchar(max) NULL,
    [VideoUrl] nvarchar(500) NULL,
    [ExperienceDate] datetime2 NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [VisitLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [PlaceId] bigint NOT NULL,
    [VisitedDate] date NOT NULL,
    [Privacy] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_VisitLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VisitLogs_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_VisitLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProposalMedia] (
    [Id] bigint NOT NULL IDENTITY,
    [ProposalId] bigint NOT NULL,
    [MediaType] nvarchar(10) NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ProposalMedia] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProposalMedia_PlaceProposals_ProposalId] FOREIGN KEY ([ProposalId]) REFERENCES [PlaceProposals] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Comments] (
    [Id] bigint NOT NULL IDENTITY,
    [ReviewId] bigint NOT NULL,
    [UserId] bigint NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_Reviews_ReviewId] FOREIGN KEY ([ReviewId]) REFERENCES [Reviews] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [ReviewMedia] (
    [Id] bigint NOT NULL IDENTITY,
    [ReviewId] bigint NOT NULL,
    [ImageUrl] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ReviewMedia] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReviewMedia_Reviews_ReviewId] FOREIGN KEY ([ReviewId]) REFERENCES [Reviews] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AccessHistories_PlaceId] ON [AccessHistories] ([PlaceId]);

CREATE INDEX [IX_AccessHistories_UserId_ViewedAt] ON [AccessHistories] ([UserId], [ViewedAt]);

CREATE INDEX [IX_AdminCategoryAssignments_CategoryId] ON [AdminCategoryAssignments] ([CategoryId]);

CREATE UNIQUE INDEX [IX_AdminCategoryAssignments_UserId_CategoryId] ON [AdminCategoryAssignments] ([UserId], [CategoryId]);

CREATE INDEX [IX_Appeals_CategoryAdminId] ON [Appeals] ([CategoryAdminId]);

CREATE INDEX [IX_Appeals_SystemAdminId] ON [Appeals] ([SystemAdminId]);

CREATE INDEX [IX_Appeals_UserId] ON [Appeals] ([UserId]);

CREATE INDEX [IX_AuditLogs_UserId_CreatedAt] ON [AuditLogs] ([UserId], [CreatedAt]);

CREATE UNIQUE INDEX [IX_Categories_Name_PlaceTypeId] ON [Categories] ([Name], [PlaceTypeId]);

CREATE INDEX [IX_Categories_PlaceTypeId] ON [Categories] ([PlaceTypeId]);

CREATE INDEX [IX_Comments_ReviewId] ON [Comments] ([ReviewId]);

CREATE INDEX [IX_Comments_UserId] ON [Comments] ([UserId]);

CREATE INDEX [IX_Favorites_PlaceId] ON [Favorites] ([PlaceId]);

CREATE INDEX [IX_FoodPlaces_PlaceId] ON [FoodPlaces] ([PlaceId]);

CREATE INDEX [IX_FoodProvinces_ProvinceId] ON [FoodProvinces] ([ProvinceId]);

CREATE UNIQUE INDEX [IX_Foods_Name] ON [Foods] ([Name]);

CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);

CREATE INDEX [IX_PlaceMedia_PlaceId] ON [PlaceMedia] ([PlaceId]);

CREATE INDEX [IX_PlaceMedia_UploadedBy] ON [PlaceMedia] ([UploadedBy]);

CREATE INDEX [IX_PlaceProposals_ProposedBy] ON [PlaceProposals] ([ProposedBy]);

CREATE INDEX [IX_PlaceProposals_ReviewedBy] ON [PlaceProposals] ([ReviewedBy]);

CREATE INDEX [IX_PlaceProposals_TargetPlaceId] ON [PlaceProposals] ([TargetPlaceId]);

CREATE INDEX [IX_Places_CategoryId] ON [Places] ([CategoryId]);

CREATE INDEX [IX_Places_Latitude_Longitude] ON [Places] ([Latitude], [Longitude]);

CREATE INDEX [IX_Places_ProvinceId] ON [Places] ([ProvinceId]);

CREATE INDEX [IX_Places_Status] ON [Places] ([Status]);

CREATE UNIQUE INDEX [IX_PlaceTypes_Name] ON [PlaceTypes] ([Name]);

CREATE INDEX [IX_ProposalMedia_ProposalId] ON [ProposalMedia] ([ProposalId]);

CREATE UNIQUE INDEX [IX_Provinces_Name_RegionId] ON [Provinces] ([Name], [RegionId]);

CREATE INDEX [IX_Provinces_RegionId] ON [Provinces] ([RegionId]);

CREATE UNIQUE INDEX [IX_Regions_Name] ON [Regions] ([Name]);

CREATE UNIQUE INDEX [IX_ReportReasons_Content] ON [ReportReasons] ([Content]);

CREATE INDEX [IX_Reports_HandledBy] ON [Reports] ([HandledBy]);

CREATE INDEX [IX_Reports_ReasonId] ON [Reports] ([ReasonId]);

CREATE UNIQUE INDEX [IX_Reports_ReporterId_TargetType_TargetId] ON [Reports] ([ReporterId], [TargetType], [TargetId]);

CREATE INDEX [IX_ReviewMedia_ReviewId] ON [ReviewMedia] ([ReviewId]);

CREATE INDEX [IX_Reviews_PlaceId] ON [Reviews] ([PlaceId]);

CREATE UNIQUE INDEX [IX_Reviews_PlaceId_UserId] ON [Reviews] ([PlaceId], [UserId]);

CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);

CREATE UNIQUE INDEX [IX_SystemConfigs_ConfigKey] ON [SystemConfigs] ([ConfigKey]);

CREATE INDEX [IX_SystemConfigs_UpdatedBy] ON [SystemConfigs] ([UpdatedBy]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_GoogleId] ON [Users] ([GoogleId]) WHERE [GoogleId] IS NOT NULL;

CREATE INDEX [IX_VisitLogs_PlaceId] ON [VisitLogs] ([PlaceId]);

CREATE INDEX [IX_VisitLogs_UserId] ON [VisitLogs] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260820080138_RefactorPlacesAndProposals', N'10.0.11');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [PlaceProposals] DROP CONSTRAINT [FK_PlaceProposals_Places_TargetPlaceId];

DROP INDEX [IX_PlaceProposals_TargetPlaceId] ON [PlaceProposals];

DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PlaceProposals]') AND [c].[name] = N'ProposalType');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [PlaceProposals] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [PlaceProposals] DROP COLUMN [ProposalType];

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PlaceProposals]') AND [c].[name] = N'TargetPlaceId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [PlaceProposals] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [PlaceProposals] DROP COLUMN [TargetPlaceId];

CREATE TABLE [PlaceEditProposals] (
    [Id] bigint NOT NULL IDENTITY,
    [PlaceId] bigint NOT NULL,
    [ProposedBy] bigint NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Address] nvarchar(255) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [Website] nvarchar(500) NULL,
    [MinPrice] decimal(12,0) NULL,
    [MaxPrice] decimal(12,0) NULL,
    [OpeningHours] nvarchar(255) NULL,
    [Latitude] decimal(10,7) NULL,
    [Longitude] decimal(10,7) NULL,
    [ProvinceId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RejectReason] nvarchar(500) NULL,
    [ReviewedBy] bigint NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [ReviewedAt] datetime2 NULL,
    CONSTRAINT [PK_PlaceEditProposals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaceEditProposals_Places_PlaceId] FOREIGN KEY ([PlaceId]) REFERENCES [Places] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlaceEditProposals_Users_ProposedBy] FOREIGN KEY ([ProposedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PlaceEditProposals_Users_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [Users] ([Id])
);

CREATE TABLE [PlaceEditProposalMedia] (
    [Id] bigint NOT NULL IDENTITY,
    [PlaceEditProposalId] bigint NOT NULL,
    [MediaType] nvarchar(10) NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PlaceEditProposalMedia] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaceEditProposalMedia_PlaceEditProposals_PlaceEditProposalId] FOREIGN KEY ([PlaceEditProposalId]) REFERENCES [PlaceEditProposals] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PlaceEditProposalMedia_PlaceEditProposalId] ON [PlaceEditProposalMedia] ([PlaceEditProposalId]);

CREATE INDEX [IX_PlaceEditProposals_PlaceId] ON [PlaceEditProposals] ([PlaceId]);

CREATE INDEX [IX_PlaceEditProposals_ProposedBy] ON [PlaceEditProposals] ([ProposedBy]);

CREATE INDEX [IX_PlaceEditProposals_ReviewedBy] ON [PlaceEditProposals] ([ReviewedBy]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260820104329_SeparateProposals', N'10.0.11');

COMMIT;
GO

