USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_InsertCheckInOutTime]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_InsertCheckInOutTime] @EmpId INT
	,@RecordTime DATETIME
	,@InOutType TINYINT
	,@Platform VARCHAR(255)
	,@IPAddress VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Insert Check In/Out Time.
DB: HRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
	DECLARE @ToDate DATETIME
	DECLARE @RecordDate DATETIME
	DECLARE @TimeRecoredId INT
	DECLARE @Status TINYINT = 3

	INSERT INTO [dbo].[TimeRecord] (
		EmpId
		,RecordTime
		,InOutType
		,InsertDate
		)
	VALUES (
		@EmpId
		,@RecordTime
		,CASE WHEN @InOutType = 0 THEN 0 ELSE 1 END
		,GETDATE()
		)

	SET @RecordDate = convert(VARCHAR(10), @RecordTime, 111)

	SELECT @TimeRecoredId = CAST(SCOPE_IDENTITY() AS INT)

	SELECT @Status = Status
	FROM dbo.WorkFromHomeRequest
	WHERE EmpId = @EmpId
	AND @RecordDate BETWEEN FromDate AND ToDate

	INSERT INTO dbo.TimeRecordExt(ID, IPAddress, [Platform], [Status])
	VALUES(@TimeRecoredId, @IPAddress, @Platform, @Status)

	SET @FromDate = convert(VARCHAR(10), dateadd(d, - 2, @RecordTime), 111)
	SET @ToDate = convert(VARCHAR(10), @RecordTime, 111)

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,0
END
GO
