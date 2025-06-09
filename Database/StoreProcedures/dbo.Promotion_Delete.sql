USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Promotion_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Promotion_Delete] @Id INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: delete a promotion 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            NOT EXISTS (
                SELECT 1
                FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
                WHERE Id = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The promotion does not exists '

        RETURN
    END    
    
   UPDATE HRM.dbo.EmployeeJobGrade
        SET IsDeleted = 1
			,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE Id = @Id

    SELECT @ErrCode = 0, @ErrMsg = ''
END


GO
