CREATE OR ALTER PROCEDURE [dbo].[ScheduledVegetarianMessage_Get]
	@YEAR INT
AS
/*
Created: 20233103@Brian.nguyen
Task: Get Vegetarian messages by year
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
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.Content LIKE N'%�?t com chay%'
	ORDER BY sm.SendDate DESC
END

GO
