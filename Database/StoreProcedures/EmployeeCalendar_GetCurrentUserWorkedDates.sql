CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetCurrentUserWorkedDates]
    @UserId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
	Created: 20180614@Oliver.Dang
	Task: Get employee working dates by date range.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250327@Henry: Update MGT Code -> MGT/COR code
		- 20250506@Henry: Add HR-ADM group code
		- 20250618@Assistant: Fix SystemSetting usage for comma-separated values
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsNotShow BIT = 0

	DECLARE @HRTeamId INT,
	@ITTeamId INT,
	@ExpId INT,
	@FinanceId INT,
	@MGTId INT,
	@CEOTeamId INT;

	SELECT @HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.GroupId END), 0),
		@ITTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'ITNetWorkTeamId' THEN g.GroupId END), 0),
		@ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.GroupId END), 0),
		@FinanceId = ISNULL(MAX(CASE WHEN ss.Name = 'FATeamId' THEN g.GroupId END), 0),
		@MGTId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.GroupId END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.GroupId END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN bodb_HRM.dbo.[Group] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'ITNetWorkTeamId', 'EXPTeamId', 'FATeamId', 'CoreTeamId', 'CEOTeamId')

	IF(
		EXISTS (
			SELECT 1 FROM 
				dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
			WHERE e.Id = @UserId
				AND 
					(e.Roles LIKE '%2%'
						OR e.Roles LIKE '%3%'
						OR e.Roles LIKE '%4%'
						OR g.Id IN (@HRTeamId, @ITTeamId, @ExpId, @FinanceId, @MGTId, @CEOTeamId)
					)
		)
	)
	BEGIN
		SET @IsNotShow = 1
	END

	CREATE TABLE #EmployeeCalendar2HalfDay
	(
		WorkDate DATE
	)

	INSERT INTO #EmployeeCalendar2HalfDay(WorkDate)
	SELECT CAST(rq.FromDate AS DATE)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
	WHERE rq.EmpId = @UserId
		AND rq.[Status] IN (1, 2)
		AND rq.TotalDays = 0.5
	GROUP BY rq.EmpId
		,CAST(rq.FromDate AS DATE)
	HAVING SUM(TotalDays) = 1

    SELECT cl.WorkDate
    FROM  dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
		AND cl.WorkDate BETWEEN rq.FromDate AND rq.ToDate
		AND rq.[Status] = 1
		AND rq.TotalDays >= 1
	LEFT JOIN #EmployeeCalendar2HalfDay AS hd ON cl.WorkDate = hd.WorkDate
    WHERE cl.EmpId = @UserId
		AND (cl.WorkDate between @FromDate and @ToDate)
		AND (cl.IsOffDay <> 1 AND ISNULL(cl.LeaveDay, 0) < 1 AND cl.IsHoliday <> 1)
		AND rq.Id IS NULL
		AND hd.WorkDate IS NULL
		AND @IsNotShow = 0

	DROP TABLE #EmployeeCalendar2HalfDay
END

SET ANSI_NULLS ON
GO
