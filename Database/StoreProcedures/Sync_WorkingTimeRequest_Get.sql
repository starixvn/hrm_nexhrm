CREATE OR ALTER PROCEDURE [dbo].[Sync_WorkingTimeRequest_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get working time request by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT EmpId, 
		RequestType, 
		FromDate, 
		ToDate, 
		TotalDays, 
		[Status],
		ModifiedDate
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
END