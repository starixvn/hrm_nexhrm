USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[JobTitle_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Insert]
    @Name NVARCHAR(100)
	,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN

    SET NOCOUNT ON;

    IF (EXISTS(SELECT 1 FROM dbo.JobTitle WITH (NOLOCK) WHERE [Name]=@Name))
    BEGIN
        SELECT @ErrCode=100,
                @ErrMsg = 'The job title already exist'
        RETURN
    END

    INSERT INTO dbo.JobTitle(Name, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
    VALUES(@Name, @CreatedBy, dbo.fnc_GetClientDateTime(), @CreatedBy, dbo.fnc_GetClientDateTime())

    SET @ErrCode=0
END



GO
