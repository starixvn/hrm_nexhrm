USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId]
    @empId INT, 
    @fromTime DATETIME , 
    @toTime DATETIME ,
    @onlyUpdate BIT = 0
AS
BEGIN
    DECLARE @RangeToScanTime INT
    DECLARE @DeductEarlyOut INT, @BreakTime INT, @tempTimeIn DATETIME
    SET @RangeToScanTime = 480 -- 480 munites

    DECLARE @TimeRecord TABLE (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)
    INSERT INTO @TimeRecord    (Id, EmpID, RecordTime, InOutType)
    SELECT Id, EmpID, RecordTime, InOutType 
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND DATEDIFF(d, @fromTime, RecordTime) >= 0
        AND DATEDIFF(d, @toTime, RecordTime) <= 1

    DELETE FROM #TimeRecord
    WHERE EmpID = @empId
        AND DATEDIFF(d, @fromTime, RecordTime) >= 0
        AND DATEDIFF(d, @toTime, RecordTime) <= 1

    DECLARE @i INT, @attendaceDate DATETIME
    DECLARE    @ID INT, 
        @EmpIdCur INT,
        @WorkDate DATETIME,
        @ShiftId INT,
        @IsHoliday BIT,
        @isOffDay BIT,
        @IsLeaveDay BIT,
        @LeaveTypeId INT,
        @LateIn INT,
        @EarlyOut INT,
        @RequestedCheckinTime DATETIME,
        @RequestedCheckoutTime DATETIME,
        @BeginCheckInTime DATETIME,
        @EndCheckInTime DATETIME,
        @BeginCheckOutTime DATETIME,
        @EndCheckOutTime DATETIME,
        @InOfMachine DATETIME,
        @OutOfMachine DATETIME,
        @ExpectedInTime DATETIME,
        @ExpectedOutTime DATETIME,
        @Absence BIT,
        @IsRightShift BIT,
        @ActualLateIn INT,
        @ActualEarlyOut INT,
        @TotalWorkHours DECIMAL(18,2),
        @BreakTimeMins INT,
        @BeginBreakTime DATETIME,
        @EndBreakTime DATETIME,
        @LeaveDay DECIMAL(18,1),
        @Notes NVARCHAR(2000),
        @RequestLeaveTimeBegin DATETIME,
        @RequestLeaveTimeEnd DATETIME,
        @adjustLateIn INT,
        @adjustEarlyOut INT,
        @TimeForPostnatal INT
    
    SET @i = 0
    SET @attendaceDate = CONVERT(VARCHAR(10), @fromTime, 111)
    WHILE (DATEDIFF(d, @attendaceDate, @toTime) >= 0)
    BEGIN 
        
        SELECT @Absence = 0,  @InOfMachine = null, @OutOfMachine = null, 
            @ID = null, @ExpectedOutTime  = null, @IsRightShift=null,
            @ExpectedInTime = null,    @ExpectedOutTime = null, 
            @ActualLateIn = 0, @ActualEarlyOut = 0,
            @RequestedCheckinTime = null, @RequestedCheckoutTime = null,
            @TotalWorkHours = null, @BreakTimeMins = 0,
            @ShiftId = null, @LeaveTypeId = null,
            @RequestLeaveTimeBegin = null, @RequestLeaveTimeEnd = null,
            @adjustLateIn = 0, @adjustEarlyOut = 0, @DeductEarlyOut = 0,
            @BeginBreakTime = null, @EndBreakTime = null, @TimeForPostnatal = 0, @tempTimeIn = null

        SELECT 
            @WorkDate = WorkDate,
            @ShiftId = ShiftId,
            @IsHoliday = IsHoliday,
            @isOffDay = IsOffDay,
            @IsLeaveDay = IsLeaveDay,
            @LeaveTypeId = LeaveTypeId,
            @LateIn = ISNULL(LateIn,0),
            @EarlyOut = ISNULL(EarlyOut,0),
            @RequestedCheckinTime = RequestedCheckinTime,
            @RequestedCheckoutTime = RequestedCheckoutTime,
            @BeginCheckInTime = BeginCheckInTime,
            @EndCheckInTime = EndCheckInTime,
            @BeginCheckOutTime = BeginCheckOutTime,
            @EndCheckOutTime = EndCheckOutTime,
            @BreakTimeMins  = BreakTimeMins,
            @BeginBreakTime = BeginBreakTime,
            @EndBreakTime = EndBreakTime,
            @LeaveDay = LeaveDay,
            @TimeForPostnatal = TimeForPostnatal
        FROM #EmpCalendar
        WHERE EmpId = @empId AND WorkDate = @attendaceDate

        -- If Has Shift
        IF @ShiftId IS NOT NULL
        BEGIN
            IF(@ShiftId = 1) -- shift E3, E4, E5
            BEGIN
                SELECT @InOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 0
                    AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

                IF @RequestedCheckinTime IS NOT NULL
                BEGIN
                    SET @InOfMachine = @RequestedCheckinTime
                END

                SELECT @OutOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 1
                    AND DATEDIFF(d, @attendaceDate, RecordTime) = 0
                    AND (@InOfMachine IS NULL OR RecordTime > DATEADD(MI, 2, @InOfMachine))

                IF @RequestedCheckoutTime IS NOT NULL
                BEGIN
                    SET @OutOfMachine = @RequestedCheckoutTime
                END

                IF (@InOfMachine IS NOT NULL)
                BEGIN
                    IF (@LateIn > 0)
                    BEGIN
                        SET @tempTimeIn = DATEADD(MI, -@LateIn, @InOfMachine)
                    END
                    ELSE
                    BEGIN
                        SET @tempTimeIn = @InOfMachine
                    END

                    IF(ISNULL(@LeaveDay, 0) = 0)
                    BEGIN
                        IF (DATEDIFF(MI, @tempTimeIn, @BeginCheckInTime) >= 0) -- <8h
                        BEGIN
                            SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
                        END
                        ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
                        BEGIN
                            SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 9, @tempTimeIn)
                        END
                        ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
                        BEGIN
                            SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                        END
                        ELSE -- >10h
                        BEGIN
                            SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                        END
                    END
                    ELSE IF (@LeaveDay = 0.5)
                    BEGIN
                        IF (@LeaveTypeId = 9) -- business trip
                        BEGIN
                            SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                            FROM #LeaveRequestHalfDay
                            WHERE EmpId = @empId AND OnDate = @attendaceDate
                            IF (DATEDIFF(MI, @RequestLeaveTimeBegin, @EndCheckInTime) >= 0)
                            BEGIN
                                IF (DATEDIFF(MI, @BeginBreakTime, @RequestLeaveTimeEnd) >= 0 
                                    AND DATEDIFF(MI, @RequestLeaveTimeEnd, @EndBreakTime) >= 0)
                                BEGIN
                                    IF (DATEDIFF(MI, @InOfMachine, DATEADD(MI, 90, @EndBreakTime)) >= 0)
                                    BEGIN
                                        SELECT @ExpectedInTime = @InOfMachine, @ExpectedOutTime = DATEADD(HH, 4, @InOfMachine)
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT @ExpectedInTime = DATEADD(MI, 90, @EndBreakTime), @ExpectedOutTime = @EndCheckOutTime
                                    END
                                END
                                ELSE
                                BEGIN
                                    SELECT @ExpectedInTime = @RequestLeaveTimeEnd, @ExpectedOutTime = DATEADD(HH, 9, @RequestLeaveTimeBegin)
                                END
                            END
                            ELSE IF (DATEDIFF(MI, @BeginCheckOutTime, @RequestLeaveTimeEnd) >= 0)
                            BEGIN
                                SELECT @ExpectedInTime = DATEADD(HH, -9, @RequestLeaveTimeEnd), @ExpectedOutTime = @RequestLeaveTimeBegin
                            END
                            ELSE
                            BEGIN
                                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
                            END
                        END
                        ELSE
                            BEGIN
                            IF (DATEDIFF(MI, @tempTimeIn, @BeginCheckInTime) >= 0) -- <8h
                            BEGIN
                                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
                            BEGIN
                                SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 4, @tempTimeIn)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
                            BEGIN
                                SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                                FROM #LeaveRequestHalfDay
                                WHERE EmpId = @empId AND OnDate = @attendaceDate

                                IF(@RequestLeaveTimeBegin < @EndBreakTime)
                                BEGIN
                                    SELECT @ExpectedInTime = @EndBreakTime, @ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                                END
                                ELSE
                                BEGIN
                                    SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = DATEADD(HH, 4, @EndCheckInTime)
                                END
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndBreakTime) >= 0) -- 10h->12h30
                            BEGIN
                                SELECT @ExpectedInTime = @EndBreakTime, @ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 5, @EndCheckInTime)) >= 0) -- 12h30-->14h00
                            BEGIN
                                SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 4, @tempTimeIn)
                            END
                            ELSE -- >14h
                            BEGIN
                                SELECT @ExpectedInTime = DATEADD(HH, 5, @EndCheckInTime), @ExpectedOutTime = @EndCheckOutTime
                            END
                        END
                    END

                    IF (DATEADD(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime)
                    BEGIN
                        -- SELECT @EarlyOut, @OutOfMachine, @ExpectedOutTime, @tempTimeIn
                        SET @ActualEarlyOut = DATEDIFF (MI, DATEADD(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime) - @DeductEarlyOut
                        IF (@ActualEarlyOut < 0)
                        BEGIN
                            SET @ActualEarlyOut = 0
                        END
                    END
                    IF (@tempTimeIn >  @ExpectedINTime) 
                    BEGIN
                        SET @ActualLateIn = DATEDIFF (MI, @ExpectedINTime, @tempTimeIn)
                        IF (@ActualLateIn < 0)
                        BEGIN
                            SET @ActualLateIn = 0
                        END
                    END
                END
                ELSE
                BEGIN
                    SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                END

                -- Break time for total hours
                IF(@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
                BEGIN
                    SET @BreakTime = 0
                    --IF (@InOfMachine < @BeginCheckInTime)
                    --BEGIN
                    --    SET @BreakTimeMins = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
                    --END

                    IF (@InOfMachine <= @BeginBreakTime AND @OutOfMachine >= @EndBreakTime)
                    BEGIN
                        SET @BreakTime = @BreakTime + @BreakTimeMins
                    END
                    ELSE IF (@InOfMachine >= @BeginBreakTime AND @OutOfMachine <= @EndBreakTime)
                    BEGIN
                        SET @BreakTime = 1440
                    END
                    ELSE IF (@InOfMachine >= @BeginBreakTime
                        AND @InOfMachine <= @EndBreakTime
                        AND @OutOfMachine >= @EndBreakTime
                        )
                    BEGIN
                        SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
                    END
                    ELSE IF (@InOfMachine <= @BeginBreakTime
                        AND @OutOfMachine >= @BeginBreakTime
                        AND @OutOfMachine <= @EndBreakTime
                        )
                    BEGIN
                        SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
                    END

                    SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime)/ 60.0 
                    IF(@TotalWorkHours < 0)
                    BEGIN
                        SET @TotalWorkHours = 0
                    END
                    
                    IF (@LeaveDay = 0.5 AND @LeaveTypeId = 9 AND @TotalWorkHours >= 8)
                    BEGIN
                        SELECT @ActualLateIn = 0, @ActualEarlyOut = 0
                    END

                END
                --SELECT @ExpectedInTime, @ExpectedOutTime, @InOfMachine, @OutOfMachine, @BreakTime, @BreakTimeMins, @BeginBreakTime, @EndBreakTime
            END
            ELSE -- shift # E3, E4, E5
            BEGIN

                SET @ExpectedInTime = @BeginCheckInTime
                SET @ExpectedOutTime = @BeginCheckOutTime
                -- get time in
                SELECT @InOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 0
                    AND RecordTime BETWEEN DATEADD(mi, -@RangeToScanTime, @ExpectedInTime) AND @ExpectedOutTime

                -- get time out
                SELECT @OutOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 1
                    AND RecordTime BETWEEN @ExpectedInTime AND DATEADD(mi, @RangeToScanTime, @ExpectedOutTime)
                    AND (@InOfMachine IS NULL OR RecordTime > @InOfMachine)
        
                IF @InOfMachine > @OutOfMachine
                BEGIN
                    SET @OutOfMachine = NULL
                END

                IF @RequestedCheckinTime IS NOT NULL
                BEGIN
                    SET @InOfMachine = @RequestedCheckinTime
                END

                IF @RequestedCheckoutTime IS NOT NULL
                BEGIN
                    SET @OutOfMachine = @RequestedCheckoutTime
                END

                IF (DATEADD(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime)
                BEGIN
                    SET @ActualEarlyOut = DATEDIFF (MINUTE, DATEADD(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime)    
                END
                IF (DATEADD(mi, -@LateIn, @InOfMachine) >  @ExpectedINTime) 
                BEGIN
                    SET @ActualLateIn = DATEDIFF (MINUTE, @ExpectedINTime, DATEADD(mi,-@LateIn , @InOfMachine)) 
                END
            
                IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
                BEGIN
                    SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 

                    IF(@IsLeaveDay = 1 and @LeaveDay = 0.5)
                    BEGIN
                        
                        SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                        FROM #LeaveRequestHalfDay
                        WHERE EmpId = @empId AND OnDate = @attendaceDate
                        IF(@RequestLeaveTimeBegin IS NOT NULL AND @RequestLeaveTimeEnd IS NOT NULL)
                        BEGIN
                            IF(@TotalWorkHours >= 4)
                            BEGIN
                                SELECT @ActualLateIn = 0, @ActualEarlyOut = 0
                            END
                            ELSE
                                BEGIN
                                IF(DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) < 5 AND DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) > -5)
                                BEGIN
                                    SET @adjustLateIn = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
                                    SET @ActualLateIn = @ActualLateIn - @adjustLateIn
                                    IF @ActualLateIn < 0
                                    BEGIN
                                        SET @ActualLateIn = 0
                                    END
                                END
                                ELSE IF (DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) < 5 AND DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) > -5)
                                BEGIN
                                    SET @adjustEarlyOut = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
                                    SET @ActualEarlyOut = @ActualEarlyOut - @adjustEarlyOut
                                    IF @ActualEarlyOut < 0
                                    BEGIN
                                        SET @ActualEarlyOut = 0
                                    END
                                END
                            END
                        END
                    END
                    -- end xu ly nghi 1/2 ngay
                END
            END

            -- hau san 7h
            IF(@TimeForPostnatal > 0 AND @ActualLateIn > 0)
            BEGIN
                IF (@TimeForPostnatal > @ActualLateIn)
                BEGIN
                    SET @TimeForPostnatal = @TimeForPostnatal - @ActualLateIn
                    SET @ActualLateIn = 0
                END
                ELSE
                BEGIN
                    SET @ActualLateIn = @ActualLateIn - @TimeForPostnatal
                    SET @TimeForPostnatal = 0
                END
            END

            IF(@TimeForPostnatal > 0 AND @ActualEarlyOut > 0)
            BEGIN
                IF (@TimeForPostnatal > @ActualEarlyOut)
                BEGIN
                    SET @TimeForPostnatal = @TimeForPostnatal - @ActualEarlyOut
                    SET @ActualEarlyOut = 0
                END
                ELSE
                BEGIN
                    SET @ActualEarlyOut = @ActualEarlyOut - @TimeForPostnatal
                    SET @TimeForPostnatal = 0
                END
            END
        END
        ELSE
        BEGIN -- No shift in attendance date 
            -- get time in
            SELECT @InOfMachine = MIN(RecordTime)
            FROM @TimeRecord 
            WHERE InOutType = 0
                AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

            -- get time out
            SELECT @OutOfMachine = MAX(RecordTime)
            FROM @TimeRecord 
            WHERE InOutType = 1
                AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

            IF @InOfMachine > @OutOfMachine
            BEGIN
                SET @OutOfMachine = NULL
            END

            IF @RequestedCheckinTime IS NOT NULL
            BEGIN
                SET @InOfMachine = @RequestedCheckinTime
            END

            IF @RequestedCheckoutTime IS NOT NULL
            BEGIN
                SET @OutOfMachine = @RequestedCheckoutTime
            END

            IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
            BEGIN
                SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine))/ 60.0 
            END
        END

        IF (@isOffDay = 1 OR @IsHoliday = 1 OR (@IsLeaveDay = 1 AND @LeaveDay = 1))
        BEGIN
            SET @Absence = 1
        END
        ELSE IF (@InOfMachine IS NULL AND @OutOfMachine IS NULL)
        BEGIN
            SET @Absence = 1
        END
        IF(@ShiftId IS NOT NULL OR @InOfMachine IS NOT NULL OR @OutOfMachine IS NOT NULL OR @IsLeaveDay = 1 OR @isOffDay = 1 OR @IsHoliday = 1)
        BEGIN
            -- insert or update
            IF (@onlyUpdate = 1 
                OR EXISTS 
                    (SELECT 1 
                        FROM dbo.EmployeeAttendance WITH (NOLOCK) 
                        WHERE EmpId = @empId 
                        AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0))
            BEGIN
                select 1, @onlyUpdate, @attendaceDate
                --update
                UPDATE dbo.EmployeeAttendance
                SET 
                    ShiftId = @ShiftId, 
                    InFixed = @InOfMachine,
                    OutFixed = @OutOfMachine,
                    ExpectedInTime = @ExpectedInTime,
                    ExpectedOutTime = @ExpectedOutTime,
                    LateIn  = @ActualLateIn,
                    EarlyOut = @ActualEarlyOut,
                    LeaveTypeId = @LeaveTypeId,
                    Absence = @Absence,
                    TotalWorkHours = @TotalWorkHours,
                    IsRightShift = @IsRightShift,
                    LeaveDay = @LeaveDay,
                    InsertDate = GETDATE()
                WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
            END
            ELSE
            BEGIN
                select 1, @onlyUpdate, @attendaceDate
                --insert
                INSERT INTO dbo.EmployeeAttendance
                (
                    EmpId,
                    AttendanceDate,
                    ShiftId,
                    InFixed,
                    OutFixed,
                    ExpectedInTime,
                    ExpectedOutTime,
                    LateIn,
                    EarlyOut,
                    LeaveTypeId,
                    Absence,
                    TotalWorkHours,
                    IsRightShift,
                    LeaveDay,
                    InsertDate
                )
                VALUES
                (
                    @empId, 
                    @attendaceDate,
                    @ShiftId,
                    @InOfMachine,
                    @OutOfMachine,
                    @ExpectedInTime,
                    @ExpectedOutTime,
                    @ActualLateIn,
                    @ActualEarlyOut,
                    @LeaveTypeId,
                    @Absence,
                    @TotalWorkHours,
                    @IsRightShift,
                    @LeaveDay,
                    GETDATE()
                )            
            END
        END
        --ELSE
        --BEGIN
        --    DELETE 
        --    FROM dbo.EmployeeAttendance 
        --    WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
        --END

        SET @i = @i + 1
        SET @attendaceDate = DATEADD(d, @i, @fromTime)
    END
END
GO
