USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledMessage_Delete]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Delete]
	@ScheduledMessageId INT
	 ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: delete message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DELETE
	FROM dbo.ScheduledMessage
	WHERE ScheduledMessageId = @ScheduledMessageId
END


GO
