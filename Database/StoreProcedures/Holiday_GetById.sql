CREATE OR ALTER PROCEDURE [dbo].[Holiday_GetById] @Id INT
AS
/*
Created: 20160224@Quinn.Vo
Task: Get holiday by id.
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
    WHERE Id = @Id
END

GO
