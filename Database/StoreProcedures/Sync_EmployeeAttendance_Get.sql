CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeAttendance_Get]
AS
/*
Created: Jay.Do@20250610
Task: Get employee attendence
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT EmpId, 
		AttendanceDate, 
		InFixed, 
		LeaveDay, 
		OutFixed, 
		LateIn,
		EarlyOut, 
		LeaveTypeId, 
		LeaveTypeId2, 
		Absence, 
		TotalWorkHours
	FROM dbo.EmployeeAttendance WITH (NOLOCK)
END
