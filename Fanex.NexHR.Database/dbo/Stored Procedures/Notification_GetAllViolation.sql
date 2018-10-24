CREATE PROCEDURE [dbo].[Notification_GetAllViolation] @EmpId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#LateinEarlyOutViolationInYear') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInYear;
	END;

	CREATE TABLE #LateinEarlyOutViolationInYear (
		EmpId INT
		,NickName VARCHAR(100)
		,Fullname NVARCHAR(max)
		,TimesWithinAYear INT
		);

	IF OBJECT_ID('#LateinEarlyOutViolationInMonth') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInMonth;
	END;

	CREATE TABLE #LateinEarlyOutViolationInMonth (
		EmpId INT
		,TimesWithinThisMonth INT
		);

	IF OBJECT_ID('#CheckInOutViolation') IS NOT NULL
	BEGIN
		DROP TABLE #CheckInOutViolation;
	END;

	CREATE TABLE #CheckInOutViolation (
		EmpId INT
		,TimesMissingThisMonth INT
		);

	IF OBJECT_ID('#ToeicExpired') IS NOT NULL
	BEGIN
		DROP TABLE #ToeicExpired;
	END;

	CREATE TABLE #ToeicExpired (
		EmpId INT
		,IsExpired BIT
		);
--------
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
		AND CAST(tr.WorkDate AS DATE) BETWEEN DATEADD(m, - 12, GETDATE())
			AND GETDATE()
		AND e.Id = @EmpId
	GROUP BY tr.EmpId
		,MONTH(tr.WorkDate)
		,YEAR(tr.WorkDate)
	HAVING COUNT(*) >= 4
---------
	--late in/early out violation--
	INSERT INTO #LateinEarlyOutViolationInYear (
		EmpId
		,NickName
		,Fullname
		,TimesWithinAYear
		)
	SELECT e.Id
		,e.NickName
		,e.FullName
		,COUNT(tr.TotalTimes) AS TotalTimes
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
	WHERE 
	   --e.GroupId NOT IN (11, 40) -- management, expat
		ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.Id = @EmpId
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

	---------------
	DECLARE @StartOfMonth DATETIME
		,@EndOfMonth DATETIME

	SET @StartOfMonth = DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
	SET @EndOfMonth = DATEADD(month, DATEDIFF(month, 0, GETDATE()) + 1, - 1)

	INSERT INTO #LateinEarlyOutViolationInMonth (
		EmpId
		,TimesWithinThisMonth
		)
	SELECT e.Id
		,COUNT(lieot.WorkDate) AS 'TimesWithinThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM dbo.LateInEarlyOutTrans lieot
		WHERE lieot.Type IN (1, 2)
			AND lieot.WorkDate BETWEEN @StartOfMonth
				AND @EndOfMonth
		) lieot ON e.Id = lieot.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.id
		,e.NickName
		,e.FullName

	-----------------------
	DECLARE @missingCheckInOut TABLE (
		WorkDate DATETIME
		,isCurrent BIT
		,TypeId BIT
		,LeaveDay FLOAT
		,EmpId INT
		,Nickname VARCHAR(100)
		,Fullname NVARCHAR(max)
		)

	INSERT @missingCheckInOut
	EXEC [dbo].[Report_EmployeeSpaceDashBoard] @EmpId = @EmpId

	INSERT INTO #CheckInOutViolation (
		EmpId
		,TimesMissingThisMonth
		)
	SELECT e.Id
		,COUNT(mcio.WorkDate) 'TimesMissingInThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM @missingCheckInOut mcio
		WHERE mcio.isCurrent = 1
			AND mcio.TypeId = 3
		) mcio ON e.Id = mcio.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.Id
		,e.NickName
		,e.FullName

	----------------
	INSERT INTO #ToeicExpired (
		EmpId
		,IsExpired
		)
	SELECT e.Id
		,ISNULL(t.EmpId, 0) AS 'IsExpired'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT t.EmpId
			,t.InputDate
		FROM dbo.TOEIC t
		WHERE t.EmpId = @EmpId
			AND t.InputDate = DATEADD(d, 30, getdate())
		) t ON e.Id = t.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId

	SELECT leoviy.TimesWithinAYear AS 'LateInEarlyOutInYear'
		,leovim.TimesWithinThisMonth AS 'LateInEarlyOutInMonth'
		,ciov.TimesMissingThisMonth AS 'TimeMisssingCheckInOutThisMonth'
		,te.IsExpired AS 'ToeicExpired'
		,leoviy.EmpId
		,leoviy.NickName
		,leoviy.Fullname		
	FROM #LateinEarlyOutViolationInYear leoviy
		JOIN #LateinEarlyOutViolationInMonth leovim ON leoviy.EmpId = leovim.EmpId
		JOIN #CheckInOutViolation ciov ON leoviy.EmpId = ciov.EmpId
		JOIN #ToeicExpired te ON leoviy.EmpId = te.EmpId

	DROP TABLE #LateinEarlyOutViolationInYear;

	DROP TABLE #LateinEarlyOutViolationInMonth;

	DROP TABLE #CheckInOutViolation;

	DROP TABLE #ToeicExpired;
	
	DROP TABLE #LateInEarlyOutSummary;
END
