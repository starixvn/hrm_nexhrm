USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[HistoryWork_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[HistoryWork_Insert] @EmpId INT
	,@AffectedDate DATETIME = NULL
	,@ModifiedBy INT = NULL
	,@JobTitleId INT = NULL
	,@GroupId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: create new working history
DB: NexHR
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

	INSERT INTO dbo.HistoryWork(
		EmpId
		,AffectedDate	
		,ModifiedBy
		,JobtitleId
		,GroupId
		,ModifiedDate
		,OldGroupId
		)
	VALUES (
		@EmpId	
		,@AffectedDate
		,@ModifiedBy
		,@JobTitleId
		,@GroupId
		,dbo.fnc_GetClientDateTime()
		,@OldGroupId
		)
END


GO
