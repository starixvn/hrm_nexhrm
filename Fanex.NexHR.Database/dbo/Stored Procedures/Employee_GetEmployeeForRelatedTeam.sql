
CREATE PROCEDURE [dbo].[Employee_GetEmployeeForRelatedTeam] @empId INT
AS

/*  
Created: 2012/08/10@Quinn  
Task: Send mail for all Staff with related Team  
DB: NexHRM  
Original:   
  
Revisions:      
Param's Explanation:  
  
*/

BEGIN
    SET NOCOUNT ON;

    SELECT e.Id,
        e.Code,
        e.NickName,
        e.FullName,
        e.Email,
        e.DomainUsername
    FROM dbo.Employee AS e WITH (NOLOCK)
    WHERE E.ID IN
    (
        SELECT gg.EmpID
        FROM dbo.EmployeeGroup_Employee AS gg WITH (NOLOCK)
        WHERE gg.GroupId IN
        (
            SELECT eg.GroupId
            FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
            WHERE eg.EmpID = @empId
        )
    )
        AND ISNULL(e.isDeleted, 0) = 0;
END;
