CREATE TABLE [dbo].[ConversationPermission](
	[ConversationId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	CONSTRAINT [PK_ConversationPermission] PRIMARY KEY CLUSTERED ([ConversationId], [EmployeeId])
)