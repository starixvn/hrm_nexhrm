CREATE OR ALTER PROCEDURE [dbo].[Employee_Insert_CheckExist] @Nickname NVARCHAR(20)
	,@Code VARCHAR(20)
AS
/*
Created: 20250306@Tim
Task: check code, nickname available
DB: HRM

Revisions:
    20250306@Tim: initial - [Redmine: #217976]
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CompanyId INT

	SELECT @CompanyId = [Value]
    FROM NexHRM.dbo.SystemSetting WITH (NOLOCK)
    WHERE [Name] = 'CompanyId'

	DECLARE @EmpId_exist INT
		,@Nickname_exist NVARCHAR(20)
		,@FullName_exist NVARCHAR(50)

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = Nickname
	FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = e.EmpId 
	WHERE Code = @Code
		AND ce.CompanyId = @CompanyId

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT 1 AS [Code]
			,N'Code ' + @Code + ' has been assigned ' + @FullName_exist AS [Message]

		RETURN
	END

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = Nickname
	FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = E.EmpId 
	WHERE NickName = @Nickname
		AND ce.CompanyId = @CompanyId
		AND (
			e.isDeleted = 0
			OR e.[Enabled] = 1
			)

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT 2 AS [Code]
			,N'Nickname  ' + @Nickname_exist + '  has been assigned to ' + @FullName_exist AS [Message]

		RETURN
	END

    SELECT 0 AS [Code], NULL AS [Message]
END