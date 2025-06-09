USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[DBA_AnnualLeave_TransferBalanceFromYear]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_TransferBalanceFromYear] @Year INT
    ,@EmpID INT = NULL
    ,@Description NVARCHAR(1000) = ''
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Oliver@2024.12.31
Task: Transfer AL unused from last year to current year
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ''

    DECLARE @FromDate DATETIME
        ,@ToDate DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year + 1) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year + 1) + '/03/31'

    BEGIN TRY
        BEGIN TRANSACTION;

		CREATE TABLE #EmployeeLastYearBalance
		 (EmpId          INT,
		  LastYearAL NUMERIC(18,1),
		  IsMaxAL BIT
		 );

		INSERT INTO #EmployeeLastYearBalance(EmpId, LastYearAL, IsMaxAL)
		SELECT e.Id
				,SUM(lt.CreditDays) - SUM(lt.DeductDays) as LastYearAL
				,(CASE 
				WHEN EXISTS (SELECT 1 FROM dbo.LeaveTrans with (nolock) where EmpId = e.Id and CreditDays >= 29 and YEAR(FromDate) = @Year + 1) 
				THEN 1 ELSE 0
			END) as IsMaxAL 
		FROM [dbo].[LeaveTrans] lt with (nolock)
		INNER JOIN dbo.Employee e with (nolock) on lt.EmpId = e.Id
		WHERE YEAR(lt.FromDate) = 2024 AND ISNULL(lt.Deleted, 0) = 0
			AND ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				e.Id = @EmpID
				OR @EmpID IS NULL
				)
		GROUP BY lt.EmpId, e.Id, e.NickName

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
        SELECT e.EmpId
            ,@Description Description
            ,@FromDate FromDate
            ,@ToDate ToDate
            ,0 AS DeductDays
            ,CASE WHEN e.LastYearAL > 3 THEN 3 ELSE e.LastYearAL END LastYearMaxAL
            ,GETDATE()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.LastYearAL > 0 AND e.IsMaxAL = 0
        COMMIT TRANSACTION;

		DROP TABLE #EmployeeLastYearBalance;

        SELECT @ErrCode = 0
            ,@ErrCode = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT @ErrCode = 100
            ,@ErrMsg = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ' failed'

        RETURN
    END CATCH;
END

GO
