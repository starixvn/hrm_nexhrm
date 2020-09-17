CREATE PROCEDURE [dbo].[Overtime_YearlyNexRequest] @year INT
	,@employeeId INT = NULL
	,@groupId INT = NULL
AS
/*
Created: 20200917-Lilac.Nguyen
Task: Get overtime yearly NEX request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SELECT ott.EmployeeId
	,e.NickName
	,eg.Name AS GroupName
	,ott.Hour
	,ott.Date
	,ec.IsHoliday
	,ec.IsOffDay
	,(CASE WHEN ec.IsHoliday = 0 AND ec.IsOffDay = 0 THEN 1 ELSE 0 END) AS IsNormalDay
	INTO #OvertimeSummary
	FROM [dbo].[OvertimeDetail] AS ott
		LEFT JOIN [dbo].EmployeeCalendar AS ec ON ec.EmpId = ott.EmployeeId AND ec.WorkDate = ott.Date
		LEFT JOIN  Employee AS e ON e.Id = ott.EmployeeId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE YEAR(ott.[Date]) = @year
	AND e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupId
			)
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.GroupId NOT IN (11, 40)
	ORDER BY ott.EmployeeId

	SELECT os.EmployeeId
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 1 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jan'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 2 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Feb'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 3 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Mar'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 4 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Apr'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 5 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'May'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 6 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jun'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 7 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jul'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 8 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Aug'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 9 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Sep'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 10 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Otc'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 11 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Nov'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 12 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Dec'
		,SUM([Hour]) 'Total'
	,os.IsHoliday
	,os.IsOffDay
	,os.IsNormalDay
	INTO #OvertimeTest
	 FROM  #OvertimeSummary AS os
	 JOIN Employee AS e ON e.Id = os.EmployeeId
	 GROUP BY os.EmployeeId, os.IsHoliday, os.IsOffDay, os.IsNormalDay

 IF OBJECT_ID('#OvertimeYearly') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeYearly
	END

	CREATE TABLE #OvertimeYearly (
		EmpId FLOAT
		,Code INT
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupId INT
		,GroupCode VARCHAR(20)
		,GroupName NVARCHAR(50)
		,IsHoliday INT
		,IsOffDay INT
		,IsNormalDay INT
		,Jan FLOAT
		,Feb FLOAT
		,Mar FLOAT
		,Apr FLOAT
		,May FLOAT
		,Jun FLOAT
		,Jul FLOAT
		,Aug FLOAT
		,Sep FLOAT
		,Oct FLOAT
		,Nov FLOAT
		,[Dec] FLOAT
		,Total FLOAT
		)

	INSERT INTO #OvertimeYearly (
		EmpId 
		,Code
		,NickName
		,FullName
		,GroupId
		,GroupCode
		,GroupName
		,IsHoliday
		,IsOffDay
		,IsNormalDay
		,Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,[Dec]
		,Total
		)
	SELECT o.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,eg.Id
		,eg.Code
		,eg.Name
		,o.IsHoliday
		,o.IsOffDay
		,o.IsNormalDay
		,o.Jan
		,o.Feb
		,o.Mar
		,o.Apr
		,o.May
		,o.Jun
		,o.Jul
		,o.Aug
		,o.Sep
		,o.Otc
		,o.Nov
		,o.[Dec]
		,o.Total
	FROM  #OvertimeTest AS o 
	JOIN Employee AS e ON e.Id = o.EmployeeId
	JOIN EmployeeGroup AS eg ON eg.Id = e.GroupId
	ORDER BY eg.[Rank]
		,eg.Code
		,(CASE e.Id
			WHEN eg.LeaderId THEN 2
			WHEN eg.AssistantLeaderId THEN 1
			ELSE 0 END
			) DESC
		,e.Code

	select Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,[Dec]
		,Total
		,IsHoliday
		,IsOffDay
		,IsNormalDay
		,EmpId 
		,Code
		,NickName
		,FullName
		,GroupId
		,GroupCode
		,GroupName
	FROM #OvertimeYearly

DROP TABLE #OvertimeSummary

DROP TABLE #OvertimeYearly

END