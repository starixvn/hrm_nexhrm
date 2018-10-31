CREATE PROCEDURE [AnnualLeaveTests].[Test AnnualLeave_DeleteTransaction return correct error when this trans is deleted before]
AS
BEGIN
	-- Create Actual and Expected table to hold the actual results
	CREATE TABLE [AnnualLeaveTests].Actual (
		[Id] INT NULL
		,[EmpId] INT NULL
		,[FromDate] DATETIME NULL
		,[ToDate] DATETIME NULL
		,[DeductDays] NUMERIC(18,1) NULL
		,[CreditDays] NUMERIC(18,1) NULL
		,[Deleted] BIT NULL
		)

	CREATE TABLE [AnnualLeaveTests].Expected (
		[Id] INT NULL
		,[EmpId] INT NULL
		,[FromDate] DATETIME NULL
		,[ToDate] DATETIME NULL
		,[DeductDays] NUMERIC(18,1) NULL
		,[CreditDays] NUMERIC(18,1) NULL
		,[Deleted] BIT NULL
		)

	--Fake Table - Isolate this test from the real data
	EXEC tSQLt.FakeTable 'dbo.LeaveTrans'
	EXEC tSQLt.FakeTable 'dbo.Employee'

	INSERT INTO dbo.LeaveTrans(
		Id
		,EmpId
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,Deleted
	)
	VALUES (
		1, 1, '2018-10-01', '2018-10-01', 2, 0, 1
	)

	INSERT INTO dbo.Employee (
		Id
		,AnnualLeaveBalance
		)
	VALUES (
		1
		,10
		)

	-- Create the expected output
	INSERT INTO [AnnualLeaveTests].Expected(
		Id
		,EmpId
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,Deleted
	)
	VALUES (
		1, 1, '2018-10-01', '2018-10-01', 2, 0, 1
	)

	--Execution
	DECLARE @ExpectedErrCode INT = 1
			,@ExpectedErrMsg VARCHAR(200) = N'Delete leave transaction unsuccessfully.'
			,@ActualErrCode INT = 0
			,@ActualErrMsg VARCHAR(200) = ''
			,@expectedRowCount INT = 1
			,@actualRowCount INT
			,@expectedAnnuaLeaveBalance NUMERIC(18,1) = 10
			,@actualAnnuaLeaveBalance NUMERIC(18,1)

	SELECT @ExpectedErrCode = 1

	EXEC dbo.AnnualLeave_DeleteTransaction 
		@LeaveTransactionId = 1
		,@ModifiedBy = 1
		,@ErrCode = @ActualErrCode OUT
		,@ErrMsg = @ActualErrMsg OUT

	INSERT INTO [AnnualLeaveTests].Actual(
		Id
		,EmpId
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,Deleted
	)
	SELECT Id
		,EmpId
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,Deleted
	FROM dbo.LeaveTrans

	SET @actualRowCount = (
			SELECT COUNT(1)
			FROM [AnnualLeaveTests].[Actual]
			)
	SELECT @actualAnnuaLeaveBalance = AnnualLeaveBalance
	FROM dbo.Employee 
	WHERE Id = 1

	-- Assert
	EXEC tSQLt.AssertEquals @expectedAnnuaLeaveBalance
		,@actualAnnuaLeaveBalance
		,'The Annual Leave Balance returned is incorrect'

	EXEC tSQLt.AssertEquals @expectedRowCount
		,@actualRowCount
		,'The number of rows returned is incorrect'

	EXEC tSQLt.AssertEquals @ExpectedErrCode
		,@ActualErrCode
		,'The ErrCode returned is incorrect'
	EXEC tSQLt.AssertEquals @ExpectedErrMsg
		,@ActualErrMsg
		,'The ErrMsg returned is incorrect'

	EXEC tSQLt.AssertEqualsTable '[AnnualLeaveTests].[Expected]'
		,'[AnnualLeaveTests].[Actual]'
		,'The column order in the result set is incorrect'

END