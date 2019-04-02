CREATE PROCEDURE [dbo].[GroupConversation_Get]
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
		,GroupName
	FROM dbo.GroupConversation WITH (NOLOCK)
END
