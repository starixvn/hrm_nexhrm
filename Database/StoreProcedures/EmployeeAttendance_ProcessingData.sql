USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_ProcessingData]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingData] 
AS
BEGIN

DECLARE @i INT
		,@TempEmpId INT
		,@TempWorkDate DATE
		,@TempShiftId INT
		,@fromDate DATE
		,@toDate DATE
		,@CurrentDateTime DATETIME
		,@TempResignedDate DATETIME

SELECT @CurrentDateTime = dbo.fnc_GetClientDateTime()


DECLARE @tblEmployee TABLE (
		EmpId INT
		,WorkDate DATE
		,ResignedDate DATETIME
		)
SELECT @fromDate = MIN(tmpAtt.WorkDate)
		,@toDate = MAX(tmpAtt.WorkDate)
	FROM dbo.tmpEmployeeAttendance AS tmpAtt WITH (NOLOCK)

	INSERT INTO @tblEmployee (
		EmpId
		,WorkDate
		)
	SELECT TOP 100 EmpId
		,WorkDate
	FROM dbo.tmpEmployeeAttendance AS tmpAtt WITH (NOLOCK)
	select * from @tblEmployee
	DELETE tmp
	FROM dbo.tmpEmployeeAttendance AS tmp
		INNER JOIN @tblEmployee AS e ON tmp.EmpId = e.EmpId
		AND tmp.WorkDate = e.WorkDate

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
	WHERE RecordTime BETWEEN @fromDate
			AND DATEADD(d, 2, @toDate)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployee
			WHERE EmpId = tr.EmpId
			)
	AND te.ID IS NULL OR (te.[Status] = 1 OR te.[Status] = 2 OR te.[Status] = 4)


IF OBJECT_ID('#LeaveRequestHalfDay') IS NOT NULL
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END 
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate
FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
WHERE [Status] IN (1, 2)
    AND TotalDays = 0.5
    AND (EXISTS (SELECT 1 FROM @tblEmployee WHERE EmpId = wt.EmpId))
    AND DATEDIFF(d, @fromDate, FromDate) >= 0
    AND DATEDIFF(d, FromDate, @toDate) >= 0

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
WHERE (EXISTS (SELECT 1 FROM @tblEmployee WHERE EmpId = ec.EmpId AND WorkDate = ec.WorkDate))
ORDER BY ec.EmpId, ec.WorkDate

SET @i = 1
IF EXISTS (
			SELECT 1
			FROM #EmpCalendar
			)
	BEGIN
		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM #EmpCalendar
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempWorkDate = WorkDate
				,@TempShiftId = ShiftId
			FROM #EmpCalendar
			WHERE Id = @i

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId
				,@TempWorkDate
				,@TempWorkDate
				,1

			SET @i = @i + 1
		END
	END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO
