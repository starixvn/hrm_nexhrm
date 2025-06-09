USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_UpdateWeekWorking]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_UpdateWeekWorking] 
	@Id int, 
	@date datetime,
	@monShift int,
	@tueShift int,
	@wedShift int,
	@thuShift int,
	@friShift int,
	@satShift int,
	@sunShift int
AS
/*
Created: 2012/04/19@Dan
Task: Update working calendar for an employee in a week
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@Id: employee ID
	@date: the date to identify the week
	@monShift: shift id for Monday
	@tueShift: shift id for Tueday
	@wedShift: shift id for Wedday
	@thuShift: shift id for Thursday
	@friShift: shift id for Friday
	@satShift: shift id for Saturday
	@sunShift: shift id for Sunday

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate datetime,
			@todate datetime,
			@dw int,
			@isHoliday bit

	SET @fromdate= DATEADD(DD, 1 - DATEPART(DW, @date), @Date)

	SET @todate= DATEADD(d, 6, @fromdate)

	UPDATE dbo.EmployeeCalendar WITH (UPDLOCK, ROWLOCK)
	SET @dw=datepart(dw, WorkDate),
		@isHoliday =  case when EXISTS(select *	from dbo.Holiday WITH (NOLOCK)
									where WorkDate between FromDate and ToDate)
							THEN 1 ELSE 0 END,
		IsHoliday = Case when @dw=1 AND @SunShift IS NULL THEN @isHoliday
						when @dw=2 AND @MonShift IS NULL THEN @isHoliday
						when @dw=3 AND @TueShift IS NULL THEN @isHoliday
						when @dw=4 AND @WedShift IS NULL THEN @isHoliday
						when @dw=5 AND @ThuShift IS NULL THEN @isHoliday
						when @dw=6 AND @FriShift IS NULL THEN @isHoliday
						when @dw=7 AND @SatShift IS NULL THEN @isHoliday
						ELSE 0
					end ,
		ShiftId =  Case when @dw=1 then @SunShift
						when @dw=2 then @MonShift
						when @dw=3 then @TueShift
						when @dw=4 then @WedShift
						when @dw=5 then @ThuShift
						when @dw=6 then @FriShift
						when @dw=7 then @SatShift
					end ,
		isOffDay = case when @dw=1 and @SunShift is null and @isHoliday=0 and LeaveTypeId is null then 1
						when @dw=2 and @MonShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=3 and @TueShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=4 and @WedShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=5 and @ThuShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=6 and @FriShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=7 and @SatShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						else 0
					end,
		ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE empId = @Id AND
		WorkDate BETWEEN @fromdate AND @todate

	-- update attendance record 
	EXEC EmployeeAttendance_ProcessingAndInsert @id, @fromdate, @todate, 1

END
GO
