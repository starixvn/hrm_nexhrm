
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetLeaveSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
AS
/*
Created: 20180515@Oliver.Dang
Task: Get employee leave schedule by date.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsHalfDayLeave
	   ,(CASE WHEN IsOffDay = 1 OR cl.IsHoliday = 1 THEN 0 ELSE cl.LeaveDay END) AS LeaveDay
	   ,cl.IsLeaveDay AS IsLeaveDaye
       ,(CASE 
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN LeaveDay = 0.5 THEN '½'
			WHEN LeaveDay = 1 THEN 'x'
            ELSE ''
            END) AS LeaveType
         ,cl.EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.Name
        ,(CASE e.Id
            WHEN g.LeaderId THEN 2
            WHEN g.AssistantLeaderId THEN 1
            ELSE 0 END
            ) AS EmployeeRank
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND cl.WorkDate BETWEEN @fromDate
            AND @toDate
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
    ORDER BY g.[Rank]
        ,g.Code
        ,EmployeeRank DESC
        ,e.JoinedDate
        ,cl.WorkDate
END


