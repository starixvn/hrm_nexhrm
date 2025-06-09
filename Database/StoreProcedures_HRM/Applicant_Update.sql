USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_Update]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_Update] 
    @Id INT
    ,@FullName NVARCHAR(100)
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
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231127@Lilac.Nguyen
Task: Update Applicant.
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

    UPDATE dbo.Applicant WITH (ROWLOCK, UPDLOCK)
    SET 
         [FullName] = @FullName
		,[PositionId] = @PositionId
		,[Phone] = @Phone
		,[Email] = @Email
		,[DOB] = @DOB
		,[Major] = @ProfessionalMajor
		,[ProfessionalLevel] = @ProfessionalLevel
		,[ApplyDate] = @ApplyDate
		,[YearsOfExperience] = @YearsOfExperience
		,[Resume] = @Resume
		,[Status] = @Status
		,[InterviewDate1] = @InterviewDate1
		,[Result1] = @Result1
		,[InterviewDate2] = @InterviewDate2
		,[Result2] = @Result2
		,[JoinDate]	= @JoinDate
		,[Source] = @Source
		,[RecruiterId] = @RecruiterId
		,[EducationBackground] = @EducationBackground
		,[Grade] = @Grade
		,[ManagementGrade] = @ManagementGrade
		,[InterviewerComment] = @InterviewerComment
		,[Note] = @Note
		,[InterviewerIds] = @InterviewerIds
		,[OfferSalary] = @OfferSalary
		,[ExpectedSalary] = @ExpectedSalary
		,[FinalComment]	= @FinalComment
    WHERE ApplicantId = @Id

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update applicant failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO
