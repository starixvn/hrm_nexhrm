USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_InsertTransaction]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_InsertTransaction] @EmployeeId INT
    ,@RequestId INT = NULL
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@ToDate DATETIME = NULL
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/17@Dan
Task: record leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,RequestId
            ,Description
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            )
        VALUES (
            @EmployeeId
            ,@RequestId
            ,@Description
            ,@FromDate
            ,@ToDate
            ,@DeductDays
            ,@CreditDays
            ,dbo.fnc_GetClientDateTime()
            ,@CreatedBy
            )

        --UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
        --SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + ISNULL(@CreditDays, 0) - ISNULL(@DeductDays, 0)
        --WHERE Id = @EmployeeId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Add leave transaction unsuccessfully.'
    END CATCH;
END

GO
