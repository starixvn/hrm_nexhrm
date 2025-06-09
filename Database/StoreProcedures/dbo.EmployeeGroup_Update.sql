USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Update] @Id INT
    ,@Code VARCHAR(20)
    ,@Name NVARCHAR(50)
    ,@DepartmentId INT = NULL
    ,@ParentId INT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: update group 
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
                WHERE Id <> @Id
                    AND (
                        Code = @Code
                        OR NAME = @Name
                        )
                    AND ISNULL(Deleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This team has already existed.'

        RETURN
    END

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Code = @Code
        ,[Name] = @Name
        ,DepartmentId = @DepartmentId
        ,ParentId = @ParentId
        ,ModifiedDate = GETDATE()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id
END

GO
