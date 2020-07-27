


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

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1

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