CREATE PROCEDURE [dbo].[Conversation_Get]
AS
/*
Created: 20190320@elton.ton
Task: Get group conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT ConversationId
		,ConversationName
	FROM dbo.Conversation WITH (NOLOCK)
END
