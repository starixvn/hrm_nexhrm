USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_GetById]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetById]
    @ApplicantId INT
AS
/*
Created: 20231123@Lilac.Nguyen
Task: Get by ApplicantId.
DB: NexHRM
Original: 

Revisions:  
	- 20250310@Henry.Dinh: add management grade
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ap.ApplicantId AS Id
	,ap.FullName
	,ap.PositionId
	,ap.Phone
	,ap.Email
	,ap.ApplyDate
	,ap.YearsOfExperience
	,ap.DOB
	,ap.ProfessionalLevel
	,ap.Major AS ProfessionalMajor
	,ap.[Status]
	,ap.[Resume]
	,ap.InterviewDate1
	,ap.Result1
	,ap.InterviewDate2
	,ap.Result2
	,ap.JoinDate
	,ap.[Source]
	,ap.RecruiterId
	,e.NickName AS NickNameRecruiter 
	,e.FullName AS FullNameRecruiter
	,ap.EducationBackground
	,ap.Grade
	,ap.ManagementGrade
	,ap.InterviewerIds AS InterviewerIdString
	,ap.InterviewerComment
	,ap.Note
	,ap.FinalComment
	,ap.OfferSalary
	,ap.ExpectedSalary
    FROM dbo.Applicant AS ap WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ap.RecruiterId
    WHERE ap.ApplicantId = @ApplicantId

END
GO
