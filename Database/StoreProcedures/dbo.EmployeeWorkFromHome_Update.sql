USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeWorkFromHome_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Update] @EmployeeWorkFromHomeId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Remark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Update Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @WorkFromHomeRequestId INT, @FromDateWFH DATE,  @ToDateWFH DATE
	SELECT @WorkFromHomeRequestId = WorkFromHomeRequestId,@FromDateWFH = ew.FromDate, @ToDateWFH = ew.ToDate
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
				 INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE ew.EmployeeWorkFromHomeId <> @EmployeeWorkFromHomeId
				AND ew.EmpId = @EmpId
				AND (
					@FromDate BETWEEN ew.FromDate
						AND ew.ToDate
					OR @ToDate BETWEEN ew.FromDate
						AND ew.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeeWorkFromHome WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,Remark = @Remark
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	UPDATE dbo.WorkFromHomeRequest WITH (ROWLOCK, UPDLOCK)
	SET FromDate = @FromDate
		,ToDate = @ToDate
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDateWFH ,@ToDateWFH , 0

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 1 

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDateWFH, @ToDateWFH, 3

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 1
END


GO
