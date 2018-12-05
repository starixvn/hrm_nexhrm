
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
BEGIN TRY
	BEGIN TRANSACTION;

	DECLARE @LeaveTypeID INT
	,@TotalDays NUMERIC(5, 1)
	,@FromDate DATETIME
	,@ToDate DATETIME

	-- insert leave record		
	IF (
			EXISTS (
				SELECT 1
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