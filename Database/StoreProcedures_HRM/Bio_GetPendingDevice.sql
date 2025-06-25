USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Bio_GetPendingDevice]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[Bio_GetPendingDevice]
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Bio Device.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ISNULL(tb.DEVID, bd.DeviceId) AS DeviceId
	,tb.NM AS DeviceName
	,bd.DisplayName
	,bd.StatusId
	,ld.Name AS Status
	,IIF (tb.DEVID IS NOT NULL, 1 , 0) AS IsActive
	FROM [ACDB].[dbo].[T_DEV] AS tb WITH (NOLOCK) 
		FULL JOIN dbo.BioDevice AS bd WITH (NOLOCK) on tb.DEVID = bd.DeviceId
		LEFT JOIN dbo.LookupData AS ld WITH (NOLOCK) ON bd.StatusId = ld.Value AND ld.LookupId = 4
	WHERE bd.StatusId IS NULL OR bd.StatusId = '1'
END
GO
