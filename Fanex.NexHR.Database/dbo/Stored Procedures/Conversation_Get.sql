

CREATE PROCEDURE [dbo].[Conversation_Get]
	@EmployeeId INT
AS
/*
Created: 2019@elton.ton
Task: Get group conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.ConversationName
	FROM dbo.Conversation c WITH (NOLOCK)
	WHERE c.IsActive = 1
		AND (c.CreatedBy = @EmployeeId 
				OR EXISTS (
					SELECT 1
					FROM dbo.ConversationPermission cp WITH (NOLOCK)
					WHERE c.ConversationId = cp.ConversationId
						AND cp.EmployeeId = @EmployeeId
				)
		)
END
