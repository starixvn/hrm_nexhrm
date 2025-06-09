USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[DBA_AnnualLeave_CreateAnnualBalance]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_CreateAnnualBalance] @Year INT
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
	- Oliver@2024.12.31: change to new rule of AL credit
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Add annual leave days for ' + CONVERT(NVARCHAR(10), @Year) + '.'

    DECLARE @CreditDays NUMERIC(18, 1)
		,@EndDate DATETIME
        ,@FromDate DATETIME
        ,@ToDate DATETIME
        ,@CalDate_Annual DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year) + '/12/31'
        ,@CalDate_Annual = CONVERT(NVARCHAR(10), @Year) + '/07/01'
		,@EndDate =  CONVERT(NVARCHAR(10), @Year) + '/12/31'

    SET @CreditDays = 12

    BEGIN TRY
        BEGIN TRANSACTION;

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
            ,@CreditDays - 1
				+ CASE 
					WHEN eg.DepartmentId IN (1, 3) 
					THEN (CASE 
						WHEN FLOOR(DATEDIFF(DAY, DATEADD(year, 1, e.GroupDate), @EndDate)/730) > 19 THEN 19 
						ELSE FLOOR(DATEDIFF(DAY, DATEADD(year, 1, e.GroupDate), @EndDate)/730) + 1
						END)
					ELSE (CASE 
						WHEN FLOOR(DATEDIFF(DAY, e.GroupDate, @EndDate)/365) > 19 THEN 19 
						ELSE FLOOR(DATEDIFF(DAY, e.GroupDate, @EndDate)/365) 
						END)
					END AS CreditDays
            ,GETDATE()
            ,NULL CreatedBy
            ,0 Deleted
            ,1 as IsOpenBalance
        FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup eg WITH (NOLOCK) ON e.GroupId = eg.Id
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
            AND e.GroupDate <= @FromDate
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

GO
