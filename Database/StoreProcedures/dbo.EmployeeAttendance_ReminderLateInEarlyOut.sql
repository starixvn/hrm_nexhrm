USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_ReminderLateInEarlyOut]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ReminderLateInEarlyOut] @EmpId INT
AS
/*
Created: 20180919@Quinn.Vo
Task: Get Employees must be created leave half day request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
	SET @ToDate = dbo.fnc_GetClientDateTime()
	SET @FromDate = DATEADD(MM, -1, @ToDate)

    SELECT att.AttendanceDate
		,att.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
	WHERE att.EmpId = @EmpId
	AND att.AttendanceDate BETWEEN @FromDate AND @ToDate
	AND (ISNULL(att.LateIn, 0) > 60 OR ISNULL(att.EarlyOut, 0) > 60)
END


GO
