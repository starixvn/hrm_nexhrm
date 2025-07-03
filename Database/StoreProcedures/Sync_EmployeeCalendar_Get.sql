CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeCalendar_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee calendar
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT cl.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		cl.WorkDate, 
		cl.IsOffDay, 
		cl.IsHoliday,
		cl.IsLeaveDay, 
		cl.LeaveDay,
		cl.ModifiedDate
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = cl.EmpId
	WHERE cl.ModifiedDate > @LastModifiedDate
END