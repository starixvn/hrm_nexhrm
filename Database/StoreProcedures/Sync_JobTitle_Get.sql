CREATE OR ALTER PROCEDURE [dbo].[Sync_JobTitle_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get job title by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT JobTitleId AS Id, 
		[Name],
		ModifiedDate
	FROM  HRM.dbo.JobTitle WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
END