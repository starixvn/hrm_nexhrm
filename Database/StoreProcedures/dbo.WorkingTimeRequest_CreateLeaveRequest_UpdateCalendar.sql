USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@ApprovedBy INT
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

	SELECT @ExistLeaveTypeId = LeaveTypeId
	FROM dbo.EmployeeCalendar
	WHERE EmpID = @EmpId
		AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)

	IF (@ExistLeaveTypeId IS NULL)
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 1
			,LeaveTypeID = @LeaveTypeID
			,LeaveDay = CASE 
				WHEN @TotalDays = 0.5 THEN @TotalDays
				ELSE 1
				END
			,ModifiedDate = GETDATE()
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111)
	END
	ELSE
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET LeaveTypeId2 = @LeaveTypeID
			,LeaveDay = 1
			,ModifiedDate = GETDATE()
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111)
	END
END
GO
