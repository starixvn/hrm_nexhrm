USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_UpdateWorkFromHome]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@IsWorkFromHome BIT
AS
/*
Created: 20211227@Teddy.le
Task: EmployeeCalendar Update Work From Home
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (
			SELECT 1
			FROM [dbo].[EmployeeCalendar] WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ( WorkDate BETWEEN @FromDate AND @ToDate )
			)
	BEGIN
		UPDATE [dbo].[EmployeeCalendar] WITH (ROWLOCK, UPDLOCK)
		SET IsWorkFromHome = @IsWorkFromHome, ModifiedDate = dbo.fnc_GetClientDateTime()
		WHERE EmpId = @EmpId  
		AND ( WorkDate BETWEEN @FromDate AND @ToDate )
	END
END

GO
