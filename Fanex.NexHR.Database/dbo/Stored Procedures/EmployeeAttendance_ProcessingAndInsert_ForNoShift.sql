
CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForNoShift]
    @empId INT
    ,@WorkDate DATETIME
    ,@ShiftId INT
    ,@IsLeaveDay BIT
    ,@LeaveTypeId INT
    ,@LateIn INT
    ,@EarlyOut INT
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
    ,@TimeForPostnatal INT
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for no shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @DeductEarlyOut INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@RangeToScanTime INT
        ,@adjustLateIn INT
        ,@adjustEarlyOut INT

    SELECT @RangeToScanTime = 480, @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime

    -- get time in
    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpId = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END

    -- get time out
    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord 
    WHERE EmpId = @empId
        AND InOutType = 1
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        AND (@InOfMachine IS NULL OR DATEDIFF(D, RecordTime, DATEADD(MI, 2, @InOfMachine)) >= 0)

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END

    IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
    BEGIN
        SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine))/ 60.0 
    END
END

