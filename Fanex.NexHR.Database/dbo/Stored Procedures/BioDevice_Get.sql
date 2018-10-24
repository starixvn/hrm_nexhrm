
CREATE PROCEDURE [dbo].[BioDevice_Get]
	@DeviceId INT
AS
/*
Created: 20180621@Elton.Ton
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
	,IIF (tb.nReaderIdn IS NOT NULL, 1 , 0) AS Active
	FROM [BioStar].[dbo].[TB_READER] AS tb WITH (NOLOCK) FULL JOIN dbo.BioDevice AS bd WITH (NOLOCK) on tb.nReaderIdn = bd.DeviceId
	LEFT JOIN dbo.LookupData AS ld WITH (NOLOCK) ON bd.StatusId = ld.Value AND ld.LookupId = 4
	WHERE bd.DeviceId = @DeviceId OR tb.nReaderIdn = @DeviceId
END

