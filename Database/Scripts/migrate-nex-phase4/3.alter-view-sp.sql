USE [NexHRM]
GO

DELETE FROM [NexHRM].[dbo].[SystemSetting]
WHERE [Name] = 'AutoApproveRequest'

DELETE FROM [LumiHRM].[dbo].[SystemSetting]
WHERE [Name] = 'AutoApproveRequest'

DELETE FROM [NexVisionHRM].[dbo].[SystemSetting]
WHERE [Name] = 'AutoApproveRequest'

GO

CREATE OR ALTER VIEW [dbo].[Employee]
AS

SELECT e.EmpId as Id, e.OldEmpId as OldId, e.Code, ec.DomainUsername, e.FullName, (CASE WHEN ec.NickName2 IS NULL THEN e.NickName ELSE ec.NickName2 END) AS NickName
    ,e.DOB, ec.Email, e.JobTitleId, ec.[Enabled], e.Phone, e.Gender, ec.GroupId, ec.Roles
    ,ec.CompanyDate as JoinedDate, ec.ConfirmedDate, ec.ResignedDate, e.Skype, e.Yahoo, e.Marriage
    ,e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ec.IsDeleted, e.PermanentAddress, e.CurrentAddress, e.IdCardNumber, e.PassportNo
    ,e.PassportExp, e.GeoMapLatitude, e.GeoMapLongtitude, e.DistanceToCompany, e.EmployeeCard, e.DateOfIDCard as IssuedDate, e.PlaceOfIDCard as IssuedPlace, ec.IsSpecialUser, e.JoinedDate as GroupDate
    ,e.PlaceOfBirth, e.ContactName, e.UserRelationship, e.ContactPhone
    ,e.ContactAddress, e.EducationBackground, e.Telegram, e.SocialInsuranceNo, ec.CompanyId, ec.SalaryGroupId, ec.AutoApproveRequest
FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId 
    INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
GO

CREATE OR ALTER VIEW [dbo].[EmployeeSupervisor]
AS
SELECT 
    [OwnerId] AS ApproverId,
    es.[EmpId],
    es.[ModifiedBy],
    es.[ModifiedDate],
    es.[IsDeleted],
    c.Name AS OuterCompanyName
FROM [bodb_HRM].[dbo].[EmployeeSupervisor] AS es WITH (NOLOCK)
INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS cea WITH (NOLOCK) 
    ON cea.EmpId = es.OwnerId AND ISNULL(cea.IsSpecialUser, 0) = 0 AND ISNULL(cea.IsDeleted, 0) = 0 AND ISNULL(cea.Enabled, 1) = 1
INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS cee WITH (NOLOCK) 
    ON cee.EmpId = es.EmpId AND ISNULL(cee.IsSpecialUser, 0) = 0 AND ISNULL(cee.IsDeleted, 0) = 0 AND ISNULL(cee.Enabled, 1) = 1
