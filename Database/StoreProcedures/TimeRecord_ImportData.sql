ALTER PROCEDURE [dbo].[TimeRecord_ImportData]
(
    @TimeRecordXml NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
)
AS
/*
    Created: 20160927
    Task: import time record
    DB: NexHR

    Revisions:    
    Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Today DATETIME
        ,@Yesterday DATETIME
		,@TempDate DATETIME
        ,@DocHandler INT

	SELECT @Today = dbo.fnc_GetClientDateTime()

    SELECT @Yesterday = DATEADD(dd, - 1, @Today)

	CREATE TABLE #tempRecord (
		EmpNo VARCHAR(32)
		,RecordTime DATETIME
		,InOutType TINYINT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)
		)

    CREATE TABLE #tempTimeRecord (
		EmpId INT
		,TimeRecord DATETIME
		)

    EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@TimeRecordXml

    INSERT INTO #tempRecord (
		EmpNo
		,RecordTime
		,InOutType
		,ReaderId
		,ReaderName
		,EventName
		)
    SELECT EmpNo
		,RecordTime
		,InOutType
		,ReaderId
		,ReaderName
		,EventName
    FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (
        EmpNo VARCHAR(32)
		,RecordTime DATETIME
		,InOutType TINYINT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)) AS d

    EXEC sp_xml_removedocument @DocHandler

    INSERT #tempTimeRecord(EmpId, TimeRecord)
	SELECT e.Id
		,f.RecordTime
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tempRecord AS f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.EmpNo AS INT), -1)
	WHERE e.isDeleted = 0
		AND	NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord AS t WITH (NOLOCK)
			WHERE t.EmpId = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.RecordTime) = 0
			)

    INSERT dbo.TimeRecord (
		EmpId
		,RecordTime
		,InOutType
		,InsertDate
		,ReaderId
		,ReaderName
		,EventName
		)
	SELECT e.Id
		,f.RecordTime
		,f.InOutType
		,dbo.fnc_GetClientDateTime()
		,f.ReaderId
		,f.ReaderName
		,f.EventName
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.EmpNo AS INT), -1)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.EmpId = e.Id
				AND DATEDIFF(SECOND, t.RecordTime, f.RecordTime) = 0
			)

    INSERT INTO dbo.tmpEmployeeAttendance(EmpId, WorkDate, InsertedDate)
	SELECT t.EmpId
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(DATEADD(DD, -1, t.TimeRecord) AS DATE)
    )

	UNION
	SELECT t.EmpId 
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(t.TimeRecord AS DATE)
    )

	EXEC dbo.EmployeeAttendance_ProcessingData

	IF EXISTS (
        SELECT 1 
		FROM dbo.Employee AS e WITH (NOLOCK) 
		    LEFT JOIN dbo.EmployeeAttendance AS a WITH (NOLOCK) ON e.Id = a.EmpId AND DATEDIFF(DD, AttendanceDate, @Today) = 0
		WHERE e.isDeleted = 0
            AND e.Enabled = 1 
		    AND e.IsSpecialUser = 0
		    AND a.EmpId IS NULL
    )
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Today
			,@Today
			,0
	END

	SELECT @TempDate = DATEADD(MM, 2, dbo.fnc_GetClientDateTime())

	IF (DATEDIFF(MI, CAST(@Today AS DATE), @Today) < 5)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
					LEFT JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
					AND WorkDate = CAST(@TempDate AS DATE)
					AND ISNULL(e.Enabled, 0) = 1
					AND ISNULL(e.isDeleted, 0) = 0
				)
		BEGIN
			EXEC dbo.EmployeeCalendar_Insert NULL
				,@TempDate
				,@TempDate
		END
	END

	DROP TABLE #tempRecord
	DROP TABLE #tempTimeRecord
END