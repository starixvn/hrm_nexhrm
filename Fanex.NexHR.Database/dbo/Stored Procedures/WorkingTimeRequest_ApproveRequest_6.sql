/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_6    Script Date: 3/15/2016 10:01:08 AM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_6] @RequestID BIGINT
	,@EmpId INT
	,@ApprovedBy INT
	,@ApprovedDate DATETIME
AS
/*
Created: 2012/04/19@Dan
Task: approve leave request form (type 6)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @LeaveTypeID INT
	,@TotalDays NUMERIC(5, 1)
	,@FromDate DATETIME
	,@ToDate DATETIME

-- get request detail
SELECT @FromDate = FromDate
	,@ToDate = ToDate
	,@LeaveTypeID = LeaveTypeID
	,@TotalDays = TotalDays
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

-- SET XACT_ABORT ON will cause the transaction to be uncommittable
-- when the constraint violation occurs. 
SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	-- Josh: Check if exist other leave request in same day --
	DECLARE @ExistLeaveTypeId INT

	SELECT @ExistLeaveTypeId = LeaveTypeId
	FROM dbo.EmployeeCalendar
	WHERE EmpID = @EmpId
		AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)

	IF (@ExistLeaveTypeId IS NULL)
	BEGIN
		-- update the working calendar to record the check-in time
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 1
			,LeaveTypeID = @LeaveTypeID
			,LeaveDay = CASE 
				WHEN @TotalDays = 0.5 THEN @TotalDays
				ELSE 1
				END
			,ModifiedDate = @ApprovedDate
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare
	END
	ELSE
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET LeaveTypeId2 = @LeaveTypeID
			,LeaveDay = 1
			,ModifiedDate = @ApprovedDate
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare
	END

	-- Josh end: Check if exist other leave request in same day --
	-- insert leave record		
	IF (
			EXISTS (
				SELECT *
				FROM dbo.LeaveType
				WHERE Id = @LeaveTypeID
					AND IsALDeduction = 1
				)
			)
	BEGIN
		EXEC AnnualLeave_InsertTransaction @EmpId
			,@RequestID
			,'Leave Request'
			,@FromDate
			,@ToDate
			,@TotalDays
			,NULL
			,@ApprovedBy
	END

	IF (@LeaveTypeID = 31)
	BEGIN
		EXEC [dbo].[Overtime_InsertTransactionFromLeaveRequest] @RequestID
			,@EmpId
			,@FromDate
			,@TotalDays
			,@ApprovedBy
	END

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2
		,-- approved
		ProcessedBy = @ApprovedBy
		,ProcessedDate = @ApprovedDate
		,ModifiedDate = @ApprovedDate
		,ModifiedBy = @ApprovedBy
	WHERE Id = @RequestID

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1

	COMMIT TRANSACTION;
END TRY

BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE()
		,@ErrorSeverity = ERROR_SEVERITY()
		,@ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

	-- Use RAISERROR inside the CATCH block to return 
	-- error information about the original error that 
	-- caused execution to jump to the CATCH block.
	RAISERROR (
			@ErrorMessage, -- Message text.
			@ErrorSeverity, -- Severity.
			@ErrorState -- State.
			);
END CATCH;