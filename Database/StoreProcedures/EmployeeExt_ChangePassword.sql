CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_ChangePassword] @Id INT
    ,@Password NVARCHAR(100)
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250507@Henry.Dinh
    Task: change password
    DB: NexHRM
    Original: 

    Revisions:    
        20250507@Henry.Dinh: change password
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WHERE EmpId = @Id)
    BEGIN
        SET @ErrCode = 1
        SET @ErrMsg = 'Employee not found'
        RETURN
    END

    UPDATE bodb_HRM.dbo.CompanyEmployee
    SET Password = @Password
    WHERE EmpId = @Id
END

SET ANSI_NULLS ON

GO
