USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_ResetRequest]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Overtime_ResetRequest] 
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
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 1081
        ,@PendingStatus		INT = 1;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
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
			FROM [dbo].[Overtime] WITH (NOLOCK)
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

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
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



GO
