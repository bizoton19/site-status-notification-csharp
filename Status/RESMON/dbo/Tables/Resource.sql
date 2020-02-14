CREATE TABLE [dbo].[Resource] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (60)   NOT NULL,
    [Type]             INT            NOT NULL,
    [URL]              VARCHAR (1000) NOT NULL,
    [Description]      VARCHAR (200)  NULL,
    [Environment_Id]   INT            NULL,
    [Insert_TimeStamp] DATETIME       NOT NULL,
    [Update_TimeStamp] DATETIME       NOT NULL,
    CONSTRAINT [PK_Resource] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Resource_ResourceType] FOREIGN KEY ([Type]) REFERENCES [dbo].[ResourceType] ([Id])
);

