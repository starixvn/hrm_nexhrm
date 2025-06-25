CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_GetById]
	@GroupId INT

AS
/*
Created: 20240613@Lilac.Nguyen
Task: get approver by groupid.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
	  
	SELECT
		eg.Id AS GroupId,
		eg.[Name] AS GroupName,
		STRING_AGG(e.Id, ', ') AS ApproverIds
	FROM dbo.EmployeeGroup AS eg WITH (NOLOCK)
	LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
		ON eg.Id = gs.GroupId AND ISNULL(gs.IsDeleted, 0) = 0
	LEFT JOIN dbo.Employee AS e 
		ON e.Id = gs.ApproverId
	WHERE ISNULL(eg.Deleted, 0) = 0	AND eg.Id = @GroupId
	GROUP BY eg.Id, eg.Code, eg.[Name];

END
GO
