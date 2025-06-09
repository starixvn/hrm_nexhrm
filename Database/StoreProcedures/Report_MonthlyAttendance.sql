CREATE OR ALTER PROCEDURE [dbo].[Report_MonthlyAttendance]
	@fromtime datetime, 
	@totime datetime, 
	@empid int = null, 	
	@groupid int = null
AS

/*
Created: 2012/03/14@Dan
Task: monthly attendance report
DB: NexHR
Original: 

Revisions:    
Param's Explanation:	
*/

select 
	a.Id,
	EmpId,
	FullName,
	NickName,
	g.Name [Team],
	AttendanceDate,
	ShiftId,	
	s.code as ShiftCode,
	ExpectedInTime,
	ExpectedOutTime,
	InFixed,
	OutFixed,
	LateIn,
	EarlyOut,
	TotalWorkHours,
	Absence,
	LeaveTypeId,		
	Notes		
from dbo.EmployeeAttendance a WITH (NOLOCK) JOIN Employee e WITH (NOLOCK) 
		ON a.Empid= e.Id
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id=a.ShiftId			
		JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid=g.Id
where (@empid is null or empid=@empid)		
	and (@groupid is null or g.ID=@groupid)
	and attendancedate between @fromtime and  @totime

order by NickName, attendancedate
GO
