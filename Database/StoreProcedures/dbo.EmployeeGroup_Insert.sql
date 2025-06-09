USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Insert] @Code VARCHAR(20)
    ,@Name VARCHAR(50)
    ,@DepartmentId INT = NULL
    ,@ParentId INT = NULL
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: insert group
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup WITH (NOLOCK)
                WHERE (Code = @Code
                    OR NAME = @Name)
                    AND ISNULL(Deleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This team has already existed.'

        RETURN
    END

    INSERT INTO dbo.EmployeeGroup (
        Code
        ,NAME
        ,DepartmentId
        ,CreatedDate
        ,CreatedBy
        ,ModifiedDate
        ,ParentId
        )
    VALUES (
        @Code
        ,@Name
        ,@DepartmentId
        ,GETDATE()
        ,@CreatedBy
        ,GETDATE()
        ,@ParentId
        )
END

GO
