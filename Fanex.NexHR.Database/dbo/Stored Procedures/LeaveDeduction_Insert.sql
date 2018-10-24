
/****** Object:  Stored Procedure dbo.LeaveDeduction_Insert    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[LeaveDeduction_Insert] @EmployeeId INT
    ,@Description NVARCHAR(4000)
    ,@DeductDate DATETIME
    ,@TotalDays NUMERIC(18, 1)
    ,@IsAnnual BIT
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Leave deduction.
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

    DECLARE @Id INT
        ,@TotalDayRequestPending NUMERIC(10, 1)
        ,@MinutesPreviousYear INT

    SELECT @TotalDayRequestPending = SUM(TotalDays)
    FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
        LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
    WHERE rq.EmpId = @EmployeeId
        AND [Status] = 1
        AND RequestType = 6
        AND ty.IsALDeduction = 1

    IF (
            @IsAnnual = 1
            AND EXISTS (
                SELECT 1
                FROM dbo.Employee WITH (NOLOCK)
                WHERE Id = @EmployeeId
                    AND ((AnnualLeaveBalance - ISNULL(@TotalDayRequestPending, 0)) < @TotalDays)
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'Selected employee does not have enough accumulated annual leave.'

        RETURN
    END

    SET XACT_ABORT ON

    BEGIN TRAN

    BEGIN TRY
        INSERT INTO dbo.LeaveConversion (
            EmpId
            ,[Date]
            ,IsAnnual
            ,LeaveDays
            ,IsDeleted
            ,ModifiedBy
            ,ModifiedDate
            ,[Description]
            ,[Type]
            )
        SELECT @EmployeeId
            ,@DeductDate
            ,@IsAnnual
            ,@TotalDays
            ,0
            ,@CreatedBy
            ,GETDATE()
            ,@Description
            ,1

        SELECT @Id = CAST(SCOPE_IDENTITY() AS INT)

        INSERT INTO dbo.LateInEarlyOutTrans (
            EmpId
            ,WorkDate
            ,[Type]
            ,[Minutes]
            ,LeaveDeductionId
            ,ModifiedBy
            ,ModifiedDate
            )
        VALUES (
            @EmployeeId
            ,@DeductDate
            ,0
            ,(@TotalDays * 8 * 60)
            ,@Id
            ,@CreatedBy
            ,GETDATE()
            )

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

        IF (@IsAnnual = 1)
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
                ,@CreatedBy
                ,- 1
                ,@Id

            UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
            SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) - @TotalDays
            WHERE Id = @EmployeeId
        END

        COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK

            SELECT @ErrCode = 100
                ,@ErrMsg = 'There is an error when inserting leave deduction.'
        END
    END CATCH
END

