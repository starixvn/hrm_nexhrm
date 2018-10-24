
/****** Object:  Stored Procedure dbo.Holiday_Insert    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[Holiday_Insert] @HolidayName NVARCHAR(50)
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*  
Created: 2012/04/20@Dan  
Task: insert a holiday  
DB: NexHR  
Original:   
  
Revisions:      
Param's Explanation:  
  
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Holiday WITH (NOLOCK)
                WHERE (
                        FromDate BETWEEN @FromDate
                            AND @ToDate
                        )
                    OR (
                        @FromDate BETWEEN FromDate
                            AND ToDate
                        )
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'There are same holiday in date range.'

        RETURN
    END

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Holiday] (
            HolidayName
            ,FromDate
            ,ToDate
            )
        VALUES (
            @HolidayName
            ,@FromDate
            ,@ToDate
            )

        DECLARE @id INT

        IF @@ERROR <> 0
            SET @ID = 0
        ELSE
            SET @id = cast(SCOPE_IDENTITY() AS INT)

        -- update working calendar for employee  
        UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
        SET IsHoliday = 1
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = GETDATE()
        WHERE WorkDate BETWEEN @FromDate
                AND @ToDate

        -- update attendate record 
        EXEC EmployeeAttendance_ProcessingAndInsert NULL
            ,@fromdate
            ,@todate
            ,1

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create holiday unsuccessfully.'
    END CATCH;
END

