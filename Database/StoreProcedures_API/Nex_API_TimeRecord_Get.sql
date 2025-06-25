ALTER PROCEDURE [dbo].[Nex_API_TimeRecord_Get]
(
    @fromDate DATETIME
    ,@toDate DATETIME
)
AS
/*
    Created: 20250611@Tim
    Task: get check in/out
    DB: bodb_HRM

    Revisions:
        20250611@Tim: init
    Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SQLString NVARCHAR(4000)
		,@CurrentMonthTableName VARCHAR(50)
		,@LastMonthTableName VARCHAR(50)
		,@fromDateString VARCHAR(10)
		,@toDateString VARCHAR(10)
		,@StartDateConnectBitoStar VARCHAR(10)

	DECLARE @toDay DATETIME, @Yesterday DATETIME
	SET @toDay =  GETDATE()
	SET @Yesterday = DATEADD(DD, -1, @toDay)

	CREATE TABLE #tmpRecord (
		nUserID VARCHAR(32)
		,dtDateTime DATETIME
		,nTNAEvent INT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)
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
		
		SET @SQLString = N'INSERT INTO #tmpRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
					SELECT a.USRID AS nUserID
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
		SET @SQLString = N'INSERT INTO #tmpRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
					SELECT a.USRID AS nUserID
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

	SELECT nUserID AS EmpNo
        ,dtDateTime AS RecordTime
        ,CASE 
	        WHEN nTNAEvent = 1 THEN 0
	        WHEN nTNAEvent = 2 THEN 1
	        ELSE NULL END AS InOutType
        ,ReaderId
        ,ReaderName
        ,EventName
	FROM #tmpRecord
	WHERE dtDateTime BETWEEN @FromDate AND @ToDate
	ORDER BY dtDateTime

	DROP TABLE #tmpRecord
END