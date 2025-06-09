USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Insert] @fullName NVARCHAR(50)
    ,@nickName NVARCHAR(20)
    ,@domainUserName VARCHAR(50)
    ,@code VARCHAR(20)
    ,@password NVARCHAR(100)
    ,@monShift INT = NULL
    ,@tueShift INT = NULL
    ,@wedShift INT = NULL
    ,@thuShift INT = NULL
    ,@friShift INT = NULL
    ,@satShift INT = NULL
    ,@sunShift INT = NULL
    ,@groupID INT
    ,@createBy INT
    ,@employeeId INT OUTPUT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: insert an employee
DB: NexHRM
Original: 

Revisions:    
    20250507@Henry.Dinh: insert password
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CompanyId INT
	SELECT @CompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

    IF EXISTS (
            SELECT 1
            FROM dbo.Employee WITH (NOLOCK)
            WHERE Code = @code
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This user has already existed !'

        RETURN
    END

    --SET XACT_ABORT ON;

    --BEGIN TRY
    --    BEGIN TRANSACTION;

        INSERT INTO HRM.dbo.Employee (
            Code
            ,FullName
            ,NickName
            ,DOB
            ,CreatedDate
            ,CreatedBy
            )
        VALUES (
            @code
            ,@fullName
            ,@nickName
            ,CAST('1-1-1990' AS SMALLDATETIME)
            ,dbo.fnc_GetClientDateTime()
            ,@createBy
            )

        SET @employeeId = CAST(SCOPE_IDENTITY() AS INT)

		INSERT INTO HRM.dbo.EmployeeCompany (EmpId, CompanyId)
		VALUES (@employeeId, @CompanyId)

		INSERT INTO dbo.EmployeeExt (
			Id
            ,DomainUsername
            ,Password
            ,GroupID
			,Enabled
			,IsSpecialUser
            )
        VALUES (
			@employeeId
            ,@domainUsername
            ,@password
            ,@groupID
			,0
			,0
            )

        INSERT INTO EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            ,[SatShift]
            ,[SunShift]
            )
        VALUES (
            @employeeId
            ,@monShift
            ,@tueShift
            ,@wedShift
            ,@thuShift
            ,@friShift
            ,@satShift
            ,@sunShift
            )

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @groupID
            ,@employeeId
            ,1
            )

        DECLARE @fromdate DATETIME
            ,@todate DATETIME

        SET @fromdate = dbo.fnc_GetClientDateTime()
		SELECT @todate = MAX(WorkDate) FROM dbo.EmployeeCalendar WITH (NOLOCK)

        EXECUTE EmployeeCalendar_Insert @employeeId
            ,@fromdate
            ,@todate

        SELECT @ErrCode = 0
            ,@ErrMsg = ''

    --    COMMIT TRANSACTION;
    --END TRY

    --BEGIN CATCH
    --    ROLLBACK TRANSACTION

    --    SELECT @ErrCode = 1
    --        ,@ErrMsg = N'Insert Employee unsuccessfully.'
    --END CATCH;
END


GO
