USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
    @id int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@LeaveTypeId INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDays NUMERIC(5, 1)
		,@RequestType SMALLINT
		,@Status INT

	SELECT 
		@EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
		,@RequestType = RequestType
		,@Status = Status
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @id

	IF (@Status = 1)
	BEGIN
		DECLARE @LogSPData VARCHAR(4000), @now DATETIME
		SET @now = GETDATE()
		SELECT @LogSPData = '<WorkingTimeRequest_Delete RequestID="'+ coalesce(cast(@id as varchar(50)),space(1)) + '" />'

		EXEC dbo.Log_WriteLog
			@LogActionID = 11,
			@LogDate = @now,
			@Description = 'Delete Request',
			@TrackingValue = @LogSPData,
			@UserID = @EmpId

		IF (@RequestType = 6)
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @id
			,@EmpId
			,@LeaveTypeId
			,@FromDate
			,@ToDate
			,@TotalDays
			,NULL

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		-- Insert statements for procedure here
		DELETE WorkingTimeRequest
		WHERE Id = @id
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 102, @ErrMsg = 'Status of request has been changed. Please check again.'
	END
END
GO
