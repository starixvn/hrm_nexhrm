CREATE PROCEDURE [AnnualLeaveTests].[Test AnnualLeave_GetById return correct Transaction when TransactionId exists]
AS
BEGIN
	--Create Actual and Expected table to hold the actual results
	CREATE TABLE [AnnualLeaveTests].Actual (
		[LeaveTransactionId] INT NULL
		,[RequestId] BIGINT NULL
		,[AffectedFrom] DATETIME NULL
		,[ToTalDays] NUMERIC(18,1) NULL
		,[Description] NVARCHAR(4000) NULL
		,[CreatedDate] DATETIME NULL
		,[EmpId] INT NULL
		,[NickName] NVARCHAR(20)
		,[FullName] NVARCHAR(50)
		)

	CREATE TABLE [AnnualLeaveTests].Expected (
		[LeaveTransactionId] INT NULL
		,[RequestId] BIGINT NULL
		,[AffectedFrom] DATETIME NULL
		,[ToTalDays] NUMERIC(18,1) NULL
		,[Description] NVARCHAR(4000) NULL
		,[CreatedDate] DATETIME NULL
		,[EmpId] INT NULL
		,[NickName] NVARCHAR(20)
		,[FullName] NVARCHAR(50)
		)

	--Fake Table - Isolate this test from the real data
	EXEC tSQLt.FakeTable 'dbo.LeaveTrans'
	EXEC tSQLt.FakeTable 'dbo.Employee'

	INSERT INTO dbo.LeaveTrans(
		Id
		,EmpId
		,RequestId
		,FromDate
		,DeductDays
		,CreditDays
		,[Description]
		,CreatedDate
		,Deleted
	)
	VALUES (
		1, 1, NULL, '2018-10-01', 2, 0, 'test', '2018-10-30', 0
	),
	(2, 1, 4, '2018-10-01', 0, 1, 'test2', '2018-10-30', 1)
	,
	(3, 1, 1, '2018-10-01', 0, 1, 'test3', '2018-10-30', 0)

	INSERT INTO dbo.Employee (
		Id
		,NickName
		,FullName
		)
	VALUES (
		1
		,'Quinn'
		,'Võ Kim Quang'
		)

	-- Create the expected output
	INSERT INTO [AnnualLeaveTests].Expected(
		[LeaveTransactionId]
		,[RequestId]
		,[AffectedFrom]
		,[ToTalDays]
		,[Description]
		,[CreatedDate]
		,[EmpId]
		,[NickName]
		,[FullName]
	)
	VALUES (
		1, NULL, '2018-10-01', -2, 'test', '2018-10-30', 1, 'Quinn', 'Võ Kim Quang'
	)

	--Execution
	DECLARE @expectedRowCount INT = 1
			,@actualRowCount INT

	INSERT INTO [AnnualLeaveTests].Actual(
		[LeaveTransactionId]
		,[RequestId]
		,[AffectedFrom]
		,[ToTalDays]
		,[Description]
		,[CreatedDate]
		,[EmpId]
		,[NickName]
		,[FullName]
	)
	EXEC [dbo].[AnnualLeave_GetByID] @LeaveTransactionId = 1

	SET @actualRowCount = (
			SELECT COUNT(1)
			FROM [AnnualLeaveTests].[Actual]
			)

	-- Assert
	EXEC tSQLt.AssertEquals @expectedRowCount
		,@actualRowCount
		,'The number of rows returned is incorrect'

	EXEC tSQLt.AssertEqualsTable '[AnnualLeaveTests].[Expected]'
		,'[AnnualLeaveTests].[Actual]'
		,'The column order in the result set is incorrect'

END