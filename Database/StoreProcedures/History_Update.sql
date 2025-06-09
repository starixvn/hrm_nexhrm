USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[History_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Update] 
	@Id INT
	,@EmpId	INT
    ,@GroupId INT = NULL
	,@JobTitleId INT = NULL
	,@JobGradeId INT = NULL
    ,@AffectedDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Update a history
DB: NexHRM
Original: 

Revisions:  
	- Remove check different from the current value for user init data
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
	--DECLARE @CurrentGroupId INT, @CurrentJobTitleId INT, @CurrentJobGradeId INT;

	--SELECT 
	--	@CurrentGroupId = GroupId,
	--	@CurrentJobTitleId = JobTitleId
	--FROM dbo.Employee WITH (NOLOCK)
	--WHERE Id = @EmpId;

	--SELECT @CurrentJobGradeId = JobGradeId
	--FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
	--WHERE EmpId = @EmpId AND IsCurrentUsed = 1

	--IF @GroupId = @CurrentGroupId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Team must be different from the current Team!';
	--	RETURN;
	--END

	--IF @JobTitleId = @CurrentJobTitleId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Title must be different from the current Job Title!';
	--	RETURN;
	--END

	--IF @JobGradeId = @CurrentJobGradeId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Grade must be different from the current Job Grade!';
	--	RETURN;
	--END

    UPDATE dbo.HistoryWork WITH (ROWLOCK, UPDLOCK)
    SET 
		AffectedDate = @AffectedDate
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE Id = @Id

	UPDATE dbo.HistoryWorkDetail WITH (ROWLOCK, UPDLOCK)
    SET 
		GroupId = @GroupId
        ,JobTitleId = @JobTitleId
        ,JobGradeId = @JobGradeId
    WHERE HistoryWorkId = @Id
  
    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update history failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0 ,@ErrMsg = ''
    END
END
GO
