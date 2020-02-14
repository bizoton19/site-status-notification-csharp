CREATE TABLE [dbo].[ResourceStatus] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Resource_Id]      INT            NOT NULL,
    [Name]             VARCHAR (200)  NOT NULL,
    [Code]             INT            NULL,
    [Description]      VARCHAR (1000) NOT NULL,
    [Insert_TimeStamp] DATETIME       NOT NULL,
    [Update_TimeStamp] DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResourceStatus_Resource] FOREIGN KEY ([Resource_Id]) REFERENCES [dbo].[Resource] ([Id])
);

