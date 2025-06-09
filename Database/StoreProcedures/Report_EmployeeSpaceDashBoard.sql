USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Report_EmployeeSpaceDashBoard]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_EmployeeSpaceDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
	DECLARE @CurrentMonth DATETIME
		,@LastMonth DATETIME
		,@Today DATETIME

	SET @Today = dbo.fnc_GetClientDateTime()

	SELECT @CurrentMonth = @Today
		,@LastMonth = DATEADD(M, - 1, @Today)

	SELECT wk.FromDate AS WorkDate
		,(CASE 
			WHEN DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,2 AS TypeId
		,0 AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType IN (1, 2)
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
		AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0
		AND EXISTS (
			SELECT 1 
			FROM dbo.EmployeeCalendar WITH (NOLOCK) 
			WHERE DATEDIFF(dd, WorkDate, wk.FromDate) = 0
			AND IsHoliday = 0
			AND IsOffDay = 0
		)
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,1 AS TypeId
		,(CASE 
			WHEN wk.TotalDays = 0.5 THEN 0.5
			ELSE 1 END
			) AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
		INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
		AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
		AND wk.EmpId = cl.EmpId
		AND cl.IsOffDay <> 1
		AND cl.IsHoliday <> 1
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType = 6
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
	--GROUP BY wk.EmpId
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,CASE 
			WHEN (
					(
						att.LateIn > 0
						OR att.EarlyOut > 0
						)
					-- AND e.Id NOT IN (88, 89, 123, 134, 153)
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
		INNER JOIN dbo.Employee AS e ON att.EmpId = e.Id
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
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
	WHERE att.EmpId = @EmpId
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
		AND (
			(
				(
					att.LateIn > 0
					OR att.EarlyOut > 0
					)
				-- AND e.Id NOT IN (88, 89, 123, 134, 153)
				)
			OR (
				ISNULL(att.Absence, 0) = 1
				AND cl.IsOffDay = 0
				AND cl.IsHoliday = 0
				AND ISNULL(att.LeaveDay, 0) <> 1
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
				AND wk.Id IS NULL
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
END
GO
