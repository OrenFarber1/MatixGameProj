USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__GAMES__CreateTim__1BFD2C07]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GAMES] DROP CONSTRAINT [DF__GAMES__CreateTim__1BFD2C07]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[GAMES]    Script Date: 05/27/2017 01:08:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GAMES]') AND type in (N'U'))
DROP TABLE [dbo].[GAMES]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[GAMES]    Script Date: 05/27/2017 01:08:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GAMES](
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

ALTER TABLE [dbo].[GAMES] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO

