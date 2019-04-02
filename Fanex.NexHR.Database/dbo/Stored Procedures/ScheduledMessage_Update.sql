CREATE PROCEDURE [dbo].[ScheduledMessage_Update]
	@ScheduledMessageId INT
	,@ConversationId VARCHAR(255)
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
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

	UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
	SET [ConversationId] = @ConversationId
      ,[SendDate] = @SendDate
      ,[Content] = @Content
	WHERE ScheduledMessageId = @ScheduledMessageId
END