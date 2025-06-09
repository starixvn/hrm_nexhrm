USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Log_WriteLog]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Log_WriteLog    Script Date: 3/13/2016 3:21:41 PM ******/
CREATE OR ALTER PROCEDURE [dbo].[Log_WriteLog]
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
GO
