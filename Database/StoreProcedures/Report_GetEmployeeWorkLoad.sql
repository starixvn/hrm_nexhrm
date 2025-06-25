CREATE OR ALTER PROCEDURE [dbo].[Report_GetEmployeeWorkLoad]
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
	20200120@Merlin.Le: Order by employee rank and code
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT @ToDate = DATEADD(dd, -1, @ToDate)

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
	AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)

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
		AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)
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
		0.0 as PercentWorkload,
		h.WorkHour,
		e.Id,
		e.Code,
		e.NickName,
		e.FullName,
		e.DomainUsername
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN #TblTeamWorkHours AS h ON e.Id = h.EmpId
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
    WHERE e.GroupId = @GroupId
	AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)
	AND e.Enabled = 1
	AND ISNULL(e.isDeleted, 0) = 0
	ORDER BY
        e.Roles DESC
        ,e.GroupDate

	DROP TABLE #TblNickNames
	DROP TABLE #TblTeamWorkHours
END

GO
