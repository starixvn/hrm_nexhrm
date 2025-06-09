USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_UpdateTransaction]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_UpdateTransaction] @LeaveTransactionId INT
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: update leave transaction and calculate annual leave balance for employee
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

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @EmpID = EmpID
            ,@OldDeductDays = ISNULL(DeductDays, 0)
            ,@OldCreditDays = ISNULL(CreditDays, 0)
        FROM dbo.LeaveTrans
        WHERE Id = @LeaveTransactionId

        UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
        SET Description = @Description
            ,FromDate = @FromDate
            ,ToDate = @FromDate
            ,DeductDays = @DeductDays
            ,CreditDays = @CreditDays
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
            ,ModifiedBy = @ModifiedBy
        WHERE Id = @LeaveTransactionId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update leave transaction unsuccessfully.'
    END CATCH;
END

GO
