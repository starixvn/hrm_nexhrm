
CREATE PROCEDURE [dbo].[Report_WorkingHoursStatistic]
	 @fromdate DATETIME
	,@todate DATETIME
	,@FromDateLastMonth DATETIME
	,@ToDateLastMonth DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:   
  20170411@Raymond.Le: create SP Report_WorkingHoursStatistic
Param's Explanation:

*/
DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT


SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9

CREATE TABLE #tmpWorkingHoursStatistic(
	TotalWorkingDay INT
	,ActualWorkingDay FLOAT
	,AnnualLeave FLOAT
	,PaidLeave FLOAT
	,SickLeave FLOAT
	,InsuranceLeave FLOAT
	,UnpaidLeave FLOAT
	,EmpId INT
	,Code VARCHAR(20)
	,NickName NVARCHAR(20)
	,FullName NVARCHAR(50)
	,GroupId INT
	,GroupCode VARCHAR(20)
	,GroupName NVARCHAR(50)
)

CREATE TABLE #tmpPreMonthUPL (
	EmpId INT
	,PreMonthTotalWorkingDay INT
	,PreMonthSickLeave FLOAT 
	,PreMonthUnpaidLeave FLOAT
)

INSERT INTO #tmpPreMonthUPL
SELECT pm.EmpId
	,pm.TotalWorkingDay 
	,pm.SickLeave AS PreMonthSickLeave
	,(pm.UnpaidLeave + ISNULL(upl.TotalUnpaidLeave, 0)) AS PreMonthUnpaidLeave
FROM (
	SELECT c.EmpId
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
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
				)) AS SickLeave
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
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
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
		AND WorkDate BETWEEN @FromDateLastMonth
			AND @ToDateLastMonth
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupid
			)
	GROUP BY c.empid
	) AS pm
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
		AND [Date] BETWEEN @FromDateLastMonth
			AND @ToDateLastMonth
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY EmpId
	) AS upl ON pm.EmpId = upl.EmpId



SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	(TotalWorkingDay + ISNULL(Holiday, 0)) AS TotalWorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(Holiday, 0)) AS ActualWorkingDay
	,(AnnualLeave + ISNULL(upl.TotalAnnualLeave, 0)) AS AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,(UnpaidLeave + ISNULL(upl.TotalUnpaidLeave, 0)) AS UnpaidLeave
	,tpu.PreMonthTotalWorkingDay
	,tpu.PreMonthSickLeave
	,tpu.PreMonthUnpaidLeave
	,A.EmpId
	,Code
	,NickName
	,FullName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,(CASE e.Id
			WHEN g.LeaderId THEN 2 -- employee is leader
			WHEN g.AssistantLeaderId THEN 1 -- employee is assistant leader
			ELSE 0 END
			) AS EmployeeRank
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
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
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
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
		,SUM(CASE WHEN c.IsHoliday = 1 THEN 1 ELSE 0 END) Holiday
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
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
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,g.LeaderId
		,g.AssistantLeaderId
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
	LEFT JOIN #tmpPreMonthUPL AS tpu
	ON tpu.EmpId = A.EmpId
ORDER BY [Rank]
	,a.GroupCode
	,a.EmployeeRank DESC
	,a.code


DROP TABLE #tmpPreMonthUPL