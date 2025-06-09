CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_TransferBalance] 
AS

BEGIN
	DECLARE @FromDate DATETIME = '2023-12-31',
		@CreatedBy INT = 125

		--CREATE TABLE #EmpBanlance (
		--	EmpId INT
		--	,LateInEarlyOut INT
		--	,NexBalance INT
		--	)

		--INSERT INTO #EmpBanlance (
		--	EmpId
		--	,LateInEarlyOut
		--	,NexBalance
		--	)
		--SELECT la.EmpId
		--	,CASE 
		--		WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) < 0 THEN 0
		--		ELSE (la.Minutes - ISNULL(ot.Minutes, 0))
		--		END AS Late
		--	,CASE 
		--		WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) > 0 THEN 0
		--		ELSE (ISNULL(ot.Minutes, 0) - la.Minutes)
		--		END AS OT
		--FROM (
		--	SELECT *
		--	FROM dbo.[LateInEarlyOutYearly] WITH (NOLOCK)
		--	WHERE [Year] = YEAR(@FromDate)
		--	) AS la
		--	LEFT JOIN (
		--	SELECT *
		--	FROM dbo.OvertimeYearly WITH (NOLOCK)
		--	WHERE [Year] = YEAR(@FromDate)
		--	) AS ot ON la.EmpId = ot.EmpId

		--INSERT INTO [dbo].[LateInEarlyOutYearly] (
		--	[EmpId]
		--	,[Year]
		--	,[Minutes]
		--	,[ModifiedBy]
		--	,[ModifiedDate]
		--	)
		--SELECT EmpId
		--	,YEAR(@FromDate) + 1
		--	,LateInEarlyOut
		--	,@CreatedBy
		--	,dbo.fnc_GetClientDateTime()
		--FROM #EmpBanlance AS bl
		--WHERE NOT EXISTS (
		--SELECT 1 
		--FROM [dbo].[LateInEarlyOutYearly] WITH (NOLOCK) 
		--WHERE bl.EmpId = EmpId AND [Year] = (YEAR(@FromDate) + 1))

		--INSERT INTO [dbo].[OvertimeYearly] (
		--	[EmpId]
		--	,[Year]
		--	,[Minutes]
		--	,[ModifiedBy]
		--	,[ModifiedDate]
		--	)
		--SELECT EmpId
		--	,YEAR(@FromDate) + 1
		--	,NexBalance
		--	,@CreatedBy
		--	,dbo.fnc_GetClientDateTime()
		--FROM #EmpBanlance AS bl
		--WHERE NOT EXISTS (
		--SELECT 1 
		--FROM [dbo].[OvertimeYearly] WITH (NOLOCK) 
		--WHERE bl.EmpId = EmpId AND [Year] = (YEAR(@FromDate) + 1))

		--DROP TABLE #EmpBanlance

		CREATE TABLE #EmpLeaveBanlance (
			EmpId INT
			,Balance NUMERIC(18,1)
			)

	INSERT INTO #EmpLeaveBanlance(EmpId, Balance)
	SELECT EmpId, (SUM(CreditDays) - SUM(DeductDays)) AS AnnualLeaveBalance
	FROM dbo.LeaveTrans as a WITH (NOLOCK)
	INNER JOIN dbo.Employee AS b WITH (NOLOCK) ON a.EmpId = b.Id AND b.isDeleted = 0
	WHERE 
		 ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@FromDate)
	GROUP BY EmpId
	HAVING (SUM(CreditDays) - SUM(DeductDays)) > 0

	
	INSERT INTO dbo.LeaveTrans(
		 [EmpId]
      ,[Description]
      ,[FromDate]
      ,[DeductDays]
      ,[CreditDays]
      ,[CreatedDate]
      ,[CreatedBy]
	)
	SELECT EmpId,
		'Transfer Balance to 2024',
		@FromDate,
		Balance,
		0,
		dbo.fnc_GetClientDateTime(),
		@CreatedBy
	FROM #EmpLeaveBanlance

	INSERT INTO dbo.LeaveTrans(
		 [EmpId]
      ,[Description]
      ,[FromDate]
	  ,[ToDate]
      ,[DeductDays]
      ,[CreditDays]
      ,[CreatedDate]
      ,[CreatedBy]
	)
	SELECT EmpId,
		'Transfer Balance from 2023',
		'2024-01-01',
		'2024-01-31',
		0,
		Balance,
		dbo.fnc_GetClientDateTime(),
		@CreatedBy
	FROM #EmpLeaveBanlance

	DROP TABLE #EmpLeaveBanlance
END

GO
