CREATE PROCEDURE [dbo].[BioStationTimeChange_Get]
    @FromDate SMALLDATETIME = NULL
    ,@ToDate SMALLDATETIME = NULL
AS
/*
Created: 20180626@Hunter.Tran
Task: Get Bio Station Time changes events in range
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT bio.DeviceId
	    ,tb.sName as DeviceName
		,dev.DisplayName
	    ,bio.TimeChange
	    ,bio.PreviousLogTime
	    ,bio.NextLogTime
    FROM dbo.BioStationTimeChange AS bio WITH(NOLOCK)
        INNER JOIN dbo.BioDevice AS dev WITH(NOLOCK) ON bio.DeviceId = dev.DeviceId
		LEFT JOIN [BioStar].[dbo].[TB_READER] AS tb WITH (NOLOCK) on tb.nReaderIdn = bio.DeviceId
    WHERE (@FromDate <= bio.TimeChange AND bio.TimeChange <= @ToDate)
        OR (@FromDate IS NULL AND bio.TimeChange <= @ToDate)
        OR (@FromDate <= bio.TimeChange AND @ToDate IS NULL)
        OR (@FromDate IS NULL AND @ToDate IS NULL)
END
