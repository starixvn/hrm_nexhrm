
/****** Object:  Stored Procedure dbo.Log_WriteLog    Script Date: 3/13/2016 3:21:41 PM ******/
CREATE PROCEDURE [dbo].[Log_WriteLog]
    @LogActionID int,
    @LogDate datetime,
    @Description NVARCHAR(4000),
    @TrackingValue NVARCHAR(MAX),
    @UserID int
AS
/*
Created: 2012/04/20@Dan
Task: write log
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT INTO [dbo].[Log]
           ([LogActionID]
           ,[LogDate]
           ,[Description]
           ,[TrackingValue]
           ,[UserID])
     VALUES
           (
            @LogActionID
           ,@LogDate
           ,@Description
           ,@TrackingValue
           ,@UserID
            )
