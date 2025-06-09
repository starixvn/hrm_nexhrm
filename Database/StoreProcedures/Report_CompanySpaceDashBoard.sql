USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Report_CompanySpaceDashBoard]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Report_CompanySpaceDashBoard    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE OR ALTER PROCEDURE [dbo].[Report_CompanySpaceDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Teddy: add list employee work from home

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
    DECLARE @Today DATETIME
        ,@Yesterday DATETIME
    SET @Today = dbo.fnc_GetClientDateTime()

    SELECT @Yesterday = DATEADD(D, - 1, @Today)

	SELECT WorkDate, IsCurrent, IsWorkFromHome, TypeId, SUM(LeaveDay) AS LeaveDay, EmpId, NickName, FullName
	FROM
	(
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,1 AS TypeId
        ,CASE 
            WHEN wk.TotalDays >= 1 THEN 1
            ELSE 0.5
            END AS LeaveDay
        ,wk.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e  WITH (NOLOCK) ON wk.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
        AND wk.EmpId = cl.EmpId
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
    ) AS a
	GROUP BY WorkDate, IsWorkFromHome, IsCurrent, TypeId, EmpId, NickName, FullName

    UNION
    SELECT wk.FromDate AS WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, wk.FromDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,2 AS TypeId
        ,0 AS LeaveDay
        ,wk.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @Yesterday, wk.FromDate) >= 0
        AND DATEDIFF(D, wk.FromDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
    UNION
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,CASE
            WHEN (
                    (
                        att.LateIn > 0
                        OR att.EarlyOut > 0
                        )
                    AND e.Id NOT IN (88, 89, 123, 134, 153)
                    ) THEN 4
            ELSE CASE 
                    WHEN (
                            (
                                ISNULL(Absence, 0) = 0
                                AND att.InFixed IS NULL
                                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                                )
                            OR (
                                ISNULL(Absence, 0) = 0
                                AND att.OutFixed IS NULL
                                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
                                )
                            ) THEN 3
                    ELSE CASE 
                            WHEN (
                                    ISNULL(att.Absence, 0) = 1
                                    AND cl.IsOffDay = 0
                                    AND cl.IsHoliday = 0
                                    AND ISNULL(att.LeaveDay, 0) <> 1
                                    AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                                    ) THEN 5
                            ELSE 0
                            END
                    END
            END AS TypeId
        ,0 AS LeaveDay
        ,att.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
        AND cl.Workdate = att.AttendanceDate
        LEFT JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON cl.EmpId = wk.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
        AND wk.RequestType IN (1, 2)
        AND wk.[Status] = 1
        AND (
            (
                att.InFixed IS NULL
                AND wk.RequestType = 1
                )
            OR (
                att.OutFixed IS NULL
                AND wk.RequestType = 2
                )
            )
        LEFT JOIN dbo.WorkingTimeRequest AS wkl WITH (NOLOCK) ON cl.EmpId = wkl.EmpId
        AND DATEDIFF(D, wkl.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wkl.ToDate) >= 0
        AND wkl.RequestType = 6
        AND wkl.[Status] IN (1, 2)
        LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
    WHERE DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
        AND (
            (
                (
                    att.LateIn > 0
                    OR att.EarlyOut > 0
                    )
                AND e.Id NOT IN (88, 89, 123, 134, 153)
                )
            OR (
                ISNULL(att.Absence, 0) = 1
                AND cl.IsOffDay = 0
                AND cl.IsHoliday = 0
                AND ISNULL(att.LeaveDay, 0) <> 1
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                AND wk.Id IS NULL
                AND (
                    wkl.Id IS NULL
                    OR (
                        wkl.Id IS NOT NULL
                        AND wkl.TotalDays = 0.5
                        AND DATEDIFF(HH, @Today, (att.AttendanceDate + ' ' + s.EndCheckOutTime)) < 4
                        )
                    )
                )
            OR (
                ISNULL(Absence, 0) = 0
                AND att.InFixed IS NULL
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                AND (
                    wk.Id IS NULL
                    OR wk.RequestType = 2
                    )
                )
            OR (
                ISNULL(Absence, 0) = 0
                AND att.OutFixed IS NULL
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
                AND (
                    wk.Id IS NULL
                    OR wk.RequestType = 1
                    )
                )
            )
	UNION
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,cl.IsWorkFromHome AS IsWorkFromHome
        ,0 AS TypeId
        ,0 AS LeaveDay
        ,cl.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = cl.EmpId
    WHERE
        DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
END


GO
