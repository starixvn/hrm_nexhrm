USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_InsertMultipleTransactions]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_InsertMultipleTransactions] @EmpIds NVARCHAR(4000) = NULL
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/17@Dan
Task: record multiple annual leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON;

    DECLARE @Emp TABLE (ID INT)

    INSERT INTO @Emp (ID)
    SELECT val
    FROM dbo.[fnc_strSplit](@EmpIDs, ',')

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,Description
            ,FromDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            )
        SELECT ID
            ,@Description
            ,@FromDate
            ,@DeductDays
            ,@CreditDays
            ,GETDATE()
            ,@CreatedBy
        FROM Employee
        WHERE (
                (
                    @EmpIds IS NULL
                    AND ISNULL(isdeleted, 0) = 0
                    )
                OR Id IN (
                    SELECT id
                    FROM @Emp
                    )
                )

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
