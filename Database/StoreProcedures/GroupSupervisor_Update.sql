CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Update]
	@GroupId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240423@Lilac.Nguyen
Task: update approver for group.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(),  @logData NVARCHAR(MAX);

	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.GroupSupervisor WITH (NOLOCK)
	WHERE GroupId = @GroupId AND IsDeleted <> 1

	CREATE TABLE #tblSupervisor (ApproverId INT)

	INSERT INTO #tblSupervisor(
            ApproverId
            )
    SELECT val
    FROM dbo.fnc_strSplit(@ApproverIds, ',')

	UPDATE su
	SET su.IsDeleted = 1
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	WHERE su.GroupId = @GroupId
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.ApproverId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.ApproverId
	WHERE su.GroupId = @GroupId
		AND su.IsDeleted = 1

	MERGE dbo.GroupSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].GroupId = @GroupId
				AND [target].ApproverId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			GroupId
			,ApproverId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		VALUES (
			@GroupId
			,[source].ApproverId
			,@ModifiedBy
			,@ModifiedDate
			,0
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES (10, dbo.fnc_GetClientDateTime(), 'Update Team Flow Setting', @logData, 0)

	DROP TABLE #tblSupervisor;
END
GO
