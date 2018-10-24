

/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetWorkingSchedule    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetWorkingSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
AS
/*
Created: 20160219@Quinn.Vo
Task: Get employee working schedule by date.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsLeaveHalfDay
       ,(CASE 
            WHEN cl.isHoliday = 0
                AND IsOffDay = 0
                AND IsLeaveDay = 0 THEN s.code
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN isLeaveDay = 1 THEN lv.Code + CASE 
                    WHEN LeaveDay = 0.5 THEN '½'
                    WHEN LeaveTypeId2 IS NOT NULL THEN '/' + lv2.Code
                    WHEN LeaveDay = 1 THEN ''
                    ELSE ''
                    END
            END) AS Title
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
    LEFT JOIN dbo.LeaveType AS lv ON lv.id = cl.LeaveTypeId
     LEFT JOIN dbo.LeaveType AS lv2 ON lv2.Id = cl.LeaveTypeId2
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


