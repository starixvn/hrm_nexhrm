
/****** Object:  Stored Procedure dbo.Overtime_RejectRequest    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_RejectRequest] @Id INT
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

    DECLARE @CreatedBy INT;

    SELECT @CreatedBy = CreatedBy
    FROM dbo.Overtime WITH (NOLOCK)
    WHERE OvertimeId = @Id;

    IF (
            @CreatedBy = @ModifiedBy
            OR EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                WHERE e.Id = @ModifiedBy
                    AND (e.Roles IN (3, 4)) -- HR, SA
                )
            )
    BEGIN
        UPDATE dbo.Overtime WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @Status
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = GETDATE()
        WHERE OvertimeId = @Id
            AND [Status] = 1
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The OT request is only rejected by its owner or HR role';
    END;
END;

