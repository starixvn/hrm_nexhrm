CREATE TABLE [dbo].[Conversation](
	[ConversationId] [int] IDENTITY(1,1) NOT NULL,
	[SkypeConversationId] [varchar](255) NOT NULL,
	[ConversationName] [nvarchar](255) NOT NULL,
	[CreatedBy] [INT] NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT ((1)),
	CONSTRAINT [PK_Conversation] PRIMARY KEY([ConversationId])
)