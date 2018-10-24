
CREATE PROCEDURE [dbo].[Employee_GetLeaders] @employeeId INT
AS
/*
Created: 2012/05/22@Dan
Task: get leader and assistant leader that can handle request for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.GroupID
        ,e.Email
        ,(CASE 
            WHEN e.ID = g.LeaderId THEN 0
            ELSE 1 END
            ) AS OrderNo
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
        LEFT JOIN dbo.EmployeeGroup AS g ON eg.Groupid = g.ID
        INNER JOIN Employee AS e WITH (NOLOCK) ON (
            e.ID = g.LeaderId
            AND g.LeaderId <> @employeeId
            )
        OR (
            e.ID = g.AssistantLeaderId
            AND g.AssistantLeaderId <> @employeeId
            )
    WHERE eg.EmpID = @employeeId
        AND ISNULL(HandleRequest, 0) = 1
        AND (ISNULL(LeaderId, 0) <> @employeeId)
        AND e.Id <> @employeeId
    ORDER BY (CASE 
            WHEN e.ID = g.LeaderId THEN 0
            ELSE 1 END
            )
END

