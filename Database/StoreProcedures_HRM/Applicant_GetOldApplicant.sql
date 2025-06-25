USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_GetOldApplicant]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetOldApplicant] 
AS
BEGIN
    SELECT b.Id, a.ApplicantId, a.OfferSalary AS OldOfferSalary, a.ExpectedSalary AS OldExpectedSalary
		,a.FinalComment
	FROM dbo.Applicant AS a WITH (NOLOCK)
	LEFT JOIN [JobApplicants].[dbo].[Applicants] AS b ON a.RefId = b.Id
	WHERE b.Id IS NULL
	AND (ISNULL(a.OfferSalary, '') <> ''
		OR ISNULL(a.ExpectedSalary, '') <> '' 
		OR ISNULL(a.FinalComment, '') <> '' 
	)
END
GO
