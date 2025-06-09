USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[History_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[History_Delete]
	@Id INT,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg VARCHAR(200) = '' OUTPUT
AS

/*
Created: 20240725@Lilac.Nguyen
Task: Delete History
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/

BEGIN
    DELETE dbo.HistoryWork
    WHERE Id = @Id;

	DELETE dbo.HistoryWorkDetail
    WHERE HistoryWorkId = @Id;
         
    IF @@ERROR <> 0
        BEGIN                 
            SELECT @ErrCode = 1,
                @ErrMsg = N'Delete History failed.';
        END;
    ELSE
        BEGIN                 
            SELECT @ErrCode = 0, @ErrMsg = '';
        END;         
END;
GO
