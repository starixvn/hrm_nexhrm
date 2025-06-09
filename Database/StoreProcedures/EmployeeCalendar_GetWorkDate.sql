CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWorkDate] -- '2022-01-04 11:20:25.000'
    @EmpId int,
	@Year int
AS
/*
Created: 20230403@Brian.nguyen
Task: Get work date calendar
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	SELECT [WorkDate]      
	  FROM [NexHRM].[dbo].[EmployeeCalendar]
	  WHERE EmpId = @EmpId and (IsHoliday = 1 OR IsOffDay = 1) and Year(WorkDate) = @Year
	  GROUP BY [WorkDate]
END

GO
