CREATE PROCEDURE [dbo].[Overtime_ResetRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200903Dwight.Tran
Task: Enhance NEX Request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 11
        ,@PendingStatus		INT = 1;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime_138194] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime_138194] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] BETWEEN 2 AND 5
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can reset NEX request when status is not pending';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND (e.[GroupId] = @GroupManagerId OR 
					(e.Roles IN (3, 4))) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to void NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime_138194] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = GETDATE()
        WHERE OvertimeId = @Id;

		EXECUTE dbo.[Overtime_DeleteTransaction] @Id, @ModifiedBy;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Reset NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;