
/****** Object:  Stored Procedure dbo.Shift_GetShifts    Script Date: 3/13/2016 3:21:43 PM ******/


CREATE PROCEDURE [dbo].[Shift_GetShifts]
AS
/*
Created: 20141002@Quinn.Vo
Task: Get all shifts
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT [Id]
      ,[Code]
      ,[Name]
      ,[BeginCheckInTime]
      ,[EndCheckInTime]
      ,[BeginCheckOutTime]
      ,[EndCheckOutTime]
      ,[BreakTimeMins]
      ,[OrderNo]
      ,[BeginBreakTime]
      ,[EndBreakTime]
      ,[IsInAfterDay]
      ,[IsOutAfterDay]
      ,[ModifiedBy]
      ,[ModifiedDate]
      ,[IsDisabled]
    FROM dbo.[Shift]
    WHERE IsDisabled = 0
    ORDER BY OrderNo
END

