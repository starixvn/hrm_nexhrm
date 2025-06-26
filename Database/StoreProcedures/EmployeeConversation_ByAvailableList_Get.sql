USE [NexHRM]
GO

CREATE OR ALTER   PROCEDURE [dbo].[EmployeeConversation_ByAvailableList_Get]
	@EmployeeJson NVARCHAR(MAX),
	@OnDate DATETIME
AS
/*
    Created: 20250624@Jay.Do
    Task: Attach conversation id to employee
    DB: NexHRM

    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @TempEmployeeConversation TABLE (
		EmpId INT,
		SysId INT,
		NickName NVARCHAR(200),
		CompanyId INT
	)

	INSERT INTO @TempEmployeeConversation
	SELECT DISTINCT
		EmpId,
		SysId,
		NickName,
		CompanyId
	FROM OPENJSON(@EmployeeJson)
	WITH (
		EmpId INT,
		SysId INT,
		NickName NVARCHAR(200),
		CompanyId INT
	)

	SELECT e.Id AS EmpId,
		tec.SysId,
		tec.NickName,
		ec.ConversationId,
		@OnDate AS OnDate,
		tec.CompanyId
	FROM Employee e
	INNER JOIN @TempEmployeeConversation tec ON e.OldId = tec.EmpId
	INNER JOIN EmployeeConversation ec ON ec.EmpId = e.Id
	WHERE e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.[Enabled] = 1

END
GO

USE [LumiHRM]
GO

CREATE OR ALTER   PROCEDURE [dbo].[EmployeeConversation_ByAvailableList_Get]
	@EmployeeJson NVARCHAR(MAX),
	@OnDate DATETIME
AS
/*
    Created: 20250624@Jay.Do
    Task: Attach conversation id to employee
    DB: NexHRM

    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @TempEmployeeConversation TABLE (
		EmpId INT,
		SysId INT,
		NickName NVARCHAR(200),
		CompanyId INT
	)

	INSERT INTO @TempEmployeeConversation
	SELECT DISTINCT
		EmpId,
		SysId,
		NickName,
		CompanyId
	FROM OPENJSON(@EmployeeJson)
	WITH (
		EmpId INT,
		SysId INT,
		NickName NVARCHAR(200),
		CompanyId INT
	)

	SELECT e.Id AS EmpId,
		tec.SysId,
		tec.NickName,
		ec.ConversationId,
		@OnDate AS OnDate,
		tec.CompanyId
	FROM Employee e
	INNER JOIN @TempEmployeeConversation tec ON e.OldId = tec.EmpId
	INNER JOIN EmployeeConversation ec ON ec.EmpId = e.Id
	WHERE e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.[Enabled] = 1

END
GO