

CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM2] 
    @empId INT
    ,@WorkDate DATETIME
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
Task: Calculate time for HR shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @DeductEarlyOut INT
        ,@BreakTime INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
		,@CheckInTemp DATETIME
		,@CheckOutTemp DATETIME

    SET @DeductEarlyOut = 0

    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END

    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 1
        -- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
        AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0
        AND (
            @InOfMachine IS NULL
            OR RecordTime > DATEADD(MI, 2, @InOfMachine)
            )

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END

    SELECT @RequestLeaveTimeBegin = FromTime
        ,@RequestLeaveTimeEnd = ToTime
    FROM #LeaveRequestHalfDay
    WHERE EmpId = @empId
        AND OnDate = @WorkDate

	SELECT @CheckInTemp = @InOfMachine, @CheckOutTemp = @OutOfMachine
    IF (@InOfMachine IS NOT NULL)
    BEGIN
        IF (ISNULL(@LeaveDay, 0) = 0)
        BEGIN
            IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
            BEGIN
                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
            END
            ELSE IF (DATEDIFF(MI, @InOfMachine, @EndCheckOutTime) >= 0) -- 8h-->8h30
            BEGIN
                SELECT @ExpectedInTime = @InOfMachine, @ExpectedOutTime = DATEADD(HH, 9, @InOfMachine)
            END
            ELSE
            BEGIN
                SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
            END
        END
        ELSE IF (@LeaveDay = 0.5)
        BEGIN
                IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
                BEGIN
                    SELECT @ExpectedInTime = @BeginCheckInTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0 ) -- 8h00->8h30
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 5, @InOfMachine)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 8h30->9h30
                BEGIN
                    IF (@RequestLeaveTimeBegin < @EndBreakTime)
                    BEGIN
                        SELECT @ExpectedInTime = @EndBreakTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                    END
                    ELSE
                    BEGIN
                        SELECT @ExpectedInTime = @EndCheckInTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndCheckInTime)
                    END

                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndBreakTime) >= 0) -- 9h30->13h
                BEGIN
                    SELECT @ExpectedInTime = @EndBreakTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(MI, 30, @EndBreakTime)) >= 0) -- 13h-->13h30
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime =  DATEADD(HH, 4, @InOfMachine)
                END
                ELSE -- > 13h30
                BEGIN
                    SELECT @ExpectedInTime = DATEADD(MI, 30, @EndBreakTime)
                        ,@ExpectedOutTime = @EndCheckOutTime
                END
        END

        IF (
                @OutOfMachine < @EndBreakTime
                AND @ExpectedOutTime > @EndBreakTime
                )
        BEGIN
            SELECT @DeductEarlyOut = CASE 
                    WHEN DATEDIFF(MI, @OutOfMachine, @EndBreakTime) > @BreakTimeMins THEN @BreakTimeMins
                    ELSE DATEDIFF(MI, @OutOfMachine, @EndBreakTime)
                    END
        END

        IF (@OutOfMachine < @ExpectedOutTime)
        BEGIN
            SET @ActualEarlyOut = DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) - @DeductEarlyOut
        END

        IF (@InOfMachine > @ExpectedINTime)
        BEGIN
            SET @ActualLateIn = DATEDIFF(MI, @ExpectedINTime, @InOfMachine)
        END
    END
    ELSE
    BEGIN
        SELECT @ExpectedInTime = @EndCheckInTime
            ,@ExpectedOutTime = @EndCheckOutTime
    END

	SELECT @InOfMachine = @CheckInTemp, @OutOfMachine = @CheckOutTemp
    
    -- Break time for total hours
    IF (
            @InOfMachine IS NOT NULL
            AND @OutOfMachine IS NOT NULL
		  AND ISNULL(@LeaveDay, 0) <> 1
            )
    BEGIN
        SET @BreakTime = 0

        IF (@InOfMachine < @BeginCheckInTime)
        BEGIN
            SET @BreakTime = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
        END

        IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + @BreakTimeMins
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = 1440
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @InOfMachine <= @EndBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
        END
        ELSE IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
        END

        SET @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime) / 60.0)

        IF (@TotalWorkHours < 0)
        BEGIN
            SET @TotalWorkHours = 0
        END
    END
END

