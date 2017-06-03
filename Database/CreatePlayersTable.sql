USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Players__CreateT__47DBAE45]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Players] DROP CONSTRAINT [DF__Players__CreateT__47DBAE45]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Players]    Script Date: 06/03/2017 22:17:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Players]') AND type in (N'U'))
DROP TABLE [dbo].[Players]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Players]    Script Date: 06/03/2017 22:17:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Players](
	[PlayerId] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[NickName] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](64) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PlayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Players] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO

