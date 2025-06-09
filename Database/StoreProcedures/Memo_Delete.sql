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
