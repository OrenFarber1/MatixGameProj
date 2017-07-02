USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__PLAYER_HISTORY_GAMEID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PlayersHistory]'))
ALTER TABLE [dbo].[PlayersHistory] DROP CONSTRAINT [FK__PLAYER_HISTORY_GAMEID]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__PLAYER_HISTORY_PLAYERID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PlayersHistory]'))
ALTER TABLE [dbo].[PlayersHistory] DROP CONSTRAINT [FK__PLAYER_HISTORY_PLAYERID]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PlayersHistory]    Script Date: 06/03/2017 22:25:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlayersHistory]') AND type in (N'U'))
DROP TABLE [dbo].[PlayersHistory]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PlayersHistory]    Script Date: 06/03/2017 22:25:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayersHistory](
	[HistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[HistoryTime] [datetime] NOT NULL,
	[PlayerId] [bigint] NOT NULL,
	[GameId] [bigint] NOT NULL,
	[Win] [bit] NOT NULL,
	[Score] [int] NOT NULL,
 CONSTRAINT [PK_PlayersHistory] PRIMARY KEY CLUSTERED 
(
	[HistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PlayersHistory]  WITH CHECK ADD  CONSTRAINT [FK__PLAYER_HISTORY_GAMEID] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([GameId])
GO

ALTER TABLE [dbo].[PlayersHistory] CHECK CONSTRAINT [FK__PLAYER_HISTORY_GAMEID]
GO

ALTER TABLE [dbo].[PlayersHistory]  WITH CHECK ADD  CONSTRAINT [FK__PLAYER_HISTORY_PLAYERID] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Players] ([PlayerId])
GO

ALTER TABLE [dbo].[PlayersHistory] CHECK CONSTRAINT [FK__PLAYER_HISTORY_PLAYERID]
GO

