CREATE PROCEDURE [dbo].[Conversation_GetById]
	@Id INT
AS
/*
Created: 20190412@elton.ton
Task: Get conversation by Id
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.Id
		,c.ConversationId
		,c.ConversationName
	FROM dbo.Conversation c WITH (NOLOCK)
	WHERE c.Id = @Id

	SELECT cp.EmployeeId
		,e.NickName
		,cp.ConversationId
	FROM dbo.ConversationPermission cp WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON cp.EmployeeId = e.Id
	WHERE cp.ConversationId = @Id
END

