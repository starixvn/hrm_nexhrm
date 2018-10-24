
CREATE PROCEDURE [dbo].[EmployeeAttendance_GetMissingCheckinOutToRemind] @EmpId INT = NULL
AS

/*
Created: 2012/03/17@Dan
Task: get missing checkin today and checkout yesterday to remind via email
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/


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
     IF @EmpId IS NULL
         BEGIN
             -- check to make sure checkin/out data has been impoted from Bioadmin
             IF(NOT EXISTS
               (
                   SELECT 1
                   FROM dbo.TimeRecord WITH (NOLOCK)
                   WHERE RecordTime > @yesterday+' 08:00'
               )
                OR DATEPART(WEEKDAY, @today) = 7
                OR DATEPART(WEEKDAY, @today) = 1) -- 1 : Sunday, 7 : Saturday
                 BEGIN
                     DROP TABLE #EmployeeAttendance;
                     RETURN;
                 END;
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           1
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
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
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 1
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
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
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
                    WHERE attendancedate BETWEEN @startday AND @yesterday
                          --AND ISNULL(Absence, 0) = 0
                          AND OutFixed IS NULL
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 2
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             SELECT a.EmpId,
                    FullName,
                    NickName,
                    g.Name [Team],
                    g.Code [TeamCode],
                    AttendanceDate,
                    s.Code ShiftCode,
                    Email,
                    a.MissingType AS [Type]
             FROM #EmployeeAttendance a
                  INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                     AND c.WorkDate = AttendanceDate
                  INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
                  LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
                  LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
             ORDER BY a.AttendanceDate DESC,
                      e.NickName;
         END;
     ELSE
     /*
     This case was added by Harvey on 03/02/2016
     - Reason: get missing check in/out by EmpId to remind
     - Task: Todo list - Dashboard
     */
         BEGIN
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           1
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
                    WHERE attendancedate BETWEEN @startday AND @today
                          --AND ISNULL(Absence, 0) = 0
                          AND InFixed IS NULL
						  AND GETDATE() > c.WorkDate + ' ' + sh.EndCheckInTime
                          AND a.EmpId = @EmpId
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND a.EmpId = @EmpId
                              AND requesttype = 1
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND a.EmpId = @EmpId
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
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
                    WHERE attendancedate BETWEEN @startday AND @yesterday
                          AND a.EmpId = @EmpId
						  AND GETDATE() > c.WorkDate + ' ' + sh.EndCheckOutTime
                          --AND ISNULL(Absence, 0) = 0
                          AND OutFixed IS NULL
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 2
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             SELECT a.EmpId,
                    FullName,
                    NickName,
                    g.Name [Team],
                    g.Code [TeamCode],
                    AttendanceDate,
                    s.Code ShiftCode,
                    Email,
                    a.MissingType
             FROM #EmployeeAttendance a
                  INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                     AND c.WorkDate = AttendanceDate
                  INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
                  LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
                  LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
             ORDER BY a.AttendanceDate DESC,
                      e.NickName;
         END;
     DROP TABLE #EmployeeAttendance;
