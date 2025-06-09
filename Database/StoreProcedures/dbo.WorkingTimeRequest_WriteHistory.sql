USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkingTimeRequest_WriteHistory]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Stored Procedure dbo.WorkingTimeRequest_WriteHistory    Script Date: 3/13/2016 3:21:44 PM ******/
-- [WorkingTimeRequest_WriteHistory] 316

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_WriteHistory]    
    @RequestID bigint
AS
/*
Created: 2012/07/09@Dan
Task: Copy request to history 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT dbo.WorkingTimeRequest_history
        (
            Id,
            EmpId,
            RequestType,
            Status,
            IsPlanned,
            Remark,
            ProcessedDate,
            ProcessedBy,
            CreatedDate,
            CreatedBy,
            ModifiedDate,
            ModifiedBy,
            FromDate,
            FromShift,
            ToDate,
            ToShift,
            LeaveTypeID,
            TotalDays,
            LateInMins,
            EarlyOutMins,
            LogDate,
			IsCrossDate
        )
    SELECT
            Id,
            EmpId,
            RequestType,
            Status,
            IsPlanned,
            Remark,
            ProcessedDate,
            ProcessedBy,
            CreatedDate,
            CreatedBy,
            ModifiedDate,
            ModifiedBy,    
            FromDate,
            FromShift,
            ToDate,
            ToShift,
            LeaveTypeID,
            TotalDays,
            LateInMins,
            EarlyOutMins,    
            getdate(),
			IsCrossDate
    FROM dbo.WorkingTimeRequest
    WHERE Id = @RequestID

GO
