SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
    SELECT Id, DomainUsername, Password, IsDeleted, Enabled
    FROM EmployeeExt WHERE DomainUsername = @DomainUsername
END
GO