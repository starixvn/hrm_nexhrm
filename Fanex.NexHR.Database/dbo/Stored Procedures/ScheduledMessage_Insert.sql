CREATE PROCEDURE [dbo].[ScheduledMessage_Insert]
	@ConversationId VARCHAR(255)
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: 
DB: NexHRM
Original: 
*/
BEGIN
    SET NOCOUNT ON;

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
END