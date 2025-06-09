CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetAllReminderLateInEarlyOut]
AS
/*
Created: 20230529@Brian.Nguyen
Task: Get all Employees must be created leave half day request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	DECLARE @FromDate DATE
		,@ToDate DATETIME
	SET @ToDate = dbo.fnc_GetClientDateTime()
	SET @FromDate = DATEADD(DD, -1, @ToDate)

    SELECT att.AttendanceDate
		,att.EmpId
		,e.NickName
		,e.FullName
		,ec.ConversationId
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
	LEFT JOIN DBO.EmployeeConversation as ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE att.AttendanceDate BETWEEN @FromDate AND @ToDate
	AND (ISNULL(att.LateIn, 0) > 60 OR ISNULL(att.EarlyOut, 0) > 60)
END


GO
