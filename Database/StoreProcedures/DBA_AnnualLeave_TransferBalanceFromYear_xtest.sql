USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[DBA_AnnualLeave_TransferBalanceFromYear_xtest]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_TransferBalanceFromYear_xtest] @Year INT
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
		
		INSERT INTO dbo.LateInEarlyOutYearly(EmpId, Minutes, Year, ModifiedBy, ModifiedDate)
		SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
			,@Year + 1
			,NULL
			,dbo.fnc_GetClientDateTime()
		FROM dbo.Employee AS e WITH (NOLOCK)
			LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
			AND la.[Year] = @Year
			LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
			AND oty.[Year] = @Year
		WHERE e.IsSpecialUser = 0
			AND ISNULL(e.isDeleted, 0) = 0

		CREATE TABLE #LastYearLIEO (EmpId INT, [Minutes] INT);
		INSERT INTO #LastYearLIEO (EmpId, [Minutes])
		SELECT e.Id
			,(
				ISNULL(la.[Minutes], 0) - CASE 
				WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
				ELSE ISNULL(la.[Minutes], 0) END
				)
		FROM dbo.Employee AS e WITH (NOLOCK)
			LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
			AND la.[Year] = @Year
			LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
			AND oty.[Year] = @Year
		WHERE e.IsSpecialUser = 0
			AND ISNULL(e.isDeleted, 0) = 0

		INSERT INTO dbo.LateInEarlyOutYearly(EmpId, Year, Minutes, ModifiedBy, ModifiedDate)
		SELECT li.EmpId, @Year + 1, li.Minutes, NULL, dbo.fnc_GetClientDateTime()
		FROM #LastYearLIEO li
		WHERE NOT EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WHERE EmpId = li.EmpId AND Year = @Year + 1)

		CREATE TABLE #EmployeeLastYearBalance
		 (EmpId          INT,
		  PreYearAL NUMERIC(18,1),
		  IsMaxAL BIT,
		  PreYearLIEO INT
		 );

		INSERT INTO #EmployeeLastYearBalance(EmpId, PreYearAL, IsMaxAL, PreYearLIEO)
		SELECT e.Id
				,SUM(lt.CreditDays) - SUM(lt.DeductDays) as LastYearAL
				,(CASE 
				WHEN EXISTS (SELECT 1 FROM dbo.LeaveTrans with (nolock) where EmpId = e.Id and CreditDays >= 29 and YEAR(FromDate) = @Year + 1) 
				THEN 1 ELSE 0
			END) as IsMaxAL
				,pLi.Minutes
		FROM [dbo].[LeaveTrans] lt with (nolock)
		INNER JOIN dbo.Employee e with (nolock) on lt.EmpId = e.Id
		LEFT JOIN #LastYearLIEO pLI on e.Id = pLi.EmpId
		WHERE YEAR(lt.FromDate) = 2024 AND ISNULL(lt.Deleted, 0) = 0
			AND ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				e.Id = @EmpID
				OR @EmpID IS NULL
				)
		GROUP BY lt.EmpId, e.Id, e.NickName, pLI.Minutes;

		UPDATE ly 
		SET ly.Minutes = 0
		FROM dbo.LateInEarlyOutYearly ly
		JOIN #EmployeeLastYearBalance tmp ON ly.EmpId = tmp.EmpId
		WHERE (tmp.IsMaxAL = 1 AND PreYearAL > 0) OR (tmp.IsMaxAL = 0 AND PreYearAL > 3) AND tmp.PreYearLIEO > 0
		
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
            ,'Transfer AL to ' + CONVERT(NVARCHAR(10), @Year + 1) + ''
            ,CONVERT(NVARCHAR(10), @Year) + '/12/31'
            ,NULL
            ,CASE WHEN e.PreYearAL > 3 THEN 3 ELSE e.PreYearAL END LastYearMaxAL
            ,0
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.PreYearAL > 0 AND e.IsMaxAL = 0
		
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
            ,CASE WHEN e.PreYearAL > 3 THEN 3 ELSE e.PreYearAL END LastYearMaxAL
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.PreYearAL > 0 AND e.IsMaxAL = 0

        COMMIT TRANSACTION;

		DROP TABLE #LastYearLIEO;
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
