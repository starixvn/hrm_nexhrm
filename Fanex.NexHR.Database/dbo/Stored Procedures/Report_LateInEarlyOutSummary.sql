
/****** Object:  Stored Procedure dbo.Report_LateInEarlyOutSummary    Script Date: 3/13/2016 3:21:43 PM ******/

-- [Report_LateInEarlyOutSummary] 2015-1-1, 2015-12-31
CREATE PROCEDURE [dbo].[Report_LateInEarlyOutSummary] @FromDate DATE
    ,@ToDate DATE
    ,@GroupId INT = NULL
    ,@EmpId INT = NULL
    ,@IsUserWarning BIT = NULL
AS
/*
Created: 20160219@Tim.Trinh
Task: Get late in/ early out summary report
DB: NexHRM
Original: 

*/
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutSummary;
    END;

    CREATE TABLE #LateInEarlyOutSummary (
        EmpId INT
        ,[Month] INT
        ,[Year] INT
        ,TotalTimes INT
        );

    INSERT INTO #LateInEarlyOutSummary
    SELECT DISTINCT tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
        ,COUNT(*)
    FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (11, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (11, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @IsUserWarning IS NULL
            OR @IsUserWarning = 0
            OR (
                @IsUserWarning = 1
                AND e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
                    )
                )
            )
    GROUP BY e.Id
        ,e.NickName
        ,e.FullName
        ,e.Code
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]
        ,g.LeaderId
        ,g.AssistantLeaderId
    ORDER BY g.[Rank]
        ,g.Code
        ,(CASE e.Id
            WHEN g.LeaderId THEN 2
            WHEN g.AssistantLeaderId THEN 1
            ELSE 0 END
            ) DESC
        ,e.Code;

    SELECT EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary
    ORDER BY EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;

