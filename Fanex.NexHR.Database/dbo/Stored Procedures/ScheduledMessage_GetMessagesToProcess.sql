CREATE PROCEDURE [dbo].[ScheduledMessage_GetMessagesToProcess]
	@FromDate DATETIME
	,@ToDate DATETIME
AS
/*
Created: 20190325@Elton.ton
Task: Get message by SendDate
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,sm.Content
		,c.SkypeConversationId
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE sm.SendDate BETWEEN @FromDate AND @ToDate
		AND sm.IsSent = 0
END