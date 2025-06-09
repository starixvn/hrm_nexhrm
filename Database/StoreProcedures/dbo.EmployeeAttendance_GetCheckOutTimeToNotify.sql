USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_GetCheckOutTimeToNotify]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetCheckOutTimeToNotify] 
AS
/*
Created: 20240422@Oliver.Dang
Task: Get employee attendance to notify when checkout.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @Today DATETIME = GETDATE()
	DECLARE @FromTime DATETIME = DATEADD(MINUTE, -5, @Today),
			@ToTime DATETIME = DATEADD(MINUTE, 5, @Today)
	
	--SELECT 363 as EmployeeId
	--	 ,'Ð?ng Hoàng H?i' as FullName
	--	 ,'Oliver' as NickName
	--	 ,'a:1n1E_0q_MKn87JQjFcY4cUNN8zGV6bvS6Ro3r0hpxJajXRv7DkQBzWHTghLeJvPKBqYJxjzjx6LihOgdnKeIwPOg1WDfpOvdOnS5seek_idXMyy820POpN8W3EaPgvoIL' as ConversationId
	--	 ,'2024-04-24 17:42:00' as CheckOutTime
	--	 , 8.25 as TotalWorkHours


    SELECT e.Id as EmployeeId
		  ,e.FullName
		  ,e.NickName
		  ,ec.ConversationId
		  ,ea.AttendanceDate
		  ,ea.OutFixed as CheckOutTime
		  ,ea.TotalWorkHours
    FROM dbo.EmployeeAttendance AS ea WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ea.EmpId
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
		LEFT JOIN dbo.LogReachedWorkingTime AS l WITH (NOLOCK) ON l.EmpId = ea.EmpId AND ea.AttendanceDate = l.WorkDate AND l.InsertedType = 'NotifyCheckOut'
    WHERE ea.AttendanceDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @Today))
		AND l.EmpId IS NULL
		AND ISNULL(ec.ConversationId, '') <> ''
        AND ea.OutFixed IS NOT NULL
		AND ea.OutFixed BETWEEN @FromTime AND @ToTime
END

GO
