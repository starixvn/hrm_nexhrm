CREATE PROCEDURE [dbo].[Bio_GetWarningTimeChange]
    @FromDate DATETIME
	,@ToDate DATETIME
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Bio Station Time changes events on date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT bio.DeviceId
	    ,tb.sName as DeviceName
		,dev.DisplayName
	    ,bio.TimeChange
	    ,bio.PreviousLogTime
	    ,bio.NextLogTime
    FROM [dbo].[BioStationTimeChange] AS bio WITH(NOLOCK)
        INNER JOIN [dbo].[BioDevice] AS dev WITH(NOLOCK) ON bio.DeviceId = dev.DeviceId
		LEFT JOIN [BioStar20180607].[dbo].[TB_READER] AS tb WITH (NOLOCK) on tb.nReaderIdn = bio.DeviceId
    WHERE bio.InsertedDate BETWEEN @FromDate AND @ToDate
		AND (bio.TimeChange NOT BETWEEN bio.PreviousLogTime AND bio.NextLogTime)
		AND (ABS(datepart(minute, bio.TimeChange) - datepart(minute, bio.PreviousLogTime)) > 2 )
		AND (ABS(datepart(minute, bio.TimeChange) - datepart(minute, bio.NextLogTime)) > 2 )
END