USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Conversation_Get]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Conversation_Get]
	@EmployeeId INT
AS
/*
Created: 2019@elton.ton
Task: Get group conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.ConversationName
	FROM HRM.dbo.Conversation c WITH (NOLOCK)
	WHERE c.IsActive = 1
		AND (c.CreatedBy = @EmployeeId 
				OR EXISTS (
					SELECT 1
					FROM HRM.dbo.ConversationPermission cp WITH (NOLOCK)
					WHERE c.ConversationId = cp.ConversationId
						AND cp.EmployeeId = @EmployeeId
				)
		)
END

GO
