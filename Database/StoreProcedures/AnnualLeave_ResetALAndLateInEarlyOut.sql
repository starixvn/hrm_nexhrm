CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_ResetALAndLateInEarlyOut] 
AS

BEGIN
	SET XACT_ABORT ON;
	DECLARE @Description NVARCHAR(4000) = NULL
	,@FromDate DATETIME = '2024-02-01'
	,@ResetDays NUMERIC(18, 1)
	,@CreatedBy INT
	,@ErrCode INT = 0
	,@ErrMsg VARCHAR(200) = ''

	DECLARE @i INT
		,@TempEmpId INT
		,@TempLeaveBalance NUMERIC(18, 1)
		,@xml XML
	DECLARE @Emp TABLE (
		Id INT IDENTITY(1, 1)
		,EmpId INT
		,LeaveBalance FLOAT
		)

	INSERT INTO @Emp (
		EmpId
		,LeaveBalance
		)
	SELECT a.EmpId, (a.CreditDays - b.DeductDays) Balance from 
(select EmpId, CreditDays from LeaveTrans where FromDate = '2024-01-01' AND ToDate = '2024-01-31')
as a 
INNER JOIN dbo.Employee AS e ON a.EmpId = e.Id AND e.IsSpecialUser = 0 AND e.NickName NOT IN ('Sean', 'Joe', 'Daniel')
LEFT JOIN
(select EmpId, SUM(DeductDays) AS DeductDays from LeaveTrans where ISNULL(Deleted,0) = 0 AND FromDate >= '2024-01-01' AND (ToDate IS NULL OR ToDate <= '2024-01-31')
GROUP BY EmpId) AS b ON a.EmpId = b.EmpId
WHERE a.CreditDays - b.DeductDays > 0
AND e.NickName = 'Bill'

	BEGIN TRY
		BEGIN TRANSACTION;

		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM @Emp
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempLeaveBalance = LeaveBalance
			FROM @Emp
			WHERE Id = @i

			IF (
					EXISTS (
						SELECT 1
						FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
						WHERE EmpId = @TempEmpId
							AND [Year] = YEAR(@FromDate)
							AND [Minutes] > 0
						)
					)
			BEGIN
				EXEC [dbo].[LeaveDeduction_Insert] @TempEmpId
					,'Deduct 0.5 day to reset annual leave and late in/early out'
					,@FromDate
					,0.5
					,1
					,125
					,@ErrCode
					,@ErrMsg
				
				SET @TempLeaveBalance = @TempLeaveBalance - 0.5
				IF @TempLeaveBalance > 0
				BEGIN
					INSERT INTO dbo.LeaveTrans (
						EmpId
						,Description
						,FromDate
						,DeductDays
						,CreditDays
						,CreatedDate
						,CreatedBy
						,IsCloseBalance
						)
					SELECT @TempEmpId
						,'Close unused AL balance of 2023'
						,@FromDate
						,@TempLeaveBalance
						,0
						,dbo.fnc_GetClientDateTime()
						,@CreatedBy
						,1
				END
			END
			ELSE
			BEGIN
				INSERT INTO dbo.LeaveTrans (
					EmpId
					,Description
					,FromDate
					,DeductDays
					,CreditDays
					,CreatedDate
					,CreatedBy
					,IsCloseBalance
					)
				SELECT @TempEmpId
					,'Close unused AL balance of 2023'
					,@FromDate
					,@TempLeaveBalance
					,1
					,dbo.fnc_GetClientDateTime()
					,@CreatedBy
					,1
			END

			SET @i = @i + 1
		END

		UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
		SET [Minutes] = 0
		WHERE [Minutes] < 0
			AND [Year] = YEAR(@FromDate)

		CREATE TABLE #EmpBanlance (
			EmpId INT
			,LateInEarlyOut INT
			,NexBalance INT
			)

		INSERT INTO #EmpBanlance (
			EmpId
			,LateInEarlyOut
			,NexBalance
			)
		SELECT la.EmpId
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) < 0 THEN 0
				ELSE (la.Minutes - ISNULL(ot.Minutes, 0))
				END AS Late
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) > 0 THEN 0
				ELSE (ISNULL(ot.Minutes, 0) - la.Minutes)
				END AS OT
		FROM (
			SELECT *
			FROM dbo.[LateInEarlyOutYearly] WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS la
			LEFT JOIN (
			SELECT *
			FROM dbo.OvertimeYearly WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS ot ON la.EmpId = ot.EmpId

		INSERT INTO [dbo].[LateInEarlyOutYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,LateInEarlyOut
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[LateInEarlyOutYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		INSERT INTO [dbo].[OvertimeYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,NexBalance
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		DROP TABLE #EmpBanlance

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset leave transaction unsuccessfully.'
	END CATCH;
END

GO
