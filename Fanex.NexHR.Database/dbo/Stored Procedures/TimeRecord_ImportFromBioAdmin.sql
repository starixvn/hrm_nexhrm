
CREATE PROCEDURE [dbo].[TimeRecord_ImportFromBioAdmin] @fromDate DATETIME
	,@toDate DATETIME
AS
/*
Created: 2012/03/13@Dan
Task: Import data from BioAdmin to HRSmart system
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
• @importdate: the date to import data
*/
BEGIN
	SET NOCOUNT ON
	DECLARE @Yesterday DATETIME
	--SET @Yesterday = DATEADD(DD, -1, GETDATE())
	SET @Yesterday =  GETDATE()

	-- get data from BioAdmin
	SELECT DATEADD(s, nDateTime, '1970-01-01') AS dtDateTime
		,nTNAEvent
		,nUserID
		,CASE WHEN rd.sName IS NULL THEN CAST(evl.nReaderIdn AS NVARCHAR(64)) ELSE rd.sName END ReaderName
		,eda.sName AS EventName
		,rd.nReaderIdn AS ReaderId
	INTO #tempFinger
	FROM [BioStar].dbo.TB_EVENT_LOG AS evl WITH (NOLOCK)
	INNER JOIN [BioStar].[dbo].[TB_EVENT_DATA] AS eda WITH (NOLOCK) ON evl.nEventIdn = eda.nEventIdn
	LEFT JOIN [BioStar].[dbo].[TB_READER] AS rd WITH (NOLOCK) ON evl.nReaderIdn = rd.nReaderIdn
	WHERE DATEADD(s, nDateTime, '1970-01-01') >= @fromDate
		AND DATEADD(s, nDateTime, '1970-01-01') <= dateadd(d, 1, @toDate) -- not use datediff because it does not use index
		AND nUserID > 0
		AND evl.nEventIdn IN (36, 38, 39, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 58, 59, 60, 61, 62, 98, 99)
		--AND nEventIdn IN (47, 58)
	ORDER BY dtDateTime
	
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.Employee e WITH (NOLOCK)
					JOIN #tempFinger f ON e.Code IS NOT NULL
					AND CAST(e.Code AS DECIMAL(18, 0)) = f.nUserID
				WHERE NOT EXISTS (
						SELECT 1
						FROM dbo.TimeRecord t WITH (NOLOCK)
						WHERE t.empID = e.ID
							AND datediff(second, RecordTime, f.dtDateTime) = 0
						)
				)
			)
	BEGIN
		-- import check-in/out data to TS_TimeRecord 
		INSERT TimeRecord (
			EmpID
			,RecordTime
			,InOutType
			,InsertDate
			,ReaderName
			,EventName
			,ReaderId
			)
		SELECT e.ID
			,dtDateTime
			,CASE 
				WHEN (
						nTNAEvent = 0
						OR nTNAEvent = 2
						) THEN 0
				WHEN (
						nTNAEvent = 1
						OR nTNAEvent = 3
						) THEN 1
				ELSE NULL
				END
			,getdate()
			,ReaderName
			,EventName
			,ReaderId
		FROM dbo.Employee e WITH (NOLOCK)
			JOIN #tempFinger f ON e.Code IS NOT NULL
			AND CAST(e.Code AS DECIMAL(18, 0)) = f.nUserID
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.TimeRecord t WITH (NOLOCK)
				WHERE t.empID = e.ID
					AND datediff(second, RecordTime, f.dtDateTime) = 0
				)
		ORDER BY dtDateTime

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@fromDate
			,@toDate
			,0
	END
	ELSE IF(NOT EXISTS (SELECT 1 FROM dbo.EmployeeAttendance WITH (NOLOCK) WHERE DATEDIFF(DD, AttendanceDate, @Yesterday) = 0))
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0
	END
	DROP TABLE #tempFinger
END

GO
GRANT EXECUTE
    ON OBJECT::[dbo].[TimeRecord_ImportFromBioAdmin] TO [NT AUTHORITY\NETWORK SERVICE]
    AS [dbo];

