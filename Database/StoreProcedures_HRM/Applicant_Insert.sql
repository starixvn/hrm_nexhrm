USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_Insert] 
    @FullName NVARCHAR(100)
    ,@PositionId INT
    ,@Phone VARCHAR(20) = NULL
	,@Email VARCHAR(50) = NULL
    ,@DOB DATETIME = NULL
	,@ProfessionalMajor NVARCHAR(100) = NULL
	,@ProfessionalLevel TINYINT = NULL
	,@ApplyDate DATETIME = NULL
	,@YearsOfExperience FLOAT = NULL
	,@Resume VARCHAR(MAX) = NULL
	,@Status TINYINT = NULL
	,@InterviewDate1 DATETIME = NULL
	,@Result1 TINYINT = NULL
	,@InterviewDate2 DATETIME = NULL
	,@Result2 TINYINT = NULL
	,@JoinDate DATETIME = NULL
	,@Source TINYINT = NULL
    ,@RecruiterId INT = NULL
	,@EducationBackground NVARCHAR(500) = NULL
	,@Grade TINYINT = NULL
	,@ManagementGrade TINYINT = NULL
	,@InterviewerComment NVARCHAR(MAX) = NULL
	,@Note NVARCHAR(MAX) = NULL
	,@InterviewerIds VARCHAR(255) = NULL
	,@OfferSalary NVARCHAR(MAX) = NULL
	,@ExpectedSalary NVARCHAR(MAX) = NULL
	,@FinalComment NVARCHAR(MAX) = NULL
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231121@Lilac.Nguyen
Task: Insert Applicant.
DB: NexHRM
Original: 

Revisions:  
	- 20250310@Henry.Dinh: add management grade
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Applicant(
        [FullName]
		,[PositionId]
		,[Phone]
		,[Email]
		,[DOB]
		,[Major]
		,[ProfessionalLevel]
		,[ApplyDate]
		,[YearsOfExperience]
		,[Resume]
		,[Status]
		,[InterviewDate1]
		,[Result1]
		,[InterviewDate2]
		,[Result2]
		,[JoinDate]
		,[Source]
		,[RecruiterId]
		,[EducationBackground]
		,[Grade]
		,[ManagementGrade]
		,[InterviewerComment]
		,[Note]
		,[InterviewerIds]
		,[OfferSalary]
		,[ExpectedSalary]
		,[FinalComment]
		,[CreatedBy]
		,[CreatedDate]
		,[IsOnTop]
        )
    VALUES (
        @FullName
		,@PositionId
		,@Phone
		,@Email
		,@DOB
		,@ProfessionalMajor
		,@ProfessionalLevel
		,@ApplyDate
		,@YearsOfExperience
		,@Resume
		,@Status
		,@InterviewDate1
		,@Result1
		,@InterviewDate2
		,@Result2
		,@JoinDate
		,@Source
		,@RecruiterId
		,@EducationBackground
		,@Grade
		,@ManagementGrade
		,@InterviewerComment
		,@Note
		,@InterviewerIds
		,@OfferSalary
		,@ExpectedSalary
		,@FinalComment
		,@CreatedBy
		,dbo.fnc_GetClientDateTime()
		,0
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO
