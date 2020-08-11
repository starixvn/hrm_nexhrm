CREATE PROCEDURE [dbo].[Overtime_Delete]
    @Id                 INT
    ,@ModifiedId        INT
    ,@ErrCode           INT                 = 0 OUTPUT
    ,@ErrMsg            VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20200302Merlin.Le
Task: Enhance NEX balance request
DB: NexHRM
Original:
Revisions: 20200729Dwight.Tran: Enhance NEX Request.
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
	DECLARE
        @PendingStatus		INT = 1;

    IF (
        NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime_138194] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
        )
    )
    BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'NEX request not exist';

        RETURN;
    END;

	IF (
        NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime_138194] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
                AND [CreatedBy] = @ModifiedId
        )
    )
    BEGIN
        SELECT
            @ErrCode = 2
            ,@ErrMsg = 'You only have a permission to delete your NEX request';

        RETURN;
    END;

	IF (
		NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime_138194] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
                AND [Status] = @PendingStatus
		)
	)
	BEGIN
        SELECT
            @ErrCode = 3
            ,@ErrMsg = 'You only can delete pending NEX request';

        RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        DELETE FROM [dbo].[Overtime_138194]
        WHERE [OvertimeId] = @Id;

        DELETE FROM [dbo].[OvertimeDetail_138194]
        WHERE [OvertimeId] = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Delete NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;