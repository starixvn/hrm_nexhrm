CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest] @RequestID BIGINT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/23@Dan
Task: reject a request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@LeaveTypeId INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDays NUMERIC(5, 1)

	SELECT @EmpId = EmpId
		,@RequestType = RequestType
		,@Status = [status]
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @RequestID

	-- if status is not pending, show error
	IF (@Status <> 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This request is not pending so it cannot be rejected'

		RETURN
	END

	-- if status is not pending, show error
	IF (
			@ProcessedBy IS NULL
			OR @ProcessedDate IS NULL
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = '@ProcessedBy or @ProcessedDate parameter is missing'

		RETURN
	END

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 3
		,-- rejected
		ProcessedBy = @ProcessedBy
		,ProcessedDate = @ProcessedDate
		,ModifiedDate = @ProcessedDate
		,ModifiedBy = @ProcessedBy
	WHERE Id = @RequestID

	SET @ErrCode = 0

	-- write to history
	EXEC [WorkingTimeRequest_WriteHistory] @RequestID

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @RequestID

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @RequestID
			,@EmpId
			,@LeaveTypeId
			,@FromDate
			,@ToDate
			,@TotalDays
			,@ProcessedBy

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
	END

	-- write log
	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = dbo.fnc_GetClientDateTime()

	--Log general info
	SELECT @LogSPData = '<WorkingTimeRequest_RejectRequest RequestID="' + coalesce(cast(@RequestID AS VARCHAR(50)), space(1)) + '" ' + 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 2
		,@LogDate = @now
		,@Description = 'Reject Request'
		,@TrackingValue = @LogSPData
		,@UserID = @ProcessedBy
END
GO
