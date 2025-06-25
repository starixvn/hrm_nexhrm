USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Position_Delete]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Position_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 202311270@Lilac.Nguyen
Task: delete a position 
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
                FROM dbo.Position WITH (NOLOCK)
                WHERE PositionId = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The position does not exists '

        RETURN
    END
    
    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Position AS p WITH (NOLOCK)
            INNER JOIN dbo.Applicant AS a WITH (NOLOCK) ON p.PositionId = a.PositionId
            WHERE p.PositionId = @Id
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The position has applicants '

        RETURN
    END
    
    DELETE FROM dbo.Position
    WHERE PositionId = @Id

    SELECT @ErrCode = 0, @ErrMsg = ''
END


GO
