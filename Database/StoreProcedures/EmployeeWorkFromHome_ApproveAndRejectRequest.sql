CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_ApproveAndRejectRequest]
    @WorkFromHomeRequestId INT,    
	@StatusId TINYINT,    
    @ProcessedBy INT,
    @ProcessedDate DATETIME,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211227@Teddy.le
Task: Approve request employee work from home
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @EmpId int, @Status tinyint, @FromDate datetime, @ToDate datetime
    
SELECT @EmpId = wfhr.EmpId, @Status = wfhr.Status, 
        @FromDate = wfhr.FromDate, @ToDate = wfhr.ToDate
FROM dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK)
INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ewfh.WorkFromHomeRequestId
WHERE wfhr.Id = @WorkFromHomeRequestId

set @FromDate = convert(varchar(10),@FromDate,111) 
set @ToDate = convert(varchar(10),@ToDate,111) 

IF (@Status<>1)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request is not pending so it cannot be approved or be rejected'

    RETURN
END

-- if not exist calendar

IF NOT EXISTS(SELECT *
                FROM dbo.EmployeeCalendar WITH (NOLOCK)
                WHERE empid=@empid
                    AND workdate = @fromdate                    
                    )
BEGIN
    SET @ErrCode = 102
    SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
    RETURN
END

UPDATE dbo.WorkFromHomeRequest
SET [Status] = @StatusId,
    ProcessedBy= @ProcessedBy,
    ProcessedDate= @ProcessedDate,
    ModifiedDate = @ProcessedDate,
    ModifiedBy = @ProcessedBy
WHERE Id = @WorkFromHomeRequestId

--reject
IF (@StatusId = 3)
BEGIN
	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0
END

EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, @StatusId

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()

--Log general info
Select @LogSPData = '<EmployeeWorkFromHome_ApproveAndRejectRequest @WorkFromHomeRequestId="'+ coalesce(cast(@WorkFromHomeRequestId as varchar(50)),space(1)) + '" ' +
'StatusId="'+ coalesce(cast(@StatusId as varchar(100)),space(1)) + '" ' +
'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
'ProcessedDate="'+ coalesce(convert(varchar(10),@ProcessedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 1,
    @LogDate = @now,
    @Description = 'Approve And Reject Employee Work From Home Request',
    @TrackingValue = @LogSPData,
    @UserID = @ProcessedBy

set @ErrCode = 0
GO
