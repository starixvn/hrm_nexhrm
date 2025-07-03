
ALTER PROCEDURE [dbo].[Employee_Get] @FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all employee from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(e.OldId, e.Id) AS EmpId
        ,'' AS [Password]
        ,e.Code
        ,e.NickName
        ,e.FullName
        ,e.EmployeeCard
        ,e.DomainUsername AS DomainName
        ,e.[Enabled] AS IsEnabled
        ,ISNULL(e.isDeleted, 0) AS IsDeleted
        ,e.JoinedDate
        ,e.GroupId AS TeamId
        ,CASE 
            WHEN e.ModifiedDate IS NULL THEN e.CreatedDate
            ELSE e.ModifiedDate
            END AS ModifiedDate
    FROM dbo.Employee AS e WITH (NOLOCK)
    WHERE (@FromDate IS NULL
        OR e.ModifiedDate > @FromDate)
		AND e.IsSpecialUser = 0
END

