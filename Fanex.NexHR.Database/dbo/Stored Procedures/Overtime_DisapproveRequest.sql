
/****** Object:  Stored Procedure dbo.Overtime_DisapproveRequest    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_DisapproveRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200903Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 11
        ,@PendingStatus     INT = 1
        ,@ApprovedStatus	INT = 2;

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
				AND [Status] = @ApprovedStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can disapprove approved NEX request';

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
			,@ErrMsg = 'Only managers or HR have a permisson to disapproved NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime_138194] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = GETDATE()
        WHERE OvertimeId = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Disapprove NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;

