USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Position_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Position_Insert] 
    @Code VARCHAR(20)
    ,@Name NVARCHAR(100)
    ,@DisplayOrder INT = NULL
    ,@IsActive BIT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231127@Lilac.Nguyen
Task: Insert Position.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Position WITH (NOLOCK)
                WHERE PositionCode = @Code OR PositionName = @Name
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Position code or name has already exists'

        RETURN
    END

    INSERT INTO dbo.Position(
        [PositionCode]
        ,[PositionName]
        ,[DisplayOrder]
        ,[IsActive]
        ,[ModifiedBy]
        ,[ModifiedDate]
        )
    VALUES (
        @Code
        ,@Name
        ,@DisplayOrder
        ,1
        ,@ModifiedBy
        ,dbo.fnc_GetClientDateTime()
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
