USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_ImportBioStationTimeChange]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportBioStationTimeChange]
AS
/*
Created: 20180626@Quinn
Task: Import timechange events from BioStation database
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @FromDate DATETIME = DATEADD(DD, -30, GETDATE())

    CREATE TABLE #EventLog
    (
        nEventLogIdn INT
	    ,nDateTime DATETIME
	    ,nReaderIdn INT
	    ,nEventIdn INT
    )

    INSERT INTO #EventLog
    (
        nEventLogIdn
        ,nDateTime
        ,nEventIdn
        ,nReaderIdn
    )
    SELECT nEventLogIdn
	    ,DATEADD(s, nDateTime, '1970-01-01')
	    ,nEventIdn
	    ,nReaderIdn
    FROM [BioStar].dbo.TB_EVENT_LOG WITH (NOLOCK)
    WHERE DATEADD(s, nDateTime, '1970-01-01') > @FromDate

	INSERT INTO [dbo].BioStationTimeChange
	(
	   [nEventLogIdn]
      ,[DeviceId]
      ,[TimeChange]
      ,[PreviousLogTime]
      ,[NextLogTime]
      ,[InsertedDate]
	)
    SELECT con.nEventLogIdn
		,con.nReaderIdn AS DeviceId
	    ,con.nDateTime AS TimeChange
	    ,pre.nDateTime AS PreviousLogTime
	    ,nex.nDateTime AS NextLogTime
		,GETDATE()
    FROM (
	SELECT timechanges.nEventLogIdn
		,(
			SELECT MIN(ee.nEventLogIdn)
            FROM #EventLog AS ee
            WHERE timechanges.nReaderIdn <> ee.nReaderIdn
                AND timechanges.nEventLogIdn < ee.nEventLogIdn
			) AS NexId
		,(
			SELECT MAX(ee.nEventLogIdn)
            FROM #EventLog AS ee
            WHERE timechanges.nReaderIdn <> ee.nReaderIdn
                AND timechanges.nEventLogIdn > ee.nEventLogIdn
			) AS PreId
        FROM #EventLog AS timechanges
        WHERE timechanges.nEventIdn = 210
	) NextPre
        INNER JOIN #EventLog AS con ON NextPre.nEventLogIdn = con.nEventLogIdn
        INNER JOIN #EventLog AS nex ON NextPre.NexId = nex.nEventLogIdn
        INNER JOIN #EventLog AS pre ON NextPre.PreId = pre.nEventLogIdn
	WHERE NOT EXISTS (
		SELECT 1 FROM dbo.BioStationTimeChange WHERE nEventLogIdn = con.nEventLogIdn
	)
    ORDER BY con.nEventLogIdn

    DROP TABLE #EventLog
END

GO
