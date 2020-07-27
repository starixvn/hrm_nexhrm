
CREATE PROCEDURE [dbo].[EmployeeAttendance_GetEmployeeReachedWorkingTime]
AS
/*
Created: 20190508@Quinn.Vo
Task: get employees which reached time out.
DB: NexHR
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATETIME = GETDATE()
		,@i INT
		,@TempEmpId INT
		,@TotalWorkHoursToday DECIMAL(18, 2)
	
	-- test
	--declare @hour int = datediff(hh, getdate(), '2019-05-16 17:00')
	--select dateadd(MI, @hour*60, @Today)

	CREATE TABLE #Employee (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,TotalWorkHours DECIMAL(18, 2)
		)
	
	CREATE TABLE #TimeRecord (
		Id INT
		,EmpID INT
		,RecordTime DATETIME
		,InOutType TINYINT
		)

	CREATE TABLE #LeaveRequestHalfDay (
		EmpID INT
		,OnDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		)

	INSERT INTO #Employee (EmpId)

	SELECT EmpId
	FROM [dbo].[EmployeeAttendance] AS att
	WHERE DATEDIFF(MI, ExpectedOutTime, dateadd(MI, 0, @Today)) <= 5
		AND DATEDIFF(MI, ExpectedOutTime, dateadd(MI, 0, @Today)) >= 0
		AND OutFixed IS NULL
		AND InFixed IS NOT NULL
		AND ISNULL(LeaveDay, 0) <> 1
		AND NOT EXISTS (
			SELECT 1 FROM dbo.LogReachedWorkingTime WITH (NOLOCK)
			WHERE EmpId = att.EmpId
			AND WorkDate = att.AttendanceDate
		)

	INSERT INTO #TimeRecord (
		Id
		,EmpID
		,RecordTime
		,InOutType
		)
	SELECT Id
		,EmpID
		,RecordTime
		,InOutType
	FROM dbo.TimeRecord AS ti WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = ti.EmpID
			)
		AND DATEDIFF(d, @Today, RecordTime) >= 0
		AND DATEDIFF(d, @Today, RecordTime) <= 1

	---- insert virtual checkout time (now)
	INSERT INTO #TimeRecord (
		Id
		,EmpID
		,RecordTime
		,InOutType
		)
	SELECT NULL
		,EmpId
		,@Today
		,1
	FROM #Employee

	INSERT INTO #LeaveRequestHalfDay (
		EmpID
		,OnDate
		,FromTime
		,ToTime
		)
	SELECT EmpId
		,CONVERT(VARCHAR(10), FromDate, 111)
		,FromDate
		,ToDate
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
	WHERE [Status] = 2
		AND TotalDays = 0.5
		AND EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = wkr.EmpID
			)
		AND DATEDIFF(d, getdate(), FromDate) >= 0

	CREATE TABLE #EmpCalendar (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,WorkDate DATETIME
		,ShiftId INT
		,IsHoliday BIT
		,IsOffDay BIT
		,IsLeaveDay BIT
		,LeaveTypeId INT NULL
		,LateIn INT
		,EarlyOut INT
		,RequestedCheckInTime DATETIME
		,RequestedCheckOutTime DATETIME
		,BeginCheckInTime DATETIME
		,EndCheckInTime DATETIME
		,BeginCheckOutTime DATETIME
		,EndCheckOutTime DATETIME
		,BreakTimeMins INT
		,BeginBreakTime DATETIME
		,EndBreakTime DATETIME
		,LeaveDay DECIMAL(18, 1)
		,TimeForPostnatal INT
		)

	INSERT INTO #EmpCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,LateIn
		,EarlyOut
		,RequestedCheckInTime
		,RequestedCheckOutTime
		,BeginCheckInTime
		,EndCheckInTime
		,BeginCheckOutTime
		,EndCheckOutTime
		,BreakTimeMins
		,BeginBreakTime
		,EndBreakTime
		,LeaveDay
		,TimeForPostnatal
		)
	SELECT ec.EmpId
		,WorkDate
		,ec.ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,LateIn
		,EarlyOut
		,RequestedCheckinTime
		,RequestedCheckoutTime
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
			END AS [BeginCheckInTime]
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS [EndCheckInTime]
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime)
			ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
			END AS [BeginCheckOutTime]
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
			END AS [EndCheckOutTime]
		,BreakTimeMins
		,CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME)
		,CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME)
		,LeaveDay
		,CASE 
			WHEN pos.EmpId IS NULL THEN 0
			ELSE (CASE 
					WHEN (
							LeaveDay = 0.5
							AND LeaveTypeId <> 9
							) THEN (pos.[Minutes] / 2.0)
					ELSE pos.[Minutes] END
					)
			END
	FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id = ec.ShiftId
		LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
		AND ec.WorkDate BETWEEN pos.FromDate
			AND pos.ToDate
	WHERE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = ec.EmpID
			)
		AND ec.WorkDate = convert(DATE, @Today)
	ORDER BY ec.EmpId
		,ec.WorkDate

	SET @i = 1

	WHILE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE Id = @i
			)
	BEGIN
		SET @TotalWorkHoursToday = 0

		SELECT @TempEmpId = EmpId
		FROM #Employee
		WHERE Id = @i

		EXEC dbo.EmployeeAttendance_TotalWorkingHoursToday_ForEmpId @empId = @TempEmpId
			,@TotalWorkHoursToday = @TotalWorkHoursToday OUTPUT

		UPDATE #Employee
		SET TotalWorkHours = @TotalWorkHoursToday
		WHERE Id = @i

		SET @i = @i + 1
	END

	SELECT e.Id AS EmployeeId
		,e.NickName
		,e.FullName
		,ee.TotalWorkHours
		,ec.ConversationId AS SkypeConversationId
	FROM #Employee AS ee
	INNER JOIN dbo.Employee AS e ON ee.EmpId = e.Id
	INNER JOIN dbo.EmployeeConversation AS ec ON ee.EmpId = ec.EmpId

	DROP TABLE #Employee
	DROP TABLE #EmpCalendar
	DROP TABLE #TimeRecord
	DROP TABLE #LeaveRequestHalfDay
END