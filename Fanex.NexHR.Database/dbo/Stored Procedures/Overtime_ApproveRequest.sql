
/****** Object:  Stored Procedure dbo.Overtime_ApproveRequest    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_ApproveRequest] 
	@Id             INT
    ,@ModifiedBy    INT
    ,@ErrCode       INT				= 0 OUTPUT
    ,@ErrMsg        VARCHAR(200)	= '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200729Dwight.Tran: Enhance NEX Request
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
		@GroupManagerCode	VARCHAR (3) = 'MGT'
		,@PendingStatus		INT = 1
		,@ApprovedStatus	INT	= 2;

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
				AND [Status] = @PendingStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can approve the pending NEX request';

		RETURN;
	END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
			WHERE e.[Id] = @ModifiedBy
				AND eg.[Code] = @GroupManagerCode
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers have a permisson to aprove NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[Overtime_138194] WITH (ROWLOCK, UPDLOCK)
        SET
            [Status]        = @ApprovedStatus
            ,[ModifiedBy]   = @ModifiedBy
            ,[ModifiedDate] = GETDATE()
        WHERE [OvertimeId] = @Id AND [Status]=1 -- pending;

		COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Approve NEX request failed';

		ROLLBACK TRANSACTION;
    END CATCH;
END;
