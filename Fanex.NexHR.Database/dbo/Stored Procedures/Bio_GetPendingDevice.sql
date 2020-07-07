CREATE PROCEDURE [dbo].[Bio_GetPendingDevice]
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Bio Device.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ISNULL(tb.nReaderIdn, bd.DeviceId) AS DeviceId
	,tb.sName AS DeviceName
	,bd.DisplayName
	,bd.StatusId
	,ld.Name AS Status
	,IIF (tb.nReaderIdn IS NOT NULL, 1 , 0) AS IsActive
	FROM [BioStar20180607].[dbo].[TB_READER] AS tb WITH (NOLOCK) 
		FULL JOIN dbo.BioDevice AS bd WITH (NOLOCK) on tb.nReaderIdn = bd.DeviceId
		LEFT JOIN dbo.LookupData AS ld WITH (NOLOCK) ON bd.StatusId = ld.Value AND ld.LookupId = 4
	WHERE bd.StatusId IS NULL OR bd.StatusId = '1'
END
