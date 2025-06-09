USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_UpdateSalary]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_UpdateSalary] 
	@ApplicantId INT
	,@OfferSalary VARCHAR(500)
	,@ExpectedSalary VARCHAR(500)
	,@FinalComment VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
    UPDATE dbo.Applicant WITH (ROWLOCK, UPDLOCK)
	SET OfferSalary = @OfferSalary
		,ExpectedSalary = @ExpectedSalary
		,FinalComment = @FinalComment
		,CreatedDate = dbo.fnc_GetClientDateTime()
	WHERE ApplicantId = @ApplicantId
END
GO
