CREATE TABLE [dbo].[ApplicationContact]
([Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Phone] [varchar](20) NULL,
	[Insert_TimeStamp] [datetime] NOT NULL,
	[Update_TimeStamp] [datetime] NOT NULL
 CONSTRAINT [PK_ApplicationContact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationContact]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationContact_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([Id])
GO

ALTER TABLE [dbo].[ApplicationContact] CHECK CONSTRAINT [FK_ApplicationContact_Application]
GO


