CREATE OR ALTER   PROCEDURE [dbo].[Sync_WorkingTimeRequest_Get] @LastModifiedDate DATETIME
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
	
	SELECT Id,
		EmpId, 
		RequestType, 
		FromDate, 
		ToDate, 
		TotalDays, 
		[Status],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END