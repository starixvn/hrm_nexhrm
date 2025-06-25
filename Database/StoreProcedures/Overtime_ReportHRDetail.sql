CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportHRDetail]
    @Id		INT
/*
Created: 20200903Dwight.Tran
Task: Enhance NEX Request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

	IF OBJECT_ID('##TempHourOT') IS NOT NULL
	BEGIN
		DROP TABLE #TempHourOT
	END

	CREATE TABLE #TempHourOT (
		EmployeeId INT
		,RealHourOT INT
		,[Date]		DATE
		)

	INSERT INTO #TempHourOT (
		EmployeeId 
		,RealHourOT 
		,[Date]
		)
	SELECT
		otd.[EmployeeId]
		,(CASE
			WHEN ISNULL(ec.[IsHoliday], 0) = 1 OR ISNULL(ec.[IsOffDay], 0) = 1
				THEN 0
			WHEN ea.[TotalWorkHours] IS NULL AND ISNULL(ec.[LeaveDay], 0) <> 1
				THEN otd.[Hour]
			WHEN ISNULL(ec.[LeaveDay], 0) = 1
				THEN IIF(otd.[Hour] < DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60)
							, otd.[Hour], DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60))
			WHEN ISNULL(ec.[LeaveDay], 0) = 0.5
				THEN IIF(otd.[Hour] < (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) + ea.[LateIn] + ea.[EarlyOut] - 4)
							,otd.[Hour], (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) + ea.[LateIn] + ea.[EarlyOut] - 4))
			WHEN ISNULL(ec.[LeaveDay], 0) = 0
				THEN IIF(otd.[Hour] < (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) - 1 + ea.[LateIn] + ea.[EarlyOut] - 8)
							,otd.[Hour], (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) - 1 + ea.[LateIn] + ea.[EarlyOut] - 8))
			ELSE otd.[Hour]
			END) as RealHourOT
		,otd.[Date]
	FROM dbo.[OvertimeDetail] as otd WITH (NOLOCK)
		LEFT JOIN dbo.[EmployeeAttendance] as ea WITH (NOLOCK)
		ON ea.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ea.[AttendanceDate], otd.[Date]) = 0
		LEFT JOIN dbo.[EmployeeCalendar] as ec WITH (NOLOCK)
		ON ec.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ec.[WorkDate], otd.[Date]) = 0
	WHERE otd.[OvertimeId] = @Id
	
	SELECT
		otd.[OvertimeId]
		,otd.[Note]
		,otd.[Date]
		,otd.[Hour]
		,(CASE
			WHEN ot.[Status] = 4	-- verified
				THEN otd.[NexHour]
			ELSE (FLOOR(2 * temp.[RealHourOT]/2)*1.5) END) as 'NexHour'
		,otd.[EmployeeId] as UserId	-- employee
		,emp.[NickName]
		,emp.[FullName]
	FROM dbo.[Overtime] as ot WITh (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail] as otd WITH (NOLOCK)
		ON ot.[OvertimeId] = otd.[OvertimeId]
		INNER JOIN #TempHourOT as temp WITH (NOLOCK)
		ON temp.[EmployeeId] = otd.[EmployeeId] AND temp.[Date] = otd.[Date]
		INNER JOIN dbo.[Employee] as emp WITH (NOLOCK)
		ON emp.[Id] = otd.[EmployeeId]
	WHERE otd.[OvertimeId] = @Id

	DROP TABLE #TempHourOT;
END;

GO
