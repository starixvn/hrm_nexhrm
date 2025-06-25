CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_1]    
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disApprove check-in request form (type 1)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @CheckinTime datetime
-- get request check-in times
select @CheckinTime = FromDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to clear the check-in time
    UPDATE dbo.EmployeeCalendar
    SET RequestedCheckinTime = null,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @CheckinTime)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @CheckinTime, @CheckinTime, 1    

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1, -- pending
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
