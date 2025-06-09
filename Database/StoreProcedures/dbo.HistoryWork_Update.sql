USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[HistoryWork_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[HistoryWork_Update]
	 @Id INT
	,@EmpId INT
	,@AffectedDate DATETIME = NULL
	,@JobTitleId INT = NULL
	,@GroupId INT = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: Update History Work
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN	
	SET NOCOUNT ON;

	DECLARE @OldGroupId INT

	SELECT @OldGroupId = GroupId
	FROM dbo.Employee WITH (NOLOCK)
	WHERE Id = @EmpId

	IF ( @GroupId = @OldGroupId )
    BEGIN
         SELECT @OldGroupId = NULL
    END

	UPDATE [dbo].[HistoryWork] WITH (ROWLOCK, UPDLOCK)
	SET 
	AffectedDate = @AffectedDate
	,ModifiedDate = dbo.fnc_GetClientDateTime()	
	,JobTitleId = @JobTitleId
	,GroupId = @GroupId
	,OldGroupId = @OldGroupId
	,ModifiedBy = @ModifiedBy 
	WHERE Id = @Id

	 IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update History Work failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO
