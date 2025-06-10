CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeCalendar_Get] @LastModifiedDate DATETIME
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
	
	SELECT EmpId, 
		WorkDate, 
		IsOffDay, 
		IsHoliday,
		IsLeaveDay, 
		LeaveDay
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
END