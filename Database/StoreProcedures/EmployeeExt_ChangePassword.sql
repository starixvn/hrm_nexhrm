USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeExt_ChangePassword]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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

    IF NOT EXISTS (SELECT 1 FROM dbo.EmployeeExt WHERE Id = @Id)
    BEGIN
        SET @ErrCode = 1
        SET @ErrMsg = 'Employee not found'
        RETURN
    END

    UPDATE dbo.EmployeeExt
    SET Password = @Password
    WHERE Id = @Id
END

SET ANSI_NULLS ON

GO
