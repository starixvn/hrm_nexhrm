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
			STRING_AGG(
				e.NickName + 
				CASE 
					WHEN es.OuterCompanyName IS NOT NULL THEN ' (' + es.OuterCompanyName + ')'
					ELSE ''
				END,
				', '
			) AS Approvers
        FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			LEFT JOIN dbo.Employee AS em WITH (NOLOCK) ON em.Id = es.EmpId
			LEFT JOIN bodb_HRM.dbo.Employee AS e ON e.EmpId = es.ApproverId
        WHERE ISNULL(es.IsDeleted, 0) = 0
		GROUP BY es.EmpId, em.NickName, em.FullName
END
GO
