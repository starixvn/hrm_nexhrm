USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_GetOldApplicant_xtest]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetOldApplicant_xtest] 
AS
BEGIN
    SELECT b.Id, a.ApplicantId, b.OfferedSalary AS OldOfferSalary, b.ExpectedSalary AS OldExpectedSalary
		,b.Comments AS FinalComment
	FROM dbo.Applicant AS a WITH (NOLOCK)
	INNER JOIN [JobApplicants].[dbo].[Applicants] AS b ON a.RefId = b.Id
	WHERE (ISNULL(b.OfferedSalary, '') <> '' 
		OR  ISNULL(b.ExpectedSalary, '') <> ''
		OR ISNULL(b.Comments, '') <> '')
	-- AND a.ApplicantId = 4188
END
GO
