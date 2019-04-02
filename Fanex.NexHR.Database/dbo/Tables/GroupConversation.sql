CREATE TABLE [dbo].[GroupConversation](
	[ConversationId] [varchar](255) NOT NULL,
	[GroupName] [nvarchar](255) NOT NULL,
	CONSTRAINT [PK_GroupConversation] PRIMARY KEY([ConversationId])
)