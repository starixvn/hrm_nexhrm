
/****** Object:  Stored Procedure dbo.Overtime_ApproveRequest    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_ApproveRequest] @Id INT
    ,@ModifiedBy INT
    ,@Status INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @CreatedBy INT, @Date DATE;
    SELECT @CreatedBy = CreatedBy
       ,@Date = [Date]
    FROM dbo.Overtime WITH (NOLOCK)
    WHERE OvertimeId = @Id;

    IF (
            @CreatedBy = @ModifiedBy
            OR EXISTS (
                SELECT 1
                FROM dbo.Employee e WITH (NOLOCK)
                WHERE e.Id = @ModifiedBy
                    AND (e.Roles IN (3, 4)) -- HR, SA
                )
            )
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            UPDATE dbo.Overtime WITH (ROWLOCK, UPDLOCK)
            SET [Status] = @Status
                ,ModifiedBy = @ModifiedBy
                ,ModifiedDate = GETDATE()
            WHERE OvertimeId = @Id
                AND STATUS = 1;-- pending

            EXECUTE dbo.Overtime_InsertTransaction @Id, @Date, @ModifiedBy

            COMMIT TRANSACTION;
        END TRY

        BEGIN CATCH
            SELECT @ErrCode = 1000
                ,@ErrMsg = 'Approve Overtime request failed';

            ROLLBACK TRANSACTION;
        END CATCH;
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The OT request is only approved by its owner or HR role';

        RETURN;
    END;
END;
