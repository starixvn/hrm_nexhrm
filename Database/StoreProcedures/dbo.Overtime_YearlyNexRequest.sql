USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_YearlyNexRequest]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_YearlyNexRequest] @year INT
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
	,ott.[Hour]
	,ott.[Date]
	,ec.IsHoliday
	,ec.IsOffDay
	,(CASE WHEN ec.IsHoliday = 0 AND ec.IsOffDay = 0 THEN 1 ELSE 0 END) AS IsNormalDay
	INTO #OvertimeSummary
	FROM [dbo].[OvertimeDetail] AS ott WITH (NOLOCK)
		LEFT JOIN [dbo].EmployeeCalendar AS ec WITH (NOLOCK) ON ec.EmpId = ott.EmployeeId AND ec.WorkDate = ott.[Date]
		LEFT JOIN  Employee AS e WITH (NOLOCK) ON e.Id = ott.EmployeeId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE YEAR(ott.[Date]) = @year

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
	 INNER JOIN Employee AS e WITH (NOLOCK) ON e.Id = os.EmployeeId
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
		,TotalHour FLOAT
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
		,TotalHour
		)
	SELECT e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,eg.Id
		,eg.Code
		,eg.Name
		,ISNULL(o.IsHoliday, 0)
		,ISNULL(o.IsOffDay, 0)
		,ISNULL(o.IsNormalDay, 0)
		,ISNULL(o.Jan, 0)
		,ISNULL(o.Feb, 0)
		,ISNULL(o.Mar, 0)
		,ISNULL(o.Apr, 0)
		,ISNULL(o.May, 0)
		,ISNULL(o.Jun, 0)
		,ISNULL(o.Jul, 0)
		,ISNULL(o.Aug, 0)
		,ISNULL(o.Sep, 0)
		,ISNULL(o.Otc, 0)
		,ISNULL(o.Nov, 0)
		,ISNULL(o.[Dec], 0)
		,ISNULL(o.Total, 0)
	FROM  Employee AS e WITH (NOLOCK)
	LEFT JOIN #OvertimeTest AS o ON e.Id = o.EmployeeId
	JOIN EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = e.GroupId
	WHERE e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupId
			)
		AND ISNULL(e.isDeleted, 0) = 0
	ORDER BY eg.[Rank]
		,eg.Code
		,e.Roles DESC
		,e.GroupDate

	SELECT Jan
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
		,TotalHour
		,IsHoliday
		,IsOffDay
		,IsNormalDay
		,EmpId AS EmployeeId
		,NickName
		,FullName
		,GroupId AS TeamId
		,GroupCode AS TeamCode
		,GroupName AS TeamName
	FROM #OvertimeYearly

DROP TABLE #OvertimeSummary

DROP TABLE #OvertimeTest

DROP TABLE #OvertimeYearly

END
GO
