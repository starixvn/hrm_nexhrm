USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledMessage_Update]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Update]
	@ScheduledMessageId INT
	,@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update message
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF (@RepeatTypeId IS NULL)
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = null
		  ,[StepNumber] = null
		  ,[RepeatTypeId] = null
		WHERE ScheduledMessageId = @ScheduledMessageId
	ELSE
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = @CronExpression
		  ,[StepNumber] = @StepNumber
		  ,[RepeatTypeId] = @RepeatTypeId
		WHERE ScheduledMessageId = @ScheduledMessageId
END


GO
