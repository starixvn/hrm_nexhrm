

CREATE PROCEDURE [dbo].[Employee_UpdateSkypeAccount] @employeeId INT
	,@ConversationId VARCHAR(255)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190312@Quinn.Vo
Task: Update Skype Account
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeConversation WITH (NOLOCK)
				WHERE EmpId = @employeeId
				)
			)
	BEGIN
		UPDATE dbo.EmployeeConversation WITH (UPDLOCK, ROWLOCK)
		SET ConversationId = @ConversationId
		WHERE EmpId = @employeeId
	END
	ELSE
	BEGIN
		INSERT dbo.EmployeeConversation (
			EmpId
			,ConversationId
			)
		VALUES (
			@employeeId
			,@ConversationId
			)
	END
END