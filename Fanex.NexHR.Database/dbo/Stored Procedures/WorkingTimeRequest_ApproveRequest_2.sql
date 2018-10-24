

/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_2    Script Date: 3/13/2016 3:21:43 PM ******/
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_2] @RequestID BIGINT
	,@EmpId INT
	,@ApprovedBy INT
	,@ApprovedDate DATETIME
AS
/*
Created: 2012/04/18@Dan
Task: approve check-out request form (type 2)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
-- get request check-in time
DECLARE @CheckoutTime DATETIME, @WorkDate DATETIME, @IsCrossDate BIT

SELECT @WorkDate = FromDate,
	@IsCrossDate = IsCrossDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

IF (
		EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
				INNER JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cl.ShiftId = sh.Id
			WHERE sh.IsOutAfterDay = 1
			AND cl.EmpId = @EmpId
			AND DATEDIFF(dd, WorkDate, @WorkDate) = 0
			)
		)
BEGIN
	SET @CheckoutTime = DATEADD(dd, 1, @WorkDate)
END
ELSE
BEGIN
	SET @CheckoutTime = @WorkDate
END

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET RequestedCheckoutTime = CASE WHEN @IsCrossDate = 1 THEN DATEADD(d, 1, @CheckoutTime)  ELSE @CheckoutTime END
		,ModifiedDate = @ApprovedDate
		,ModifiedBy = @ApprovedBy
	WHERE EmpID = @EmpId
		AND datediff(d, WorkDate, @WorkDate) = 0

	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@CheckoutTime
		,@CheckoutTime
		,1

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

