USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId_new]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId_new] 
    @empId INT
    ,@fromTime DATETIME
    ,@toTime DATETIME
    ,@onlyUpdate BIT = 0
AS
BEGIN
    DECLARE @RangeToScanTime INT
        ,@IsFlexibleTime BIT
        ,@DeductEarlyOut INT
        ,@BreakTime INT
        ,@i INT
        ,@WorkDate DATETIME
        ,@ShiftId INT
        ,@IsHoliday BIT
        ,@isOffDay BIT
        ,@IsLeaveDay BIT
        ,@LeaveTypeId INT
		,@LeaveTypeId2 INT
        ,@LateIn INT
        ,@EarlyOut INT
        ,@RequestedCheckinTime DATETIME
        ,@RequestedCheckoutTime DATETIME
        ,@BeginCheckInTime DATETIME
        ,@EndCheckInTime DATETIME
        ,@BeginCheckOutTime DATETIME
        ,@EndCheckOutTime DATETIME
        ,@InOfMachine DATETIME
        ,@OutOfMachine DATETIME
        ,@ExpectedInTime DATETIME
        ,@ExpectedOutTime DATETIME
        ,@Absence BIT
        ,@ActualLateIn INT
        ,@ActualEarlyOut INT
        ,@TotalWorkHours DECIMAL(18, 2)
        ,@BreakTimeMins INT
        ,@BeginBreakTime DATETIME
        ,@EndBreakTime DATETIME
        ,@LeaveDay DECIMAL(18, 1)
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@adjustLateIn INT
        ,@adjustEarlyOut INT
        ,@TimeForPostnatal INT

    SET @i = 0
    SET @WorkDate = CONVERT(VARCHAR(10), @fromTime, 111)
	
    WHILE (DATEDIFF(d, @WorkDate, @toTime) >= 0)
    BEGIN
        SELECT @Absence = 0
            ,@InOfMachine = NULL
            ,@OutOfMachine = NULL
            ,@ExpectedOutTime = NULL
            ,@ExpectedInTime = NULL
            ,@ExpectedOutTime = NULL
            ,@ActualLateIn = 0
            ,@ActualEarlyOut = 0
            ,@RequestedCheckinTime = NULL
            ,@RequestedCheckoutTime = NULL
            ,@TotalWorkHours = NULL
            ,@BreakTimeMins = 0
            ,@ShiftId = NULL
            ,@LeaveTypeId = NULL
			,@LeaveTypeId2 = NULL
            ,@RequestLeaveTimeBegin = NULL
            ,@RequestLeaveTimeEnd = NULL
            ,@adjustLateIn = 0
            ,@adjustEarlyOut = 0
            ,@DeductEarlyOut = 0
            ,@BeginBreakTime = NULL
            ,@EndBreakTime = NULL
            ,@TimeForPostnatal = 0

        SELECT @WorkDate = WorkDate
            ,@ShiftId = ShiftId
            ,@IsHoliday = IsHoliday
            ,@isOffDay = IsOffDay
            ,@IsLeaveDay = IsLeaveDay
            ,@LeaveTypeId = LeaveTypeId
			,@LeaveTypeId2 = LeaveTypeId2
            ,@LateIn = ISNULL(LateIn, 0)
            ,@EarlyOut = ISNULL(EarlyOut, 0)
            ,@RequestedCheckinTime = RequestedCheckinTime
            ,@RequestedCheckoutTime = RequestedCheckoutTime
            ,@BeginCheckInTime = BeginCheckInTime
            ,@EndCheckInTime = EndCheckInTime
            ,@BeginCheckOutTime = BeginCheckOutTime
            ,@EndCheckOutTime = EndCheckOutTime
            ,@BreakTimeMins = BreakTimeMins
            ,@BeginBreakTime = BeginBreakTime
            ,@EndBreakTime = EndBreakTime
            ,@LeaveDay = LeaveDay
            ,@TimeForPostnatal = TimeForPostnatal
        FROM #EmpCalendar
        WHERE EmpId = @empId
            AND WorkDate = @WorkDate

        -- If Has Shift
        IF @ShiftId IS NOT NULL
        BEGIN
			IF (@LeaveTypeId = 9 OR @LeaveTypeId = 32 OR @LeaveTypeId2 = 9 OR @LeaveTypeId2 = 32)
			BEGIN
				EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift_BusinessTrip] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
					,@LeaveTypeId = @LeaveTypeId
					,@LeaveTypeId2 = @LeaveTypeId2
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
			END
			ELSE IF (@ShiftId = 10) -- shift M1
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM1] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 11) -- shift M2
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM2] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 12) -- shift M3
            BEGIN
			
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM3] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE -- shift # M1, M2, M3
            BEGIN
				
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@IsLeaveDay = @IsLeaveDay
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
        END
        ELSE
        BEGIN -- No shift in attendance date 
            EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForNoShift] @empId = @empId
                ,@WorkDate = @WorkDate
                ,@ShiftId = @ShiftId
                ,@IsLeaveDay = @IsLeaveDay
                ,@LeaveTypeId = @LeaveTypeId
                ,@LateIn = @LateIn
                ,@EarlyOut = @EarlyOut
                ,@RequestedCheckinTime = @RequestedCheckinTime
                ,@RequestedCheckoutTime = @RequestedCheckoutTime
                ,@BeginCheckInTime = @BeginCheckInTime
                ,@EndCheckInTime = @EndCheckInTime
                ,@BeginCheckOutTime = @BeginCheckOutTime
                ,@EndCheckOutTime = @EndCheckOutTime
                ,@BreakTimeMins = @BreakTimeMins
                ,@BeginBreakTime = @BeginBreakTime
                ,@EndBreakTime = @EndBreakTime
                ,@LeaveDay = @LeaveDay
                ,@TimeForPostnatal = @TimeForPostnatal
                ,@InOfMachine = @InOfMachine OUT
                ,@OutOfMachine = @OutOfMachine OUT
                ,@ExpectedInTime = @ExpectedInTime OUT
                ,@ExpectedOutTime = @ExpectedOutTime OUT
                ,@ActualLateIn = @ActualLateIn OUT
                ,@ActualEarlyOut = @ActualEarlyOut OUT
                ,@TotalWorkHours = @TotalWorkHours OUT
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

        IF(@LateIn > 0 AND @ActualLateIn > 0)
        BEGIN
            IF (@LateIn > @ActualLateIn)
            BEGIN
                SET @ActualLateIn = 0
            END
            ELSE
            BEGIN
                SET @ActualLateIn = @ActualLateIn - @LateIn
            END
        END

        IF(@EarlyOut > 0 AND @ActualEarlyOut > 0)
        BEGIN
            IF (@EarlyOut > @ActualEarlyOut)
            BEGIN
                SET @ActualEarlyOut = 0
            END
            ELSE
            BEGIN
                SET @ActualEarlyOut = @ActualEarlyOut - @EarlyOut
            END
        END

        IF (
                @isOffDay = 1
                OR @IsHoliday = 1
                OR (
                    @IsLeaveDay = 1
                    AND @LeaveDay = 1
                    )
                )
        BEGIN
            SET @Absence = 1
        END
        ELSE IF (
                @InOfMachine IS NULL
                AND @OutOfMachine IS NULL
                )
        BEGIN
            SET @Absence = 1
        END

        IF (
                @ShiftId IS NOT NULL
                OR @InOfMachine IS NOT NULL
                OR @OutOfMachine IS NOT NULL
                OR @IsLeaveDay = 1
                OR @isOffDay = 1
                OR @IsHoliday = 1
                )
        BEGIN
            -- insert or update
            IF (
                    EXISTS (
                        SELECT 1
                        FROM dbo.EmployeeAttendance WITH (NOLOCK)
                        WHERE EmpId = @empId
                            AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
                        )
                    )
            BEGIN
                UPDATE dbo.EmployeeAttendance
                SET ShiftId = @ShiftId
                    ,InFixed = @InOfMachine
                    ,OutFixed = @OutOfMachine
                    ,ExpectedInTime = @ExpectedInTime
                    ,ExpectedOutTime = @ExpectedOutTime
                    ,LateIn = @ActualLateIn
                    ,EarlyOut = @ActualEarlyOut
                    ,LeaveTypeId = @LeaveTypeId
                    ,Absence = @Absence
                    ,TotalWorkHours = @TotalWorkHours
                    ,IsRightShift = NULL
                    ,LeaveDay = @LeaveDay
                    ,InsertDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @empId
                    AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
            END
            ELSE
            BEGIN
				
                INSERT INTO dbo.EmployeeAttendance (
                    EmpId
                    ,AttendanceDate
                    ,ShiftId
                    ,InFixed
                    ,OutFixed
                    ,ExpectedInTime
                    ,ExpectedOutTime
                    ,LateIn
                    ,EarlyOut
                    ,LeaveTypeId
                    ,Absence
                    ,TotalWorkHours
                    ,IsRightShift
                    ,LeaveDay
                    ,InsertDate
                    )
                VALUES (
                    @empId
                    ,@WorkDate
                    ,@ShiftId
                    ,@InOfMachine
                    ,@OutOfMachine
                    ,@ExpectedInTime
                    ,@ExpectedOutTime
                    ,@ActualLateIn
                    ,@ActualEarlyOut
                    ,@LeaveTypeId
                    ,@Absence
                    ,@TotalWorkHours
                    ,NULL
                    ,@LeaveDay
                    ,dbo.fnc_GetClientDateTime()
                    )
            END
        END
        ELSE
        BEGIN
            DELETE
            FROM dbo.EmployeeAttendance
            WHERE EmpId = @empId
                AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
        END

		IF (@onlyUpdate = 1)
        BEGIN
            EXEC [dbo].[LateInEarlyOut_InsertTransaction] @empId, @WorkDate, @ActualLateIn, @ActualEarlyOut
        END

        SET @i = @i + 1
        SET @WorkDate = DATEADD(d, @i, @fromTime)
    END
END
GO
