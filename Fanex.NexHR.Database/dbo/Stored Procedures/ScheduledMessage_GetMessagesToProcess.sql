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

	SELECT sm.ConversationId
		,sm.Content
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
	WHERE sm.SendDate BETWEEN @FromDate AND @ToDate
END