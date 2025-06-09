CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroupsByManagerID] 
    @managerID int
AS
/*
Created: 2012/05/18@Dan
Task: Get group of leader or assistant leader
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT g.[Id]
		,g.[Id]
        ,g.[Code]
        ,g.[Name]
        ,g.[Rank]
        ,g.[Deleted]
        ,g.ParentId
        ,g.[DepartmentId]
    FROM dbo.EmployeeGroup AS g WITH (NOLOCK)
	INNER JOIn dbo.GroupSupervisor AS gs WITH (NOLOCK) ON g.Id = gs.GroupId AND gs.IsDeleted = 0
    WHERE 
		gs.ApproverId = @managerID
		AND ISNULL(Deleted,0)=0
    ORDER BY [Name]
END

GO
