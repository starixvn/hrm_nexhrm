USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_Delete]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231120@Lilac.Nguyen
Task: delete a applicant 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

	
    IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.Applicant WITH (NOLOCK)
                WHERE ApplicantId = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The applicant does not exists '

        RETURN
    END

	DELETE FROM dbo.Applicant 
	WHERE ApplicantId = @Id;

	 SELECT @ErrCode = 0, @ErrMsg = ''

END


GO
