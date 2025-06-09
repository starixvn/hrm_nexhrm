USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_Transfer]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

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

	SELECT @FromDomain = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
	SELECT @FromDomainEmail = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'
	SELECT @FromCompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

	IF (@FromCompanyId = 1)
	BEGIN
		SET @ToCompanyId = 2
		SELECT @ToDomain = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM LumiHRM.dbo.EmployeeExt WITH (NOLOCK) WHERE Id = @EmployeeId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.JoinedDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM LumiHRM.dbo.EmployeeExt AS a 
			INNER JOIN dbo.EmployeeExt AS b ON a.Id = b.Id 
			WHERE a.Id = @employeeId
		END
		ELSE
		BEGIN
			INSERT INTO LumiHRM.dbo.EmployeeExt (
				Id
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,JoinedDate
				,ConfirmedDate
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
			FROM dbo.EmployeeExt WITH (NOLOCK)
			WHERE Id = @EmployeeId
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

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1)

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
				,GETDATE()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

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
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, GETDATE(), 1)
			END
			
			INSERT INTO LumiHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), GETDATE())

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
							,GETDATE()
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
					,GETDATE()
				)
		END
	END
	ELSE IF (@FromCompanyId = 2)
	BEGIN
		SET @ToCompanyId = 1
		SELECT @ToDomain = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM NexHRM.dbo.EmployeeExt WITH (NOLOCK) WHERE Id = @EmployeeId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.JoinedDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM NexHRM.dbo.EmployeeExt AS a 
			INNER JOIN dbo.EmployeeExt AS b ON a.Id = b.Id 
			WHERE a.Id = @employeeId
		END
		ELSE
		BEGIN
			INSERT INTO NexHRM.dbo.EmployeeExt (
				Id
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,JoinedDate
				,ConfirmedDate
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
			FROM dbo.EmployeeExt WITH (NOLOCK)
			WHERE Id = @EmployeeId
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

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1)

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
				,GETDATE()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

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
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, GETDATE(), 1)
			END
			
			INSERT INTO NexHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), GETDATE())

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
							,GETDATE()
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
					,GETDATE()
				)
		END
	END

	IF NOT EXISTS (SELECT 1 FROM HRM.dbo.EmployeeCompany WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
	BEGIN
		INSERT INTO HRM.dbo.EmployeeCompany(EmpId, CompanyId)
		VALUES (@EmployeeId, @ToCompanyId)
	END

	UPDATE dbo.EmployeeExt WITH (ROWLOCK, UPDLOCK)
	SET [Enabled] = 0
		,ResignedDate = @LastWorkingDate
	WHERE Id = @EmployeeId

	UPDATE HRM.dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE Id = @EmployeeId
END
GO
