USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__PLAYERS_LOGIN_PLAYERID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PlayersLogin]'))
ALTER TABLE [dbo].[PlayersLogin] DROP CONSTRAINT [FK__PLAYERS_LOGIN_PLAYERID]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__PLAYERS_L__Creat__30F848ED]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PlayersLogin] DROP CONSTRAINT [DF__PLAYERS_L__Creat__30F848ED]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PlayersLogin]    Script Date: 06/03/2017 22:26:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlayersLogin]') AND type in (N'U'))
DROP TABLE [dbo].[PlayersLogin]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PlayersLogin]    Script Date: 06/03/2017 22:26:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayersLogin](
	[LoginId] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerId] [bigint] NOT NULL,
	[LoginTime] [datetime] NOT NULL,
	[IPAddress] [nvarchar](50) NOT NULL,
	[LogoutTime] [datetime] NULL,
	[Reason] [nvarchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[LoginId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PlayersLogin]  WITH CHECK ADD  CONSTRAINT [FK__PLAYERS_LOGIN_PLAYERID] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Players] ([PlayerId])
GO

ALTER TABLE [dbo].[PlayersLogin] CHECK CONSTRAINT [FK__PLAYERS_LOGIN_PLAYERID]
GO

ALTER TABLE [dbo].[PlayersLogin] ADD  DEFAULT (getdate()) FOR [LoginTime]
GO

