USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_TotalWorkingHoursToday]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_TotalWorkingHoursToday]
    @empid INT,
    @TotalWorkHoursToday DECIMAL(18, 2) OUT
AS
BEGIN
DECLARE @i INT, @TempEmpId INT, @CurrentDateTime DATETIME, @TempResignedDate DATETIME

SELECT @CurrentDateTime = GETDATE()

IF OBJECT_ID('#Employee') IS NOT NULL
BEGIN
    DROP TABLE #Employee
END 
CREATE TABLE #Employee (Id INT PRIMARY KEY IDENTITY(1,1), EmpId INT, ResignedDate DATETIME)

IF OBJECT_ID('#TimeRecord') IS NOT NULL
BEGIN
    DROP TABLE #TimeRecord
END 
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord    (Id, EmpID, RecordTime, InOutType)
SELECT Id, EmpID, RecordTime, InOutType 
FROM dbo.TimeRecord WITH (NOLOCK) 
WHERE (@empId IS NULL OR EmpID = @empId)
    AND DATEDIFF(d, @CurrentDateTime, RecordTime) >= 0
    AND DATEDIFF(d, @CurrentDateTime, RecordTime) <= 1
---- insert virtual checkout time (now)
INSERT INTO #TimeRecord (Id, EmpID, RecordTime, InOutType)
VALUES(null, @empid, getdate(), 1)

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
    AND DATEDIFF(d, getdate(), FromDate) >= 0

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
        CASE WHEN pos.EmpId IS NULL THEN 0 ELSE (CASE WHEN (LeaveDay = 0.5 AND LeaveTypeId <> 9) THEN (pos.[Minutes]/2.0) ELSE pos.[Minutes] END) END
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.Id = ec.ShiftId
LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
AND ec.WorkDate BETWEEN pos.FromDate AND pos.ToDate
WHERE (@EmpId IS NULL OR ec.EmpId = @EmpId)
AND ec.WorkDate = convert(date, getdate())
ORDER BY ec.EmpId, ec.WorkDate

IF @empId IS NOT NULL
BEGIN
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) WHERE Id = @empid
END
BEGIN
    SELECT @TempEmpId = EmpId, @TempResignedDate = ResignedDate FROM #Employee WHERE Id = @empId    
    EXEC [EmployeeAttendance_TotalWorkingHoursToday_ForEmpId] @empId, @TotalWorkHoursToday = @TotalWorkHoursToday OUTPUT    

END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO
