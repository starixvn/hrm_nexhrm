CREATE PROCEDURE [DepartmentTests].[Test Department_Insert return correct error when Department Code exists]
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

	INSERT INTO dbo.Department (
		[DepartmentId]
		,[Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[DepartmentLevel]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		1
		,'Web'
		,'Web Team'
		,NULL
		,1
		,0
		,0
		,1
		,'2018-10-24'
		),
		(2
		,'Nex'
		,'Nexus'
		,1
		,2
		,1
		,0
		,2
		,'2018-10-24'
		)

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
		),
		('Nex'
		,'Nexus'
		,1
		,2
		,1
		,0
		,2
		,'2018-10-24'
		)

		DECLARE @ExpectedErrCode INT = 1
			,@ExpectedErrMsg VARCHAR(200) = N'Department code Web has already exists'
			,@ActualErrCode INT = 0
			,@ActualErrMsg VARCHAR(200) = ''
		
		-- Execution
		EXEC dbo.Department_Insert 
			@Code = 'Web'
			,@Name = 'Nexus'
			,@ParentId = 1
			,@ManagerId = 2
			,@CreatedBy = 2
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
	SET @expectedRowCount = 2

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
