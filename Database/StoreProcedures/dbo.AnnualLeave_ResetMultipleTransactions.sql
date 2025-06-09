USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[AnnualLeave_ResetMultipleTransactions]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_ResetMultipleTransactions] @EmpValue NVARCHAR(MAX) = NULL
	,@Description NVARCHAR(4000) = NULL
	,@FromDate DATETIME
	,@ResetDays NUMERIC(18, 1)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/12/24@Edward
Task: reset anual balance
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
    @EmpValue:
        <ROOT><Employee Id="1" LeaveBalance="2"/></ROOT>
*/
BEGIN
	SET XACT_ABORT ON;

	DECLARE @i INT
		,@TempEmpId INT
		,@TempLeaveBalance NUMERIC(18, 1)
		,@xml XML
	DECLARE @Emp TABLE (
		Id INT IDENTITY(1, 1)
		,EmpId INT
		,LeaveBalance FLOAT
		)

	SET @xml = CONVERT(XML, @EmpValue)

	INSERT INTO @Emp (
		EmpId
		,LeaveBalance
		)
	SELECT T.C.value('./@Id', 'int')
		,T.C.value('./@LeaveBalance', 'numeric(18,1)')
	FROM @xml.nodes('/ROOT/Employee') AS [T](C) --T: table, C: column

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
					,@Description
					,@FromDate
					,@TempLeaveBalance
					,1
					,@CreatedBy
					,@ErrCode
					,@ErrMsg
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
					,@Description
					,@FromDate
					,@TempLeaveBalance
					,0
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
