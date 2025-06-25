USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledMessage_GetMessagesToProcess]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_GetMessagesToProcess]
    @FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
Created: 20190325@Elton.ton
Task: Get message by SendDate
DB: NexHRM
Original:
    - 20250507@Henry: Update hardcode conversationId
Revisions:
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT sm.ScheduledMessageId
        ,sm.ConversationId
        ,sm.Content
        ,sm.CronExpression
        ,c.SkypeConversationId
    FROM dbo.ScheduledMessage sm WITH (NOLOCK)
        INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
    WHERE ((sm.SendDate BETWEEN @FromDate AND @ToDate
            AND sm.CronExpression IS NULL)
            OR (@ToDate >= sm.SendDate
            AND sm.CronExpression IS NOT NULL))
        AND sm.IsSent = 0
        AND CHARINDEX('B08J3UJ8WMA:' , c.SkypeConversationId ) = 1
END
GO
