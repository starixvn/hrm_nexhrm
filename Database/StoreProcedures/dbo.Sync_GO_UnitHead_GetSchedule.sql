USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Sync_GO_UnitHead_GetSchedule]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_GO_UnitHead_GetSchedule] @FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20230307@Tim.Trinh
Task: get unit head schedule
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tempUnitHead (EmpId INT PRIMARY KEY)

	INSERT INTO #tempUnitHead (EmpId)
	SELECT emp.Id
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.Code IN ('02197')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0

	SELECT empca.EmpId
		,empca.WorkDate
		,CASE 
			WHEN empca.IsLeaveDay = 0
				AND empca.ShiftId IS NOT NULL THEN 1
			ELSE 0
			END AS IsWorkday
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL
				OR empca.IsHoliday = 1 THEN 0
			ELSE empca.IsOffDay
			END AS IsOffDay
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL THEN 0
			ELSE empca.IsHoliday
			END AS IsHoliday
		,empca.IsLeaveDay
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.Code
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.Code
			END AS LeaveTypeCode
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.[Name]
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.[Name]
			END AS LeaveTypeName
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		LEFT JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON empca.LeaveTypeId = ltype.Id
		LEFT JOIN dbo.LeaveType AS ltype2 WITH (NOLOCK) ON empca.LeaveTypeId2 = ltype2.Id
	WHERE empca.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND EXISTS (
			SELECT 1
			FROM #tempUnitHead AS tmp WITH (NOLOCK)
			WHERE empca.EmpId = tmp.EmpId
			)

	DROP TABLE #tempUnitHead
END

GO
