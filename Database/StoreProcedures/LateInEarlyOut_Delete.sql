USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LateInEarlyOut_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.LateInEarlyOut_Delete    Script Date: 3/13/2016 3:21:41 PM ******/
-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    19/02/2016
-- Description:    Task "LateIn/EarlyOut Notification"
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Delete] @Id      INT,
                                              @ErrCode INT          = 0 OUTPUT,
                                              @ErrMsg  VARCHAR(200) = '' OUTPUT
AS
     BEGIN
         -- SET NOCOUNT ON added to prevent extra result sets from
         -- interfering with SELECT statements.
         SET NOCOUNT ON;
         IF(EXISTS
           (
               SELECT 1
               FROM dbo.LateInEarlyOut lieo
               WHERE lieo.Id = @Id
           ))
             BEGIN
                 DELETE dbo.LateInEarlyOut
                 WHERE dbo.LateInEarlyOut.Id = @Id;
                 IF @@ERROR <> 0
                     BEGIN
                         SELECT @ErrCode = 1,
                                @ErrMsg = N'Delete Late in/Early out failed.';
                     END;
                 ELSE
                     BEGIN
                         SELECT @ErrCode = 0,
                                @ErrMsg = '';
                     END;
             END;
         ELSE
             BEGIN
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Can not find this request.';
             END;
     END;

GO
