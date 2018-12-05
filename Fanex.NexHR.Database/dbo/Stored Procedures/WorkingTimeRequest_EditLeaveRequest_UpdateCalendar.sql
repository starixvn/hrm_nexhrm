
CREATE PROCEDURE [dbo].[WorkingTimeRequest_EditLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@OldLeaveTypeId INT
	,@OldFromDate DATETIME
	,@OldToDate DATETIME
	,@OldTotalDays NUMERIC(5, 1)
	,@ModifiedBy INT
AS
/*
Created: Quinn.Vo
Task: Update Calendar when edit leave request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @RequestId
		,@EmpId
		,@OldLeaveTypeId
		,@OldFromDate
		,@OldToDate
		,@OldTotalDays
		,@ModifiedBy

	EXEC [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] @RequestId
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@ModifiedBy

	IF (DATEDIFF(DD, @FromDate, @OldFromDate) >= 0 AND DATEDIFF(DD, @OldFromDate, @ToDate) >= 0)
	 OR (DATEDIFF(DD, @FromDate, @OldToDate) >= 0 AND DATEDIFF(DD, @OldToDate, @ToDate) >= 0)
	 OR (DATEDIFF(DD, @OldFromDate, @FromDate) >= 0 AND DATEDIFF(DD, @FromDate, @OldToDate) >= 0)
	 OR (DATEDIFF(DD, @OldFromDate, @ToDate) >= 0 AND DATEDIFF(DD, @ToDate, @OldToDate) >= 0)
	 BEGIN
		DECLARE @TempFromDate DATETIME
			,@TempToDate DATETIME

		IF(@FromDate > @OldFromDate)
		BEGIN
			SET @TempFromDate = @OldFromDate
		END
		ELSE
		BEGIN
			SET @TempFromDate = @FromDate
		END

		IF(@ToDate > @OldToDate)
		BEGIN
			SET @TempToDate = @ToDate
		END
		ELSE
		BEGIN
			SET @TempToDate = @OldToDate
		END

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@TempFromDate
			,@TempToDate
			,1
	 END
	 ELSE
	 BEGIN
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@OldFromDate
			,@OldToDate
			,1
	 END
END