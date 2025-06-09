USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkFromHomeRequest_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Delete] @WorkFromHomeRequestId INT
	,@DeletedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211224@Teddy.le
Task: Delete Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @empId INT, @fromDate DATE, @toDate DATE
	SELECT @empId = wfhr.EmpId , @fromDate = wfhr.FromDate, @toDate = wfhr.ToDate
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	DELETE
	FROM dbo.WorkFromHomeRequest 
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @fromDate, @toDate, 3

	SELECT @ErrCode = 0
END
GO
