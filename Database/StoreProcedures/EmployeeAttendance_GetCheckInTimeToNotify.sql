CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetCheckInTimeToNotify] 
AS
/*
Created: 20240701@Oliver.Dang
Task: Get employee attendance to notify when checkin.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @Today DATETIME = dbo.fnc_GetClientDateTime()
	DECLARE @FromTime DATETIME = DATEADD(MINUTE, -5, @Today),
			@ToTime DATETIME = DATEADD(MINUTE, 5, @Today)
	
	--SELECT 363 as EmployeeId
	--	 ,'Oliver' as NickName
	--	 ,'a:1n1E_0q_MKn87JQjFcY4cUNN8zGV6bvS6Ro3r0hpxJajXRv7DkQBzWHTghLeJvPKBqYJxjzjx6LihOgdnKeIwPOg1WDfpOvdOnS5seek_idXMyy820POpN8W3EaPgvoIL' as ConversationId
	--	 ,'2024-04-24 8:42:00' as CheckinTime


    SELECT e.Id as EmployeeId
		  ,e.NickName
		  ,ec.ConversationId
		  ,ea.AttendanceDate
		  ,ea.InFixed as CheckInTime
    FROM dbo.EmployeeAttendance AS ea WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ea.EmpId
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
		LEFT JOIN dbo.LogReachedWorkingTime AS l WITH (NOLOCK) ON l.EmpId = ea.EmpId 
					AND ea.AttendanceDate = l.WorkDate 
					AND  l.InsertedType = 'NotifyCheckIn'
    WHERE ea.AttendanceDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @Today))
		AND l.EmpId IS NULL
		AND ISNULL(ec.ConversationId, '') <> ''
        AND ea.InFixed IS NOT NULL
		AND ea.InFixed BETWEEN @FromTime AND @ToTime
END

GO
