CREATE OR ALTER PROCEDURE [dbo].[Report_WorkingHoursSummary] @fromdate DATETIME
	,@todate DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
    ,@departmentid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@BusinessLeave INT

SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@BusinessLeave = 32

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	WorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0)) AS ActualWorkingDay
	,(AnnualLeave + ISNULL(upl.TotalAnnualLeave, 0)) AS AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,(UnpaidLeave + ISNULL(upl.TotalUnpaidLeave, 0)) AS UnpaidLeave
	,LateIn
	,EarlyOut
	,TotalHours
	,AverageHours
	,A.EmpId
	,Code
	,NickName
	,FullName
	,DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate		
		,d.Name AS DepartmentName
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 THEN 1
				ELSE 0
				END) AS WorkingDay
		,SUM(CASE 
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 0 THEN 1
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 1
					AND c.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) [ActualWorkingDay]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessTrip
						OR lv2.Id = @BusinessTrip
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessTrip
								OR lv2.Id <> @BusinessTrip
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [BusinessTrip]
		,SUM(CASE 
			WHEN isOffday = 0
				AND c.isHoliday = 0
				AND IsLeaveDay = 1
				AND (
					lv1.Id = @BusinessLeave
					OR lv2.Id = @BusinessLeave
					) THEN CASE 
					WHEN ISNULL(lv2.Id, 0) <> 0
						AND (
							lv1.Id <> @BusinessLeave
							OR lv2.Id <> @BusinessLeave
							) THEN 0.5
					ELSE c.LeaveDay
					END
			ELSE 0
			END) AS [BusinessLeave]
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [SickLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		,SUM(a.LateIn) LateIn
		,SUM(a.EarlyOut) EarlyOut
		,SUM(ISNULL(a.TotalWorkHours, 0)) TotalHours
		,SUM(ISNULL(a.TotalWorkHours, 0)) / CASE 
			WHEN SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END) = 0 THEN 1
			ELSE SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END)
			END AS AverageHours
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @fromdate
			AND @todate
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,d.Name
	) A
	LEFT JOIN (
	SELECT EmpId
		,SUM(CASE 
				WHEN IsAnnual = 0 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalUnpaidLeave
		,SUM(CASE 
				WHEN IsAnnual = 1 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalAnnualLeave
	FROM dbo.LeaveConversion AS lv WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lv.EmpId = e.Id
	WHERE lv.[Type] = 1
		AND (
			@empid IS NULL
			OR empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND [Date] BETWEEN @fromdate
			AND @todate
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY EmpId
	) AS upl ON A.EmpId = upl.EmpId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate
GO
