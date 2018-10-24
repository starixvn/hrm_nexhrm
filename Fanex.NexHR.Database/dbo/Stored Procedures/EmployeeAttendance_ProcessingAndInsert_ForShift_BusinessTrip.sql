

CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift_BusinessTrip] 
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
Created: 20180919@Quinn.Vo
Task: Calculate time for shift M1 & Business Trip.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
		,@TempTimeIn DATETIME
		,@TempTimeOut DATETIME
        ,@IsUserStandby BIT
		,@AdjustTotalHours DECIMAL(5,2) = 0

    SELECT @IsUserStandby = 0
    IF (EXISTS(SELECT 1 FROM dbo.Employee WITH (NOLOCK) WHERE GroupId IN (61, 59, 46)))
    BEGIN
        SET @IsUserStandby = 1
    END

    IF @RequestedCheckinTime IS NULL
    BEGIN
        SELECT @InOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType = 0
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0
    END
	ELSE
	BEGIN
		SET @InOfMachine = @RequestedCheckinTime
	END

    IF @RequestedCheckoutTime IS NULL
    BEGIN
        SELECT @OutOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType = 1
			-- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
			AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
			AND ((@IsUserStandby = 0 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0)
				OR (@IsUserStandby = 1 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 9, DATEADD(D, 1, @WorkDate))) >= 0) -- to 9h
			)
			AND (
				@InOfMachine IS NULL
				OR RecordTime > DATEADD(MI, 2, @InOfMachine)
				)
    END
	ELSE
	BEGIN
		SET @OutOfMachine = @RequestedCheckoutTime
	END

    SELECT @RequestLeaveTimeBegin = FromTime
        ,@RequestLeaveTimeEnd = ToTime
		,@TempTimeIn = FromTime
		,@TempTimeOut = ToTime
    FROM #LeaveRequestHalfDay
    WHERE EmpId = @empId
        AND OnDate = @WorkDate

		IF(ISNULL(@LeaveDay, 0) = 1)
		BEGIN
			IF(@InOfMachine IS NULL OR @OutOfMachine IS NULL)
			BEGIN
				SELECT @TotalWorkHours = ((DATEDIFF(mi, @EndCheckInTime, @EndCheckOutTime) - @BreakTimeMins) / 60.0)
			END
			ELSE
			BEGIN
				SELECT @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTimeMins) / 60.0)
				IF(@TotalWorkHours < 8)
				BEGIN
					SELECT @TotalWorkHours = 8, @ActualLateIn = 0, @ActualEarlyOut = 0
				END
			END
		END
		ELSE
		BEGIN
			--0.5 day
			IF (@InOfMachine IS NOT NULL)
			BEGIN
				-- Get @ExpectedInTime, @ExpectedOutTime
				IF (@RequestLeaveTimeBegin > @InOfMachine)
				BEGIN
					SELECT @TempTimeIn = @InOfMachine
				END
				
				IF (@OutOfMachine IS NOT NULL AND @RequestLeaveTimeEnd < @OutOfMachine)
				BEGIN
					SELECT @TempTimeOut = @OutOfMachine
				END

				IF (ISNULL(@BreakTimeMins, 0) = 0)
				BEGIN
					SELECT @ExpectedInTime = @EndCheckInTime
						,@ExpectedOutTime = @EndCheckOutTime
				END
				ELSE
				BEGIN
					IF (DATEDIFF(MI, @TempTimeIn, @BeginCheckInTime) >= 0) -- <8h
					BEGIN
						SELECT @ExpectedInTime = @BeginCheckInTime
							,@ExpectedOutTime = @BeginCheckOutTime
					END
					ELSE IF (DATEDIFF(MI, @TempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
					BEGIN
						SELECT @ExpectedInTime = @TempTimeIn
							,@ExpectedOutTime = DATEADD(HH, 9, @TempTimeIn)
					END
					ELSE IF (DATEDIFF(MI, @TempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
					BEGIN
						SELECT @ExpectedInTime = @EndCheckInTime
							,@ExpectedOutTime = @EndCheckOutTime
					END
					ELSE -- >10h
					BEGIN
						SELECT @ExpectedInTime = @EndCheckInTime
							,@ExpectedOutTime = @EndCheckOutTime
					END
				END
				-- calculate total working hours, late in/early out
				IF (@TempTimeOut < @ExpectedOutTime)
				BEGIN
					SET @ActualEarlyOut = DATEDIFF(MI, @TempTimeOut, @ExpectedOutTime)
				END

				IF (@TempTimeIn > @ExpectedInTime)
				BEGIN
					SET @ActualLateIn = DATEDIFF(MI, @ExpectedInTime, @TempTimeIn)
				END

				SET @TotalWorkHours = DATEDIFF(mi, @TempTimeIn, @TempTimeOut) / 60.0

				IF (@OutOfMachine IS NOT NULL)
				BEGIN
					IF (ISNULL(@BreakTimeMins, 0) = 0)
					BEGIN
						IF (@RequestLeaveTimeEnd < @InOfMachine)
						BEGIN
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@OutOfMachine < @RequestLeaveTimeBegin)
						BEGIN
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE
						BEGIN
							SELECT @TotalWorkHours = DATEDIFF(mi, @TempTimeIn, @TempTimeOut) / 60.0
						END
					END
					ELSE
					BEGIN
						IF(@RequestLeaveTimeEnd < @InOfMachine)
					BEGIN
						IF(@EndBreakTime <@RequestLeaveTimeBegin)
						BEGIN
							--1: @BeginBreakTime < @EndBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@RequestLeaveTimeBegin >= @BeginBreakTime AND @RequestLeaveTimeBegin <= @EndBreakTime
							AND @EndBreakTime < @RequestLeaveTimeEnd)
						BEGIN
							-- 2: @BeginBreakTime < @RequestLeaveTimeBegin < @EndBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
								- DATEDIFF(mi, @RequestLeaveTimeBegin, @EndBreakTime)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							-- 3: @RequestLeaveTimeBegin < @BeginBreakTime < @EndBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd 
							AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime <= @InOfMachine)
						BEGIN
							-- 4: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @EndBreakTime, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @EndBreakTime, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime < @InOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime < @InOfMachine)
						BEGIN
							-- 5: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime <= @InOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--6: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @InOfMachine < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							-- 7: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @BeginBreakTime < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							-- 8: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @BeginBreakTime < @OutOfMachine < @EndBreakTime 
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine) 
								- DATEDIFF(mi, @BeginBreakTime, @OutOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime > @OutOfMachine)
						BEGIN
							--9: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @BeginBreakTime < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime <= @RequestLeaveTimeBegin AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime <= @InOfMachine)
						BEGIN
							--10: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeBegin, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @EndBreakTime, @InOfMachine) - DATEDIFF(mi, @BeginBreakTime, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--11: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime <= @InOfMachine
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							--12: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime <= @RequestLeaveTimeBegin AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--13: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @EndBreakTime < @OutOfMachine 
							SELECT @TotalWorkHours = (DATEDIFF(mi, @EndBreakTime, @OutOfMachine)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							--14: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @BeginBreakTime)) / 60.0
						END
						ELSE
						BEGIN
							--15: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = 0
						END
					END
					ELSE IF (@OutOfMachine < @RequestLeaveTimeBegin)
					BEGIN
						IF(@EndBreakTime <@InOfMachine)
						BEGIN
							-- 1
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @InOfMachine <= @EndBreakTime
							AND @EndBreakTime < @OutOfMachine)
						BEGIN
							-- 2
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
								- DATEDIFF(mi, @InOfMachine, @EndBreakTime)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							-- 3
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine 
							AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime <= @RequestLeaveTimeBegin)
						BEGIN
							-- 4
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime < @RequestLeaveTimeBegin
							AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime < @RequestLeaveTimeBegin)
						BEGIN
							-- 5
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(MI, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime <= @RequestLeaveTimeBegin
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--6
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							-- 7
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							-- 8
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin) 
								- DATEDIFF(mi, @BeginBreakTime, @RequestLeaveTimeEnd)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime > @RequestLeaveTimeEnd)
						BEGIN
							--9	
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime <= @RequestLeaveTimeBegin)
						BEGIN
							--10
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--11
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime <= @RequestLeaveTimeBegin
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							--12
							SELECT @TotalWorkHours = (DATEDIFF(mi, @InOfMachine, @OutOfMachine)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--13
							SELECT @TotalWorkHours = (DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeEnd)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							--14
							SELECT @TotalWorkHours = (DATEDIFF(mi, @InOfMachine, @BeginBreakTime)) / 60.0
						END
						ELSE
						BEGIN
							--15
							SELECT @TotalWorkHours = 0
						END
					END
					ELSE
					BEGIN
						-- 6 case
						IF (@BeginBreakTime <= @TempTimeIn AND @EndBreakTime >= @TempTimeIn AND @EndBreakTime <= @TempTimeOut)
						BEGIN
							-- 2:
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @TempTimeIn, @EndBreakTime)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @TempTimeIn AND @BeginBreakTime <= @TempTimeOut
							AND @EndBreakTime >= @TempTimeIn AND @EndBreakTime <= @TempTimeOut)
						BEGIN
							--3:
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @TempTimeIn AND @BeginBreakTime <= @TempTimeOut
							AND @EndBreakTime >= @TempTimeOut)
						BEGIN
							-- 4
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @BeginBreakTime, @TempTimeOut)) / 60.0
						END
						ELSE IF (@BeginBreakTime <= @TempTimeIn AND @EndBreakTime >= @TempTimeOut)
						BEGIN
							--6
							SELECT @TotalWorkHours = 0
						END
						-- case 1&5: nothing
					END
					END
				END
			END

			IF (@OutOfMachine IS NULL)
			BEGIN
				SELECT @ActualEarlyOut = 0, @TotalWorkHours = 0
			END
		END

		IF (@TempTimeIn IS NULL)
		BEGIN
			SELECT @TempTimeIn = @InOfMachine
		END

		IF (@TempTimeIn < @BeginCheckInTime)
		BEGIN
			SELECT @AdjustTotalHours = DATEDIFF(MI, @TempTimeIn, @BeginCheckInTime) / 60.0
		END

		SELECT @TotalWorkHours = @TotalWorkHours - @AdjustTotalHours
		IF (@TotalWorkHours < 0)
		BEGIN
			SELECT @TotalWorkHours = 0
		END
END


