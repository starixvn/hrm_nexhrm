CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_UpdateByHR] @WorkFromHomeRequestId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@HRRemark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Update Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @FromDateWFH DATE,  @ToDateWFH DATE
	SELECT @FromDateWFH = FromDate, @ToDateWFH = ToDate
			FROM dbo.WorkFromHomeRequest WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
	IF EXISTS (
			SELECT 1
			FROM  dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE wfhr.Id <> @WorkFromHomeRequestId
				AND wfhr.EmpId = @EmpId
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

	UPDATE dbo.WorkFromHomeRequest WITH (ROWLOCK, UPDLOCK)
	SET  FromDate = @FromDate
		,ToDate = @ToDate
		,HRRemark = @HRRemark
		,ModifiedBy = @ModifiedBy
		,ProcessedBy = @ModifiedBy
		,ProcessedDate = dbo.fnc_GetClientDateTime()
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDateWFH, @ToDateWFH, 3

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 2

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDateWFH ,@ToDateWFH , 0

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 2
END


GO
