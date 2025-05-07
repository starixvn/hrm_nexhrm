SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[EmployeeConversation_GetTeamHr]
AS
/*
	Created: 2023@Brian.nguyen
	Task: Get group conversations team human resources
	DB: NexHRM
	Revisions: 
		20250507@Henry.Dinh: update group id from 38 to 1104
	Original: 

*/
BEGIN
	SET NOCOUNT ON; 

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:1KmVXImsgANnLEyWFPGem8Y4K4fMFvYN-tE_QfuS0SfASNxMSmouUgyega3ebG6yZ' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		AND GroupId = 1104

END

GO
