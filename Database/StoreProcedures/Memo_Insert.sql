USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Memo_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Memo_Insert    Script Date: 3/13/2016 3:21:42 PM ******/
CREATE OR ALTER PROCEDURE [dbo].[Memo_Insert] @RefNo      NVARCHAR(50),
                                    @Contents   NVARCHAR(MAX),
                                    @Subject    NVARCHAR(4000),
                                    @SentTo     NVARCHAR(1000),
                                    @SentFrom   NVARCHAR(1000),
                                    @ForInfo    NVARCHAR(1000),
                                    @TotalPages INT,
                                    @SentDate   DATETIME,
                                    @Category   INT,
                                    @Type       INT,
                                    @Status     BIT,
                                    @MemoId     INT            = 0 OUTPUT,
                                    @ErrCode    INT            = 0 OUTPUT,
                                    @ErrMsg     VARCHAR(200)   = '' OUTPUT
AS
     BEGIN
         INSERT INTO MEMO
         (RefNo,
          Contents,
          Subject,
          SentTo,
          SentFrom,
          ForInfo,
          TotalPages,
          SentDate,
          Category,
          [Type],
          [Status]
         )
         VALUES
         (@RefNo,
          @Contents,
          @Subject,
          @SentTo,
          @SentFrom,
          @ForInfo,
          @TotalPages,
          @SentDate,
          @Category,
          @Type,
          @Status
         );
         DECLARE @Id INT;
         IF @@ERROR <> 0
             BEGIN
                 SET @Id = 0;
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Create Memo failed.';
             END;
         ELSE
             BEGIN
                 SET @Id = CAST(SCOPE_IDENTITY() AS INT);
                 SELECT @ErrCode = 0,
                        @ErrMsg = '';
             END;
         SET @MemoId = @Id;
     END;
GO
