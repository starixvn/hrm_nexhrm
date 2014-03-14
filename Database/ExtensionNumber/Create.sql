USE [NexHRM]
GO

/****** Object:  Table [dbo].[ExtensionNumbers]    Script Date: 3/14/2014 9:19:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExtensionNumbers](
	[ExtID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](200) NULL,
	[NickName] [nvarchar](100) NULL,
	[ExtensionNo] [nvarchar](200) NULL,
	[IsInternal] [bit] NULL,
	[IsGroup] [bit] NULL,
	[EmpID] [int] NULL,
	[GroupID] [int] NULL,
	[OrderNo] [smallint] NULL,
 CONSTRAINT [PK_ExtensionNumbers] PRIMARY KEY CLUSTERED 
(
	[ExtID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ExtensionNumbers] ADD  CONSTRAINT [DF_ExtensionNumbers_IsMembers]  DEFAULT ((0)) FOR [IsInternal]
GO

ALTER TABLE [dbo].[ExtensionNumbers] ADD  CONSTRAINT [DF_ExtensionNumbers_IsGroup]  DEFAULT ((0)) FOR [IsGroup]
GO
