USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeWorkFromHome_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Delete] @EmployeeWorkFromHomeId INT
	,@DeletedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Delete Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @empId INT, @workFromHomeRequestId INT, @fromDate DATE, @toDate DATE
	SELECT @empId = ewfh.EmpId , @workFromHomeRequestId = ewfh.WorkFromHomeRequestId ,@fromDate = ewfh.FromDate, @toDate = ewfh.ToDate
			FROM dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ewfh.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			INNER JOIN dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK) ON ewfh.WorkFromHomeRequestId = wfhr.Id
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId AND wfhr.CreatedBy = @DeletedBy
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Cannot delete employee request'

		RETURN
	END

	DELETE
	FROM dbo.EmployeeWorkFromHome 
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
	
	DELETE
	FROM dbo.WorkFromHomeRequest 
	WHERE Id = @workFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @fromDate, @toDate, 3

	SELECT @ErrCode = 0
END
GO
