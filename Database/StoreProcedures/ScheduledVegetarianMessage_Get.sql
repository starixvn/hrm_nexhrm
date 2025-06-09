USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledVegetarianMessage_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Stored Procedure dbo.ScheduledVegetarianMessage_Get    Script Date: 3/31/2023 8:22:45 AM ******/

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
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.Content LIKE N'%Ð?t com chay%'
	ORDER BY sm.SendDate DESC
END

GO
