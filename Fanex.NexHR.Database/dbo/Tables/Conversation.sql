CREATE TABLE [dbo].[Conversation](
	[ConversationId] [varchar](255) NOT NULL,
	[ConversationName] [nvarchar](255) NOT NULL,
	[CreatedBy] [INT] NOT NULL,
	CONSTRAINT [PK_Conversation] PRIMARY KEY([ConversationId])
)