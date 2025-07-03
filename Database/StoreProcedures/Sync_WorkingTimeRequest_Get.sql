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
	
	SELECT wtr.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		wtr.RequestType, 
		wtr.FromDate, 
		wtr.ToDate, 
		wtr.TotalDays, 
		wtr.[Status],
		ISNULL(wtr.ModifiedDate, wtr.CreatedDate) AS ModifiedDate
	FROM dbo.WorkingTimeRequest AS wtr WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wtr.EmpId
	WHERE ISNULL(wtr.ModifiedDate, wtr.CreatedDate) > @LastModifiedDate
END