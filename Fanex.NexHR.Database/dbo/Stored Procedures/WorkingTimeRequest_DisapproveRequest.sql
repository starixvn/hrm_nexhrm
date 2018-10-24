
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_DisapproveRequest    Script Date: 3/13/2016 3:21:43 PM ******/
CREATE PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest]
    @RequestID bigint,        
    @ModifiedDate datetime,
    @ModifiedBy int,
    @ErrCode int = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/04/25@Dan
Task: disapprove request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @EmpId int, @RequestType smallint, @Status tinyint

-- get employee and requestype of the request        
SELECT @EmpId = EmpId, @RequestType = RequestType, @Status = [status]
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

-- if status is rejected, show error
IF (@Status=3)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request was rejected so it could not be rejected again'

    RETURN
END

-- if status is pending
IF (@Status=1)
BEGIN
    SET @ErrCode = 101
    SET @ErrMsg = 'This request is pending so it cannot be disapproved'
    
    RETURN
END

-- call appropriate SP for each request form
IF (@RequestType=1)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_1 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=2)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_2 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=3)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_3 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=4)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_4 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=5)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_5 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=6)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_6 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END

-- write to history
EXEC [WorkingTimeRequest_WriteHistory] @RequestID

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=getdate()
--Log general info
Select @LogSPData = '<WorkingTimeRequest_DisapproveRequest RequestID="'+ coalesce(cast(@RequestID as varchar(50)),space(1)) + '" ' +
'ModifiedBy="'+ coalesce(cast(@ModifiedBy as varchar(50)),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 3,
    @LogDate = @now,
    @Description = 'Disapprove Request',
    @TrackingValue = @LogSPData,
    @UserID = @ModifiedBy

set @ErrCode=0
