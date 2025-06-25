USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[JobTitle_Delete]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Delete] @JobTitleId INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20240731@Quinn.Vo
Task: delete jobtitle.
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                WHERE e.JobTitleId = @JobTitleId
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The employee is not empty so cannot delete it'

        RETURN
    END

    DELETE FROM dbo.JobTitle
    WHERE JobTitleId = @JobTitleId

    SET @ErrCode = 0
END

GO
