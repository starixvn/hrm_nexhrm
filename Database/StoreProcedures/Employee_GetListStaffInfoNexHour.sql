CREATE OR ALTER PROCEDURE [dbo].[Employee_GetListStaffInfoNexHour] @ExcelData NVARCHAR(MAX)
AS
/*
Created: 20240702@Brian.Nguyen
Task: Get list staff info by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- Parse data from @ExcelData to temp table #tmpEmployee
    DECLARE @iDoc INT
    EXECUTE sys.sp_xml_preparedocument @iDoc OUTPUT, @ExcelData

    CREATE TABLE #tmpEmployee (
		NickName    NVARCHAR(20),
		Duration	NUMERIC(18,1),
		[Date]		DATETIME, 
		Notes		NVARCHAR(250),
		[Unique]		NVARCHAR(200)
    )

    INSERT INTO #tmpEmployee (
        NickName,
        Duration,
		[Date],
		Notes,
		[Unique]
    )
    SELECT ox.NickName,
           ox.Duration,
           ox.Date,
		   ox.Notes,
		   ox.[Unique]
    FROM OPENXML(@iDoc, '/ROOT/OvertimeExcelDataViewModel', 1)
    WITH (NickName NVARCHAR(20), Duration NUMERIC(18,1), [Date] DATETIME, Notes NVARCHAR(250), [Unique] NVARCHAR(200)) AS ox

	
	SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,e.[NickName]
        ,[DOB] AS DateOfBirth
        ,[Email]
        ,[Phone]
        ,[JoinedDate]
		,[GroupDate]
        ,[Skype]
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
        ,jt.NAME AS JobTitle
		,CASE WHEN EXISTS(
							SELECT 1 
							FROM dbo.[EmployeeCalendar] WITH (NOLOCK)
							WHERE DATEDIFF(DAY, tmpN.[Date], WorkDate) = 0
							AND ( IsOffDay = 1 OR IsHoliday = 1) 
							AND EmpId =  e.[Id]
						)
			   THEN
				   0
			   ELSE
				   tmpN.Duration*1.5
			   END  as NexHour
		,CASE WHEN EXISTS(
							SELECT 1 
							FROM dbo.[EmployeeCalendar] WITH (NOLOCK)
							WHERE DATEDIFF(DAY, tmpN.[Date], WorkDate) = 0
							AND ( IsOffDay = 1 OR IsHoliday = 1) 
							AND EmpId =  e.[Id]
						)
			   THEN
				   0
			   ELSE
				   1
			   END as IsWorkDay
		,tmpN.Duration AS [Hour]
		,tmpN.[Date]
		,tmpN.Notes
		,tmpN.[Unique]
        FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		INNER JOIN #tmpEmployee AS tmpN WITH (NOLOCK) ON tmpN.NickName = e.NickName
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        WHERE 
            e.IsSpecialUser = 0
			AND e.Enabled = 1
			AND e.isDeleted = 0 
		ORDER BY tmpN.[Date] ASC

	DROP TABLE #tmpEmployee
END

GO
