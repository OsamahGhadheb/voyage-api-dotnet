﻿CREATE TABLE [dbo].[Notification]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Text] NVARCHAR(255) NOT NULL, 
    [AssignedToUserId] NVARCHAR(128) NOT NULL,
	[IsRead] BIT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(128) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE()
)