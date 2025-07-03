CREATE OR ALTER   PROCEDURE [dbo].[Employee_GetByDomainName] @DomainUsername VARCHAR(50)
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by domain username.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.[Id]
	,e.[Code]
	,e.[FullName]
	,e.[NickName]
	,e.[Email]
	,e.[Enabled]
	,e.[IsDeleted]
	,e.[Roles]
	,ISNULL(g.Id, 0) AS TeamId
	,g.Code AS TeamCode
	,g.GroupTeamId
	,CAST(CASE WHEN e.Id = 125 OR e.Id = 142 THEN 1 ELSE 0 END AS BIT) AS IsLeaderHR
	FROM dbo.Employee AS e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE e.DomainUsername = @DomainUsername
	AND e.isDeleted = 0
END

