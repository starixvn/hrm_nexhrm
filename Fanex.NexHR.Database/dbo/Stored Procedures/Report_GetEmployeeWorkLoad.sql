--Exec [Report_GetEmployeeWorkLoad] '2018/07/01','2018/07/10', 8, 1
CREATE PROCEDURE [dbo].[Report_GetEmployeeWorkLoad]
	@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupId INT
	,@Debug BIT = 0
AS
/*
Created: 20160321@Quinn.Vo
Task: Get effort of every member in a team.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT @ToDate = DATEADD(dd, -1, @ToDate)

	DECLARE @Sql NVARCHAR(MAX)
	
	IF OBJECT_ID('#TblReport') IS NOT NULL
	BEGIN
		DROP TABLE #TblReport
	END

	CREATE TABLE #TblReport (
		EmpId INT
		,SpentTime DECIMAL(10, 2)
	)

	IF OBJECT_ID('#TblTeamWorkHours') IS NOT NULL
	BEGIN
		DROP TABLE #TblTeamWorkHours
	END

	CREATE TABLE #TblTeamWorkHours (
		EmpId INT
		,WorkHour INT
	)

	IF OBJECT_ID('#TblNickNames') IS NOT NULL
	BEGIN
		DROP TABLE #TblNickNames
	END
	
	CREATE TABLE #TblNickNames (EmpId INT, NickName VARCHAR(100))
	
     INSERT INTO #TblNickNames(EmpId, NickName)
	SELECT e.Id, REPLACE(e.DomainUsername,'spvn\','')
	FROM dbo.Employee AS e
	WHERE e.GroupId = @GroupId

	SET @Sql = 'INSERT INTO #TblReport(EmpId, SpentTime)
	   SELECT EmpId
		  ,SUM(SpentTime) AS SpentTime
	   FROM
	   (
		  SELECT e.EmpId
			 ,te.Hours AS SpentTime
		  FROM [RedmineNexcel].[dbo].[Projects] AS pr
		  INNER JOIN [RedmineNexcel].[dbo].[TimeEntries] AS te
			 ON te.ProjectId = pr.Id
		  INNER JOIN [RedmineNexcel].[dbo].[Users] AS u
			 ON u.Id = te.UserId
		  INNER JOIN #TblNickNames AS e ON u.Login = e.NickName
		  WHERE pr.Id <> 16
			 AND te.SpentOn BETWEEN ''' + CONVERT(VARCHAR(10),@FromDate,102) + ''' AND ''' + CONVERT(VARCHAR(10),@ToDate,102) + '''
		  UNION ALL
		  SELECT e.EmpId
			 ,((DATEDIFF(SECOND, 0, m.EndHour) - DATEDIFF(SECOND, 0, m.StartHour))/3600.00) AS SpentTime
		  FROM [RedmineNexcel].[dbo].[Projects] AS pr
		  INNER JOIN [RedmineNexcel].[dbo].[Meetings] AS m
			 ON m.ProjectId = pr.Id
		  INNER JOIN [RedmineNexcel].[dbo].[MeetingUsers] AS mu
			 ON mu.MeetingId = m.Id
		  INNER JOIN [RedmineNexcel].[dbo].[Users] AS u
			 ON u.Id = mu.UserId
		  INNER JOIN #TblNickNames AS e ON u.Login = e.NickName
		  WHERE pr.Id <> 16
			 AND m.date BETWEEN ''' + CONVERT(VARCHAR(10),@FromDate,102) + ''' AND ''' + CONVERT(VARCHAR(10),@ToDate,102) + ''' 
	   ) AS Sub
	   GROUP BY EmpId'

	EXEC (@Sql)

	IF @Debug = 1
	BEGIN
		SELECT @Sql

	END

	-- get work hours from hrm
	INSERT INTO #TblTeamWorkHours
	SELECT
		e.Id
		,SUM(CASE 
				WHEN cl.LeaveDay = 0.5 THEN 0.5
				ELSE 1
				END) * 8
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON cl.EmpId = e.Id
	WHERE e.GroupId = @GroupId
		AND e.DomainUsername IS NOT NULL
		AND cl.WorkDate BETWEEN @FromDate AND @ToDate
		AND cl.IsHoliday = 0
		AND cl.IsOffDay = 0
		AND (
			cl.IsLeaveDay = 0
			OR (
				cl.IsLeaveDay = 1
				AND cl.LeaveDay = 0.5
				)
			)
	GROUP BY e.Id
	
	--Member Workload
	SELECT 
		(CASE WHEN ISNULL(h.WorkHour,0) = 0 
			THEN 0
			ELSE ISNULL(re.SpentTime,0)/ISNULL(h.WorkHour, 0) END) AS [Workload],
		e.Id,
		e.Code,
		e.NickName,
		e.FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN #TblTeamWorkHours AS h ON e.Id = h.EmpId
		LEFT JOIN #TblReport AS re ON e.Id = re.EmpId
     WHERE e.GroupId = @GroupId
	AND e.Enabled = 1
	AND ISNULL(e.isDeleted, 0) = 0

	DROP TABLE #TblReport
	DROP TABLE #TblNickNames
	DROP TABLE #TblTeamWorkHours
END