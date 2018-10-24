

CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift]
	@empId INT
	,@WorkDate DATETIME
	,@IsLeaveDay BIT
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
Task: Calculate time for shift #M1, M2, M3.
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
		,@AdjustTotalHours DECIMAL(5,2) = 0

	SELECT @RangeToScanTime = 480, @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime

	-- get time in
	SELECT @InOfMachine = MIN(RecordTime)
	FROM #TimeRecord
	WHERE EmpId = @empId
		AND InOutType = 0
		AND RecordTime BETWEEN DATEADD(mi, -@RangeToScanTime, @ExpectedInTime) AND @ExpectedOutTime

	IF @RequestedCheckinTime IS NOT NULL
	BEGIN
		SET @InOfMachine = @RequestedCheckinTime
	END

	-- get time out
	SELECT @OutOfMachine = MIN(RecordTime)
	FROM #TimeRecord 
	WHERE EmpId = @empId
		AND InOutType = 1
		AND RecordTime BETWEEN @ExpectedInTime AND DATEADD(mi, @RangeToScanTime, @ExpectedOutTime)
		AND (@InOfMachine IS NULL OR DATEDIFF(D, RecordTime, DATEADD(MI, 2, @InOfMachine)) >= 0)

	IF @RequestedCheckoutTime IS NOT NULL
	BEGIN
		SET @OutOfMachine = @RequestedCheckoutTime
	END

	IF (DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) > 0)
	BEGIN
		SET @ActualEarlyOut = DATEDIFF (MI, @OutOfMachine, @ExpectedOutTime)	
	END
	IF (DATEDIFF(MI, @ExpectedINTime, @InOfMachine) > 0) 
	BEGIN
		SET @ActualLateIn = DATEDIFF (MI, @ExpectedINTime, @InOfMachine) 
	END
			
	IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
	BEGIN
		SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 

		IF(@IsLeaveDay = 1 and @LeaveDay = 0.5)
		BEGIN 
			SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
			FROM #LeaveRequestHalfDay
			WHERE EmpId = @empId AND OnDate = @WorkDate
			IF(@RequestLeaveTimeBegin IS NOT NULL AND @RequestLeaveTimeEnd IS NOT NULL)
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

		IF (@InOfMachine < @BeginCheckInTime)
		BEGIN
			SELECT @AdjustTotalHours = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) / 60.0
		END

		SELECT @TotalWorkHours = @TotalWorkHours - @AdjustTotalHours
		IF (@TotalWorkHours < 0)
		BEGIN
			SELECT @TotalWorkHours = 0
		END
	END
END

