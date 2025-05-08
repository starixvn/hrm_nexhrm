SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_LogIn] @DomainUsername VARCHAR(50)
    ,@Password NVARCHAR(100)
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
    DECLARE @IsLoginSuccess BIT = 0;

    IF EXISTS (SELECT 1 FROM EmployeeExt WHERE DomainUsername = @DomainUsername AND Password = @Password AND Enabled = 1)
    BEGIN
        SET @IsLoginSuccess = 1;
    END

    SELECT @IsLoginSuccess;
END
GO