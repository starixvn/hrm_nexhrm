

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
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

	SELECT 
		@EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @id

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
    where Id = @id and Status != 2
END

