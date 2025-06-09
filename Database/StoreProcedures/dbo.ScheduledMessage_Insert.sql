USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledMessage_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Insert]
	@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CreatedBy INT
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: 
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF (@RepeatTypeId IS NULL)
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy])
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,GETDATE()
           ,@CreatedBy)
	ELSE
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy]
		   ,[CronExpression]
		   ,[StepNumber]
		   ,[RepeatTypeID])
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,GETDATE()
           ,@CreatedBy
		   ,@CronExpression
		   ,@StepNumber
		   ,@RepeatTypeId)
END;


GO
