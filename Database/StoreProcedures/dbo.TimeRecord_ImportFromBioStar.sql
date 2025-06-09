USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_ImportFromBioStar]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- [TimeRecord_ImportFromBioStar] '2020-06-30', '2020-07-01'

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportFromBioStar]
	@fromDate DATETIME
	,@toDate DATETIME
AS
/*
Created: 20170914@QuinnVo
Task: Import data from BioStar to HR system
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @SQLString NVARCHAR(4000)
		,@CurrentMonthTableName VARCHAR(50)
		,@LastMonthTableName VARCHAR(50)
		,@fromDateString VARCHAR(10)
		,@toDateString VARCHAR(10)
		,@StartDateConnectBitoStar VARCHAR(10)

	DECLARE @toDay DATETIME, @Yesterday DATETIME
	SET @toDay =  GETDATE()
	SET @Yesterday = DATEADD(DD, -1, @toDay)

	CREATE TABLE #tempRecord (
		nUserID VARCHAR(32)
		,dtDateTime DATETIME
		,nTNAEvent INT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)
		)

	CREATE TABLE #tempTimeRecord (
		EmpId INT
		,TimeRecord DATETIME
		)

	SELECT @CurrentMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, GETDATE(), 112), 6)
		,@LastMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, DATEADD(MM, - 1, GETDATE()), 112), 6)
		,@fromDateString = CONVERT(VARCHAR, @fromDate, 112)
		,@toDateString = CONVERT(VARCHAR, @toDate, 112)
		,@StartDateConnectBitoStar = '2022-10-01'

	IF EXISTS (
			SELECT 1
			FROM [ACDB].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @LastMonthTableName
			)
	BEGIN
		
		SET @SQLString = N'INSERT INTO #tempRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
						SELECT 
						a.USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,a.[TNAKEY] AS nTNAEvent
						  ,b.[DEVID]
						  ,b.[NM]
						  ,c.NM
					FROM [ACDB].[dbo].' + @LastMonthTableName + ' AS a
					LEFT JOIN [ACDB].[dbo].[T_DEV] AS b ON a.DEVUID = b.[DEVID]
					LEFT JOIN [ACDB].[dbo].[T_EVTTYP] AS c ON a.EVT = c.EVT
					WHERE a.USRID <> '''' AND a.EVT < 5400 AND a.USRGRUID <> 2250
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END

	IF EXISTS (
			SELECT 1
			FROM [ACDB].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @CurrentMonthTableName
			)
	BEGIN
		SET @SQLString = N'INSERT INTO #tempRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
						SELECT 
					 a.USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,a.[TNAKEY] AS nTNAEvent
						  ,b.[DEVID]
						  ,b.[NM]
						  ,c.NM
	  
					FROM [ACDB].[dbo].' + @CurrentMonthTableName + ' AS a 
					LEFT JOIN [ACDB].[dbo].[T_DEV] AS b ON a.DEVUID = b.[DEVID]
					LEFT JOIN [ACDB].[dbo].[T_EVTTYP] AS c ON a.EVT = c.EVT
					WHERE a.USRID <> '''' AND a.EVT < 5400 AND a.USRGRUID <> 2250
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END

	INSERT #tempTimeRecord(EmpId, TimeRecord)
	SELECT e.Id
		,dtDateTime
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.nUserID AS INT), -1)
	WHERE e.isDeleted = 0
		AND	NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.empID = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.dtDateTime) = 0
			)

	INSERT TimeRecord (
		EmpID
		,RecordTime
		,InOutType
		,InsertDate
		,ReaderId
		,ReaderName
		,EventName
		)

	SELECT e.Id
		,dtDateTime
		,CASE 
			WHEN nTNAEvent = 1 THEN 0
			WHEN nTNAEvent = 2 THEN 1
			ELSE NULL
			END
		,GETDATE()
		,f.ReaderId
		,f.ReaderName
		,f.EventName
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.nUserID AS INT), -1)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.empID = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.dtDateTime) = 0
			)

	INSERT INTO dbo.tmpEmployeeAttendance(EmpId, WorkDate, InsertedDate)
	SELECT t.EmpId
		,CAST(TimeRecord AS DATE)
		,GETDATE()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(DATEADD(DD, -1, t.TimeRecord) AS DATE))
	UNION
	SELECT t.EmpId 
		,CAST(TimeRecord AS DATE)
		,GETDATE()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(t.TimeRecord AS DATE))

	EXEC dbo.EmployeeAttendance_ProcessingData

	IF(EXISTS (SELECT 1 
		FROM dbo.Employee AS e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeAttendance AS a WITH (NOLOCK) ON e.Id = a.EmpId AND DATEDIFF(DD, AttendanceDate, @toDate) = 0
		WHERE e.isDeleted = 0 AND e.Enabled = 1 
		AND e.IsSpecialUser = 0
		AND a.EmpId IS NULL))
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@toDay
			,@toDay
			,0
	END

	DROP TABLE #tempRecord
	DROP TABLE #tempTimeRecord
END
GO
