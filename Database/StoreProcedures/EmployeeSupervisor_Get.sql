CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Get]
AS
/*
Created: 20240426@Lilac.Nguyen
Task: Get Employee Supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
   
		SELECT
            es.EmpId,
			em.NickName,
			em.FullName,
			STRING_AGG(e.NickName, ', ') AS Approvers
        FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			LEFT JOIN dbo.Employee AS em WITH (NOLOCK) ON em.Id = es.EmpId
			LEFT JOIN dbo.Employee AS e ON e.Id = es.ApproverId
        WHERE ISNULL(es.IsDeleted, 0) = 0
		GROUP BY es.EmpId, em.NickName, em.FullName
    
END
GO
