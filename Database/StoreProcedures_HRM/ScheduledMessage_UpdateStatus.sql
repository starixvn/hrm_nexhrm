USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[ScheduledMessage_UpdateStatus]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_UpdateStatus]
	@ScheduledMessageIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update selected messages are sent
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
	SET IsSent = 1
	WHERE EXISTS (
		SELECT 1
		FROM dbo.fnc_strSplit(@ScheduledMessageIds, ',')
		WHERE val = ScheduledMessageId
	)
END


GO
