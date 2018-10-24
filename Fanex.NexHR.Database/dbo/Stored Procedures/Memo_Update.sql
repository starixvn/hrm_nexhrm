
/****** Object:  Stored Procedure dbo.Memo_Update    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Memo_Update] @Id INT
    ,@RefNo NVARCHAR(50)
    ,@Contents NVARCHAR(MAX)
    ,@Subject NVARCHAR(4000)
    ,@SentTo NVARCHAR(1000)
    ,@SentFrom NVARCHAR(1000)
    ,@ForInfo NVARCHAR(1000)
    ,@TotalPages INT
    ,@SentDate DATETIME
    ,@Category INT
    ,@Type INT
    ,@Status BIT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    UPDATE MEMO
    SET RefNo = @RefNo
        ,Contents = @Contents
        ,Subject = @Subject
        ,SentTo = @SentTo
        ,SentFrom = @SentFrom
        ,ForInfo = @ForInfo
        ,TotalPages = @TotalPages
        ,SentDate = @SentDate
        ,Category = @Category
        ,[Type] = @Type
        ,[Status] = @status
    WHERE Id = @Id;

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Memo failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;

