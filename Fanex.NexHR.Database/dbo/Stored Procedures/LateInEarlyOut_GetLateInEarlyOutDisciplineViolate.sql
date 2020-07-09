ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutDisciplineViolate] 
	@FromDate DATE
    ,@ToDate DATE
AS
/*
Created: 20200709@Merlin.Le
Task: Get late in/ early out violate disciplines
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
    FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (11, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND (CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate)
       
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM [dbo].Employee AS e WITH (NOLOCK)
        INNER JOIN [dbo].EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (11, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND  (e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
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

	HAVING COUNT(tr.TotalTimes) >=3
    ORDER BY g.[Rank]
        ,g.Code
        ,(CASE e.Id
            WHEN g.LeaderId THEN 2
            WHEN g.AssistantLeaderId THEN 1
            ELSE 0 END
            ) DESC
        ,e.Code;

    SELECT su.EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary AS su WITH (NOLOCK)
	INNER JOIN (
		SELECT EmpId
		FROM #LateInEarlyOutSummary
		GROUP BY EmpId
		HAVING COUNT(EmpId) >=3) AS tmp ON tmp.EmpId = su.EmpId
    ORDER BY su.EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;