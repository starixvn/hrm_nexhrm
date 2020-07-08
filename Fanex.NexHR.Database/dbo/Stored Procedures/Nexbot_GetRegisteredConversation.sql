ALTER PROCEDURE [dbo].[Nexbot_GetRegisteredConversation]
	@CategoryId INT
AS
/*
Created: 20200703@Merlin.Le
Task: Get ConversationIds, whom register receive remind message
DB: NexHR & NexBot
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT cr.ConversationId
	FROM [NexBot].[dbo].[ConversationRegister] AS cr WITH (NOLOCK)
		LEFT JOIN [NexBot].[dbo].[CategoryMessage] AS c WITH (NOLOCK) ON cr.CategoryId = c.CategoryId
	WHERE cr.CategoryId = @CategoryId AND ISNULL(c.IsActive, 0) <> 0
END
