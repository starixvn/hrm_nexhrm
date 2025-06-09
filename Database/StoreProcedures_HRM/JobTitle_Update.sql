USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[JobTitle_Update]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Update]
    @JobTitleId INT,
    @Name NVARCHAR(100),
    @ErrMsg NVARCHAR(500) = '',
    @ErrCode INT = NULL
AS
BEGIN

    SET NOCOUNT ON;

    IF (NOT EXISTS(SELECT 1 FROM dbo.JobTitle WITH (NOLOCK)
            WHERE JobTitleId = @JobTitleId))
    BEGIN
        SELECT @ErrCode = 100,
                @ErrMsg = 'The job title does not exist'
        RETURN
    END
    
	UPDATE dbo.JobTitle WITH (UPDLOCK, ROWLOCK)
	SET Name = @Name
    WHERE JobTitleId = @JobTitleId

END



GO
