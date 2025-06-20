CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeOverTime_Get] @LastModifiedDate DATETIME
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
		ISNULL(e.OldId, e.Id) AS EmployeeId, 
		otd.[Date], 
		otd.[Hour],
		ISNULL(ot.ModifiedDate, ot.CreatedDate) AS ModifiedDate
	FROM dbo.Overtime AS ot WITH (NOLOCK) 
	INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId
	WHERE ISNULL(ot.ModifiedDate, ot.CreatedDate) > @LastModifiedDate
END