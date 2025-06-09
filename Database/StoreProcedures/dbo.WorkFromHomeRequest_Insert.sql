USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkFromHomeRequest_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@CreatedBy INT
	,@Remark NVARCHAR(2000) = ''
	,@HRRemark NVARCHAR(2000) = ''
	,@StatusId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211223@Teddy.le
Task: Insert Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ProcessedBy INT
		,@ProcessedDate DATETIME
		,@CurrentTime DATETIME

	SELECT @CurrentTime = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE wfhr.EmpId = @EmpId
				AND (
					@FromDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					OR @ToDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	IF(@StatusId = 2)
	BEGIN
		SELECT @ProcessedDate = @CurrentTime, @ProcessedBy = @CreatedBy
	END

	INSERT INTO dbo.WorkFromHomeRequest(
		EmpId,
		Status,
		Remark,
		HRRemark,
		FromDate,
		ToDate,
		CreatedDate,
		CreatedBy,
		ProcessedBy,
		ProcessedDate
	)
	VALUES (
		@EmpId
	   ,@StatusId
	   ,@Remark
	   ,@HRRemark
	   ,@FromDate
	   ,@ToDate
	   ,GETDATE()
	   ,@CreatedBy
	   ,@ProcessedBy
	   ,@ProcessedDate
		)

	
	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, @StatusId

	BEGIN
		EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate, @StatusId 
	END	
END

GO
