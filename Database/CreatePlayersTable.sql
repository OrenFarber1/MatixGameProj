USE [MATIX_GAME]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Players__CreateT__4F7CD00D]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Players] DROP CONSTRAINT [DF__Players__CreateT__4F7CD00D]
END

GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Players]    Script Date: 07/12/2017 19:02:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Players]') AND type in (N'U'))
DROP TABLE [dbo].[Players]
GO

USE [MATIX_GAME]
GO

/****** Object:  Table [dbo].[Players]    Script Date: 07/12/2017 19:02:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Players](
	[PlayerId] [bigint] IDENTITY(10,1) NOT NULL,
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

USE [MATIX_GAME]
GO


/****** Object:  Index [IX_Players_Email]    Script Date: 07/12/2017 19:16:28 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Players_Email] ON [dbo].[Players] 
(
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [MATIX_GAME]
GO

/****** Object:  Index [IX_Players_NickName]    Script Date: 07/12/2017 19:16:59 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Players_NickName] ON [dbo].[Players] 
(
	[NickName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


/****** Script for SelectTopNRows command from SSMS  ******/
SET IDENTITY_INSERT [MATIX_GAME].[dbo].[Players] ON

insert into [MATIX_GAME].[dbo].[Players](PlayerId, CreateTime, FirstName, LastName, NickName, PasswordHash, Email)
values(1,GETDATE(), 'M1', 'Server','Matix-1', 'B995219E68D2F3EC29F34D62CDDBD826CFC8096C9D3087901865C08FF5A0347E', 'm1@matix.com'  )


SET IDENTITY_INSERT [MATIX_GAME].[dbo].[Players] ON