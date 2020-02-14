CREATE TABLE [dbo].[Environment] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50)  NOT NULL,
    [Description]      VARCHAR (500) NOT NULL,
    [Insert_TimeStamp] DATETIME      NOT NULL,
    [Update_TimeStamp] DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