LEFT JOIN [bodb_HRM].[dbo].[Company] AS c WITH (NOLOCK) 
    ON c.CompanyId = cea.CompanyId 
    AND c.CompanyId != (SELECT [Value] FROM [dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
WHERE es.ActionType = 3 AND es.RequestType = 6 
AND cee.CompanyId = (SELECT [Value] FROM [dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Get]
AS
/*
Created: 20240426@Lilac.Nguyen
Task: Get Employee Supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
   
		SELECT
            es.EmpId,
			em.NickName,
			em.FullName,
			STRING_AGG(
				e.NickName + 
				CASE 
					WHEN es.OuterCompanyName IS NOT NULL THEN ' (' + es.OuterCompanyName + ')'
					ELSE ''
				END,
				', '
			) AS Approvers
        FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			LEFT JOIN dbo.Employee AS em WITH (NOLOCK) ON em.Id = es.EmpId
			LEFT JOIN bodb_HRM.dbo.Employee AS e ON e.EmpId = es.ApproverId
        WHERE ISNULL(es.IsDeleted, 0) = 0
		GROUP BY es.EmpId, em.NickName, em.FullName
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Insert]
	@EmpId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240426@Lilac.Nguyen
Task: Insert approver for employee.
DB: NexHRM
Original: 

Revisions:  
    20250722@Henry: Updated to work with new table structure in bodb_HRM.dbo.EmployeeSupervisor
Param's Explanation:

*/

BEGIN
    SET NOCOUNT ON;
	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(), @logData NVARCHAR(MAX);
	DECLARE @ActionType INT = 3, @RequestType INT = 6; -- Approver for Leave
	
	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.EmployeeSupervisor WITH (NOLOCK)
	WHERE EmpId = @EmpId AND IsDeleted <> 1

    IF (
            EXISTS (
                SELECT 1
                FROM [bodb_HRM].[dbo].[EmployeeSupervisor] WITH (NOLOCK)
                WHERE EmpId = @EmpId AND ActionType = @ActionType AND RequestType = @RequestType AND ISNULL(IsDeleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Employee has already exists'

        RETURN
    END

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
	WHERE su.EmpId = @EmpId
		AND su.ActionType = @ActionType
		AND su.RequestType = @RequestType
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
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.OwnerId
	WHERE su.EmpId = @EmpId
		AND su.ActionType = @ActionType
		AND su.RequestType = @RequestType
		AND su.IsDeleted = 1

    MERGE [bodb_HRM].[dbo].[EmployeeSupervisor] AS [target]
	USING #tblSupervisor AS [source] ON (
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
	VALUES(8, dbo.fnc_GetClientDateTime(), 'Insert Personal Flow Setting', @logData, @EmpId)

	DROP TABLE #tblSupervisor;

    SELECT @ErrCode = 0, @ErrMsg = ''
END
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

CREATE OR ALTER PROCEDURE [dbo].[Employee_Transfer] @EmployeeId INT
	,@LastWorkingDate DATE
	,@AffectedDate DATE
	,@ToGroupId INT
	,@ShiftId INT
	,@IsTransferLateInEarlyOut BIT
	,@IsTransferAnnualLeaveBalance BIT
	,@IsTransferNexBalance BIT
	,@IsSpecialUser BIT = 0
	,@ModifiedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210901@Quinn.Vo
Task: transfer employee
DB: NexHRM
Original: 

Revisions:
     - 20250620@Henry: migrate to bodb_HRM
     - 20250717@Henry: Added transaction support for data integrity + clear old data
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

	DECLARE @FromDomain VARCHAR(20)
		,@ToDomain VARCHAR(20)
		,@FromDomainEmail VARCHAR(20)
		,@ToDomainEmail VARCHAR(20)
		,@FromCompanyId INT
		,@ToCompanyId INT
		,@Todate DATETIME
		,@NexBalance INT = 0
		
	DECLARE @CreditDays NUMERIC(18, 1)
		,@LateInEarlyOut INT

	DECLARE @NexCompanyId INT = 1
		,@LumiCompanyId INT = 2

	SELECT @FromDomain = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
	SELECT @FromDomainEmail = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'
	SELECT @FromCompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

	IF (@FromCompanyId = @NexCompanyId)
	BEGIN
		SET @ToCompanyId = @LumiCompanyId
		SELECT @ToDomain = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		DELETE FROM LumiHRM.dbo.EmployeeGroup_Employee WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.EmployeeShift WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.LeaveTrans WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.LateInEarlyOutTrans WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.LateInEarlyOutYearly WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.OvertimeTrans WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.OvertimeYearly WHERE EmpID = @EmployeeId;
		DELETE FROM LumiHRM.dbo.EmployeeCalendar WHERE EmpID = @EmployeeId;

		DELETE FROM LumiHRM.dbo.EmployeeAttendance WHERE EmpId = @EmployeeId;
		DELETE FROM LumiHRM.dbo.WorkingTimeRequest_history WHERE EmpId = @EmployeeId;
		DELETE FROM LumiHRM.dbo.WorkingTimeRequest WHERE EmpId = @EmployeeId;
		DELETE FROM LumiHRM.dbo.LeaveConversion WHERE EmpId = @EmployeeId;
		DELETE FROM LumiHRM.dbo.LogReachedWorkingTime WHERE EmpId = @EmployeeId;
		DELETE FROM LumiHRM.dbo.TimeRecord WHERE EmpId = @EmployeeId;

		IF EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.CompanyDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
				,a.SalaryGroupId = b.SalaryGroupId
			FROM bodb_HRM.dbo.CompanyEmployee AS a 
			INNER JOIN bodb_HRM.dbo.CompanyEmployee AS b ON a.EmpId = b.EmpId AND b.CompanyId = @FromCompanyId
			WHERE a.EmpId = @employeeId AND a.CompanyId = @ToCompanyId
		END
		ELSE
		BEGIN
			INSERT INTO bodb_HRM.dbo.CompanyEmployee (
				EmpId
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,CompanyDate
				,ConfirmedDate
				,SalaryGroupId
				,CompanyId
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
				,SalaryGroupId
				,@ToCompanyId
			FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
			WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
		END

		INSERT INTO LumiHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO LumiHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE LumiHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO LumiHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = CASE 
				WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
				ELSE 0
				END
			FROM dbo.LateInEarlyOutYearly la WITH (NOLOCK)
			LEFT JOIN dbo.OvertimeYearly oty WITH (NOLOCK) ON la.EmpId = oty.EmpId AND oty.[Year] = YEAR(@AffectedDate)
			WHERE la.EmpId = @EmployeeId AND la.[Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO LumiHRM.dbo.LateInEarlyOutTrans(
					[EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO LumiHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
				WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
				ELSE 0
				END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO LumiHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO LumiHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END
	ELSE IF (@FromCompanyId = @LumiCompanyId)
	BEGIN
		SET @ToCompanyId = @NexCompanyId
		SELECT @ToDomain = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		DELETE FROM NexHRM.dbo.EmployeeGroup_Employee WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.EmployeeShift WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.LeaveTrans WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.LateInEarlyOutTrans WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.LateInEarlyOutYearly WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.OvertimeTrans WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.OvertimeYearly WHERE EmpID = @EmployeeId;
		DELETE FROM NexHRM.dbo.EmployeeCalendar WHERE EmpID = @EmployeeId;

		DELETE FROM NexHRM.dbo.EmployeeAttendance WHERE EmpId = @EmployeeId;
		DELETE FROM NexHRM.dbo.WorkingTimeRequest_history WHERE EmpId = @EmployeeId;
		DELETE FROM NexHRM.dbo.WorkingTimeRequest WHERE EmpId = @EmployeeId;
		DELETE FROM NexHRM.dbo.LeaveConversion WHERE EmpId = @EmployeeId;
		DELETE FROM NexHRM.dbo.LogReachedWorkingTime WHERE EmpId = @EmployeeId;
		DELETE FROM NexHRM.dbo.TimeRecord WHERE EmpId = @EmployeeId;

		IF EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.CompanyDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM bodb_HRM.dbo.CompanyEmployee AS a 
			INNER JOIN bodb_HRM.dbo.CompanyEmployee AS b ON a.EmpId = b.EmpId AND b.CompanyId = @FromCompanyId
			WHERE a.EmpId = @employeeId AND a.CompanyId = @ToCompanyId
		END
		ELSE
		BEGIN
			INSERT INTO bodb_HRM.dbo.CompanyEmployee (
				EmpId
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,CompanyDate
				,ConfirmedDate
				,SalaryGroupId
				,CompanyId
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
				,SalaryGroupId
				,@ToCompanyId
			FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
			WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
		END

		INSERT INTO NexHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO NexHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE NexHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO NexHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = CASE 
				WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
				ELSE 0
				END
			FROM dbo.LateInEarlyOutYearly la WITH (NOLOCK)
			LEFT JOIN dbo.OvertimeYearly oty WITH (NOLOCK) ON la.EmpId = oty.EmpId AND oty.[Year] = YEAR(@AffectedDate)
			WHERE la.EmpId = @EmployeeId AND la.[Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO NexHRM.dbo.LateInEarlyOutTrans(
				  [EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO NexHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
				WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
				ELSE 0
				END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO NexHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO NexHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END

	UPDATE bodb_HRM.dbo.CompanyEmployee WITH (ROWLOCK, UPDLOCK)
	SET [Enabled] = 0
		,ResignedDate = @LastWorkingDate
	WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId

	UPDATE bodb_HRM.dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET ModifiedDate = dbo.fnc_GetClientDateTime()
		,ModifiedBy = @ModifiedBy
	WHERE EmpId = @EmployeeId

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Employee transfer failed: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetById] @EmployeeId INT
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by id
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	20252602@Henry.Dinh: Add Management Grade
    20250723@Henry.Dinh: Add AutoApproveRequest
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATETIME
        ,@AnnualLeaveBalancePending NUMERIC(5, 1)
        ,@AnnualLeaveBalance NUMERIC(5, 1)
        ,@SubGroup VARCHAR(50)
		,@Approvers VARCHAR(255)

    SET @Today = dbo.fnc_GetClientDateTime()

    SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
    FROM dbo.LeaveTrans WITH (NOLOCK)
    WHERE EmpId = @EmployeeId
        AND ISNULL(Deleted, 0) = 0
        AND YEAR(FromDate) = YEAR(@Today)

    SELECT @AnnualLeaveBalancePending = SUM(ISNULL(wk.TotalDays, 0))
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE e.Id = @EmployeeId
        AND wk.RequestType = 6
        AND wk.[Status] = 1
        AND wk.LeaveTypeID IN (2, 30)
    GROUP BY e.Id

    SELECT @SubGroup = ''

    SELECT @SubGroup = @SubGroup + COALESCE(CAST(eg.GroupId AS VARCHAR(10)) + ',', '')
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
    WHERE eg.EmpID = @EmployeeId
        AND ISNULL(isMainGroup, 0) = 0
		AND ISNULL(HandleRequest, 0) = 0

    SELECT @SubGroup = CASE 
            WHEN LEN(@SubGroup) > 0 THEN LEFT(@SubGroup, LEN(@SubGroup) - 1)
            ELSE ''
            END

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[EducationBackground]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
        ,@SubGroup AS SubGroups
		,gra.JobGradeId
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.Description
		,gra.AffectedDate
		,gra.Note
        ,mgra.ManagementGradeId
		,mgra.Code AS ManagementGradeCode
		,mgra.Name AS ManagementGradeName
		,mgra.Description AS ManagementDescription
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote
        ,e.SalaryGroupId
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy]
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[IsDeleted]
        ,[AutoApproveRequest]
		,[IsSpecialUser]
		,[GroupDate]
        ,[PermanentAddress] AS [Address]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,ISNULL([MonShift], 0) AS MonShiftId
        ,ISNULL(sh2.NAME, 'Off') AS MonShiftName
        ,ISNULL([TueShift], 0) AS TueShiftId
        ,ISNULL(sh3.NAME, 'Off') AS TueShiftName
        ,ISNULL([WedShift], 0) AS WedShiftId
        ,ISNULL(sh4.NAME, 'Off') AS WedShiftName
        ,ISNULL([ThuShift], 0) AS ThuShiftId
        ,ISNULL(sh5.NAME, 'Off') AS ThuShiftName
        ,ISNULL([FriShift], 0) AS FriShiftId
        ,ISNULL(sh6.NAME, 'Off') AS FriShiftName
        ,ISNULL([SatShift], 0) AS SatShiftId
        ,ISNULL(sh7.NAME, 'Off') AS SatShiftName
        ,ISNULL([SunShift], 0) AS SunShiftId
        ,ISNULL(sh8.NAME, 'Off') AS SunShiftName
        ,LRExamDate
        ,Listening
        ,OldListening
        ,Reading
        ,OldReading
        ,SWExamDate
        ,Speaking
        ,OldSpeaking
        ,Writing
        ,OldWriting
        ,InputDate
        ,ISNULL(GroupId, 0) AS GroupId
        ,g.Code
        ,g.NAME
        ,ISNULL(e.JobTitleId, 0) AS JobTitleId
        ,jt.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        LEFT JOIN dbo.EmployeeShift AS es WITH (NOLOCK) ON e.Id = es.EmpID
        LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN HRM.dbo.TOEIC AS toeic WITH (NOLOCK) ON e.Id = toeic.EmpId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.[Shift] AS sh2 WITH (NOLOCK) ON es.MonShift = sh2.Id
        LEFT JOIN dbo.[Shift] AS sh3 WITH (NOLOCK) ON es.TueShift = sh3.Id
        LEFT JOIN dbo.[Shift] AS sh4 WITH (NOLOCK) ON es.WedShift = sh4.Id
        LEFT JOIN dbo.[Shift] AS sh5 WITH (NOLOCK) ON es.ThuShift = sh5.Id
        LEFT JOIN dbo.[Shift] AS sh6 WITH (NOLOCK) ON es.FriShift = sh6.Id
        LEFT JOIN dbo.[Shift] AS sh7 WITH (NOLOCK) ON es.SatShift = sh7.Id
        LEFT JOIN dbo.[Shift] AS sh8 WITH (NOLOCK) ON es.SunShift = sh8.Id
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.EmpId = @EmployeeId 
			  AND ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
		LEFT JOIN (
			SELECT emg.EmpId
			,emg.AffectedDate
			,emg.Note
			,mg.ManagementGradeId
			,mg.Code
			,mg.Name
			,mg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			  WHERE emg.EmpId = @EmployeeId 
			  AND emg.IsCurrentUsed = 1
			  AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId

    WHERE e.Id = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_UpdateEmployee] @employeeId INT
    ,@code VARCHAR(20) = NULL
    ,@domainUsername VARCHAR(50) = NULL
    ,@fullName NVARCHAR(50) = NULL
    ,@nickName NVARCHAR(20) = NULL
    ,@EducationBackground NVARCHAR(500) = NULL
    ,@EmployeeCard VARCHAR(11) = ''
    ,@dob DATETIME = NULL
    ,@email VARCHAR(50) = NULL
    ,@jobTitleId INT = NULL
    ,@enabled BIT = NULL
	,@isSpecialUser BIT = NULL
	,@autoApproveRequest BIT = NULL
    ,@phone VARCHAR(50) = NULL
    ,@gender TINYINT = NULL
    ,@mainGroupID INT = NULL
    ,@subGroups VARCHAR(50) = NULL
    ,@Roles VARCHAR(20) = NULL
    ,@joinedDate DATETIME = NULL
    ,@confirmedDate DATETIME = NULL
    ,@resignedDate DATETIME = NULL
    ,@skype VARCHAR(100) = NULL
    ,@yahoo VARCHAR(100) = NULL
    ,@marriage BIT = NULL
    ,@address NVARCHAR(250) = NULL
    ,@currAddress NVARCHAR(250) = NULL
    ,@idCardNo NVARCHAR(20) = NULL
    ,@issuedDate DATETIME = NULL
    ,@issuedPlace NVARCHAR(250) = NULL
    ,@passportNo NVARCHAR(35) = NULL
    ,@passportExp DATETIME = NULL
	,@socialInsuranceNo VARCHAR(50) = NULL
    ,@geoMapLatitude VARCHAR(50) = ''
    ,@geoMapLongtitude VARCHAR(50) = ''
    ,@distanceToCompany VARCHAR(11) = ''
    ,@modifiedBy INT = NULL
	,@placeOfBirth NVARCHAR(20) = NULL
	,@contactName NVARCHAR(50) = NULL
	,@userRelationship NVARCHAR(11) = NULL
	,@contactPhone VARCHAR(50) = NULL
	,@contactAddress NVARCHAR(250) = NULL
	,@groupDate DATE = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: update employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE 
		@logData NVARCHAR(MAX)
		,@xml XML
		,@Groups VARCHAR(50)

    BEGIN TRY
        BEGIN TRANSACTION;

		SELECT @Groups = 
			COALESCE(@Groups + ', ' + CAST(GroupId AS VARCHAR(10)), CAST(GroupId AS VARCHAR(10)))
		FROM dbo.EmployeeGroup_Employee 
		WHERE EmpID = @employeeId

		SELECT @logData = '<Employee_Old Code="'+ COALESCE(CAST(Code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(DomainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(FullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(NickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(EducationBackground,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(DOB AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(Email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(JobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST([Enabled] AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(IsSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'AutoApproveRequest="'+ COALESCE(CAST(AutoApproveRequest AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(Phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(Gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(GroupId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Roles="'+ COALESCE([Roles],SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(JoinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(GroupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(ConfirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(ResignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(Skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(Yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(Marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(PermanentAddress,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(CurrentAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(IdCardNumber,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(IssuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(IssuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(PassportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(PassportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(SocialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(PlaceOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(ContactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(UserRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(ContactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(ContactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(ModifiedBy AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Groups="'+ COALESCE(@Groups,SPACE(1)) + '" />' + 

		'<Employee_new Code="'+ COALESCE(CAST(@code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(@domainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(@fullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(@nickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(@EducationBackground,SPACE(1)) + '" ' +
		'EmployeeCard="'+ COALESCE(@EmployeeCard,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(@dob AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(@email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(@jobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST(@enabled AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(@isSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'AutoApproveRequest="'+ COALESCE(CAST(@autoApproveRequest AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(@phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(@gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(@mainGroupID AS VARCHAR(50)),SPACE(1)) + '" ' +

		'Roles="'+ COALESCE(@Roles,SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(@joinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(@groupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(@confirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(@resignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(@skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(@yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(@marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(@address,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(@currAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(@idCardNo,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(@issuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(@issuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(@passportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(@passportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(@socialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(@placeOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(@contactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(@userRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(@contactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(@contactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(@modifiedBy AS VARCHAR(11)),SPACE(1)) + '" />'

		FROM dbo.Employee WITH (NOLOCK)
		WHERE Id = @employeeId

        UPDATE bodb_HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET Code = @code
            ,FullName = @fullName
            ,NickName = @nickName
            ,EmployeeCard = @EmployeeCard
            ,DOB = @dob
            ,JobTitleId = @jobTitleId
            ,Phone = @phone
            ,Gender = @gender
            ,Skype = @skype
            ,Yahoo = @yahoo
            ,Marriage = @marriage
            ,PermanentAddress = @address
            ,CurrentAddress = @currAddress
            ,IdCardNumber = @idCardNo
            ,DateOfIDCard = @issuedDate
            ,PlaceOfIDCard = @issuedPlace
            ,PassportNo = @passportNo
            ,PassportExp = @passportExp
			,SocialInsuranceNo = @socialInsuranceNo
            ,GeoMapLatitude = @geoMapLatitude
            ,GeoMapLongtitude = @geoMapLongtitude
            ,DistanceToCompany = @distanceToCompany
			,PlaceOfBirth = @placeOfBirth
			,ContactName = @contactName
			,UserRelationship = @userRelationship
			,ContactPhone = @contactPhone
			,ContactAddress = @contactAddress
			,JoinedDate = @groupDate
            ,ModifiedDate = GETDATE()
            ,ModifiedBy = @modifiedBy
			,EducationBackground = @EducationBackground
        WHERE EmpId = @employeeId

		DECLARE @CompanyId int = (SELECT [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId')

		UPDATE bodb_HRM.dbo.CompanyEmployee
		SET DomainUsername = @domainUsername
            ,Email = @email
			,IsSpecialUser = @isSpecialUser
			,[Enabled] = @enabled
            ,GroupId = @mainGroupID
            ,Roles = @roles
            ,CompanyDate = @joinedDate
            ,ConfirmedDate = @confirmedDate
            ,ResignedDate = @resignedDate
			,AutoApproveRequest = @autoApproveRequest
		WHERE EmpId = @employeeId
		AND CompanyId = @CompanyId

		-- UPDATE dbo.EmployeeExt WITH (UPDLOCK, ROWLOCK)
        -- SET DomainUsername = @domainUsername
        --     ,Email = @email
		-- 	,IsSpecialUser = @isSpecialUser
		-- 	,Enabled = @enabled
        --     ,GroupId = @mainGroupID
        --     ,Roles = @roles
        --     ,JoinedDate = @joinedDate
        --     ,ConfirmedDate = @confirmedDate
        --     ,ResignedDate = @resignedDate
        -- WHERE Id = @employeeId

		INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
		VALUES(5, GETDATE(), 'Update Employee', @logData, @employeeId)

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Employee unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Insert] @id BIGINT OUT
	,@EmpId INT
	,@RequestType SMALLINT
	,@IsPlanned BIT
	,@Remark NVARCHAR(2000) = NULL
	,@FromDate DATETIME
	,@FromShift INT = NULL
	,@ToDate DATETIME = NULL
	,@ToShift INT = NULL
	,@LeaveTypeID INT = NULL
	,@TotalDays NUMERIC(5, 1) = NULL
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@IsCrossDate BIT = 0
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
	Created: 2012/04/18@Dan
	Task: insert a request form
	DB: NexHR
	Original: 

	Revisions:    
		- 20250507@Henry.Dinh: update HR code to HR-ADM
	Param's Explanation:
*/
BEGIN
	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT

	DECLARE @HRGroupId INT;
	DECLARE @AutoApproveRequest BIT;

	SELECT @AutoApproveRequest = e.AutoApproveRequest
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.Id = @EmpId

	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.GroupId END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN bodb_HRM.dbo.[Group] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'

	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

	IF (@RequestType IN (3, 4))
	BEGIN
		IF (DATEDIFF(d, @FromDate, '2015-07-31') < 0)
		BEGIN
			SELECT @ErrCode = 104
				,@ErrMsg = 'You can not request Late in/Early out later than 31 July, 2015.'

			RETURN;
		END
	END

	IF (@RequestType IN (1, 2))
	BEGIN
		IF (@FromDate IS NULL)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = 'Missing check in/out request has the invalid parameter'

			RETURN
		END

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.WorkingTimeRequest WITH (NOLOCK)
					WHERE empid = @empid
						AND RequestType = @RequestType
						AND STATUS IN (1, 2)
						AND DATEDIFF(d, fromdate, @FromDate) = 0
					)
				)
		BEGIN
			SELECT @ErrCode = 106
				,@ErrMsg = 'There is another request in the same date.'

			RETURN
		END

		-- IF (NOT EXISTS (SELECT 1 FROM dbo.Employee WHERE Id = @CreatedBy AND (Roles = '4' OR (Roles like '%3%'))))
		-- BEGIN
		IF (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						--AND (cl.ShiftId = 14 OR cl.ShiftId IS NULL)
						--AND DATEPART(DW, @FromDate) IN (1, 7)
						AND IsHoliday = 0
						AND IsOffDay = 0
					)
				)
		BEGIN
			IF (
					EXISTS (
						SELECT COUNT(1)
						FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
							AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
						WHERE wk.EmpId = @empid
							AND wk.RequestType IN (1, 2)
							AND wk.[Status] IN (1, 2)
							AND DATEDIFF(M, wk.FromDate, @FromDate) = 0
							AND cl.IsHoliday <> 1
							AND cl.IsOffDay <> 1
						HAVING COUNT(1) >= 2
						)
					)
			BEGIN
				IF (
						NOT EXISTS (
							SELECT 1
							FROM dbo.Employee WITH (NOLOCK)
							WHERE Id = @CreatedBy
								AND GroupId = @HRGroupId
							)
						) -- HR
				BEGIN
					SELECT @ErrCode = 106
						,@ErrMsg = 'The maximum request in a month has reached. You can contact HR for more information.'

					RETURN
				END
				ELSE
				BEGIN
					IF (
							NOT EXISTS (
								SELECT 1
								FROM dbo.Employee WITH (NOLOCK)
								WHERE Id = @EmpId
									AND GroupId = @HRGroupId
								)
							)
					BEGIN
						SELECT @ErrCode = - 1
					END
				END
			END
		END

		-- END
	END
	ELSE IF (@RequestType = 6)
	BEGIN
		IF (
				@FromDate IS NULL
				OR @ToDate IS NULL
				OR @LeaveTypeID IS NULL
				OR ISNULL(@TotalDays, 0) <= 0
				)
		BEGIN
			SELECT @ErrCode = 105
				,@ErrMsg = 'Leave request has the invalid parameter'

			RETURN
		END

		-- check range year
		IF (
				(
					@LeaveTypeID = @annualLeaveType
					OR @LeaveTypeID = @callInLeaveType
					)
				AND YEAR(@FromDate) <> YEAR(@ToDate)
				)
		BEGIN
			SELECT @ErrCode = 112
				,@ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

			RETURN;
		END

		IF (
				(@TotalDays = 0.5 AND
				(
				EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(MI, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
								AND RequestType = @RequestType
								AND TotalDays <> 0.5
								AND STATUS IN (1, 2)
								AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(DD, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(DD, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(DD, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(DD, ToDate, @ToDate) >= 0
									)
						)
					)
				)				
			)
			OR (
				@TotalDays <> 0.5
				AND EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND RequestType = @RequestType
							AND STATUS IN (1, 2)
							AND (
								(
									DATEDIFF(DD, FromDate, @FromDate) >= 0
									AND DATEDIFF(DD, @FromDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, FromDate, @ToDate) >= 0
									AND DATEDIFF(DD, @ToDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, FromDate) >= 0
									AND DATEDIFF(DD, FromDate, @ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, ToDate) >= 0
									AND DATEDIFF(DD, ToDate, @ToDate) >= 0
								)
						)
					)
				)
			)
		BEGIN
			SELECT @ErrCode = 107
				,@ErrMsg = 'There is another request in the same range'

			RETURN
		END

		-- check the total day is valid
		IF (
				@TotalDays < 1
				AND DATEDIFF(d, @FromDate, @ToDate) <> 0
				)
		BEGIN
			SELECT @ErrCode = 109
				,@ErrMsg = 'The total days is not correct'

			RETURN
		END
		ELSE
		BEGIN
			IF (
					(@TotalDays >= 1
					AND @TotalDays <> (
						SELECT COUNT(1)
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate BETWEEN @FromDate
								AND @ToDate
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND IsLeaveDay = 0
						))
					OR (@TotalDays = 0.5 AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate = CONVERT(DATE, @FromDate) AND WorkDate = CONVERT(DATE, @ToDate)
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND ISNULL(LeaveDay, 0) <> 1
					))
				)
			BEGIN
				SELECT @ErrCode = 110
					,@ErrMsg = 'The total days is not correct'

				RETURN
			END
		END
		
		--IF(@CreatedBy = @EmpId 
		--	OR NOT EXISTS (
		--	SELECT Id 
		--	FROM dbo.Employee WITH (NOLOCK) 
		--	WHERE Id = @CreatedBy AND GroupId = @HRGroupId)
		--)
		--BEGIN
		--	IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		--	BEGIN
		--		SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, null)

		--		IF(@IsValidLeaveDay = 0)
		--		BEGIN
		--			SELECT @ErrCode = 111
		--			,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

		--			RETURN
		--		END
		--	END
		--END

		-- check the total day is not greater than annual leave balance
		SELECT @TotalPedingDays = SUM(TotalDays)
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND [Status] = 1
			AND RequestType = 6
			AND (
				LeaveTypeID = 2
				OR LeaveTypeID = 30
				)
			AND YEAR(FromDate) = YEAR(@FromDate)

		SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND ISNULL(Deleted, 0) = 0
			AND YEAR(FromDate) = YEAR(@FromDate)

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.LeaveType
					WHERE Id = @LeaveTypeID
						AND IsALDeduction = 1
					)
				AND (@TotalDays + ISNULL(@TotalPedingDays, 0)) > ISNULL(@AnnualLeaveBalance, 0)
				)
		BEGIN
			SELECT @ErrCode = 111
				,@ErrMsg = 'You have not enough annual leave days to request'

			RETURN
		END

		-- Annual leave 2
		IF (@LeaveTypeID = @annualLeaveType)
		BEGIN
			IF (
					(
						@TotalDays = 0.5
						AND DATEDIFF(D, @FromDate, dbo.fnc_GetClientDateTime()) > 0
						)
					OR (
						@TotalDays = 1
						AND 1 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 2
						AND @TotalDays < 5
						AND 2 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 5
						AND 5 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					)
			BEGIN
				SELECT @ErrCode = 112
					,@ErrMsg = 'Request must be made in advance, in accordance with the policy established.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @callInLeaveType)
		BEGIN
			IF (
					NOT EXISTS (
						SELECT 1
						FROM dbo.Employee WITH (NOLOCK)
						WHERE id = @CreatedBy
							AND (
								Roles = '4'
								OR (Roles LIKE '%1%')
								OR (Roles LIKE '%2%')
								)
						)
					)
			BEGIN
				SELECT @ErrCode = 114
					,@ErrMsg = 'This leave type must be submitted by Leader on behalf of the employee.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @NEXLeaveType)
		BEGIN
			DECLARE @NEXBalance INT
				,@NEXLeavePending NUMERIC(5, 1)

			SELECT @NEXLeavePending = SUM(TotalDays)
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND [Status] = 1
				AND RequestType = 6
				AND LeaveTypeID = @NEXLeaveType

			SELECT @NEXBalance = (oty.[Minutes] - ISNULL(la.[Minutes], 0))
			FROM dbo.OvertimeYearly AS oty WITH (NOLOCK)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON oty.EmpId = la.EmpId
				AND oty.[Year] = la.[Year]
			WHERE oty.EmpId = @EmpId
				AND oty.[Year] = YEAR(@FromDate)

			IF (ISNULL(@NEXBalance, 0) - (ISNULL(@NEXLeavePending, 0) * 480) < (@TotalDays * 480))
			BEGIN
				SELECT @ErrCode = 115
					,@ErrMsg = 'You have not enough NEX days to request.'

				RETURN
			END
		END
	END

	INSERT INTO dbo.WorkingTimeRequest (
		STATUS
		,EmpId
		,RequestType
		,IsPlanned
		,Remark
		,CreatedDate
		,CreatedBy
		,ModifiedDate
		,ModifiedBy
		,FromDate
		,FromShift
		,ToDate
		,ToShift
		,LeaveTypeID
		,TotalDays
		,LateInMins
		,EarlyOutMins
		,IsCrossDate
		)
	SELECT 
		1
		,-- pending status as default
		@EmpId
		,@RequestType
		,@IsPlanned
		,@Remark
		,@CreatedDate
		,@CreatedBy
		,@CreatedDate
		,@CreatedBy
		,@FromDate
		,@FromShift
		,@ToDate
		,@ToShift
		,@LeaveTypeID
		,@TotalDays
		,@LateInMins
		,@EarlyOutMins
		,@IsCrossDate
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.WorkingTimeRequest WITH (NOLOCK) 
		WHERE EmpId = @EmpId
			AND Status = 1
			AND RequestType = @RequestType
			AND FromDate = @FromDate
			AND ToDate = @ToDate
	)

	SELECT @id = cast(SCOPE_IDENTITY() AS BIGINT)

	EXEC [WorkingTimeRequest_WriteHistory] @id

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@CreatedBy

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
	END

	IF (
			NOT EXISTS (
				SELECT 
					CASE WHEN es.ApproverId IS NULL THEN gs.ApproverId ELSE es.ApproverId END
				FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.GroupId = gs.GroupId AND gs.IsDeleted = 0 AND e.Id <> gs.ApproverId
				LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON e.Id = es.EmpId AND es.IsDeleted = 0 AND e.Id <> es.ApproverId
				WHERE e.Id = @empId
					AND (gs.ApproverId IS NOT NULL OR es.ApproverId IS NOT NULL))
			OR @AutoApproveRequest = 1
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = -1
			,@ApprovedDate = @CreatedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Update]
    @Id bigint,
    @IsPlanned bit,
    @Remark nvarchar(2000)= null,
    @FromDate datetime, 
    @FromShift int = null, 
    @ToDate datetime = null, 
    @ToShift int = null,
    @LeaveTypeID int = null, 
    @TotalDays numeric(5,1) = null, 
    @LateInMins int = null,
    @EarlyOutMins int = null,
	@IsCrossDate BIT = 0,
    @ModifiedDate datetime, 
    @ModifiedBy int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
    Created: 2012/05/08@Dan
    Task: update a request form
    DB: NexHR
    Original: 

    Revisions:
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:
*/

BEGIN
    DECLARE @empId INT, @requestType INT,@Status INT

	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT
		,@OldLeaveTypeId INT
		,@OldFromDate DATETIME
		,@OldToDate DATETIME
		,@OldTotalDays NUMERIC(5, 1)

	DECLARE @HRGroupId INT;
	DECLARE @AutoApproveRequest BIT;

	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.GroupId END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN bodb_HRM.dbo.[Group] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'


	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

    SELECT @empId = EmpId
		,@requestType = RequestType
		,@Status = [Status]
		,@OldFromDate = FromDate
		,@OldToDate = ToDate
		,@OldLeaveTypeId = LeaveTypeID
		,@OldTotalDays = TotalDays
    FROM dbo.WorkingTimeRequest WITH (NOLOCK)
    WHERE Id = @Id

	SELECT @AutoApproveRequest = e.AutoApproveRequest
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.Id = @EmpId

	IF (@RequestType IN (1, 2))
	BEGIN
		IF (@FromDate IS NULL)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = 'Missing check in/out request has the invalid parameter'

			RETURN
		END

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.WorkingTimeRequest WITH (NOLOCK)
					WHERE Id <> @Id
						AND empid = @empid
						AND RequestType = @RequestType
						AND STATUS IN (1, 2)
						AND DATEDIFF(d, fromdate, @FromDate) = 0
					)
				)
		BEGIN
			SELECT @ErrCode = 106
				,@ErrMsg = 'There is another request in the same date.'

			RETURN
		END

		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						AND IsHoliday = 0
						AND IsOffDay = 0
					)
				)
		BEGIN
			IF (
					EXISTS (
						SELECT COUNT(1)
						FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
							AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
						WHERE wk.Id <> @Id
							AND wk.EmpId = @empid
							AND wk.RequestType IN (1, 2)
							AND wk.[Status] IN (1, 2)
							AND DATEDIFF(M, wk.FromDate, @FromDate) = 0
							AND cl.IsHoliday <> 1
							AND cl.IsOffDay <> 1
						HAVING COUNT(1) >= 2
						)
					)
			BEGIN
				IF (
						NOT EXISTS (
							SELECT 1
							FROM dbo.Employee WITH (NOLOCK)
							WHERE Id = @ModifiedBy
								AND GroupId = @HRGroupId
							)
						) -- HR
				BEGIN
					SELECT @ErrCode = 106
						,@ErrMsg = 'The maximum request in a month has reached. You can contact HR for more information.'

					RETURN
				END
				ELSE
				BEGIN
					IF (
							NOT EXISTS (
								SELECT 1
								FROM dbo.Employee WITH (NOLOCK)
								WHERE Id = @EmpId
									AND GroupId = @HRGroupId
								)
							)
					BEGIN
						SELECT @ErrCode = - 1
					END
				END
			END
		END

	END
    ELSE IF (@RequestType = 6)
    BEGIN
		IF (
				@FromDate IS NULL
				OR @ToDate IS NULL
				OR @LeaveTypeID IS NULL
				OR ISNULL(@TotalDays, 0) <= 0
				)
		BEGIN
			SELECT @ErrCode = 105
				,@ErrMsg = 'Leave request has the invalid parameter'

			RETURN
		END

		-- check range year
		IF (
				(
					@LeaveTypeID = @annualLeaveType
					OR @LeaveTypeID = @callInLeaveType
					)
				AND YEAR(@FromDate) <> YEAR(@ToDate)
				)
		BEGIN
			SELECT @ErrCode = 112
				,@ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

			RETURN;
		END

		IF (
				(@TotalDays = 0.5 AND
				(
				EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
								AND Id <> @Id
								AND RequestType = @RequestType
								AND TotalDays <> 0.5
								AND STATUS IN (1, 2)
								AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(DD, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(DD, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(DD, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(DD, ToDate, @ToDate) >= 0
									)
						)
					)
				)
			)
			OR (
				@TotalDays <> 0.5
				AND EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND STATUS IN (1, 2)
							AND (
								(
									DATEDIFF(DD, FromDate, @FromDate) >= 0
									AND DATEDIFF(DD, @FromDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, FromDate, @ToDate) >= 0
									AND DATEDIFF(DD, @ToDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, FromDate) >= 0
									AND DATEDIFF(DD, FromDate, @ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, ToDate) >= 0
									AND DATEDIFF(DD, ToDate, @ToDate) >= 0
								)
						)
					)
				)
			)
		BEGIN
			SELECT @ErrCode = 107
				,@ErrMsg = 'There is another request in the same range'

			RETURN
		END

		-- check the total day is valid
		IF (
				@TotalDays < 1
				AND DATEDIFF(d, @FromDate, @ToDate) <> 0
				)
		BEGIN
			SELECT @ErrCode = 109
				,@ErrMsg = 'The total days is not correct'

			RETURN
		END
		--ELSE
		--BEGIN
		--	IF (
		--			@TotalDays >= 1
		--			AND @TotalDays <> (
		--				SELECT COUNT(1)
		--				FROM dbo.EmployeeCalendar
		--				WHERE EmpID = @EmpId
		--					AND WorkDate BETWEEN @FromDate
		--						AND @ToDate
		--					AND IsHoliday = 0
		--					AND IsOffDay = 0
		--					AND IsLeaveDay = 0
		--				)
		--			)
		--	BEGIN
		--		SELECT @ErrCode = 110
		--			,@ErrMsg = 'The total days is not correct'

		--		RETURN
		--	END
		--END

		--SELECT @TotalPedingDays = SUM(TotalDays)
		--FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		--WHERE id <> @Id 
		--	AND EmpId = @EmpId
		--	AND [Status] = 1
		--	AND RequestType = 6
		--	AND (
		--		LeaveTypeID = @annualLeaveType
		--		OR LeaveTypeID = @callInLeaveType
		--		)

		--SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		--FROM dbo.LeaveTrans WITH (NOLOCK)
		--WHERE EmpId = @EmpId
		--	AND ISNULL(Deleted, 0) = 0
		--	AND YEAR(FromDate) = YEAR(@FromDate)

		IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		BEGIN
			SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, @Id)

			IF(@IsValidLeaveDay = 0)
			BEGIN
				SELECT @ErrCode = 111
				,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

				RETURN
			END
		END

		-- Annual leave 2
		IF (@LeaveTypeID = @annualLeaveType)
		BEGIN
			IF (
					(
						@TotalDays = 0.5
						AND DATEDIFF(D, @FromDate, dbo.fnc_GetClientDateTime()) > 0
						)
					OR (
						@TotalDays = 1
						AND 1 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 2
						AND @TotalDays < 5
						AND 2 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 5
						AND 5 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					)
			BEGIN
				SELECT @ErrCode = 112
					,@ErrMsg = 'Request must be made in advance, in accordance with the policy established.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @callInLeaveType)
		BEGIN
			IF (
					NOT EXISTS (
						SELECT 1
						FROM dbo.Employee WITH (NOLOCK)
						WHERE id = @ModifiedBy
							AND (
								Roles = '4'
								OR (Roles LIKE '%1%')
								OR (Roles LIKE '%2%')
								)
						)
					)
			BEGIN
				SELECT @ErrCode = 114
					,@ErrMsg = 'This leave type must be submitted by Leader on behalf of the employee.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @NEXLeaveType)
		BEGIN
			DECLARE @NEXBalance INT
				,@NEXLeavePending NUMERIC(5, 1)

			SELECT @NEXLeavePending = SUM(TotalDays)
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE Id <> @id
				AND EmpId = @EmpId
				AND [Status] = 1
				AND RequestType = 6
				AND LeaveTypeID = @NEXLeaveType

			SELECT @NEXBalance = (oty.[Minutes] - ISNULL(la.[Minutes], 0))
			FROM dbo.OvertimeYearly AS oty WITH (NOLOCK)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON oty.EmpId = la.EmpId
				AND oty.[Year] = la.[Year]
			WHERE oty.EmpId = @EmpId
				AND oty.[Year] = YEAR(@FromDate)

			IF (ISNULL(@NEXBalance, 0) - (ISNULL(@NEXLeavePending, 0) * 480) < (@TotalDays * 480))
			BEGIN
				SELECT @ErrCode = 115
					,@ErrMsg = 'You have not enough NEX days to request.'

				RETURN
			END
		END
    END

    IF(@Status <> 1 OR @empId <> @ModifiedBy)
    BEGIN
       SELECT @ErrCode = 111
                ,@ErrMsg = 'You can not update this request. Please contact HR for more detail.'

            RETURN
    END

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_EditLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@OldLeaveTypeId
		,@OldFromDate
		,@OldToDate
		,@OldTotalDays
		,@ModifiedBy
	END

    UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
    SET     
            IsPlanned = @IsPlanned,
            Remark = @Remark ,
            FromDate = @FromDate, 
            FromShift = @FromShift, 
            ToDate = @ToDate, 
            ToShift = @ToShift, 
            LeaveTypeID = @LeaveTypeID, 
            TotalDays = @TotalDays, 
            LateInMins = @LateInMins,
            EarlyOutMins = @EarlyOutMins,
            ModifiedDate = @ModifiedDate,
            ModifiedBy = @ModifiedBy,
			IsCrossDate = @IsCrossDate 
     WHERE Id=@Id

    EXEC [WorkingTimeRequest_WriteHistory] @Id

	IF (
			NOT EXISTS (
				SELECT 
					CASE WHEN es.ApproverId IS NULL THEN gs.ApproverId ELSE es.ApproverId END
				FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.GroupId = gs.GroupId AND gs.IsDeleted = 0 AND e.Id <> gs.ApproverId
				LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON e.Id = es.EmpId AND es.IsDeleted = 0 AND e.Id <> es.ApproverId
				WHERE e.Id = @empId
					AND (gs.ApproverId IS NOT NULL OR es.ApproverId IS NOT NULL))
			OR @AutoApproveRequest = 1
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = -1
			,@ApprovedDate = @ModifiedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END

GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetById] 
    @id BIGINT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by id.
DB: NexEPA
Original: 

Revisions:  
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 1
    SELECT r.Id
        ,r.empID
        ,RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,ProcessedDate
        ,0.0 AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate	
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,@IsAllowUpdateStatus as IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
        JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
            AND c.WorkDate = convert(VARCHAR(10), r.FromDate, 111)
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.id = @id
END

GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByDate] @role INT
    ,@fromdate DATETIME
    ,@todate DATETIME
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@leaveTypeId INT = NULL
    ,@status TINYINT = NULL
	,@actorId INT = NULL
    ,@excludedEmpID INT = NULL
    ,@requestType VARCHAR(10) = ''
    ,@isFilterByEffectedDate BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexEPA
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
    DECLARE @IsManagement BIT
        ,@IsLeaderOfStandByTeam BIT
    DECLARE @RequestTypeId TABLE (RequestType INT)
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
		,@CoreTeamId INT = 0,
        @CEOTeamId INT = 0;

	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
        @CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('CoreTeamId', 'CEOTeamId')

	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'


    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END

    CREATE TABLE #WorkingTimeRequest (
        Id BIGINT
        ,EmpId INT
        ,RequestType SMALLINT
        ,RequestedDate DATETIME
        ,CreatedDate DATETIME
        ,[Status] TINYINT
        ,Requester NVARCHAR(20)
        ,FullNameRequester NVARCHAR(50)
        ,EmailRequester VARCHAR(50)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,ProcessedBy NVARCHAR(20)
        ,FullNameProcessedBy NVARCHAR(50)
        ,EmailProcessedBy VARCHAR(50)
        ,ProcessedDate DATETIME
        ,TotalDaysRequestInMonth DECIMAL
        ,FromShift VARCHAR(10)
        ,ToShift VARCHAR(10)
        ,TotalMins INT
        ,FromDate DATETIME
        ,ToDate DATETIME
        ,TotalDays NUMERIC(5, 1)
        ,LeaveTypeId INT
        ,LeaveTypeName NVARCHAR(100)
        ,Remark NVARCHAR(2000)
        ,IsAllowUpdateStatus TINYINT
		,IsCrossDate BIT
		,UnitCode VARCHAR(200)
		,DepartmentName NVARCHAR(200)
        )

    IF (
            EXISTS (
                SELECT 1
                FROM [dbo].[Employee] AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.ID = @excludedEmpID
                    AND g.Id IN (@CoreTeamId, @CEOTeamId)
                )
            )
    BEGIN
        SET @IsManagement = 1
    END
    ELSE
    BEGIN
        SET @IsManagement = 0
    END

	IF (@excludedEmpID = @LeaderOfStandbyTeam)
	BEGIN
		SET @IsLeaderOfStandByTeam = 1
	END
	ELSE
    BEGIN
        SET @IsLeaderOfStandByTeam = 0
    END

	IF ISNULL(@requestType, '') = ''
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		VALUES (1)
			,(2)
			,(3)
			,(4)
			,(5)
			,(6)
	END
	ELSE
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@requestType, ',')
	END

    INSERT INTO #WorkingTimeRequest
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,(CASE 
            WHEN (
                    @IsLeaderOfStandByTeam = 0
                    AND r.RequestType IN (1, 2)
                    AND DATEPART(DW, c.WorkDate) IN (1, 7)
					AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
                    ) THEN 0
            ELSE (CASE 
                    WHEN (
                            @IsManagement = 0
                            AND r.RequestType = 6
                            AND (
                                r.LeaveTypeID = 26
                                OR (
                                    r.LeaveTypeID IN (2, 30, 31)
                                    AND r.TotalDays >= 5
                                    )
                                )
                            ) THEN 0
                    ELSE 1 END
                    ) END
            ) AS IsAllowUpdateStatus
			,r.IsCrossDate
			,u.[Code] AS UnitCode
			,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
    WHERE EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
                    @role NOT IN (1, 2)
                    OR r.empID IN (
						SELECT EmpId FROM dbo.EmployeeSupDetail WITH (NOLOCK) WHERE ApproverId = @excludedEmpID
                        )
            )
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    --ORDER BY r.FromDate DESC
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
		,u.[Code] AS UnitCode
		,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
    WHERE @IsManagement = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType = 6
            AND (
                r.LeaveTypeID = 26
                OR (
                    r.LeaveTypeID IN (2, 30, 31)
                    AND r.TotalDays >= 5
                    )
                )
            )
        AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode 
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
		,u.[Code] AS UnitCode
		,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
    WHERE @IsLeaderOfStandByTeam = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType IN (1, 2)
            AND DATEPART(DW, c.WorkDate) IN (1, 7)
			AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    ORDER BY r.CreatedDate DESC
        ,r.FromDate DESC

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @ItemsPerPage ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC
    END

    SELECT @TotalItem = COUNT(*)
    FROM #WorkingTimeRequest

    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory]
    @requestid bigint
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexHRM
Original: 
Revisions:  
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 0
    SELECT
        r.Id,
        r.empID,
        RequestType,
        (CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s.Code END
            ) AS FromShift,
        s2.Code as ToShift,
        r.FromDate AS RequestedDate,
        r.FromDate,
        r.ToDate,
        r.TotalDays,
        lv.Id AS LeaveTypeId,
        lv.NAME AS LeaveTypeName,
        case when RequestType = 3 then LateInMins else EarlyOutMins end as TotalMins,    
        Remark,
        r.CreatedDate,
        Status,
        e.NickName AS Requester,
        e.FullName AS FullNameRequester,
        '' AS EmailRequester,
        g.Name AS Team,
        g.Code AS TeamCode,
        CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy,
        CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy,
        '' AS EmailProcessedBy,
        ProcessedDate,
        0.0 AS TotalDaysRequestInMonth,
        @IsAllowUpdateStatus as IsAllowUpdateStatus,
		r.IsCrossDate
    FROM dbo.WorkingTimeRequest_history r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
        LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) 
        ON (
        (ap.EmpId = r.ProcessedBy AND r.ProcessedBy IS NOT NULL)
        OR (ap.EmpId = r.CreatedBy AND r.ProcessedBy IS NULL)
        )
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= r.ToShift
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.Id = @requestid
    ORDER BY keyid
END

GO

CREATE OR ALTER PROCEDURE [dbo].[Supervisor_Get] 
    @keyword NVARCHAR(256),
    @groupId INT = NULL,
    @departmentId INT = NULL
AS
/*
Created: 20240516@Lilac.Nguyen
Task: Get Supervisor detail.
DB: NexHRM
Original: 

Revisions:  
    20250516@Henry.Dinh: Add filter by department ID
Param's Explanation:
    @keyword: Search keyword for employee name
    @groupId: Filter by group ID
    @departmentId: Filter by department ID
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @q NVARCHAR(256)

    SET @q = '%' + @keyword + '%'

    IF LEN(@keyword) = 0
        SET @keyword = NULL

	SELECT
		e.Id AS EmpId,
		e.NickName,
		e.FullName,
		d.[Name] AS DepartmentName,
		g.[Name] AS GroupName,
		STRING_AGG(CONVERT(NVARCHAR(MAX), sup.ApproverNickName), ', ') AS Approvers,
		e.AutoApproveRequest
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON sup.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g ON e.GroupId = g.Id
		LEFT JOIN dbo.Department AS d ON g.DepartmentId = d.DepartmentId
	WHERE 
		ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] <> 0 
		AND e.IsSpecialUser = 0
		AND (
			@keyword IS NULL
			OR e.NickName LIKE @q
			OR e.FullName LIKE @q
		)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
		)
		AND (
			@departmentId IS NULL
			OR g.DepartmentId = @departmentId
		)
	GROUP BY 
		e.Id,
		e.NickName,
		e.FullName,
		d.Name,
		g.[Name],
		g.[Rank],
		g.Code,
		e.Roles,
		e.GroupDate,
		e.AutoApproveRequest
	ORDER BY 
		d.[Name],
		g.[Rank],
		g.Code,
		e.Roles DESC,
		e.GroupDate
END
GO
