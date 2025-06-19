CREATE OR ALTER PROCEDURE [dbo].[Report_LateInEarlyOutSummary] @FromDate DATE
    ,@ToDate DATE
    ,@GroupId INT = NULL
    ,@EmpId INT = NULL
    ,@IsUserWarning BIT = NULL
AS
/*
    Created: 20160219@Tim.Trinh
    Task: Get late in/ early out summary report
    DB: NexHRM
    Revisions: 
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Original: 

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @CoreTeamId INT;
	DECLARE @ExpId INT;
	SELECT @ExpId = MAX(CASE WHEN ss.Name = 'EXPCode' THEN gs.GroupId END),
		@CoreTeamId = MAX(CASE WHEN ss.Name = 'CoreTeamCode' THEN gs.GroupId END)
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name IN ('EXPCode', 'CoreTeamCode')
	WHERE gs.OldGroupCode = ss.Value

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
        AND e.GroupId NOT IN (@CoreTeamId, @ExpId) -- management, expat
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
    WHERE e.GroupId NOT IN (@CoreTeamId, @ExpId) -- management, expat
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
		,e.Roles
        ,e.GroupDate
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    SELECT EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary
    ORDER BY EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;

GO
