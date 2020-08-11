CREATE PROCEDURE [dbo].[Overtime_ReportHR]
    @employeeId		INT = NULL
	,@createdBy		INT = NULL
	,@verified		INT
	,@fromDate		DATETIME
	,@toDate		DATETIME
/*
Created: 20200804Dwight.Tran
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
		OvertimeId INT
		,EmployeeId INT
		,RealHourOT INT
		,Title		NVARCHAR(10)
		,IsLeaveHalfDay	BIT
		)

	INSERT INTO #TempHourOT (
		OvertimeId 
		,EmployeeId 
		,RealHourOT 
		,Title
		,IsLeaveHalfDay
		)
	SELECT 
		otd.[OvertimeId]
		,otd.[EmployeeId]
		,(CASE
			WHEN ISNULL(ec.[IsHoliday], 0) = 1 OR ISNULL(ec.[IsOffDay], 0) = 1
				THEN 0
			WHEN ISNULL(ec.[LeaveDay], 0) = 1
				THEN IIF(otd.[Hour] < ea.[TotalWorkHours], otd.[Hour], ea.[TotalWorkHours])
			WHEN ISNULL(ec.[LeaveDay], 0) = 0.5
				THEN IIF(otd.[Hour] < (ea.[TotalWorkHours] + ea.[LateIn] + ea.[EarlyOut] - 4)
							,otd.[Hour], (ea.[TotalWorkHours] + ea.[LateIn] + ea.[EarlyOut] - 4))
			WHEN ISNULL(ec.[LeaveDay], 0) = 0
				THEN IIF(otd.[Hour] < (ea.[TotalWorkHours] + ea.[LateIn] + ea.[EarlyOut] - 8)
							,otd.[Hour], (ea.[TotalWorkHours] + ea.[LateIn] + ea.[EarlyOut] - 8))
			ELSE otd.[Hour]
			END) as RealHourOT
		,(CASE 
            WHEN ec.[IsHoliday] = 0
                AND ec.[IsOffDay] = 0
                AND ec.[IsLeaveDay] = 0 THEN s.code
            WHEN ec.[IsHoliday] = 1 THEN 'PH'
            WHEN ec.[IsOffDay] = 1 THEN 'OFF'
            WHEN ec.[IsLeaveDay] = 1 THEN lv.Code + CASE 
                    WHEN ec.[LeaveDay] = 0.5 THEN '½'
                    WHEN ec.[LeaveTypeId2] IS NOT NULL THEN '/' + lv2.Code
                    WHEN ec.[LeaveDay] = 1 THEN ''
                    ELSE ''
                    END
            END) AS Title
		,(CASE WHEN ec.[IsLeaveDay] = 1 AND ec.[LeaveDay] = 0.5 THEN 1 ELSE 0 END) AS IsLeaveHalfDay
	FROM dbo.[OvertimeDetail_138194] as otd WITH (NOLOCK)
		LEFT JOIN dbo.[EmployeeAttendance] as ea WITH (NOLOCK)
		ON ea.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ea.[AttendanceDate], otd.[Date]) = 0
		LEFT JOIN dbo.[EmployeeCalendar] as ec WITH (NOLOCK)
		ON ec.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ec.[WorkDate], otd.[Date]) = 0
		LEFT JOIN dbo.[LeaveType] as lv WITH (NOLOCK)
		ON lv.[Id] = ec.[LeaveTypeId]
		LEFT JOIN dbo.[LeaveType] as lv2 WITH (NOLOCK)
		ON lv2.[Id] = ec.[LeaveTypeId2]
		LEFT JOIN dbo.[Shift] as s WITH (NOLOCK)
		ON s.Id = ec.ShiftId
	WHERE (
			@employeeId IS NULL 
			OR otd.[EmployeeId] = @employeeId
		)
		AND (
			@verified = 2
			OR otd.[Verified] = @verified
		)
		AND DATEDIFF(DAY, @fromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @toDate, otd.[Date]) <= 0
	
	SELECT 
		ot.[OvertimeId]
		,ot.[Description]
		,otd.[Date]
		,temp.[Title]
		,otd.[Hour]
		,CASE WHEN otd.[Verified] = 1 THEN otd.[NexHour]
			  ELSE 0 END as 'AdjustedHour'
		,(FLOOR(2 * temp.[RealHourOT]/2)*1.5) as 'NexHour'
		,otd.[EmployeeId]		-- employee
		,emp.[NickName]
		,emp.[FullName]
		,ot.[CreatedBy]			-- createdBy
		,cre.[NickName]
		,cre.[FullName]
	FROM dbo.[Overtime_138194] as ot WITH (NOLOCK)
		LEFT JOIN dbo.[OvertimeDetail_138194] as otd WITH (NOLOCK)
		ON otd.[OvertimeId] = ot.[OvertimeId]
		LEFT JOIN #TempHourOT as temp WITH (NOLOCK)
		ON temp.[OvertimeId] = ot.[OvertimeId] AND temp.[EmployeeId] = otd.[EmployeeId]
		LEFT JOIN dbo.[Employee] as emp WITH (NOLOCK)
		ON emp.[Id] = otd.[EmployeeId]
		LEFT JOIN dbo.[Employee] as cre WITH (NOLOCK)
		ON cre.[Id] = ot.[CreatedBy]
	WHERE (
			@employeeId IS NULL 
			OR otd.[EmployeeId] = @employeeId
		)
		AND (
			@createdBy IS NULL
			OR ot.[CreatedBy] = @createdBy
		)
		AND ot.[Status] = 2		-- Approved
		AND (
			@verified = 2
			OR otd.[Verified] = @verified
		)
		AND DATEDIFF(DAY, @fromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @toDate, otd.[Date]) <= 0

	DROP TABLE #TempHourOT;

	DECLARE 
		@realHourOT			NUMERIC(18,1) = 0
		,@minHour			NUMERIC(18,1);
END;