CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Get]
AS
/*
Created: 20240423@Lilac.Nguyen
Task: Get group supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT
		eg.Id AS GroupId,
		eg.Code AS GroupCode,
		eg.[Name] AS GroupName,
		STRING_AGG(e.NickName, ', ') AS Approvers
	FROM dbo.EmployeeGroup AS eg WITH (NOLOCK)
	LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
		ON eg.Id = gs.GroupId AND ISNULL(gs.IsDeleted, 0) = 0
	LEFT JOIN dbo.Employee AS e 
		ON e.Id = gs.ApproverId
	WHERE ISNULL(eg.Deleted, 0) = 0
	AND eg.IsSpecialTeam = 0
	GROUP BY eg.Id, eg.Code, eg.[Name];
    
END
GO
