CREATE TABLE [dbo].[ScheduledMessage](
	[ScheduledMessageId] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [varchar](255) NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_ScheduledMessage] PRIMARY KEY ([ScheduledMessageId])
)
