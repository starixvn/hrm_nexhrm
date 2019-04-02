CREATE PROCEDURE [dbo].[ScheduledMessage_GetById]
	@ScheduledMessageId INT
AS
/*
Created: 20190325@Elton.ton
Task: Get message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,gc.GroupName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.GroupConversation gc WITH (NOLOCK) ON sm.ConversationId = gc.ConversationId
	WHERE sm.ScheduledMessageId = @ScheduledMessageId
END
