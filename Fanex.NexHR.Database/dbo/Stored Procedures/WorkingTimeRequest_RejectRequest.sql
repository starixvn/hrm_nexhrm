
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest    Script Date: 3/13/2016 3:21:44 PM ******/
/*
declare @now datetime
set @now = getdate()
declare @ErrCode int ,
    @ErrMsg nvarchar(200)
exec [WorkingTimeRequest_RejectRequest] 3,88, @now,88, @ErrCode output, @ErrMsg output
select @ErrMsg, @ErrCode
*/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest]
    @RequestID bigint,        
    @ProcessedBy int,
    @ProcessedDate datetime,
    @ErrCode int = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/04/23@Dan
Task: reject a request form (main SP)
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

-- if status is not pending, show error
IF (@Status<>1)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request is not pending so it cannot be rejected'

    RETURN
END

-- if status is not pending, show error
IF (@ProcessedBy is null or @ProcessedDate is null)
BEGIN
    SET @ErrCode = 101
    SET @ErrMsg = '@ProcessedBy or @ProcessedDate parameter is missing'

    RETURN
END

-- update status of request
UPDATE dbo.WorkingTimeRequest
SET [Status] = 3, -- rejected
    ProcessedBy = @ProcessedBy,
    ProcessedDate = @ProcessedDate,
    ModifiedDate = @ProcessedDate,
    ModifiedBy = @ProcessedBy 
WHERE Id = @RequestID

SET @ErrCode = 0

-- write to history
EXEC [WorkingTimeRequest_WriteHistory] @RequestID

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=getdate()
--Log general info
Select @LogSPData = '<WorkingTimeRequest_RejectRequest RequestID="'+ coalesce(cast(@RequestID as varchar(50)),space(1)) + '" ' +
'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
'ProcessedDate="'+ coalesce(convert(varchar(10),@ProcessedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 2,
    @LogDate = @now,
    @Description = 'Reject Request',
    @TrackingValue = @LogSPData,
    @UserID = @ProcessedBy
