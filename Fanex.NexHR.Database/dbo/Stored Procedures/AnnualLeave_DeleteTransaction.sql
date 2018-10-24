
CREATE PROCEDURE [dbo].[AnnualLeave_DeleteTransaction] @LeaveTransactionId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: soft delete leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON;

    DECLARE @OldDeductDays NUMERIC(18, 1)
        ,@OldCreditDays NUMERIC(18, 1)
        ,@EmpID INT
        ,@isDeleted BIT

    SELECT @EmpID = EmpID
        ,@OldDeductDays = ISNULL(DeductDays, 0)
        ,@OldCreditDays = ISNULL(CreditDays, 0)
        ,@isDeleted = Deleted
    FROM dbo.LeaveTrans WITH (NOLOCK)
    WHERE Id = @LeaveTransactionId

    IF (ISNULL(@isDeleted, 0) = 1)
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete leave transaction unsuccessfully.'

        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
        SET Deleted = 1
            ,ModifiedDate = GETDATE()
            ,ModifiedBy = @ModifiedBy
        WHERE Id = @LeaveTransactionId

        UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
        SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + @OldDeductDays - @OldCreditDays
        WHERE Id = @EmpId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete leave transaction unsuccessfully.'
    END CATCH;
END

