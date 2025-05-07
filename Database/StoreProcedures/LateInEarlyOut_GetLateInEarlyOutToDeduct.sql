SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutToDeduct] @Year INT
	
AS
/*
Created: 20200709@Merlin.Le
Task: Get late in/early out need to deduct
DB: NexHRM
Original: 
Revisions:
	- 20250328@Henry.Dinh: Update GroupID 11-40 -> GroupCode MGT/COR/EXP
*/
BEGIN
	SET NOCOUNT ON;
	IF OBJECT_ID('#tblAnnualLeaveBalance') IS NOT NULL
	BEGIN
		DROP TABLE #tblAnnualLeaveBalance;
	END;

	CREATE TABLE #tblAnnualLeaveBalance (
		EmpId INT
		,AnnualLeaveBalance NUMERIC(10, 1)
		);

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

	INSERT INTO #tblAnnualLeaveBalance(EmpId, AnnualLeaveBalance)
	SELECT lt.EmpId, SUM(lt.CreditDays) - SUM(lt.DeductDays)
	FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lt.EmpId = e.Id
	WHERE e.Enabled = 1 
	AND e.IsDeleted = 0
	AND YEAR(lt.FromDate) = @Year
	AND lt.Deleted = 0
	GROUP BY lt.EmpId

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT e.Id, SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
		AND YEAR(wk.FromDate) = @Year
	GROUP BY e.Id


	SELECT dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,al.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN [dbo].EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		INNER JOIN (
			SELECT EmpId
			,(SUM(CreditDays) - SUM(DeductDays)) AS AnnualLeaveBalance
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e ON lt.EmpId = e.Id AND e.isDeleted = 0
			WHERE YEAR(FromDate) = @Year
			AND ISNULL(lt.Deleted, 0) = 0
			GROUP BY EmpId
		) AS leasum ON e.Id = leasum.EmpId
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #tblAnnualLeaveBalance AS al ON al.EmpId = pv.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE eg.Code NOT IN ('MGT', 'COR', 'EXP')
		AND ISNULL(cu.[Minutes], 0) >= 240
		AND (leasum.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) > 0)
		
	ORDER BY e.NickName;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;
GO
