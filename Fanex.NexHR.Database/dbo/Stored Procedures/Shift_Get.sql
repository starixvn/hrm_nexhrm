
/****** Object:  Stored Procedure dbo.Shift_Get    Script Date: 3/13/2016 3:21:43 PM ******/

CREATE PROCEDURE [dbo].[Shift_Get] 
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

    SELECT Id AS ShiftId
    ,Code
    ,Name
    ,BeginCheckInTime
    ,EndCheckInTime
    ,BeginCheckOutTime
    ,EndCheckOutTime
    ,IsInAfterDay
    ,IsOutAfterDay
    ,IsDisabled
    FROM dbo.[Shift] WITH (NOLOCK)
END

