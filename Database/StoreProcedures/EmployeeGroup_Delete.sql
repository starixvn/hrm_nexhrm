USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.EmployeeGroup_Delete    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Delete] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: soft delete a group 
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
                FROM dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                    INNER JOIN Employee e WITH (NOLOCK) ON eg.EmpID = e.ID
                WHERE eg.GroupID = @Id
                    AND ISNULL(isDeleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The group is not empty so cannot delete it'

        RETURN
    END

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Deleted = 1
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id

    SET @ErrCode = 0
END

GO
