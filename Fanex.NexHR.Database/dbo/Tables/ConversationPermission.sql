CREATE TABLE [dbo].[ConversationPermission](
	[ConversationId] [varchar](255) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	CONSTRAINT [PK_ConversationPermission] PRIMARY KEY CLUSTERED ([ConversationId], [EmployeeId])
)