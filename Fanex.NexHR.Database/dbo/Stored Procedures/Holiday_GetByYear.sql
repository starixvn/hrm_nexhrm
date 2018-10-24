
/****** Object:  Stored Procedure dbo.Holiday_GetByYear    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[Holiday_GetByYear] @Year INT
AS
/*
Created: 20160224@Quinn.Vo
Task: Get holiday by year.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT Id
        ,HolidayName AS NAME
        ,FromDate
        ,ToDate
    FROM HOLIDAY WITH (NOLOCK)
    WHERE YEAR(FromDate) = @Year
END

