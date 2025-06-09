USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Holiday_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Holiday_Update    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE OR ALTER PROCEDURE [dbo].[Holiday_Update] @Id INT
    ,@HolidayName NVARCHAR(50)
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: update a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    SET XACT_ABORT ON;

    DECLARE @OldFromdate DATETIME
        ,@Oldtodate DATETIME

    SELECT @OldFromdate = fromdate
        ,@Oldtodate = todate
    FROM dbo.Holiday WITH (NOLOCK)
    WHERE Id = @id

    BEGIN TRY
        BEGIN TRANSACTION;

        IF (
                @OldFromdate = @FromDate
                AND @ToDate = @Oldtodate
                )
        BEGIN
            UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
            SET HolidayName = @HolidayName
            WHERE ID = @ID
        END
        ELSE
        BEGIN
            -- update working calendar for employee tu turn off the isHoliday for previous  time range
            UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
            SET IsHoliday = 0
            WHERE WorkDate BETWEEN @OldFromdate
                    AND @Oldtodate

            -- just update attendate record from past to today
            EXEC EmployeeAttendance_ProcessingAndInsert NULL
                ,@OldFromdate
                ,@Oldtodate
                ,1

            -- update working calendar for employee tu turn on the isHoliday  for new range
            UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
            SET IsHoliday = 1
				,ShiftId = NULL
                ,ModifiedBy = @ModifiedBy
                ,ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE WorkDate BETWEEN @FromDate
                    AND @ToDate

            -- update attendate record 
            EXEC EmployeeAttendance_ProcessingAndInsert NULL
                ,@fromdate
                ,@todate
                ,1

            UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
            SET HolidayName = @HolidayName
                ,FromDate = @FromDate
                ,ToDate = @ToDate
            WHERE ID = @ID
        END

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

GO
