USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_GetLeaveSchedule]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetLeaveSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
    ,@DepartmentId INT = NULL
AS
/*
Created: 20180515@Oliver.Dang
Task: Get employee leave schedule by date.
DB: NexEPA
Original: 

Revisions:  
    - 20250515@Henry.Dinh: Add department
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
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.Name AS TeamName
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
    INNER JOIN dbo.Department AS d ON d.DepartmentId = g.DepartmentId
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
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
        AND (
            @DepartmentId IS NULL
            OR g.DepartmentId = @DepartmentId
            )
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
        ,cl.WorkDate
END


GO
