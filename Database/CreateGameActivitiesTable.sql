USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__GameActiv__Activ__33D4B598]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GameActivities] DROP CONSTRAINT [DF__GameActiv__Activ__33D4B598]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[GameActivities]    Script Date: 05/31/2017 23:03:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GameActivities]') AND type in (N'U'))
DROP TABLE [dbo].[GameActivities]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[GameActivities]    Script Date: 05/31/2017 23:03:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GameActivities](
	[ActivityId] [bigint] IDENTITY(1,1) NOT NULL,
	[GameId] [bigint] NOT NULL,
	[PlayerId] [bigint] NOT NULL,
	[ActivityTime] [datetime] NOT NULL,
	[CellRow] [int] NOT NULL,
	[CellColumn] [int] NOT NULL,
	[CellValue] [int] NOT NULL,
 CONSTRAINT [PK_GameActivities] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GameActivities] ADD  DEFAULT (getdate()) FOR [ActivityTime]
GO


ALTER TABLE [dbo].[GameActivities]  WITH CHECK ADD  CONSTRAINT [FK__GAMES_ACTIVITY_PLAYERID] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Players] ([PlayerId])
GO

ALTER TABLE [dbo].[GameActivities] CHECK CONSTRAINT [FK__GAMES_ACTIVITY_PLAYERID]
GO


ALTER TABLE [dbo].[GameActivities]  WITH CHECK ADD  CONSTRAINT [FK__GAMES_ACTIVITY_GAMEID] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([GameId])
GO

ALTER TABLE [dbo].[GameActivities] CHECK CONSTRAINT [FK__GAMES_ACTIVITY_GAMEID]
GO
