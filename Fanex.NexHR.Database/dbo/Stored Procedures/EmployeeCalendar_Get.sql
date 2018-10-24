
/****** Object:  Stored Procedure dbo.EmployeeCalendar_Get    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE PROCEDURE [dbo].[EmployeeCalendar_Get] 
    @FromDate DATETIME = NULL
AS
/*
Created: 20141002@Quinn.Vo
Task: Get employee calendar from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT --TOP 10
      [WorkDate]
      ,[EmpId] AS UserId
      ,[ShiftId]
      ,[IsHoliday]
      ,[IsOffDay]
      ,[IsLeaveDay]
      ,[LeaveTypeId]
      ,[LeaveDay]
      ,cl.ModifiedDate
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
    WHERE @FromDate IS NULL
    OR cl.ModifiedDate > @FromDate
    ORDER BY cl.ModifiedDate
END

