
CREATE PROCEDURE [dbo].[Employee_GetByNickName] @NickName VARCHAR(50)
AS
/*
Created: 20180829@Quinn.Vo
Task: Get employee info by nick name.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @EmployeeId INT

    SELECT @EmployeeId = Id
    FROM dbo.Employee WITH (NOLOCK)
    WHERE NickName = @NickName

    EXECUTE dbo.Employee_GetById @EmployeeId = @EmployeeId
END

