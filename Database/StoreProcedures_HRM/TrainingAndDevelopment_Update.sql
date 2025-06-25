USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopment_Update]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Update] 
	@Id INT
    ,@ConductedBy NVARCHAR(100) = NULL
    ,@Publish NVARCHAR(20)
    ,@TypeId Int
    ,@Name NVARCHAR(200)
    ,@Organization NVARCHAR(200)
	,@Description NVARCHAR(500) = NULL
    ,@StartDate DATETIME = NULL
    ,@EndDate DATETIME = NULL
    ,@ExamDate DATETIME = NULL
    ,@ExpirationDate DATETIME = NULL
    ,@ModifyBy INT
	,@Certificate VARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230417@Brian.Nguyen
Task: Update Training And Development
DB: NexHRM
Original: 

Revisions:  
- 20241202@Lilac: Add certificate upload
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE 
		@logData VARCHAR(MAX)
		,@employeeId INT

	SELECT @logData = '<TrainingAndDevelopment_Old Id="'+ COALESCE(CAST(Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(Description,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(Organization,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), StartDate),SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(Certificate,SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE( CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) + '" />' +
		'<TrainingAndDevelopment_new Id="'+ COALESCE(CAST(@Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(@Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(@ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(@Description,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(@TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(@Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(@Organization,SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(@Certificate,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), @StartDate),SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), @EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), @ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), @ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE(CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(@ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) +  '" />'
		,@employeeId = EmpId
		FROM dbo.[TrainingAndDevelopment] WITH (NOLOCK)
		WHERE Id = @Id

	UPDATE [dbo].[TrainingAndDevelopment] WITH (ROWLOCK, UPDLOCK)
	   SET [ConductedBy] = @ConductedBy
		  ,[TypeId] = @TypeId
		  ,[Name] = @Name
		  ,[Organization] = @Organization
		  ,[StartDate] = @StartDate
		  ,[EndDate] = @EndDate
		  ,[ExamDate] = @ExamDate
		  ,[ExpirationDate] = @ExpirationDate
		  ,[ModifyDate] = dbo.fnc_GetClientDateTime()
		  ,[ModifyBy] = @ModifyBy
		  ,[Description] = @Description
		  ,[Publish] = @Publish
		  ,[Certificate] = @Certificate
	 WHERE Id = @Id

	INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
	VALUES(6, dbo.fnc_GetClientDateTime(), 'Update Training And Development', @logData, @employeeId)

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
