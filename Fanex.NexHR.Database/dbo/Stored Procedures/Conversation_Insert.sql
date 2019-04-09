CREATE PROCEDURE [dbo].[Conversation_Insert]
	@ConversationId VARCHAR(255)
	,@ConversationName NVARCHAR(MAX)
	,@CreatedBy INT
	,@PermittedEmployeeIds VARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190409@Elton.ton
Task: create new conversation
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) WHERE c.ConversationId = @ConversationId)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The conversation is already existed.'
		RETURN
	END

	INSERT INTO [dbo].[Conversation]
           ([ConversationId]
           ,[ConversationName]
           ,[CreatedBy])
     VALUES
           (@ConversationId
           ,@ConversationName
           ,@CreatedBy)

	IF @PermittedEmployeeIds IS NOT NULL
	BEGIN

		INSERT INTO [dbo].[ConversationPermission]
           ([ConversationId]
           ,[EmployeeId])
		SELECT @ConversationId
			,val
		FROM dbo.fnc_strSplit(@PermittedEmployeeIds, ',')
	END
END