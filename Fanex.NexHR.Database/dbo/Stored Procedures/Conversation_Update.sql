CREATE PROCEDURE [dbo].[Conversation_Update]
	@ConversationId INT
	,@SkypeConversationId VARCHAR(255)
	,@ConversationName NVARCHAR(255)
	,@PermittedEmployeeIds VARCHAR(MAX) = NULL
	,@EditedBy INT
	,@IsActive BIT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190409@Elton.ton
Task: update conversation
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) WHERE c.SkypeConversationId = @SkypeConversationId AND c.ConversationId <> @ConversationId)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The conversation is already existed.'
		RETURN
	END

	IF NOT EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) WHERE c.ConversationId = @ConversationId AND c.CreatedBy = @EditedBy)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission to edit this conversation.'
		RETURN
	END

	UPDATE dbo.Conversation WITH (ROWLOCK, UPDLOCK)
	SET SkypeConversationId = @SkypeConversationId
		,ConversationName = @ConversationName
		,IsActive = @IsActive
	WHERE ConversationId = @ConversationId

	SELECT val AS EmployeeId
	INTO #permittedEmployee
	FROM dbo.fnc_strSplit(@PermittedEmployeeIds, ',')

	INSERT INTO [dbo].[ConversationPermission]
           ([ConversationId]
           ,[EmployeeId])
	SELECT @ConversationId
		,pe.EmployeeId
	FROM #permittedEmployee pe
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.ConversationPermission cp WITH (NOLOCK)
		WHERE cp.ConversationId = @ConversationId AND cp.EmployeeId = pe.EmployeeId
	)

	DELETE cp
	FROM dbo.ConversationPermission cp
	WHERE ConversationId = @ConversationId
		AND NOT EXISTS (
			SELECT 1
			FROM #permittedEmployee pe
			WHERE cp.EmployeeId = pe.EmployeeId
		)

	DROP TABLE #permittedEmployee
END;