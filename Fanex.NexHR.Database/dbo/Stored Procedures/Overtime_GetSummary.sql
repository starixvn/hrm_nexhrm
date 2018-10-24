

/****** Object:  Stored Procedure dbo.Overtime_GetSummary    Script Date: 3/15/2016 10:01:06 AM ******/
CREATE PROCEDURE [dbo].[Overtime_GetSummary] @year INT
	,@employeeId INT = NULL
	,@groupId INT = NULL
AS
BEGIN
	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance
	END

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		)

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance
	END

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,LateInEarlyOut INT
		)

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(oty.[Minutes], 0) - CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0)
			END
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,LateInEarlyOut
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN dbo.LateInEarlyOutYearly AS la ON e.Id = la.EmpId
		AND la.[Year] = @year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			);

	SELECT ott.EmpId AS EmployeeId
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 1
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Jan'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 2
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Feb'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 3
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Mar'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 4
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Apr'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 5
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'May'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 6
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Jun'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 7
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Jul'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 8
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Aug'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 9
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Sep'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 10
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Oct'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 11
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Nov'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 12
					AND ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'Dec'
		,SUM(CASE 
				WHEN ott.[Type] = 1 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'TotalHour'
		,SUM(CASE 
				WHEN ott.[Type] = 0 THEN ISNULL(ott.Minutes, 0)
				ELSE 0
				END) 'LeaveConversion'
	INTO #OvertimeSummary
	FROM dbo.OvertimeTrans ott
	WHERE YEAR(ott.[Date]) = @year
	GROUP BY ott.EmpId

	SELECT ISNULL(os.Jan, 0) / 60.0 AS 'Jan'
		,ISNULL(os.Feb, 0) / 60.0 AS 'Feb'
		,ISNULL(os.Mar, 0) / 60.0 AS 'Mar'
		,ISNULL(os.Apr, 0) / 60.0 AS 'Apr'
		,ISNULL(os.May, 0) / 60.0 AS 'May'
		,ISNULL(os.Jun, 0) / 60.0 AS 'Jun'
		,ISNULL(os.Jul, 0) / 60.0 AS 'Jul'
		,ISNULL(os.Aug, 0) / 60.0 AS 'Aug'
		,ISNULL(os.Sep, 0) / 60.0 AS 'Sep'
		,ISNULL(os.Oct, 0) / 60.0 AS 'Oct'
		,ISNULL(os.Nov, 0) / 60.0 AS 'Nov'
		,ISNULL(os.DEC, 0) / 60.0 AS 'Dec'
		,ISNULL(os.TotalHour, 0) / 60.0 AS TotalHour
		,ISNULL(os.LeaveConversion, 0) / 60.0 AS LeaveConversion
		,ISNULL(prev.Minutes, 0) / 60.0 AS BalanceLastYear
		,ISNULL(cur.[Minutes], 0) / 60.0 AS BalanceCurrentYear
		,ISNULL(cur.LateInEarlyOut, 0) / 60.0 AS LateInEarlyOut
		,e.Id 'EmpId'
		,e.Code
		,e.NickName
		,e.FullName
		,e.GroupId
		,eg.Code
		,eg.NAME
		,eg.Rank
	FROM dbo.Employee e
		INNER JOIN dbo.EmployeeGroup eg ON e.GroupId = eg.Id
		LEFT JOIN #OvertimeSummary os ON e.Id = os.EmployeeId
		LEFT JOIN #CurrentYearBalance AS cur ON e.Id = cur.EmpId
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
	WHERE e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupid
			)
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.GroupId NOT IN (11, 40)
	ORDER BY eg.Rank
		,eg.Code
		,(CASE e.Id
			WHEN eg.LeaderId THEN 2
			WHEN eg.AssistantLeaderId THEN 1
			ELSE 0 END
			) DESC
		,e.Code

	DROP TABLE #OvertimeSummary

	DROP TABLE #PreviousYearBalance

	DROP TABLE #CurrentYearBalance
END

