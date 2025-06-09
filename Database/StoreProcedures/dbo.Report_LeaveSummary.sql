USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Report_LeaveSummary]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Report_LeaveSummary    Script Date: 3/13/2016 3:21:43 PM ******/

/*
declare @empid int, @groupid int,
        @todate datetime, @year datetime
set @todate = '2012/06/1'
exec Report_LeaveSummary @empid , @groupid, 2012
*/
CREATE OR ALTER PROCEDURE [dbo].[Report_LeaveSummary] @empid INT = NULL
    ,@groupid INT = NULL
    ,@year INT
AS
/*
Created: 2012/04/24@Dan
Task: leave summary report
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    DECLARE @fromdate DATETIME
        ,@todate DATETIME

    -- get the first date of year
    SET @fromdate = cast(@year AS VARCHAR(10)) + '/01/01'
    SET @todate = DATEADD(yy, 1, @fromdate)

    -- get the openning balace for annual leave to first date of year
    SELECT e.Id AS Empid
        ,SUM(ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0)) AS OpenBalance
    INTO #OpenBalance
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT *
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND FromDate < @fromdate
        ) t ON e.Id = t.Empid
    --LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId
    WHERE -- (FromDate < @fromdate OR FromDate IS NULL) 
        (
            @EmpId IS NULL
            OR e.id = @empid
            )
        -- AND ISNULL(t.Deleted,0)=0
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            ISNULL(e.isDeleted, 0) = 0
            OR YEAR(ResignedDate) >= year(@todate)
            )
    GROUP BY e.Id

    -- select * from #OpenBalance order by Empid
    -- calculate the balance form the first date of year to @todate
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
        SELECT *
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND FromDate >= @fromdate
            AND FromDate < @todate
        ) t
        -- FROM dbo.LeaveTrans t WITH (NOLOCK)    
        --    JOIN Employee e WITH (NOLOCK) 
        ON e.Id = t.Empid
    WHERE (
            @EmpId IS NULL
            OR e.id = @empid
            )
        -- AND ISNULL(t.Deleted,0)=0
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            ISNULL(e.isDeleted, 0) = 0
            OR YEAR(ResignedDate) >= year(@todate)
            )
    GROUP BY e.ID

    --  select * from #CurrenBalace order by Empid
    -- show the report
    SELECT *
    FROM (
        SELECT cb.EmpId
            ,e.Code
            ,FullName
            ,NickName
			,GroupDate
			,Roles
            ,g.Id AS TeamId
            ,g.Code AS TeamCode
            ,g.NAME AS TeamName
            ,g.[Rank]
            ,ConfirmedDate
            ,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
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
            ,
            --        ISNULL(DeductDays,0) LeaveTaken,
            ISNULL(OpenBalance, 0) + ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS LeaveBalance
            ,'' AS Remark
        FROM #CurrenBalace cb
            LEFT JOIN #OpenBalance op ON OP.empid = cb.empid
            JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        
        UNION ALL
        SELECT op.EmpId
            ,e.Code
            ,FullName
            ,NickName
			,Roles
			,GroupDate
            ,g.Id AS TeamId
            ,g.Code AS TeamCode
            ,g.NAME AS TeamName
            ,g.[Rank]
            ,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
            ,0 AS [AnnualLeaveCurrentYear]
            ,0 AS Jan
            ,0 AS Feb
            ,0 AS Mar
            ,0 AS Apr
            ,0 AS May
            ,0 AS Jun
            ,0 AS Jul
            ,0 AS Aug
            ,0 AS Sep
            ,0 AS Oct
            ,0 AS Nov
            ,0 AS DEC
            ,ISNULL(OpenBalance, 0) AS LeaveBalance
            ,'' AS Remark
        FROM #OpenBalance op
            JOIN Employee e WITH (NOLOCK) ON e.Id = op.Empid
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        WHERE Empid NOT IN (
                SELECT EmpID
                FROM #CurrenBalace cb
                )
        ) A
    ORDER BY a.[Rank]
        ,a.TeamCode
        ,a.Roles DESC
        ,a.GroupDate

    DROP TABLE #OpenBalance

    DROP TABLE #CurrenBalace
END

GO
