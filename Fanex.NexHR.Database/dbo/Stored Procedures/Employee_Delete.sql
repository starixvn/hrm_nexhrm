
CREATE PROCEDURE [dbo].[Employee_Delete] @employeeId INT
    ,@ResignedDate SMALLDATETIME
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

        UPDATE dbo.Employee
        SET ResignedDate = @ResignedDate
            ,isDeleted = 1
            ,ModifiedDate = GETDATE()
        WHERE ID = @employeeId

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

