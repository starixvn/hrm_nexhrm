USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[CheckInCheckOut_GetByConversationId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[CheckInCheckOut_GetByConversationId] 
	@ConversationId VARCHAR(255)
AS

/*
Created: 20240719@Oliver
Task: get missing checkin today and checkout yesterday by conversationId
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
     DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME, @startday DATETIME, @empId INT;
     SET @now = DATEADD(hh, 7, GETUTCDATE());
     SET @today = CONVERT(VARCHAR(10), @now, 111); --use UTC date because the server time is GMT -4, not GMT+7
     SET @yesterday = DATEADD(d, -1, @today);

	 SELECT @empId = c.EmpId
	 FROM dbo.EmployeeConversation c with (nolock) where c.ConversationId = @ConversationId

	 --SET @empId = 363
     
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
							AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckInTime
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
							AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckOutTime
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

				IF EXISTS (SELECT 1 FROM #EmployeeAttendance)
				BEGIN
					SELECT a.EmpId,
						AttendanceDate,
						s.Code ShiftCode,
						a.MissingType,
						s.BeginCheckInTime,
						s.EndCheckOutTime
					FROM #EmployeeAttendance a
						INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
																			AND c.WorkDate = AttendanceDate
						LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
					ORDER BY a.AttendanceDate DESC
				END
				ELSE
				BEGIN
					DECLARE @attendance datetime = dbo.fnc_GetClientDateTime()
					IF (DATEPART(WEEKDAY, @attendance) = 1 OR DATEPART(WEEKDAY, @attendance) = 7)
					BEGIN
						SET @attendance = DATEADD(DAY, 
						(CASE WHEN DATEPART(WEEKDAY, @attendance) = 1 THEN -2 ELSE -1 END ),
						@attendance)
					END

					SELECT TOP 1 c.EmpId,
								cast(@attendance as date) as AttendanceDate,
								s.Code ShiftCode,
								0 as MissingType,
								s.BeginCheckInTime,
								s.EndCheckOutTime
					FROM dbo.EmployeeCalendar c WITH (NOLOCK)
					LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
					WHERE CAST(c.WorkDate as DATE) = CAST(@attendance as DATE)
					AND c.EmpId = @empId
				END

			DROP TABLE #EmployeeAttendance;
	END
END
GO
