CREATE PROCEDURE [dbo].[HRM_GetRedmineUsers]
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Bio Device.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id AS EmpId
		  ,e.FullName
		  ,e.NickName
		  ,r.Id AS RedmineId
		  ,c.ConversationId
	FROM [NexHRM].[dbo].[EmployeeSetting] AS s WITH (NOLOCK)
		INNER JOIN [NexHRM].[dbo].[Employee] AS e WITH (NOLOCK) ON s.EmpId = e.Id
		INNER JOIN [RedmineNexcel].[dbo].[Users] AS r WITH (NOLOCK) ON r.[Login] = REPLACE(e.DomainUsername, 'spvn\','')
		LEFT JOIN [NexBot].[dbo].[ConversationUser] AS c WITH (NOLOCK) ON e.NickName = c.NickName
	WHERE e.[Enabled] = 1 
		AND e.isDeleted = 0
		AND s.DisplayRedmineTask = 1
END
