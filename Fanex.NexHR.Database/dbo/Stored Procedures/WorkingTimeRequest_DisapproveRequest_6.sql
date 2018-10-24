/****** Object:  Stored Procedure dbo.WorkingTimeRequest_DisapproveRequest_6    Script Date: 3/15/2016 10:01:09 AM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_6] @RequestID BIGINT
	,@EmpId INT
	,@ModifiedDate DATETIME
	,@ModifiedBy INT
AS
/*
Created: 2012/04/25@Dan
Task: disapprove leave request form (type 6)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @LeaveTypeID INT
	,@TotalDays NUMERIC(5, 1)
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@LeaveTransId BIGINT

--init param
SET @LeaveTransId = 0

-- get all request detail 
SELECT @FromDate = FromDate
	,@ToDate = ToDate
	,@LeaveTypeID = LeaveTypeID
	,@TotalDays = TotalDays
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	-- Josh: Check if exist other leave request in same day --
	DECLARE @ExistLeaveTypeId INT
		,@ExistLeaveTypeId2 INT

	SELECT @ExistLeaveTypeId = LeaveTypeId
		,@ExistLeaveTypeId2 = LeaveTypeId2
	FROM dbo.EmployeeCalendar
	WHERE EmpID = @EmpId
		AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)

	IF (@ExistLeaveTypeId2 IS NULL)
	BEGIN
		-- update the working calendar to clear the check-in time
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 0
			,LeaveTypeID = NULL
			,LeaveDay = 0
			,ModifiedDate = @ModifiedDate
			,ModifiedBy = @ModifiedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare
	END
	ELSE
	BEGIN
		IF (@ExistLeaveTypeId <> @ExistLeaveTypeId2)
		BEGIN
			IF (@ExistLeaveTypeId2 = @LeaveTypeID)
			BEGIN
				UPDATE dbo.EmployeeCalendar
				SET LeaveTypeId2 = NULL
					,LeaveDay = 0.5
					,ModifiedDate = @ModifiedDate
					,ModifiedBy = @ModifiedBy
				WHERE EmpID = @EmpId
					AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
						AND CONVERT(VARCHAR(10), @ToDate, 111)
			END

			IF (@ExistLeaveTypeId = @LeaveTypeID)
			BEGIN
				UPDATE dbo.EmployeeCalendar
				SET LeaveTypeId = @ExistLeaveTypeId2
					,LeaveTypeId2 = NULL
					,LeaveDay = 0.5
					,ModifiedDate = @ModifiedDate
					,ModifiedBy = @ModifiedBy
				WHERE EmpID = @EmpId
					AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
						AND CONVERT(VARCHAR(10), @ToDate, 111)
			END
		END
		ELSE
		BEGIN
			UPDATE dbo.EmployeeCalendar
			SET LeaveTypeId2 = NULL
				,LeaveDay = 0.5
				,ModifiedDate = @ModifiedDate
				,ModifiedBy = @ModifiedBy
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
					AND CONVERT(VARCHAR(10), @ToDate, 111)
		END
	END

	-- Josh end: Check if exist other leave request in same day --
	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1

	-- get id from the leave trans
	SELECT @LeaveTransId = Id
	FROM dbo.LeaveTrans
	WHERE RequestId = @RequestId
		AND EmpID = @EmpId
		AND FromDate = @FromDate
		AND ToDate = @ToDate
		AND ISNULL(Deleted, 0) = 0

	-- delete the leave transaction
	IF (ISNULL(@LeaveTransId, 0) <> 0)
		EXEC AnnualLeave_DeleteTransaction @LeaveTransId
			,@ModifiedBy

	IF (@LeaveTypeID = 31)
	BEGIN
		EXEC [dbo].[Overtime_DeleteTransactionFromLeaveRequest] @RequestID
			,@EmpId
			,@FromDate
			,@TotalDays
			,@ModifiedBy
	END

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 1
		,-- pending
		ProcessedBy = @ModifiedBy
		,ProcessedDate = @ModifiedDate
		,ModifiedDate = @ModifiedDate
		,ModifiedBy = @ModifiedBy
	WHERE Id = @RequestID

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