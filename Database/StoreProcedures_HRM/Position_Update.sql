USE [HRM]
GO

CREATE OR ALTER PROCEDURE [dbo].[Position_Update] 
    @PositionId INT
    ,@PositionCode VARCHAR(20)
    ,@PositionName NVARCHAR(100)
    ,@IsActive BIT
    ,@DisplayOrder INT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250626@Henry.Dinh
    Task: Update Position.
    DB: NexHRM
    Original: 
    Revisions:  
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Position WITH (NOLOCK)
                WHERE PositionId <> @PositionId AND (PositionCode = @PositionCode OR PositionName = @PositionName)
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Position code or name has already exists'

        RETURN
    END

    UPDATE dbo.Position
    SET
        PositionCode = @PositionCode
        ,PositionName = @PositionName
        ,DisplayOrder = @DisplayOrder
        ,IsActive = @IsActive
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE PositionId = @PositionId

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
