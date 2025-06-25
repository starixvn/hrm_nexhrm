CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetNexHour]
    @employeeId INT
	,@hour	NUMERIC(18,1)
	,@date	DATETIME
/*
Created: 20200306@Merlin.Le
Task: Enhance Nex Leave
DB: NexHRM
Original: 

Revisions:  20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE 
		@realHourOT			NUMERIC(18,1) = 0
		,@minHour			NUMERIC(18,1);

	IF EXISTS(
		SELECT 1 FROM dbo.[EmployeeCalendar]
		WHERE DATEDIFF(DAY, @date, WorkDate) = 0
		AND ( IsOffDay = 1 OR IsHoliday = 1) 
		AND EmpId = @employeeId
	)
	BEGIN
		SELECT 0 as [Value], 0 as [IsWorkDay]
	END

	ELSE
	BEGIN
		SELECT @hour*1.5 as [Value], 1 as [IsWorkDay]
	END
END;

GO
