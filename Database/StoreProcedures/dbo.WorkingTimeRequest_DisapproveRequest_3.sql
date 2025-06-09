USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_DisapproveRequest_3]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.WorkingTimeRequest_DisapproveRequest_3    Script Date: 3/13/2016 3:21:43 PM ******/
CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_3]
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disapprove late in request form (type 3)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @RequestDetail table
        (Id int primary key identity(1,1),
        WorkDate datetime,
        LateIn int)

DECLARE @WorkDate datetime, @LateIn int
-- get late in date
SELECT @WorkDate = FromDate, @LateIn = LateInMins
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET LateIn = 0,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0
        
    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1
     
    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1 ,-- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID

    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO
