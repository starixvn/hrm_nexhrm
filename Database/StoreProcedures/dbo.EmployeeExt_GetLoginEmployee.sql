USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeExt_GetLoginEmployee]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[EmployeeExt_GetLoginEmployee] @DomainUsername VARCHAR(50)
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
