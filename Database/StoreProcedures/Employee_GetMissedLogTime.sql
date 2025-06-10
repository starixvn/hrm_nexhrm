CREATE OR ALTER PROCEDURE [dbo].[Employee_GetMissedLogTime]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
/*
Created: 20250228@Henry.dinh
Task: Insert/Update get missed logging 
DB: NexHRM
Original: Employee_GetMissedLogTime
Revisions:  
	- 20250307 - Added date range parameters and changed to use Date instead of Month/Year
				 Default date range is now first day to last day of current month
	- 20240319 - Removed RedmineNexcel dependencies as it's now handled by a separate API
Param's Explanation:
- @FromDate: Optional start date for filtering (defaults to first day of current month if NULL)
- @ToDate: Optional end date for filtering (defaults to last day of current month if NULL)
*/
BEGIN
    DECLARE @Yesterday DATE = 
        CASE 
            WHEN DATEPART(WEEKDAY, dbo.fnc_GetClientDateTime()) = 2 THEN
                DATEADD(DAY, -3, dbo.fnc_GetClientDateTime())
            ELSE
                DATEADD(DAY, -1, dbo.fnc_GetClientDateTime())
        END;
   
    DECLARE @FirstDayOfMonth DATE = DATEFROMPARTS(YEAR(dbo.fnc_GetClientDateTime()), MONTH(dbo.fnc_GetClientDateTime()), 1);
    DECLARE @LastDayOfMonth DATE = EOMONTH(dbo.fnc_GetClientDateTime());
    
    SET @FromDate = ISNULL(@FromDate, @FirstDayOfMonth);
    SET @ToDate = ISNULL(@ToDate, @LastDayOfMonth);
    
    WITH ExtractedDomainUsers AS (
        SELECT 
            Id,
            -- Extract username part after domain (e.g., "henry.dinh" from "spvn\henry.dinh")
            CASE 
                WHEN CHARINDEX('\', DomainUsername) > 0 
                THEN SUBSTRING(DomainUsername, CHARINDEX('\', DomainUsername) + 1, LEN(DomainUsername)) 
                ELSE DomainUsername 
            END AS ExtractedUsername,
            DomainUsername
        FROM [dbo].[Employee] WITH (NOLOCK)
        WHERE IsDeleted = 0 OR IsDeleted IS NULL
    ),
    ValidLeaders AS (
        -- Get leaders from GroupSupervisor
        SELECT DISTINCT gs.ApproverId
        FROM [dbo].[GroupSupervisor] gs WITH (NOLOCK)
        WHERE (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    ),
    EmployeesToExclude AS (
        -- Get emps is not Leader but exist in EmployeeSupervisor
        SELECT DISTINCT EmpId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.EmpId NOT IN (SELECT ApproverId FROM ValidLeaders)
        UNION
        SELECT DISTINCT ApproverId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.ApproverId NOT IN (SELECT ApproverId FROM ValidLeaders)
    ),
    MissedCounts AS (
        SELECT 
            EmpID,
            COUNT(*) AS MissedCount
        FROM [dbo].[EmployeeLogTimeMissed] WITH (NOLOCK)
        WHERE [Date] BETWEEN @FromDate AND @ToDate
        GROUP BY EmpID
    )
    SELECT DISTINCT
        eg.Code AS TeamCode,
        e.Id AS EmpID,
        e.NickName,
        CASE 
            WHEN e.Id = gs.ApproverId THEN 1 ELSE 0
        END AS IsLeader,
        ISNULL(mc.MissedCount, 0) AS MissedCount,
		jt.Name As JobTitle,
        ec.ConversationId,
        edu.DomainUsername
    FROM [dbo].[EmployeeGroup] eg WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] e WITH (NOLOCK) ON eg.Id = e.GroupId
    LEFT JOIN [dbo].[GroupSupervisor] gs WITH (NOLOCK) ON eg.Id = gs.GroupId AND (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    LEFT JOIN [dbo].[EmployeeSupervisor] es WITH (NOLOCK) ON e.Id = es.EmpId AND (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
	LEFT JOIN [HRM].[dbo].[JobTitle] jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
    LEFT JOIN ExtractedDomainUsers edu ON e.Id = edu.Id
    LEFT JOIN MissedCounts mc ON e.Id = mc.EmpID
    LEFT JOIN [dbo].[EmployeeConversation] ec WITH (NOLOCK) ON e.Id = ec.EmpId
    LEFT JOIN [dbo].[EmployeeAttendance] ea WITH (NOLOCK)
        ON e.Id = ea.EmpId 
        AND ea.AttendanceDate = @Yesterday
    WHERE 
        (eg.Deleted = 0 OR eg.Deleted IS NULL)
        AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
        AND e.Enabled = 1
        AND (
            e.Id = gs.ApproverId
            OR (ea.InFixed IS NOT NULL AND (ea.LeaveDay != 1 OR ea.LeaveDay IS NULL))
        )
        -- Exclude EmpId is not Leader but exist in EmployeeSupervisor
        AND e.Id NOT IN (SELECT EmpId FROM EmployeesToExclude)
    ORDER BY 
        eg.Code, 
        IsLeader DESC, 
        e.NickName;
END
GO
