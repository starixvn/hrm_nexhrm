CREATE PROCEDURE [dbo].[ScheduledMessage_Get]
	@YEAR INT
	,@CreatedBy INT
AS
/*
Created: 20190325@Elton.ton
Task: Get messages by year
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.CreatedBy = @CreatedBy
	ORDER BY sm.SendDate DESC
END