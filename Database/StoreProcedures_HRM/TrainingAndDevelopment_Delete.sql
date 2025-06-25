USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopment_Delete]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Delete] 
	@Id INT
	,@DeletedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230417@Brian.Nguyen
Task: Delete Training And Development.
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

	SELECT @logData = '<TrainingAndDevelopment Id="'+ COALESCE(CAST(Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(Description,SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(Certificate,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(Organization,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), StartDate),SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE(CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DeletedDate="'+ COALESCE(CAST(dbo.fnc_GetClientDateTime() AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DeletedBy="'+ COALESCE(CAST(@DeletedBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) +  '" />'
		,@employeeId = EmpId
	FROM dbo.[TrainingAndDevelopment] WITH (NOLOCK)
	WHERE Id = @Id

	DELETE FROM [dbo].[TrainingAndDevelopment]
	WHERE Id = @Id

	INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
	VALUES(7, dbo.fnc_GetClientDateTime(), 'Delete Training And Development', @logData, @employeeId)

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
