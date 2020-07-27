CREATE PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutToDeduct] @Year INT
	
AS
/*
Created: 20200709@Merlin.Le
Task: Get late in/early out need to deduct
DB: NexHRM
Original: 
*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest;
	END;

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance;
	END;

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		);

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance;
	END;

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,Overtime INT
		);

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		);

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0)
			END
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,Overtime
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.IsSpecialUser = 0
		AND YEAR(WorkDate) = @Year
	GROUP BY EmpId;

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT rq.EmpId
		,SUM(rq.TotalDays)
	FROM [dbo].WorkingTimeRequest AS rq WITH (NOLOCK)
		LEFT JOIN [dbo].LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
	WHERE [Status] = 1
		AND RequestType = 6
		AND ty.IsALDeduction = 1
	GROUP BY rq.EmpId;

	SELECT dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,e.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE e.GroupId NOT IN (11, 40)
		AND ISNULL(cu.[Minutes], 0) >= 240
		AND (e.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) > 0)
		
	ORDER BY e.NickName;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;