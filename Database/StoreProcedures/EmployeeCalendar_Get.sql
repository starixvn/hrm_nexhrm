CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Get] -- '2022-01-04 11:20:25.000'
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

    SELECT 
      [WorkDate]
      ,[EmpId] AS UserId
      ,CASE WHEN ISNULL(IsWorkFromHome, 0) = 1 THEN NULL ELSE [ShiftId] END [ShiftId]
	  --,[ShiftId]
      ,[IsHoliday]
      ,CAST(CASE WHEN [IsOffDay] = 1 OR ISNULL(IsWorkFromHome, 0) = 1 THEN 1 ELSE 0 END AS BIT) AS [IsOffDay]
       ,[IsLeaveDay]
      ,[LeaveTypeId]
      ,[LeaveDay]
      ,cl.ModifiedDate
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
    WHERE @FromDate IS NULL
    OR cl.ModifiedDate > @FromDate
    ORDER BY cl.ModifiedDate
END

GO
