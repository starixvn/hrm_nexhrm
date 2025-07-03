CREATE OR ALTER   PROCEDURE [dbo].[Employee_Delete] @employeeId INT
    ,@ResignedDate SMALLDATETIME
	,@ModifiedBy INT = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Delete employee
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    IF (@ResignedDate IS NULL)
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The user cannot be deleted without resigned date'

        RETURN
    END

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @CompanyId int = (SELECT [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId')

        UPDATE bodb_HRM.dbo.CompanyEmployee WITH (UPDLOCK, ROWLOCK)
        SET ResignedDate = @ResignedDate
            ,isDeleted = 1
        WHERE EmpId = @employeeId
		AND CompanyId = @CompanyId

		UPDATE bodb_HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE EmpId = @employeeId

        DELETE dbo.EmployeeCalendar
        WHERE WorkDate > @ResignedDate
            AND empid = @employeeId;

        DELETE dbo.EmployeeAttendance
        WHERE AttendanceDate > @ResignedDate
            AND empid = @employeeId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete Employee unsuccessfully.'
    END CATCH
END
GO