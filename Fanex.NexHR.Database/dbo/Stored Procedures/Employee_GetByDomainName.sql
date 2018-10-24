
CREATE PROCEDURE [dbo].[Employee_GetByDomainName] @DomainUsername VARCHAR(50)
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by domain username.
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
    WHERE DomainUsername = @DomainUsername

    EXECUTE dbo.Employee_GetById @EmployeeId = @EmployeeId
END

