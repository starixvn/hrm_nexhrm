
/****** Object:  Stored Procedure dbo.LeaveDeduction_Update    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[LeaveDeduction_Update] 
    @LeaveDeductionId INT
    ,@Description NVARCHAR(4000)
    ,@DeductDate DATETIME
    ,@TotalDays NUMERIC(18, 1)
    ,@IsAnnual BIT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Leave deduction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id INT, @TotalDayRequestPending NUMERIC(10, 1), @OldTotalDays NUMERIC(10, 1), @OldIsAnnual BIT, @EmployeeId INT,
    @OldDeductDate DATETIME, @MinutesPreviousYear INT, @oldMinutes INT

    SELECT @OldTotalDays = LeaveDays, @oldMinutes = (LeaveDays * 8 * 60) , @OldIsAnnual = IsAnnual, @EmployeeId = EmpId, @OldDeductDate = [Date]
    FROM dbo.LeaveConversion WITH (NOLOCK)
    WHERE Id = @LeaveDeductionId 

    SELECT @TotalDayRequestPending = SUM(TotalDays)
    FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
    LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
    WHERE rq.EmpId = @EmployeeId
        AND [Status] = 1
        AND RequestType = 6
        AND ty.IsALDeduction = 1
	
    IF (@OldIsAnnual = 0)
    BEGIN
        SET @OldTotalDays = 0
    END

    SELECT @OldTotalDays = ISNULL(@OldTotalDays, 0), @TotalDayRequestPending =  ISNULL(@TotalDayRequestPending, 0)

    IF (@IsAnnual = 1 
        AND EXISTS (SELECT 1 FROM dbo.Employee WITH (NOLOCK) 
            WHERE Id = @EmployeeId AND ((AnnualLeaveBalance + @OldTotalDays - @TotalDayRequestPending) < @TotalDays)))
    BEGIN
        SELECT @ErrCode = 100
                ,@ErrMsg = 'Selected employee does not have enough accumulated annual leave.'
        RETURN
    END

    SET XACT_ABORT ON
    BEGIN TRAN
    BEGIN TRY
        UPDATE dbo.LeaveConversion WITH (ROWLOCK, UPDLOCK)
        SET EmpId = @EmployeeId,
            [Date] = @DeductDate,
            IsAnnual = @IsAnnual,
            LeaveDays = @TotalDays,
            [Description] = @Description,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE Id = @LeaveDeductionId

        UPDATE dbo.LateInEarlyOutTrans WITH (ROWLOCK, UPDLOCK)
        SET EmpId = @EmployeeId,
            WorkDate = @DeductDate,
            [Minutes] = (@TotalDays * 8 * 60),
            LeaveDeductionId = @LeaveDeductionId,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE LeaveDeductionId = @LeaveDeductionId

        UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
            SET [Minutes] = [Minutes] + @oldMinutes, ModifiedDate = GETDATE()
            WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@OldDeductDate)

        IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmployeeId AND [Year] = YEAR(@DeductDate)))
        BEGIN
            UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
            SET [Minutes] = [Minutes] - (@TotalDays * 8 * 60), ModifiedDate = GETDATE()
            WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@DeductDate)
        END
        ELSE
        BEGIN
            SELECT @MinutesPreviousYear = [Minutes]
            FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
            WHERE EmpId = @EmployeeId 
                AND [Year] = YEAR(@DeductDate) - 1

            INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
            VALUES (@EmployeeId, YEAR(@DeductDate), (ISNULL(@MinutesPreviousYear, 0) - (@TotalDays * 8 * 60)), GETDATE())
        END

        IF(@IsAnnual = 1 OR @OldIsAnnual = 1)
        BEGIN
        DELETE FROM dbo.LeaveTrans WHERE LeaveConversionId = @LeaveDeductionId
        IF(@IsAnnual = 1)
        BEGIN
            INSERT INTO dbo.LeaveTrans (
                EmpId
                ,Description
                ,FromDate
                ,DeductDays
                ,CreditDays
                ,CreatedDate
                ,CreatedBy
                ,RequestId
                ,LeaveConversionId
                )
            SELECT @EmployeeId
                ,@Description
                ,@DeductDate
                ,@TotalDays
                ,0
                ,GETDATE()
                ,@ModifiedBy
                ,-1
                ,@LeaveDeductionId
        END
        UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
            SET AnnualLeaveBalance = (ISNULL(AnnualLeaveBalance, 0) + @OldTotalDays - @TotalDays)
            WHERE Id = @EmployeeId
        END

        COMMIT
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK

            SELECT @ErrCode = 100
                ,@ErrMsg = 'There is an error when updating leave deduction.'
        END
    END CATCH
END

