USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Employee_Delete] @employeeId INT
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

        UPDATE dbo.EmployeeExt WITH (UPDLOCK, ROWLOCK)
        SET ResignedDate = @ResignedDate
            ,isDeleted = 1
        WHERE ID = @employeeId

		UPDATE HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
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

GO
