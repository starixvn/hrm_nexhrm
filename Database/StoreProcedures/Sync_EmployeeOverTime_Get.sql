CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeOverTime_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee ot by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT ot.OvertimeId,
		otd.EmployeeId, 
		otd.[Date], 
		otd.[Hour]
	FROM NexHRM.dbo.Overtime AS ot WITH (NOLOCK) 
	INNER JOIN NexHRM.dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	WHERE ot.ModifiedDate > @LastModifiedDate
END