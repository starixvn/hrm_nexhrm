

CREATE PROCEDURE [dbo].[AnnualLeave_GetSummary] @year INT
    ,@employeeId INT = NULL
    ,@groupId INT = NULL
    ,@minAnnualBalance INT = 0
AS
/*
Created: 20160222@Quinn.Vo
Task: Get annual leave summary.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

  --  SELECT e.Id AS Empid
  --      ,SUM(ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0)) AS OpenBalance
  --  INTO #OpenBalance
  --  FROM Employee e WITH (NOLOCK)
  --      LEFT JOIN (
  --      SELECT EmpId
  --          ,CreditDays
  --          ,DeductDays
  --      FROM dbo.LeaveTrans WITH (NOLOCK)
  --      WHERE ISNULL(Deleted, 0) = 0
  --          AND YEAR(FromDate) = @year - 1
  --      ) t ON e.Id = t.Empid
  --  WHERE e.IsSpecialUser = 0
		--AND 
		--	(
  --          @employeeId IS NULL
  --          OR e.id = @employeeId
  --          )
  --      AND (
  --          @groupId IS NULL
  --          OR e.groupId = @groupId
  --          )
  --      AND (ISNULL(e.isDeleted, 0) = 0)
  --  GROUP BY e.Id

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
        AND (ISNULL(e.isDeleted, 0) = 0)
    GROUP BY e.ID

    SELECT (CASE e.Id
            WHEN g.LeaderId THEN 2
            WHEN g.AssistantLeaderId THEN 1
            ELSE 0 END
            ) AS EmployeeRank
        --,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
        ,ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
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
        ,e.GroupId
        ,g.Code
        ,g.NAME
    FROM #CurrenBalace cb
        --LEFT JOIN #OpenBalance op ON OP.empid = cb.empid
        INNER JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
     WHERE 
	 e.IsSpecialUser = 0
		AND (ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0)) >= @minAnnualBalance
    ORDER BY g.[Rank]
        ,g.Code
        ,EmployeeRank DESC
        ,e.code

    DROP TABLE #CurrenBalace
END
