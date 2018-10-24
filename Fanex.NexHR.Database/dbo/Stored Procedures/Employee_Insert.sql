
CREATE PROCEDURE [dbo].[Employee_Insert] @fullName NVARCHAR(50)
    ,@nickName NVARCHAR(20)
    ,@domainUserName VARCHAR(50)
    ,@code VARCHAR(20)
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
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

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

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO Employee (
            Code
            ,DomainUsername
            ,FullName
            ,NickName
            ,DOB
            ,Enabled
            ,GroupID
            ,CreatedDate
            ,CreatedBy
            ,AnnualLeaveBalance
            )
        VALUES (
            @code
            ,@domainUsername
            ,@fullName
            ,@nickName
            ,CAST('1-1-1990' AS SMALLDATETIME)
            ,0
            ,@groupID
            ,GETDATE()
            ,@createBy
            ,0
            )

        SET @employeeId = CAST(SCOPE_IDENTITY() AS INT)

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

        SET @fromdate = GETDATE()
        SET @todate = DATEADD(DAY, - 1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @fromdate) + 2, 0))

        EXECUTE EmployeeCalendar_Insert @employeeId
            ,@fromdate
            ,@todate

        SELECT @ErrCode = 0
            ,@ErrMsg = ''

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Insert Employee unsuccessfully.'
    END CATCH;
END

