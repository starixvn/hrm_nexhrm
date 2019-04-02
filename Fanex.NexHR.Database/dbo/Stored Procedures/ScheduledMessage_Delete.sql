CREATE PROCEDURE [dbo].[ScheduledMessage_Delete]
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