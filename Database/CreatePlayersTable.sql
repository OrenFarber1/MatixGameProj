USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__PLAYERS__CreateT__20C1E124]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PLAYERS] DROP CONSTRAINT [DF__PLAYERS__CreateT__20C1E124]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PLAYERS]    Script Date: 05/27/2017 01:08:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PLAYERS]') AND type in (N'U'))
DROP TABLE [dbo].[PLAYERS]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[PLAYERS]    Script Date: 05/27/2017 01:08:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PLAYERS](
	[PlayerId] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[NickName] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PlayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PLAYERS] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO

