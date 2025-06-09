CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert] --null, '2015-10-12', '2015-10-12' 
    @empid INT,
    @fromTime DATETIME, 
    @toTime DATETIME,
    @onlyUpdate BIT = 1
AS
BEGIN
DECLARE @i INT, @TempEmpId INT, @CurrentDateTime DATETIME, @TempResignedDate DATETIME

SELECT @fromTime = CAST(@fromTime AS DATE), @toTime = CAST(@toTime AS DATE), @CurrentDateTime = dbo.fnc_GetClientDateTime()

IF(@onlyUpdate = 0 
AND (
(DATEPART(HOUR, @CurrentDateTime) >= 18 AND DATEPART(HOUR, @CurrentDateTime) < 24) 
OR (DATEPART(HOUR, @CurrentDateTime) >= 10 AND DATEPART(HOUR, @CurrentDateTime) < 11)
))
BEGIN
    SET @onlyUpdate = 1
END

IF OBJECT_ID('#Employee') IS NOT NULL
BEGIN
    DROP TABLE #Employee
END 
CREATE TABLE #Employee (Id INT PRIMARY KEY IDENTITY(1,1), EmpId INT, ResignedDate DATETIME)

IF OBJECT_ID('#TimeRecord') IS NOT NULL
BEGIN
    DROP TABLE #TimeRecord
END 
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT NULL)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord    (Id, EmpID, RecordTime, InOutType)
SELECT tr.Id, EmpID, RecordTime, InOutType 
FROM dbo.TimeRecord AS tr WITH (NOLOCK)
LEFT JOIN dbo.TimeRecordExt AS te WITH (NOLOCK) ON tr.Id = te.ID
WHERE (@empId IS NULL OR EmpID = @empId)
    AND DATEDIFF(d, @fromTime, RecordTime) >= 0
    AND DATEDIFF(d, @toTime, RecordTime) <= 1
	AND te.ID IS NULL OR (te.[Status] = 1 OR te.[Status] = 2 OR te.[Status] = 4)

IF OBJECT_ID('#LeaveRequestHalfDay') IS NOT NULL
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END 
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate
FROM dbo.WorkingTimeRequest WITH (NOLOCK)
WHERE [Status] IN (1, 2)
    AND TotalDays = 0.5
    AND (@empId IS NULL OR EmpId = @empId)
    AND DATEDIFF(d, @fromTime, FromDate) >= 0
    AND DATEDIFF(d, FromDate, @toTime) >= 0

-- Get all calendars of employees into temp table
IF OBJECT_ID('#EmpCalendar') IS NOT NULL
BEGIN
    DROP TABLE #EmpCalendar
END 
CREATE TABLE #EmpCalendar (
        Id INT PRIMARY KEY IDENTITY(1,1), 
        EmpId INT,
        WorkDate DATETIME ,
        ShiftId INT,
        IsHoliday BIT,
        IsOffDay BIT,
        IsLeaveDay BIT,
        LeaveTypeId INT NULL,
		LeaveTypeId2 INT NULL,
        LateIn INT,
        EarlyOut INT,
        RequestedCheckInTime DATETIME ,
        RequestedCheckOutTime DATETIME ,
        BeginCheckInTime DATETIME,
        EndCheckInTime DATETIME,
        BeginCheckOutTime DATETIME,
        EndCheckOutTime DATETIME,
        BreakTimeMins INT,
        BeginBreakTime DATETIME,
        EndBreakTime DATETIME,
        LeaveDay DECIMAL(18,1),
        TimeForPostnatal INT)

INSERT INTO #EmpCalendar (
        EmpId,
        WorkDate,
        ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckInTime,
        RequestedCheckOutTime,
        BeginCheckInTime,
        EndCheckInTime,
        BeginCheckOutTime,
        EndCheckOutTime,
        BreakTimeMins,
        BeginBreakTime,
        EndBreakTime,
        LeaveDay,
        TimeForPostnatal)
SELECT     
        ec.EmpId,
        WorkDate,
        ec.ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckinTime,
        RequestedCheckoutTime,
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
            END AS [BeginCheckInTime],
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
            END AS [EndCheckInTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
            END AS [BeginCheckOutTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
            END AS [EndCheckOutTime],
        BreakTimeMins,
        CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME),
        CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME),
        LeaveDay,
        CASE WHEN pos.EmpId IS NULL THEN 0 ELSE (CASE WHEN (LeaveDay = 0.5 AND LeaveTypeId <> 9) THEN 30 ELSE pos.[Minutes] END) END
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.Id = ec.ShiftId
LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
AND ec.WorkDate BETWEEN pos.FromDate AND pos.ToDate
WHERE (@EmpId IS NULL OR ec.EmpId = @EmpId)
AND ec.WorkDate BETWEEN @FromTime AND @ToTime
ORDER BY ec.EmpId, ec.WorkDate

IF @empId IS NULL
BEGIN
    -- Get all enable staff to calculate 
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) 
	WHERE [Enabled] = 1 
	AND ISNULL(isDeleted, 0) = 0
	AND IsSpecialUser = 0
END
ELSE
BEGIN
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) WHERE Id = @empid
END

SET @i = 1
WHILE EXISTS (SELECT 1 FROM #Employee WHERE Id = @i)
BEGIN
    SELECT @TempEmpId = EmpId, @TempResignedDate = ResignedDate FROM #Employee WHERE Id = @i
    IF (DATEDIFF(D, @fromTime, '2015-08-01') > 0 AND DATEDIFF(D, '2015-08-01', @toTime) >= 0)
    BEGIN
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId, @fromTime, '2015-07-31', @onlyUpdate
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, '2015-08-01', @toTime, @onlyUpdate
    END
    ELSE IF (DATEDIFF(D, @fromTime, '2015-08-01') > 0)
    BEGIN
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId, @fromTime, @toTime, @onlyUpdate
    END
    ELSE
    BEGIN
        IF(@TempResignedDate IS NOT NULL OR DATEDIFF(D, @TempResignedDate, @toTime) > 0)
        BEGIN
            DELETE FROM EmployeeAttendance WHERE EmpId = @TempEmpId AND AttendanceDate > @TempResignedDate
            EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, @fromTime, @TempResignedDate, @onlyUpdate
        END
        ELSE
        BEGIN
            EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, @fromTime, @toTime, @onlyUpdate
        END
    END
    SET @i = @i + 1
END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO
