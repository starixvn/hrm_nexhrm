-- [Report_GetLateInEarlyOutSummary] 2015
CREATE PROCEDURE [dbo].[Report_GetLateInEarlyOutSummary] @Year INT
	,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20150805@Quinn.Vo
Task: Get late in/ early out summary report
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest
	END

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		)

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance
	END

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		)

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance
	END

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		)

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary
	END

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,Jan INT
		,Feb INT
		,Mar INT
		,Apr INT
		,May INT
		,Jun INT
		,Jul INT
		,Aug INT
		,Sep INT
		,Oct INT
		,Nov INT
		,[Dec] INT
		,TimesInJan INT
		,TimesInFeb INT
		,TimesInMar INT
		,TimesInApr INT
		,TimesInMay INT
		,TimesInJun INT
		,TimesInJul INT
		,TimesInAug INT
		,TimesInSep INT
		,TimesInOct INT
		,TimesInNov INT
		,TimesInDec INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		)

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		)
	SELECT e.Id
		,la.[Minutes]
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
	WHERE ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		)
	SELECT e.Id
		,la.[Minutes]
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
	WHERE ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
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
		,TimesInJan
		,TimesInFeb
		,TimesInMar
		,TimesInApr
		,TimesInMay
		,TimesInJun
		,TimesInJul
		,TimesInAug
		,TimesInSep
		,TimesInOct
		,TimesInNov
		,TimesInDec
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0
						THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE YEAR(WorkDate) = @Year
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
	GROUP BY EmpId

	INSERT INTO #PendingAnnualLeaveRequest (EmpId, TotalDays)
	SELECT rq.EmpId, SUM(rq.TotalDays)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
	LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
	WHERE [Status] = 1
		AND RequestType = 6
		AND ty.IsALDeduction = 1
	GROUP BY rq.EmpId

	SELECT pv.[Minutes] AS BalanceLastYear
		,dt.Jan
		,dt.Feb
		,dt.Mar
		,dt.Apr
		,dt.May
		,dt.Jun
		,dt.Jul
		,dt.Aug
		,dt.Sep
		,dt.Oct
		,dt.Nov
		,dt.[Dec]
		,dt.TimesInJan
		,dt.TimesInFeb
		,dt.TimesInMar
		,dt.TimesInApr
		,dt.TimesInMay
		,dt.TimesInJun
		,dt.TimesInJul
		,dt.TimesInAug
		,dt.TimesInSep
		,dt.TimesInOct
		,dt.TimesInNov
		,dt.TimesInDec
		,dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,cu.[Minutes] AS BalanceCurrentYear
		,pv.EmpId
		,e.NickName
		,e.FullName
		,e.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
		,e.GroupId
		,g.Code
		,g.Name
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE e.GroupId NOT IN (11,40)
	ORDER BY g.[Rank] , g.Code , (CASE e.Id
			WHEN g.LeaderId THEN
			 2
			WHEN g.AssistantLeaderId THEN
			 1
			ELSE
			 0
		   END) DESC, e.code

	DROP TABLE #PreviousYearBalance
	DROP TABLE #CurrentYearBalance
	DROP TABLE #LateInEarlyOutSummary
	DROP TABLE #PendingAnnualLeaveRequest
END
