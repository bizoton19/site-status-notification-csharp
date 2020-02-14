CREATE TABLE [dbo].[ResourceStatusArchive] (
    [Id]                      INT      IDENTITY (1, 1) NOT NULL,
    [Resource_Id]             INT      NOT NULL,
    [Resource_Status_Type_Id] INT      NOT NULL,
    [Insert_TimeStamp]        DATETIME NOT NULL,
    [Update_TimeStamp]        DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

