/****** Object:  Stored Procedure dbo.Report_EmployeeDashBoard    Script Date: 3/15/2016 10:01:07 AM ******/

/*
--set @empid= 165--165--143
[Report_EmployeeDashBoard] 125
*/
CREATE PROCEDURE [dbo].[Report_EmployeeDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
	DECLARE @CurrentMonth DATETIME
		,@LastMonth DATETIME
		,@Today DATETIME
		,@Yesterday DATETIME
		,@CheckInOutCurrentMonth INT
		,@CheckInOutLastMonth INT
		,@MissingCheckInOutCurrentMonth INT
		,@MissingCheckInOutLastMonth INT
		,@LateInEarlyOutCurrentMonth INT
		,@LateInEarlyOutLastMonth INT
		,@AbsentCurrentMonth INT
		,@AbsentLastMonth INT
		,@AnnualLeaveBalancePending NUMERIC(5, 1)
		,@AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalWorkingHoursToday DECIMAL(18, 2)
		,@TotalWorkingHoursCurrentMonth DECIMAL(18, 2)
		,@TotalWorkingHoursLastMonth DECIMAL(18, 2)
		,@NumberWorkingDayCurrentMonth DECIMAL(18, 2)
		,@NumberWorkingDayLastMonth DECIMAL(18, 2)
		,@LeaveCurrentMonth DECIMAL(18, 1)
		,@LeaveLastMonth DECIMAL(18, 1)
		,@CheckInTime DATETIME
		,@CheckOutTime DATETIME
		,@MinuteLateInEarlyOutCurrentMonth int
		,@TimesLateInEarlyOutCurrentMonth int

	SET @Today = GETDATE()
	SELECT @CurrentMonth = @Today
		,@LastMonth = DATEADD(M, - 1, @Today)
		,@Yesterday = DATEADD(D, - 1, @Today)

	SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId 
		AND ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@Today)

	SELECT @AnnualLeaveBalancePending = SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE e.Id = @EmpId
		AND wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
	GROUP BY e.Id

	SELECT @TotalWorkingHoursCurrentMonth = SUM(CASE 
				WHEN DATEDIFF(M, att.AttendanceDate, @CurrentMonth) = 0 THEN TotalWorkHours
				ELSE 0
				END)
		,@TotalWorkingHoursLastMonth = SUM(CASE 
				WHEN DATEDIFF(M, att.AttendanceDate, @LastMonth) = 0 THEN TotalWorkHours
				ELSE 0
				END)
		,@NumberWorkingDayCurrentMonth = SUM(CASE 
				WHEN (
						DATEDIFF(M, att.AttendanceDate, @CurrentMonth) = 0
						AND ISNULL(att.TotalWorkHours, 0) <> 0
						) THEN (CASE 
						WHEN att.LeaveDay = 0.5 THEN 0.5
						ELSE 1 END
						)
				ELSE 0
				END)
		,@NumberWorkingDayLastMonth = SUM(CASE 
				WHEN (
						DATEDIFF(M, att.AttendanceDate, @LastMonth) = 0
						AND ISNULL(att.TotalWorkHours, 0) <> 0
						) THEN (CASE 
						WHEN att.LeaveDay = 0.5 THEN 0.5
						ELSE 1 END
						)
				ELSE 0
				END)
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON att.EmpId = cl.EmpId
		AND att.AttendanceDate = cl.WorkDate
	WHERE att.EmpId = @EmpId
		AND DATEDIFF(M, @LastMonth, att.AttendanceDate) > = 0
		AND DATEDIFF(M, att.AttendanceDate, @CurrentMonth) > = 0
		

	SELECT @CheckInTime = CASE WHEN cl.RequestedCheckinTime IS NULL THEN att.InFixed ELSE cl.RequestedCheckinTime END
		,@CheckOutTime = CASE WHEN cl.RequestedCheckoutTime IS NULL THEN att.OutFixed ELSE cl.RequestedCheckoutTime END
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON att.EmpId = cl.EmpId
		AND att.AttendanceDate = cl.WorkDate
	WHERE cl.EmpId = @EmpId
		AND DATEDIFF(D, cl.WorkDate, @Today) = 0

	EXEC [EmployeeAttendance_TotalWorkingHoursToday] @EmpId, @TotalWorkHoursToday = @TotalWorkingHoursToday OUTPUT

     -----Get Late in/Early out current month (hour/time) for dashboard-------
	SELECT @MinuteLateInEarlyOutCurrentMonth = SUM(tr.Minutes), @TimesLateInEarlyOutCurrentMonth = COUNT(tr.Minutes)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE --MONTH(WorkDate) = MONTH(GETDATE()) 
		DATEDIFF(MM, WorkDate, GETDATE()) = 0
		AND tr.Type <> 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
	GROUP BY EmpId;
	
	SELECT ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
		,@TotalWorkingHoursToday AS TotalWorkingHoursToday
		,@TotalWorkingHoursCurrentMonth AS TotalWorkingHoursCurrentMonth
		,@TotalWorkingHoursLastMonth AS TotalWorkingHoursLastMonth
		,@NumberWorkingDayCurrentMonth AS NumberWorkingDayCurrentMonth
		,@NumberWorkingDayLastMonth AS NumberWorkingDayLastMonth
		,@CheckInTime AS CheckInTime
		,@CheckOutTime AS CheckOutTime
		,@MinuteLateInEarlyOutCurrentMonth AS MinutesLateInEarlyOutCurrentMonth
		,@TimesLateInEarlyOutCurrentMonth AS TimesLateInEarlyOutCurrentMonth
END