USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeSupervisor_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Update]
	@EmpId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240422@Lilac.Nguyen
Task: update approver.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE(), @logData NVARCHAR(MAX);
	
	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.EmployeeSupervisor WITH (NOLOCK)
	WHERE EmpId = @EmpId AND IsDeleted <> 1

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
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	WHERE su.EmpId = @EmpId
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
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.ApproverId
	WHERE su.EmpId = @EmpId
		AND su.IsDeleted = 1

	MERGE dbo.EmployeeSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].EmpId = @EmpId
				AND [target].ApproverId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			EmpId
			,ApproverId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		VALUES (
			@EmpId
			,[source].ApproverId
			,@ModifiedBy
			,@ModifiedDate
			,0
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES(9, GETDATE(), 'Update Personal Flow Setting', @logData, @EmpId)

	DROP TABLE #tblSupervisor;

END
GO
