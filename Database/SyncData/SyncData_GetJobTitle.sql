SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SyncData_GetJobTitle]
AS
/*
Created: 20201130@Oliver.Dang
Task: Get Jobtitle to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT lkd.Id as JobTitleId
		  ,lkd.Name
	FROM dbo.LookupData lkd WITH (NOLOCK)
	WHERE lkd.LookupId = 1
END
