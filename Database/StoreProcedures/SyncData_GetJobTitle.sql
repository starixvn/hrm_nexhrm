CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetJobTitle]
AS
/*
Created: 20201130@Oliver.Dang
Task: Get Jobtitle to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT jt.JobTitleId
		  ,jt.Name
	FROM HRM.dbo.JobTitle jt WITH (NOLOCK)
END

GO
