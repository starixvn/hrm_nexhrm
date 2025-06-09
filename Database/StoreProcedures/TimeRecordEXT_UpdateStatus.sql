USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecordEXT_UpdateStatus]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[TimeRecordEXT_UpdateStatus] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Status TINYINT
AS
/*
Created: 20211229@Teddy.le
Task: Update status time record
DB: HRM
Original: 

Revisions:  

*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (
			SELECT 1
			FROM [dbo].[TimeRecord] AS tr WITH (NOLOCK)
				 INNER JOIN [dbo].[TimeRecordExt] AS tre WITH (NOLOCK) ON tr.Id = tre.ID
			WHERE tr.EmpId = @EmpId
				AND convert(VARCHAR(10), tr.RecordTime, 111) BETWEEN @FromDate AND @ToDate
			)
	BEGIN
		UPDATE tre WITH (ROWLOCK, UPDLOCK)
		SET Status = @Status
		FROM [dbo].[TimeRecordExt] AS tre WITH  (ROWLOCK, UPDLOCK) 
				 INNER JOIN  [dbo].[TimeRecord] AS tr WITH  (ROWLOCK, UPDLOCK) ON tr.Id = tre.ID
		WHERE tr.EmpId = @EmpId
			AND convert(VARCHAR(10), tr.RecordTime, 111)  BETWEEN @FromDate AND @ToDate
	END
END
GO
