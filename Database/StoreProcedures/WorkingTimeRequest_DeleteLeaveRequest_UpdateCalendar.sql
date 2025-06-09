CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@ModifiedBy INT = NULL
AS
/*
Created: Quinn.Vo
Task: Update Calendar when approve leave request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
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
			,ModifiedDate = dbo.fnc_GetClientDateTime()
			,ModifiedBy = @ModifiedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111) 
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
					,ModifiedDate = dbo.fnc_GetClientDateTime()
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
					,ModifiedDate = dbo.fnc_GetClientDateTime()
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
				,ModifiedDate = dbo.fnc_GetClientDateTime()
				,ModifiedBy = @ModifiedBy
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
					AND CONVERT(VARCHAR(10), @ToDate, 111)
		END
	END

	
END
GO
