CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeAttendance_Get] @LastModifiedDate DATETIME
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

	SELECT att.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		att.AttendanceDate, 
		att.InFixed, 
		att.LeaveDay, 
		att.OutFixed, 
		att.LateIn,
		att.EarlyOut, 
		att.LeaveTypeId, 
		att.LeaveTypeId2, 
		att.Absence, 
		att.TotalWorkHours,
		att.InsertDate
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = att.EmpId
	WHERE att.InsertDate > @LastModifiedDate
	
END