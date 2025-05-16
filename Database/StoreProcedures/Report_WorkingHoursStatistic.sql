SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[Report_WorkingHoursStatistic]
	 @fromdate DATETIME
	,@todate DATETIME
	,@FromDateLastMonth DATETIME
	,@ToDateLastMonth DATETIME
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
  20170411@Raymond.Le: create SP Report_WorkingHoursStatistic
  20191206@Merlin.Le: PreMonthSickLeave contains Maternity and Parternity
  20250515@Henry.Dinh: Add DepartmentId and DepartmentName
Param's Explanation:

*/

/* Script run
USE [NexHRM]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[Report_WorkingHoursStatistic]
		@fromdate = N'8/1/2016 12:00:00 AM',
		@todate = N'8/25/2016 12:00:00 AM',
		@FromDateLastMonth = N'7/26/2016 12:00:00 AM',
		@ToDateLastMonth = N'7/31/2016 12:00:00 AM'

SELECT	'Return Value' = @return_value

GO
*/

/* Result
PreMonthSickLeave of Julien is 2
*/

DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@MaternityLeave INT
	,@PaternityLeave INT
	,@BusinessLeave INT


SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@MaternityLeave = 13
	,@PaternityLeave = 27
	,@BusinessLeave = 32

CREATE TABLE #tmpWorkingHoursStatistic(
	TotalWorkingDay INT
	,ActualWorkingDay FLOAT
	,AnnualLeave FLOAT
	,PaidLeave FLOAT
	,SickLeave FLOAT
	,InsuranceLeave FLOAT
	,UnpaidLeave FLOAT
	,BusinessLeave FLOAT
	,WorkFromHome FLOAT
	,EmpId INT
	,Code VARCHAR(20)
	,NickName NVARCHAR(20)
	,FullName NVARCHAR(50)
	,GroupId INT
	,GroupCode VARCHAR(20)
	,GroupName NVARCHAR(50)
	,DepartmentId INT
	,DepartmentName NVARCHAR(50)
)

CREATE TABLE #tmpPreMonthUPL (
	EmpId INT
	,PreMonthTotalWorkingDay INT
	,PreMonthSickLeave FLOAT 
	,PreMonthUnpaidLeave FLOAT
	,PreMonthBusinessLeave FLOAT
	,PreWorkFromHome FLOAT
	,PreInsuranceLeave FLOAT
	,PrePaidLeave FLOAT
	,PreAnnualLeave FLOAT
)

INSERT INTO #tmpPreMonthUPL(
	EmpId
	,PreMonthTotalWorkingDay 
	,PreMonthSickLeave  
	,PreMonthUnpaidLeave 
	,PreMonthBusinessLeave 
	,PreWorkFromHome
	,PreInsuranceLeave
	,PrePaidLeave
	,PreAnnualLeave
)
SELECT pm.EmpId
	,pm.TotalWorkingDay 
	,pm.SickLeave 
	,pm.UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,InsuranceLeave
	,PaidLeave
	,AnnualLeave
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
					AND (c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave
					OR c.LeaveTypeId = @MaternityLeave
					OR c.LeaveTypeId2 = @MaternityLeave
					OR c.LeaveTypeId = @PaternityLeave
					OR c.LeaveTypeId2 = @PaternityLeave) THEN CASE 
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
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @BusinessLeave
					OR c.LeaveTypeId2 = @BusinessLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
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
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
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

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	(TotalWorkingDay + ISNULL(Holiday, 0)) AS TotalWorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0) + ISNULL(Holiday, 0)) AS ActualWorkingDay
	,AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,tpu.PreMonthTotalWorkingDay
	,tpu.PreMonthSickLeave
	,tpu.PreMonthUnpaidLeave
	,tpu.PreMonthBusinessLeave
	,tpu.PreWorkFromHome
	,tpu.PreInsuranceLeave
	,tpu.PrePaidLeave
	,tpu.PreAnnualLeave
	,A.EmpId
	,A.Code
	,NickName
	,FullName
	,A.DepartmentId
	,d.Name AS DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,g.DepartmentId
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
				END) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
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
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,g.DepartmentId
	) A
	LEFT JOIN #tmpPreMonthUPL AS tpu
	ON tpu.EmpId = A.EmpId
	LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = A.DepartmentId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate


DROP TABLE #tmpPreMonthUPL
GO
