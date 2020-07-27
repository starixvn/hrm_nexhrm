

CREATE PROCEDURE [dbo].[SkypeAccount_GetAllEmployee]
	@EmployeeId INT = NULL
	,@GroupId INT = NULL
	,@IsConfirmed INT = NULL

AS
/*
Created: 20180725@Oliver.Dang
Task: Get all Employee with Skype account map from Nexbot
DB: NexHR & NexBot
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.ConversationId
		  ,e.Id AS EmpId
		  ,e.Code 
		  ,e.FullName
		  ,e.NickName
		  ,g.Id AS GroupId
		  ,g.Code
		  ,g.Name
	FROM [dbo].Employee AS e WITH (NOLOCK)
	LEFT JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
	LEFT JOIN [dbo].EmployeeConversation AS u WITH (NOLOCK) ON e.iD = u.EmpId
	WHERE e.[Enabled] = 1 AND e.isDeleted = 0
		AND (@EmployeeId IS NULL OR @EmployeeId < 0 OR e.Id = @EmployeeId)
		AND (@GroupId IS NULL OR g.Id = @GroupId)
		AND (@IsConfirmed IS NULL
			 OR ((CASE WHEN u.EmpId IS NULL THEN 0 ELSE 1 END) = @IsConfirmed))
	ORDER BY g.[Rank]
			,g.Code
			,e.Code
END