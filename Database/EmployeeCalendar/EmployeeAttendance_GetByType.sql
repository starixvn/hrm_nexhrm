USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeAttendance_GetByType]    Script Date: 3/14/2014 1:11:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EmployeeAttendance_GetByType]
	@fromtime datetime, 
	@totime datetime, 
	@empid int = null, 	
	@groupid int = null,
	@excludeUserDelete int = 1, 	
	@type smallint 
AS

/*
Created: 2012/03/17@Dan
Task: get attentance list of employees by type
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/


DECLARE @LeaveTypeDefaultId as int , @note varchar(200), @today datetime, @now datetime
SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

set @now = dateadd(hh,7, GETUTCDATE())
set @today = convert(varchar(10),@now,111) -- use UTC because the server use GMT-4

IF (@empid IS NOT NULL)
BEGIN
SELECT 	T.Id, 
		T.Code,		
		T.EmpId,
		T.FullName,
		T.NickName,
		T.Team, 
		T.TeamCode, 
		T.[Rank],
		T.AttendanceDate,
		T.ShiftId,	
		T.ShiftCode,
		T.InFixed,
		T.OutFixed,
		T.LateIn,
		T.EarlyOut,
		T.TotalWorkHours,
		T.Absence,
		T.LeaveTypeId,
		T.LeaveDay,
		T.isLateIn,
		T.isEarlyOut,
		CASE WHEN T.isMissingCheckin = 1 AND NOT EXISTS(SELECT 1 FROM dbo.WorkingTimeRequest WHERE RequestType = 1 AND convert(varchar(10),FromDate,111) = T.AttendanceDate)
		THEN 1 ELSE 0 END AS isMissingCheckin,
		CASE WHEN T.isMissingCheckOut = 0 OR EXISTS(SELECT 1 FROM dbo.WorkingTimeRequest WHERE RequestType = 2 AND convert(varchar(10),FromDate,111) = T.AttendanceDate)
		THEN 0 ELSE 1 END AS isMissingCheckOut,
		stuff(
			case when (isLateIn=1 AND LateIn > 15) then (', Late In') else '' end +  
			case when (isEarlyOut=1 AND EarlyOut > 15) then ', Early Out' else '' end   +
			case when isMissingCheckin=1 then ', Missing Check-in' else '' end   +
			case when isMissingCheckOut=1 then ', Missing Check-out' else '' end  + 		
			case when isOffday=1 then ', Off day' else '' end  + 
			case when isNotWork =1 then ', Absent' 
				else case when lv.Name is null or LeaveTypeId = @LeaveTypeDefaultId then ''
						else ', ' + lv.Name + case when LeaveDay=0.5 then ' (Half day)' else '' end
					 end
				end, 1,2,'') 	
		as  Notes
from (
	select 
		a.Id, e.Code,		
		a.EmpId,
		FullName,
		NickName,
		g.Name [Team], g.Code [TeamCode], g.[Rank],
		AttendanceDate,
		a.ShiftId,	
		s.code as ShiftCode,
		--ExpectedInTime,
		--ExpectedOutTime,
		a.InFixed,
		a.OutFixed,
		a.LateIn,
		a.EarlyOut,
		TotalWorkHours,
		Absence,
		a.LeaveTypeId,
		LeaveDay,
		--Notes,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.LateIn,0) > 0 then 1 else 0 end isLateIn,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.EarlyOut,0) > 0 then 1 else 0 end isEarlyOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and InFixed is null then 1 else 0 end isMissingCheckin,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and OutFixed is null and AttendanceDate < @today then 1 else 0 end isMissingCheckOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=1 
				and a.LeaveTypeId = @LeaveTypeDefaultId and @now> (AttendanceDate + ' ' + s.EndCheckInTime) -- over the check in time
				then 1 else 0 end isNotWork,
		case when isnull(Absence,0)=1 and a.LeaveTypeId is null then 1 else 0 end isOffday
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
		--JOIN dbo.EmployeeCalendar c WITH (NOLOCK) 	ON c.EmpId=a.EmpId and c.Workdate = a.AttendanceDate
		JOIN Employee e WITH (NOLOCK) 	ON a.Empid= e.Id
			LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id=a.ShiftId			
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid=g.Id			
	WHERE attendancedate between @fromtime and  @totime
			and ((e.isDeleted is null or e.isDeleted = 0) or @excludeUserDelete = 0)
			and (@empid is null or a.empid=@empid)		
			and (@groupid is null or g.ID=@groupid)
) T
LEFT JOIN LeaveType lv WITH (NOLOCK)  ON lv.ID = T.LeaveTypeId
WHERE @type=0 or 
		(@type =1 and isMissingCheckin =1) or
		(@type =2 and isMissingCheckout=1) or
		(@type =3 and isLateIn =1) or
		(@type =4 and isEarlyOut =1) or
		(@type =5 and isNotWork =1)
		
ORDER BY attendancedate DESC, [Rank] , TeamCode, T.code	

END
ELSE
BEGIN
SELECT 	T.Id, 
		T.Code,		
		T.EmpId,
		T.FullName,
		T.NickName,
		T.Team, 
		T.TeamCode, 
		T.[Rank],
		T.AttendanceDate,
		T.ShiftId,	
		T.ShiftCode,
		T.InFixed,
		T.OutFixed,
		T.LateIn,
		T.EarlyOut,
		T.TotalWorkHours,
		T.Absence,
		T.LeaveTypeId,
		T.LeaveDay,
		T.isLateIn,
		T.isEarlyOut,
		T.isMissingCheckin,
		T.isMissingCheckOut,
		stuff(
			case when (isLateIn=1 AND LateIn > 15) then (', Late In') else '' end +  
			case when (isEarlyOut=1 AND EarlyOut > 15) then ', Early Out' else '' end   +
			case when isMissingCheckin=1 then ', Missing Check-in' else '' end   +
			case when isMissingCheckOut=1 then ', Missing Check-out' else '' end  + 		
			case when isOffday=1 then ', Off day' else '' end  + 
			case when isNotWork =1 then ', Absent' 
				else case when lv.Name is null or LeaveTypeId = @LeaveTypeDefaultId then ''
						else ', ' + lv.Name + case when LeaveDay=0.5 then ' (Half day)' else '' end
					 end
				end, 1,2,'') 	
		as  Notes
from (
	select 
		a.Id, e.Code,		
		a.EmpId,
		FullName,
		NickName,
		g.Name [Team], g.Code [TeamCode], g.[Rank],
		AttendanceDate,
		a.ShiftId,	
		s.code as ShiftCode,
		--ExpectedInTime,
		--ExpectedOutTime,
		a.InFixed,
		a.OutFixed,
		a.LateIn,
		a.EarlyOut,
		TotalWorkHours,
		Absence,
		a.LeaveTypeId,
		LeaveDay,
		--Notes,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.LateIn,0) > 0 then 1 else 0 end isLateIn,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and isnull(a.EarlyOut,0) > 0 then 1 else 0 end isEarlyOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and InFixed is null then 1 else 0 end isMissingCheckin,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=0 
				and OutFixed is null and AttendanceDate < @today then 1 else 0 end isMissingCheckOut,
		case when e.nickname not in ('Thien.Pham', 'Viet.Hoang','Phuong.Doan') and isnull(Absence,0)=1 
				and a.LeaveTypeId = @LeaveTypeDefaultId and @now> (AttendanceDate + ' ' + s.EndCheckInTime) -- over the check in time
				then 1 else 0 end isNotWork,
		case when isnull(Absence,0)=1 and a.LeaveTypeId is null then 1 else 0 end isOffday
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
		--JOIN dbo.EmployeeCalendar c WITH (NOLOCK) 	ON c.EmpId=a.EmpId and c.Workdate = a.AttendanceDate
		JOIN Employee e WITH (NOLOCK) 	ON a.Empid= e.Id
			LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id=a.ShiftId			
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid=g.Id			
	WHERE attendancedate between @fromtime and  @totime
			and ((e.isDeleted is null or e.isDeleted = 0) or @excludeUserDelete = 0)
			and (@empid is null or a.empid=@empid)		
			and (@groupid is null or g.ID=@groupid)
) T
LEFT JOIN LeaveType lv WITH (NOLOCK)  ON lv.ID = T.LeaveTypeId
WHERE @type=0 or 
		(@type =1 and isMissingCheckin =1) or
		(@type =2 and isMissingCheckout=1) or
		(@type =3 and isLateIn =1) or
		(@type =4 and isEarlyOut =1) or
		(@type =5 and isNotWork =1)
		
ORDER BY attendancedate DESC, [Rank] , TeamCode, T.code	

END