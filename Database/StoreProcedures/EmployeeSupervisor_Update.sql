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
    20250722@Henry: Updated to work with new table structure in bodb_HRM.dbo.EmployeeSupervisor
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(), @logData NVARCHAR(MAX);
	DECLARE @ActionType INT = 3, @RequestType INT = 6; -- Approver for Leave
	DECLARE @CurrentCompanyId INT;
	
	-- Get current company ID from SystemSetting
	SELECT @CurrentCompanyId = [Value] 
	FROM dbo.SystemSetting WITH (NOLOCK) 
	WHERE [Name] = 'CompanyId'
	
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
	FROM [bodb_HRM].[dbo].[EmployeeSupervisor] AS su WITH (ROWLOCK, UPDLOCK)
	INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS ce WITH (NOLOCK) ON ce.EmpId = su.OwnerId
	WHERE su.EmpId = @EmpId
		AND su.ActionType = @ActionType
		AND su.RequestType = @RequestType
		AND ce.CompanyId = @CurrentCompanyId
		AND ISNULL(ce.IsDeleted, 0) = 0
		AND ISNULL(ce.Enabled, 1) = 1
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.OwnerId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM [bodb_HRM].[dbo].[EmployeeSupervisor] AS su WITH (ROWLOCK, UPDLOCK)
	INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS ce WITH (NOLOCK) ON ce.EmpId = su.OwnerId
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.OwnerId
	WHERE su.EmpId = @EmpId
		AND su.ActionType = @ActionType
		AND su.RequestType = @RequestType
		AND ce.CompanyId = @CurrentCompanyId
		AND ISNULL(ce.IsDeleted, 0) = 0
		AND ISNULL(ce.Enabled, 1) = 1
		AND su.IsDeleted = 1

	MERGE [bodb_HRM].[dbo].[EmployeeSupervisor] AS [target]
	USING (
		SELECT sp.ApproverId
		FROM #tblSupervisor AS sp
		INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS ce WITH (NOLOCK) ON ce.EmpId = sp.ApproverId
		WHERE ce.CompanyId = @CurrentCompanyId
			AND ISNULL(ce.IsDeleted, 0) = 0
			AND ISNULL(ce.Enabled, 1) = 1
	) AS [source] ON (
				[target].EmpId = @EmpId
				AND [target].OwnerId = [source].ApproverId
				AND [target].ActionType = @ActionType
				AND [target].RequestType = @RequestType
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			OwnerId
			,EmpId
			,ActionType
			,RequestType
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			,OrderNo
			,OrderCode
			)
		VALUES (
			[source].ApproverId
			,@EmpId
			,@ActionType
			,@RequestType
			,@ModifiedBy
			,@ModifiedDate
			,0
			,1
			,NULL
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES(9, dbo.fnc_GetClientDateTime(), 'Update Personal Flow Setting', @logData, @EmpId)

	DROP TABLE #tblSupervisor;

END
GO
