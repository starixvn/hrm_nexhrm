CREATE PROCEDURE [DepartmentTests].[Test Department_Get procedure only return childs when ParentId not null]
AS
BEGIN
	-- Create Actual and Expected table to hold the actual results of Department_Get SP
	CREATE TABLE [DepartmentTests].Actual (
		[Id] INT NULL
		,[Code] VARCHAR(20) NULL
		,[Name] NVARCHAR(100) NULL
		,[ParentId] INT NULL
		,[ParentName] NVARCHAR(100) NULL
		,[ModifiedDate] DATETIME NULL
		,[CreatedDate] DATETIME NULL
		,[IsDisabled] BIT NULL
		,[DepartmentLevel] INT NULL
		,[DisplayOrder] INT NULL
		,[CreatedBy] INT NULL
		,[CreatedByFullName] NVARCHAR(50)
		,[CreatedByNickName] NVARCHAR(20)
		,[ManagerId] INT NULL
		,[ManagerFullName] NVARCHAR(50)
		,[ManagerNickName] NVARCHAR(20)
		,[ModifiedBy] INT NULL
		,[ModifiedByFullName] NVARCHAR(50)
		,[ModifiedByNickName] NVARCHAR(20)
		)

	CREATE TABLE [DepartmentTests].Expected (
		[Id] INT NULL
		,[Code] VARCHAR(20) NULL
		,[Name] NVARCHAR(100) NULL
		,[ParentId] INT NULL
		,[ParentName] NVARCHAR(100) NULL
		,[ModifiedDate] DATETIME NULL
		,[CreatedDate] DATETIME NULL
		,[IsDisabled] BIT NULL
		,[DepartmentLevel] INT NULL
		,[DisplayOrder] INT NULL
		,[CreatedBy] INT NULL
		,[CreatedByFullName] NVARCHAR(50)
		,[CreatedByNickName] NVARCHAR(20)
		,[ManagerId] INT NULL
		,[ManagerFullName] NVARCHAR(50)
		,[ManagerNickName] NVARCHAR(20)
		,[ModifiedBy] INT NULL
		,[ModifiedByFullName] NVARCHAR(50)
		,[ModifiedByNickName] NVARCHAR(20)
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
		,[DisplayOrder]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		,[ModifiedBy]
		,[ModifiedDate]
		)
	VALUES (
		1
		,'Web'
		,'Web Team'
		,NULL
		,1
		,0
		,0
		,0
		,1
		,'2018-10-24'
		,1
		,'2018-10-24'
		)
		,(
		2
		,'Nex'
		,'Nexus'
		,1
		,2
		,1
		,1
		,0
		,2
		,'2018-10-24'
		,2
		,'2018-10-24'
		)

	EXEC tSQLt.FakeTable 'dbo.Employee'

	INSERT INTO dbo.Employee (
		[Id]
		,[FullName]
		,[NickName]
		)
	VALUES (
		1
		,'Phạm Nguyễn Thanh Thiện'
		,'Thien'
		)
		,(
		2
		,'Võ Kim Quang'
		,'Quinn'
		)

	-- Create the expected output
	INSERT [DepartmentTests].[Expected] (
		[Id]
		,[Code]
		,[Name]
		,[ParentId]
		,[ParentName]
		,[ModifiedDate]
		,[CreatedDate]
		,[IsDisabled]
		,[DepartmentLevel]
		,[DisplayOrder]
		,[CreatedBy]
		,[CreatedByFullName]
		,[CreatedByNickName]
		,[ManagerId]
		,[ManagerFullName]
		,[ManagerNickName]
		,[ModifiedBy]
		,[ModifiedByFullName]
		,[ModifiedByNickName]
		)
	VALUES (
		2
		,'Nex'
		,'Nexus'
		,1
		,'Web Team'
		,'2018-10-24'
		,'2018-10-24'
		,0
		,1
		,1
		,2
		,'Võ Kim Quang'
		,'Quinn'
		,2
		,'Võ Kim Quang'
		,'Quinn'
		,2
		,'Võ Kim Quang'
		,'Quinn'
		)

	--Execution
	INSERT [DepartmentTests].[Actual] (
		[Id]
		,[Code]
		,[Name]
		,[ParentId]
		,[ParentName]
		,[ModifiedDate]
		,[CreatedDate]
		,[IsDisabled]
		,[DepartmentLevel]
		,[DisplayOrder]
		,[CreatedBy]
		,[CreatedByFullName]
		,[CreatedByNickName]
		,[ManagerId]
		,[ManagerFullName]
		,[ManagerNickName]
		,[ModifiedBy]
		,[ModifiedByFullName]
		,[ModifiedByNickName]
		)
	EXEC dbo.Department_Get 1
		,NULL

	-- Check that Expected and Actual tables contain the same results
	DECLARE @expectedRowCount INT;

	SET @expectedRowCount = 1;

	DECLARE @actualRowCount INT;

	SET @actualRowCount = (
			SELECT COUNT(1)
			FROM [DepartmentTests].[Actual]
			)

	--! Assert
	EXEC tSQLt.AssertEquals @expectedRowCount
		,@actualRowCount
		,'The number of rows returned is incorrect'

	EXEC tSQLt.AssertEqualsTable '[DepartmentTests].[Expected]'
		,'[DepartmentTests].[Actual]'
		,'The column order in the result set is incorrect'
END