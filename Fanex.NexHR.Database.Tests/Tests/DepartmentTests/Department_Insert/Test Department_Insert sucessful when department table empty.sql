CREATE PROCEDURE [DepartmentTests].[Test Department_Insert sucessful when department table empty]
AS
BEGIN
	-- Create Actual and Expected table to hold the actual results
	CREATE TABLE [DepartmentTests].Actual (
		[DepartmentId] INT NULL
		,[Code] VARCHAR(20) NULL
		,[Name] NVARCHAR(100) NULL
		,[ParentId] INT NULL
		,[ManagerId] INT NULL
		,[DepartmentLevel] INT NULL
		,[DisplayOrder] INT NULL
		,[IsDisabled] BIT NULL
		,[CreatedBy] INT NULL
		,[CreatedDate] DATETIME NULL
		,[ModifiedBy] INT NULL
		,[ModifiedDate] DATETIME NULL
		)

	CREATE TABLE [DepartmentTests].Expected (
		[DepartmentId] INT NULL
		,[Code] VARCHAR(20) NULL
		,[Name] NVARCHAR(100) NULL
		,[ParentId] INT NULL
		,[ManagerId] INT NULL
		,[DepartmentLevel] INT NULL
		,[DisplayOrder] INT NULL
		,[IsDisabled] BIT NULL
		,[CreatedBy] INT NULL
		,[CreatedDate] DATETIME NULL
		,[ModifiedBy] INT NULL
		,[ModifiedDate] DATETIME NULL
		)

	--Fake Table - Isolate this test from the real data
	EXEC tSQLt.FakeTable 'dbo.Department'

	INSERT INTO [DepartmentTests].Expected (
		[Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[DepartmentLevel]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		'Web'
		,'Web Team'
		,NULL
		,1
		,0
		,0
		,1
		,'2018-10-24'
		)

		DECLARE @ExpectedErrCode INT = 0
			,@ExpectedErrMsg VARCHAR(200) = ''
			,@ActualErrCode INT = 0
			,@ActualErrMsg VARCHAR(200) = ''
		
		-- Execution
		EXEC dbo.Department_Insert 
			@Code = 'Web'
			,@Name = 'Web Team'
			,@ParentId = NULL
			,@ManagerId = 1
			,@CreatedBy = 1
			,@CreatedDate = '2018-10-24'
			,@ErrCode = @ActualErrCode OUT
			,@ErrMsg = @ActualErrMsg OUT

	INSERT [DepartmentTests].[Actual] (
		[Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[DepartmentLevel]
		,[DisplayOrder]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		,[ModifiedBy]
		,[ModifiedDate]
		)
	SELECT [Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[DepartmentLevel]
		,[DisplayOrder]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		,[ModifiedBy]
		,[ModifiedDate]
	FROM dbo.Department

	DECLARE @expectedRowCount INT
	DECLARE @actualRowCount INT
	SET @expectedRowCount = 1

	SET @actualRowCount = (
			SELECT COUNT(1)
			FROM [DepartmentTests].[Actual]
			)

	-- Assert
	EXEC tSQLt.AssertEquals @expectedRowCount
		,@actualRowCount
		,'The number of rows returned is incorrect'

	EXEC tSQLt.AssertEquals @ExpectedErrCode
		,@ActualErrCode
		,'The ErrCode returned is incorrect'
	EXEC tSQLt.AssertEquals @ExpectedErrMsg
		,@ActualErrMsg
		,'The ErrMsg returned is incorrect'

	EXEC tSQLt.AssertEqualsTable '[DepartmentTests].[Expected]'
		,'[DepartmentTests].[Actual]'
		,'The column order in the result set is incorrect'
END
