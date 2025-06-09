USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_UpdateOnTop]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_UpdateOnTop] 
    @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240328@Lilac.Nguyen
Task: Update On Top.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.Applicant WITH (UPDLOCK, ROWLOCK)
	SET IsOnTop = CASE WHEN ISNULL(IsOnTop, 0) = 0 THEN 1 ELSE 0 END
	WHERE ApplicantId = @Id;

	SELECT @ErrCode = 0, @ErrMsg = ''

END
GO
