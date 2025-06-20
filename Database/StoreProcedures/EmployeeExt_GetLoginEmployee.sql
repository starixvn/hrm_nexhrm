CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_GetLoginEmployee] @DomainUsername VARCHAR(50)
AS
/*
    Created: 20250507@Henry.Dinh
    Task: log in
    DB: HRM
    Original: 
    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT EmpId AS Id, DomainUsername, Password, IsDeleted, Enabled
    FROM bodb_HRM.dbo.CompanyEmployee WHERE DomainUsername = @DomainUsername
END

GO
