ALTER PROCEDURE [dbo].[Employee_Insert]
(
    @UserId INT
    ,@FullName NVARCHAR(50)
    ,@Nickname NVARCHAR(20)
    ,@DomainUsername VARCHAR(50)
    ,@code VARCHAR(20)
    ,@password NVARCHAR(100)
    ,@monShift INT = NULL
    ,@tueShift INT = NULL
    ,@wedShift INT = NULL
    ,@thuShift INT = NULL
    ,@friShift INT = NULL
    ,@satShift INT = NULL
    ,@sunShift INT = NULL
    ,@GroupId INT
    ,@createBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
)
AS
/*
    Created: 20160218@Quinn.Vo
    Task: insert an employee
    DB: NexHRM

    Revisions:    
        20250507@Henry.Dinh: insert password
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CompanyId INT
        ,@OldEmpId INT

	SELECT @CompanyId = [Value]
    FROM dbo.SystemSetting WITH (NOLOCK)
    WHERE [Name] = 'CompanyId'

    SELECT @OldEmpId = MAX(e.OldEmpId)
    FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN bodb_HRM.dbo.CompanyEmployee AS c WITH (NOLOCK) ON e.EmpId = c.EmpId
    WHERE c.CompanyId = @CompanyId

    SET @OldEmpId = ISNULL(@OldEmpId, 0) + 1

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

    INSERT INTO bodb_HRM.dbo.Employee (
        Code
        ,FullName
        ,Nickname
        ,DOB
        ,CreatedDate
        ,CreatedBy
        ,OldEmpId
		,EmpId
		,[Enabled]
        )
    VALUES (
        @Code
        ,@FullName
        ,@NickName
        ,CAST('1-1-1990' AS SMALLDATETIME)
        ,dbo.fnc_GetClientDateTime()
        ,@createBy
        ,@OldEmpId
		,@UserId
		,1
        )

	INSERT INTO bodb_HRM.dbo.CompanyEmployee(CompanyId, EmpId, DomainUsername, [Password], GroupId, [Enabled], IsDeleted, IsSpecialUser)
	VALUES (@CompanyId, @UserId, @DomainUsername, @Password, @GroupId, 0, 0, 0)

    INSERT INTO dbo.EmployeeShift (
        [EmpId]
        ,[MonShift]
        ,[TueShift]
        ,[WedShift]
        ,[ThuShift]
        ,[FriShift]
        ,[SatShift]
        ,[SunShift]
        )
    VALUES (
        @UserId
        ,@monShift
        ,@tueShift
        ,@wedShift
        ,@thuShift
        ,@friShift
        ,@satShift
        ,@sunShift
        )

    INSERT INTO dbo.EmployeeGroup_Employee (GroupId, EmpId, isMainGroup)
    VALUES (@groupId, @UserId, 1)

    DECLARE @fromdate DATETIME
        ,@todate DATETIME

    SET @fromdate = dbo.fnc_GetClientDateTime()
	SELECT @todate = MAX(WorkDate)
    FROM dbo.EmployeeCalendar WITH (NOLOCK)

    EXECUTE dbo.EmployeeCalendar_Insert @UserId
        ,@fromdate
        ,@todate

    SELECT @ErrCode = 0
        ,@ErrMsg = ''
END