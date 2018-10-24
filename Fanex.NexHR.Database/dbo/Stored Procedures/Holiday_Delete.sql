
/****** Object:  Stored Procedure dbo.Holiday_Delete    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[Holiday_Delete] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: delete a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    DECLARE @Fromdate DATETIME
        ,@todate DATETIME

    SELECT @fromdate = fromdate
        ,@todate = todate
    FROM dbo.Holiday WITH (NOLOCK)
    WHERE Id = @id

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- update working calendar for employee tu turn off the isHoliday 
        UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
        SET IsHoliday = 0
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = GETDATE()
        WHERE WorkDate BETWEEN @FromDate
                AND @ToDate

        -- update the attendance data
        EXEC EmployeeAttendance_ProcessingAndInsert NULL
            ,@fromdate
            ,@todate
            ,1

        -- delete the holiday
        DELETE Holiday
        WHERE Id = @id

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete holiday unsuccessfully.'
    END CATCH;
END

