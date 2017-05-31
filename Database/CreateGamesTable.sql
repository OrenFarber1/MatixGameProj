USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__GAMES_H_PLAYERID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Games]'))
ALTER TABLE [dbo].[Games] DROP CONSTRAINT [FK__GAMES_H_PLAYERID]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__GAMES_V_PLAYERID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Games]'))
ALTER TABLE [dbo].[Games] DROP CONSTRAINT [FK__GAMES_V_PLAYERID]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Games__CreateTim__3B75D760]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Games] DROP CONSTRAINT [DF__Games__CreateTim__3B75D760]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Games]    Script Date: 05/31/2017 21:28:35 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Games]') AND type in (N'U'))
DROP TABLE [dbo].[Games]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Games]    Script Date: 05/31/2017 21:28:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Games](
	[GameId] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[HorizontalPlayerId] [bigint] NOT NULL,
	[VerticalPlayerId] [bigint] NOT NULL,
	[CellsMatrix] [xml] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Games]  WITH CHECK ADD  CONSTRAINT [FK__GAMES_H_PLAYERID] FOREIGN KEY([HorizontalPlayerId])
REFERENCES [dbo].[Players] ([PlayerId])
GO

ALTER TABLE [dbo].[Games] CHECK CONSTRAINT [FK__GAMES_H_PLAYERID]
GO

ALTER TABLE [dbo].[Games]  WITH CHECK ADD  CONSTRAINT [FK__GAMES_V_PLAYERID] FOREIGN KEY([VerticalPlayerId])
REFERENCES [dbo].[Players] ([PlayerId])
GO

ALTER TABLE [dbo].[Games] CHECK CONSTRAINT [FK__GAMES_V_PLAYERID]
GO

ALTER TABLE [dbo].[Games] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO

