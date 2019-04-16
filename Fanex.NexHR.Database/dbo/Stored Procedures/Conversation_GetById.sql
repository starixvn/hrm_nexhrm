CREATE PROCEDURE [dbo].[Conversation_GetById]
	@ConversationId INT
AS
/*
Created: 20190412@elton.ton
Task: Get conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.SkypeConversationId
		,c.ConversationName
		,c.IsActive
	FROM dbo.Conversation c WITH (NOLOCK)
	WHERE c.ConversationId = @ConversationId

	SELECT cp.EmployeeId
		,e.NickName
		,cp.ConversationId
	FROM dbo.ConversationPermission cp WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON cp.EmployeeId = e.Id
	WHERE cp.ConversationId = @ConversationId
END

