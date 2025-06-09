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
