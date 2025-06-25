USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Conversation_GetByCreator]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Conversation_GetByCreator]
	@CreatedBy INT
AS
/*
Created: 20190412@elton.ton
Task: Get conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.SkypeConversationId
		,c.ConversationName
		,c.IsActive
	FROM HRM.dbo.Conversation c WITH (NOLOCK)
	WHERE c.CreatedBy = @CreatedBy

	SELECT cp.EmployeeId
		,e.NickName
		,cp.ConversationId
	FROM HRM.dbo.ConversationPermission cp WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON cp.EmployeeId = e.Id
	WHERE EXISTS (
		SELECT 1
		FROM HRM.dbo.Conversation c WITH (NOLOCK)
		WHERE c.CreatedBy = @CreatedBy AND c.ConversationId = cp.ConversationId
	)
END


GO
