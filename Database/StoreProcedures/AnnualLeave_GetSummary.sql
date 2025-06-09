CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetSummary] @year INT
    ,@employeeId INT = NULL
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@minAnnualBalance INT = 0
AS
/*
Created: 20160222@Quinn.Vo
Task: Get annual leave summary.
DB: NexHR
Original: 
Revisions:
    - 20250516@Henry.Dinh: add department id filter
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    SELECT e.Id AS EmpId
        ,SUM(ISNULL(CreditDays, 0)) CreditDays
        ,SUM(ISNULL(DeductDays, 0)) DeductDays
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 1 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jan
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 2 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Feb
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 3 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Mar
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 4 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Apr
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 5 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) May
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 6 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jun
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 7 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jul
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 8 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Aug
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 9 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Sep
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 10 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Oct
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 11 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Nov
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 12 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) DEC
    INTO #CurrenBalace
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT EmpId
            ,CreditDays
            ,DeductDays
            ,FromDate
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND YEAR(FromDate) = @year
        ) t ON e.Id = t.Empid
    WHERE e.IsSpecialUser = 0
		AND (
            @employeeId IS NULL
            OR e.id = @employeeId
            )
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR e.groupId IN (SELECT Id FROM EmployeeGroup WHERE DepartmentId = @departmentId)
            )
        AND (ISNULL(e.isDeleted, 0) = 0)
    GROUP BY e.ID

    SELECT 
        ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
        ,Jan
        ,Feb
        ,Mar
        ,Apr
        ,May
        ,Jun
        ,Jul
        ,Aug
        ,Sep
        ,Oct
        ,Nov
        ,DEC
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS LeaveBalance
        ,cb.EmpId
        ,e.Code
        ,FullName
        ,NickName
        ,d.DepartmentId
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
    FROM #CurrenBalace cb
        INNER JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        INNER JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
     WHERE 
		e.IsSpecialUser = 0
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    DROP TABLE #CurrenBalace
END
GO
