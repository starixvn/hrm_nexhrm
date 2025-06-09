USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Memo_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Memo_Delete    Script Date: 3/13/2016 3:21:42 PM ******/
CREATE OR ALTER PROCEDURE [dbo].[Memo_Delete] @Id      INT,
                                    @ErrCode INT          = 0 OUTPUT,
                                    @ErrMsg  VARCHAR(200) = '' OUTPUT
AS
     BEGIN
         DELETE MEMO
         WHERE Id = @Id;
         
         IF @@ERROR <> 0
             BEGIN                 
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Delete Memo failed.';
             END;
         ELSE
             BEGIN                 
                 SELECT @ErrCode = 0,
                        @ErrMsg = '';
             END;         
     END;
GO
