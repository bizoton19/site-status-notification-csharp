CREATE TABLE [dbo].[AppResource] (
    [ApplicationId]    INT      NOT NULL,
    [ResourceId]       INT      NOT NULL,
    [Insert_TimeStamp] DATETIME NOT NULL,
    [Update_TimeStamp] DATETIME NOT NULL,
    CONSTRAINT [PK_AppResource] PRIMARY KEY CLUSTERED ([ApplicationId] ASC, [ResourceId] ASC),
    CONSTRAINT [FK_AppResource_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_AppResource_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([Id])
);

