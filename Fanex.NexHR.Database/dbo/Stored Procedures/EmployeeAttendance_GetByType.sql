CREATE PROCEDURE [dbo].[EmployeeAttendance_GetByType] @fromtime DATETIME
    ,@totime DATETIME
    ,@type SMALLINT
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@excludeUserDelete BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 2012/03/17@Dan
Task: get attentance list of employees by type
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
    @type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @LeaveTypeDefaultId AS INT
        ,@note VARCHAR(200)
        ,@today DATETIME
        ,@now DATETIME

    SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

    SET @now = dateadd(hh, 7, GETUTCDATE())
    SET @today = convert(VARCHAR(10), @now, 111) -- use UTC because the server use GMT-4

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END

    CREATE TABLE #EmployeeAttendance (
        Id INT
        ,Code VARCHAR(20)
        ,EmpId INT
        ,FullName NVARCHAR(50)
        ,NickName NVARCHAR(20)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,[Rank] INT
        ,EmployeeRank INT
        ,AttendanceDate DATETIME
        ,ShiftIf INT
        ,ShiftCode VARCHAR(10)
        ,InFixed DATETIME
        ,OutFixed DATETIME
        ,LateIn INT
        ,EarlyOut INT
        ,TotalWorkHours DECIMAL(18, 2)
        ,Absence BIT
        ,LeaveTypeId INT
        ,LeaveDay DECIMAL(18, 1)
        ,IsLateIn INT
        ,IsEarlyOut INT
        ,IsMissingCheckin INT
        ,IsMissingCheckOut INT
        ,Notes NVARCHAR(100)
        )

    INSERT INTO #EmployeeAttendance
    SELECT T.Id
        ,T.Code
        ,T.EmpId
        ,T.FullName
        ,T.NickName
        ,T.Team
        ,T.TeamCode
        ,T.[Rank]
        ,T.EmployeeRank
        ,T.AttendanceDate
        ,T.ShiftId
        ,T.ShiftCode
        ,T.InFixed
        ,T.OutFixed
        ,T.LateIn
        ,T.EarlyOut
        ,T.TotalWorkHours
        ,T.Absence
        ,T.LeaveTypeId
        ,T.LeaveDay
        ,T.isLateIn
        ,T.isEarlyOut
        ,T.isMissingCheckin
        ,T.isMissingCheckOut
        ,stuff(CASE 
                WHEN ISNULL(T.Notes, '') = '' THEN ''
                ELSE ', ' + T.Notes
                END + CASE 
                WHEN isLateIn = 1 THEN (', Late In')
                ELSE ''
                END + CASE 
                WHEN isEarlyOut = 1 THEN ', Early Out'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckin = 1 THEN ', Missing Check-in'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckOut = 1 THEN ', Missing Check-out'
                ELSE ''
                END + CASE 
                WHEN T.isHoliday = 1 THEN ', Public Holiday'
                ELSE ''
                END + CASE 
                WHEN isOffday = 1 THEN ', Off day'
                ELSE ''
                END + CASE 
                WHEN (
                        isNotWork = 1
                        AND T.isHoliday <> 1
                        AND T.isOffday <> 1
                        ) THEN ', Absent'
                ELSE CASE 
                        WHEN lv.NAME IS NULL THEN ''
                        ELSE (CASE 
                                WHEN (
                                        T.isHoliday <> 1
                                        AND T.isOffday <> 1
                                        ) THEN (', ' + lv.NAME)
                                ELSE '' END
                                ) + CASE 
                                WHEN LeaveDay = 0.5 THEN ' (Half day)'
                                ELSE ''
                                END
                        END
                END, 1, 2, '') AS Notes
    FROM (
        SELECT a.Id
            ,e.Code
            ,a.EmpId
            ,FullName
            ,NickName
            ,g.NAME [Team]
            ,g.Code [TeamCode]
            ,g.[Rank]
            ,(CASE e.Id
                WHEN g.LeaderId THEN 2 -- employee is leader
                WHEN g.AssistantLeaderId THEN 1 -- employee is assistant leader
                ELSE 0 END
                ) AS EmployeeRank
            ,AttendanceDate
            ,a.ShiftId
            ,s.code AS ShiftCode
            ,a.InFixed
            ,a.OutFixed
            ,a.LateIn
            ,a.EarlyOut
            ,TotalWorkHours
            ,Absence
            ,a.LeaveTypeId
            ,a.LeaveDay
            ,a.Notes
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND ISNULL(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.LateIn, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.LateIn, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isLateIn
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.EarlyOut, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.EarlyOut, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isEarlyOut
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND InFixed IS NULL THEN 1
                ELSE 0
                END isMissingCheckin
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND OutFixed IS NULL
                    AND AttendanceDate < @today THEN 1
                ELSE 0
                END isMissingCheckOut
            ,CASE 
                WHEN (
                        e.Id NOT IN (88, 89, 123, 134, 153)
                        AND isnull(Absence, 0) = 1
                        AND c.IsOffDay = 0
                        AND c.IsHoliday = 0
                        AND ISNULL(a.LeaveDay, 0) <> 1
                        AND @now > (AttendanceDate + ' ' + s.EndCheckInTime)
                        ) THEN 1
                ELSE 0
                END isNotWork
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsOffDay = 1 THEN 1
                ELSE 0
                END isOffday
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsHoliday = 1 THEN 1
                ELSE 0
                END isHoliday
        FROM dbo.EmployeeAttendance a WITH (NOLOCK)
            LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.EmpId = a.EmpId
            AND c.Workdate = a.AttendanceDate
            INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
            LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id = a.ShiftId
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
        WHERE e.IsSpecialUser = 0
			AND attendancedate BETWEEN @fromtime
                AND @totime
            AND (
                (
                    e.isDeleted IS NULL
                    OR e.isDeleted = 0
                    )
                OR @excludeUserDelete = 0
                )
            AND (
                @empid IS NULL
                OR a.empid = @empid
                )
            AND (
                @groupid IS NULL
                OR g.ID = @groupid
                )
        ) T
        LEFT JOIN LeaveType lv WITH (NOLOCK) ON lv.ID = T.LeaveTypeId
    WHERE @type = 0
        OR (
            @type = 1
            AND isMissingCheckin = 1
            )
        OR (
            @type = 2
            AND isMissingCheckout = 1
            )
        OR (
            @type = 3
            AND isLateIn = 1
            )
        OR (
            @type = 4
            AND isEarlyOut = 1
            )
        OR (
            @type = 5
            AND isNotWork = 1
            )

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,EmployeeRank DESC
            ,Code OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @itemsPerPage ROWS ONLY
    END
    ELSE
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,EmployeeRank DESC
            ,Code
    END

    SELECT wk.Id
        ,wk.FromDate
        ,(CASE 
            WHEN wk.RequestType IN (1, 2) THEN wk.FromDate
            ELSE wk.ToDate END
            ) AS ToDate
        ,wk.TotalDays
        ,CASE 
            WHEN wk.RequestType = 3 THEN wk.LateInMins
            ELSE wk.EarlyOutMins
            END AS TotalMins
        ,wk.CreatedDate
        -- ,wk.[Status]
        ,wk.Remark
        ,wk.RequestType AS RequestTypeId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Id
            ELSE s3.Id END
            ), 0) AS FromShiftId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ), '') AS FromShiftName
        ,s2.Id AS ToShiftId
        ,s2.Code AS ToShiftName
        ,wk.EmpId AS RequesterId
        ,e.NickName
        ,e.FullName
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wk.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.empid = wk.empid
        AND DATEDIFF(D, cl.WorkDate, wk.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = wk.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = wk.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = cl.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = wk.LeaveTypeID
    WHERE wk.[Status] IN (1, 2)
        AND (
            (
                wk.ToDate IS NULL
                AND DATEDIFF(D, @fromtime, wk.FromDate) >= 0
                AND DATEDIFF(D, wk.FromDate, @totime) >= 0
                )
            OR (
                NOT (
                    DATEDIFF(D, @totime, wk.FromDate) > 0
                    OR DATEDIFF(D, wk.ToDate, @fromtime) > 0
                    )
                )
            )
        AND (
            (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
            OR @excludeUserDelete = 0
            )
        AND (
            @empid IS NULL
            OR wk.empid = @empid
            )
        AND (
            @groupid IS NULL
            OR e.GroupId = @groupid
            )

    SELECT @TotalItem = COUNT(*)
    FROM #EmployeeAttendance

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END
END