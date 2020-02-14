CREATE TABLE [dbo].[Application] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50)   NOT NULL,
    [Description]      VARCHAR (5000) NOT NULL,
    [Insert_TimeStamp] DATETIME       NOT NULL,
    [Update_TimeStamp] DATETIME       NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([Id] ASC)
);

