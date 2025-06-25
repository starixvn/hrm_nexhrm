CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Insert] @empid INT = NULL
	,@fromdate DATETIME
	,@todate DATETIME
	,@setOffDay BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/06@Dan
Task: Insert default working calendar for employee
DB: HRMS
Original: 

Revisions:    
Param's Explanation:

*/
-- cut off time
SET @fromdate = convert(VARCHAR(10), @fromdate, 111)
SET @todate = convert(VARCHAR(10), @todate, 111)

DECLARE @dw INT
	,@isHoliday BIT

WHILE @fromdate <= @todate
BEGIN
	SET @isHoliday = 0
	SET @dw = datepart(dw, @fromdate)

	--print @dw
	-- kiem tra @fromdate co phai la ngay nghi
	IF (
			EXISTS (
				SELECT *
				FROM dbo.Holiday WITH (NOLOCK)
				WHERE @fromdate BETWEEN FromDate
						AND ToDate
				)
			)
		SET @isHoliday = 1;

	-- se xu ly leave request o day neu can
	
	-- check neu da co data thi Update
	IF (EXISTS (SELECT *
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE WorkDate = @fromdate))
		BEGIN
		UPDATE dbo.EmployeeCalendar WITH (UPDLOCK, ROWLOCK)
		SET IsHoliday = @isHoliday
									,isOffDay = CASE WHEN @isHoliday = 1 THEN 0
													WHEN ShiftId IS NULL THEN 1
													WHEN @setOffDay = 0 THEN 0	
													WHEN @setOffDay = 1 THEN 1
													ELSE 0 END
									,ModifiedDate = dbo.fnc_GetClientDateTime()
									WHERE (WorkDate = @fromdate) AND (@empid is null OR EmpId = @empid)
		END	
	-- insert vao working calendar
	INSERT INTO dbo.EmployeeCalendar (
		EmpID
		,WorkDate
		,ShiftID
		,isOffDay
		,IsHoliday
		,ModifiedDate
		)
	SELECT empid
		,@fromdate AS WorkDate
		,CASE 
			WHEN @dw = 1 THEN SunShift
			WHEN @dw = 2 THEN MonShift
			WHEN @dw = 3 THEN TueShift
			WHEN @dw = 4 THEN WedShift
			WHEN @dw = 5 THEN ThuShift
			WHEN @dw = 6 THEN FriShift
			WHEN @dw = 7 THEN SatShift
			END AS ShiftID
		,CASE 
			WHEN @isHoliday = 1 THEN 0
			WHEN @setOffDay = 1 THEN 1
			WHEN @dw = 1
				AND SunShift IS NULL THEN 1
			WHEN @dw = 2
				AND MonShift IS NULL THEN 1
			WHEN @dw = 3
				AND TueShift IS NULL THEN 1
			WHEN @dw = 4
				AND WedShift IS NULL THEN 1
			WHEN @dw = 5
				AND ThuShift IS NULL THEN 1
			WHEN @dw = 6
				AND FriShift IS NULL THEN 1
			WHEN @dw = 7
				AND SatShift IS NULL THEN 1
			ELSE 0
			END AS isOffDay
		,@isHoliday
		,dbo.fnc_GetClientDateTime()
	FROM dbo.EmployeeShift s WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK)
			ON e.id = s.empid AND e.isDeleted = 0 AND e.IsSpecialUser = 0
	WHERE
		-- isnull(IsDeleted,0)=0 and 
		(
			@empid IS NULL
			OR empid = @empid
			)
		-- AND isnull(ResignedDate, '2079/01/01') >= @fromdate
		AND NOT EXISTS (
			SELECT *
			FROM EmployeeCalendar c WITH (NOLOCK)
			WHERE WorkDate = @fromdate
				AND s.empid = c.empid
			)

	SET @fromdate = dateadd(d, 1, @fromdate)
END
GO
