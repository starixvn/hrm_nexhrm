CREATE PROCEDURE [dbo].[EmployeeAttendance_GetMissingCheckInCheckOut]
AS

/*
Created: 20200706@Merlin.Le
Task: get missing checkin today and checkout yesterday to remind via Skype chanel
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN

     CREATE TABLE #EmployeeAttendance
     (EmpId          INT,
      AttendanceDate DATETIME,
      MissingType    TINYINT
     );
     DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME, @startday DATETIME;
     SET @now = DATEADD(hh, 7, GETUTCDATE());
     SET @today = CONVERT(VARCHAR(10), @now, 111); --use UTC date because the server time is GMT -4, not GMT+7
     SET @yesterday = DATEADD(d, -1, @today);

     -- SELECT @startday = '2015-04-28', @yesterday = '2015-04-28'
     IF(DATEPART(WEEKDAY, @today) = 2
        OR DATEPART(WEEKDAY, @today) = 3
        OR DATEPART(WEEKDAY, @today) = 4)
         BEGIN
             SET @startday = DATEADD(d, -5, @today);
         END;
     ELSE
         BEGIN
             SET @startday = DATEADD(d, -3, @today);
         END;
             
        INSERT INTO #EmployeeAttendance
        (EmpId,
        AttendanceDate,
        MissingType
        )
            SELECT a.EmpId,
                    AttendanceDate,
                    1
            FROM [dbo].[EmployeeAttendance] AS a WITH (NOLOCK)
                    INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                    AND c.WorkDate = AttendanceDate
            WHERE attendancedate BETWEEN @startday AND @yesterday
                    --AND ISNULL(Absence, 0) = 0
                    AND InFixed IS NULL
                    AND isOffday = 0
                    AND IsHoliday = 0
                    AND (IsLeaveDay = 0
                        OR (IsLeaveDay = 1
                            AND a.LeaveDay = 0.5))
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid 
                        AND requesttype = 1
                        AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
            )
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid
                        AND requesttype = 6
                        AND r.TotalDays >= 1
                        AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                        AND r.[Status] <> 3
            );
        INSERT INTO #EmployeeAttendance
        (EmpId,
        AttendanceDate,
        MissingType
        )
            SELECT a.EmpId,
                    AttendanceDate,
                    2
            FROM [dbo].[EmployeeAttendance] AS a WITH (NOLOCK)
                    INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                    AND c.WorkDate = AttendanceDate
            WHERE attendancedate BETWEEN @startday AND @yesterday
                    AND OutFixed IS NULL
                    AND isOffday = 0
                    AND IsHoliday = 0
                    AND (IsLeaveDay = 0
                        OR (IsLeaveDay = 1
                            AND a.LeaveDay = 0.5))
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid 
                        -- AND r.CreatedDate >= @yesterday 
                        AND requesttype = 2
                        AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
            )
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid
                        AND requesttype = 6
                        AND r.TotalDays >= 1
                        AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                        AND r.[Status] <> 3
            );
        SELECT a.EmpId,
            FullName,
            e.NickName,
			u.ConversationId,
            a.AttendanceDate,
            s.Code AS ShiftCode,
            a.MissingType AS [Type]
        FROM #EmployeeAttendance a
            INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                AND c.WorkDate = AttendanceDate
            INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON a.Empid = e.Id
			LEFT JOIN [NexBot].[dbo].[ConversationUser] AS u WITH (NOLOCK) ON e.NickName = u.NickName
            LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
		WHERE e.[Enabled] = 1 AND e.[isDeleted] = 0
        ORDER BY a.AttendanceDate DESC,
                e.NickName;
     
     DROP TABLE #EmployeeAttendance;
END