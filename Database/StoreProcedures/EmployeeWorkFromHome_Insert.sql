USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeWorkFromHome_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@CreatedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211209@Teddy.le
Task: Insert Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @WorkFromHomeRequestId INT
	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
				 INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE ew.EmpId = @EmpId
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
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	INSERT INTO dbo.WorkFromHomeRequest(
		EmpId,
		Status,
		Remark,
		ProcessedDate,
		ProcessedBy,
		FromDate,
		ToDate,
		CreatedDate,
		CreatedBy
	)
	VALUES (
		@EmpId
	   ,2
	   ,@Remark
	   ,dbo.fnc_GetClientDateTime()
	   ,@CreatedBy
	   ,@FromDate
	   ,@ToDate
	   ,dbo.fnc_GetClientDateTime()
	   ,@CreatedBy
		)

	SELECT @WorkFromHomeRequestId = CAST(SCOPE_IDENTITY() AS INT)

	INSERT INTO dbo.EmployeeWorkFromHome (
		 WorkFromHomeRequestId
		,EmpId
		,FromDate
		,ToDate
		,Remark
		,ModifiedBy
		,ModifiedDate
		)
	VALUES (
		@WorkFromHomeRequestId
		,@EmpId
		,@FromDate
		,@ToDate
		,@Remark
		,@CreatedBy
		,dbo.fnc_GetClientDateTime()
		)

	BEGIN
		EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 1 
	END	

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 2
END

GO
