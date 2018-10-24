
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_CreateAnnualBalance] @Year INT
    ,@EmpID INT = NULL
    ,@Description NVARCHAR(1000) = ''
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@2014.12.26
Task: Add annual balance leave for new year.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Add annual leave days for ' + CONVERT(NVARCHAR(10), @Year) + '.'

    DECLARE @CreditDays NUMERIC(18, 1)
        ,@FromDate DATETIME
        ,@ToDate DATETIME
        ,@CalDate_Annual DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year) + '/12/31'
        ,@CalDate_Annual = CONVERT(NVARCHAR(10), @Year) + '/07/01'

    SET @CreditDays = 12

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE e
        SET e.AnnualLeaveBalance = e.AnnualLeaveBalance + @CreditDays + (@Year - 1 - YEAR(e.CompanyDate)) / 5 + (CASE 
                WHEN (@Year - 1 - YEAR(e.CompanyDate)) % 5 = 4 THEN (CASE 
                        WHEN MONTH(e.CompanyDate) < MONTH(@CalDate_Annual) THEN 1
                        ELSE 0 END
                        )
                ELSE 0 END
                )
			+ CASE WHEN e.GroupId IN (1, 2, 38) THEN 0 ELSE(CASE WHEN DATEADD(yy, 5, e.CompanyDate) < @CalDate_Annual THEN 3 ELSE
			(
				CASE WHEN dateadd(yy, 3, e.CompanyDate) < @CalDate_Annual THEN 2 ELSE 1
				END
			)
			END)
			END
        FROM dbo.Employee e WITH (ROWLOCK, UPDLOCK)
            LEFT JOIN (
            SELECT MAX(FromDate) FromDate
                ,EmpId
            FROM dbo.LeaveTrans lt
            WHERE lt.FromDate >= @FromDate
                AND lt.CreditDays > 0
                AND ISNULL(Deleted, 0) = 0
            GROUP BY EmpId
            ) lt ON e.Id = lt.EmpId
        WHERE ISNULL(isdeleted, 0) = 0
            AND e.[Enabled] = 1
            AND e.CompanyDate <= @FromDate
            AND e.ResignedDate IS NULL
            AND (
                e.Id = @EmpID
                OR @EmpID IS NULL
                )
            AND lt.EmpId IS NULL

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,[Description]
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            ,Deleted
            ,IsOpenBalance
            )
        SELECT e.Id
            ,@Description Description
            ,@FromDate FromDate
            ,@ToDate ToDate
            ,0 AS DeductDays
            ,@CreditDays + (@Year - 1 - YEAR(e.CompanyDate)) / 5 + (CASE 
                WHEN (@Year - 1 - YEAR(e.CompanyDate)) % 5 = 4 THEN (CASE 
                        WHEN MONTH(e.CompanyDate) < MONTH(@CalDate_Annual) THEN 1
                        ELSE 0 END
                        )
                ELSE 0 END
                )
			+ CASE WHEN e.GroupId IN (1, 2, 38) THEN 0 ELSE(CASE WHEN DATEADD(yy, 5, e.CompanyDate) < @CalDate_Annual THEN 3 ELSE
			(
				CASE WHEN dateadd(yy, 3, e.CompanyDate) < @CalDate_Annual THEN 2 ELSE 1
				END
			)
			END)
			END AS CreditDays
            ,GETDATE()
            ,NULL CreatedBy
            ,0 Deleted
            ,1
        FROM dbo.Employee e WITH (NOLOCK)
            LEFT JOIN (
            SELECT MAX(FromDate) FromDate
                ,EmpId
            FROM dbo.LeaveTrans lt
            WHERE lt.FromDate >= @FromDate
                AND lt.CreditDays > 0
                AND ISNULL(Deleted, 0) = 0
            GROUP BY EmpId
            ) lt ON e.Id = lt.EmpId
        WHERE ISNULL(isdeleted, 0) = 0
            AND e.[Enabled] = 1
            AND e.CompanyDate <= @FromDate
            AND e.ResignedDate IS NULL
            AND (
                e.Id = @EmpID
                OR @EmpID IS NULL
                )
            AND lt.EmpId IS NULL

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrCode = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT @ErrCode = 100
            ,@ErrMsg = N'Add annual balance failed.'

        RETURN
    END CATCH;
END

