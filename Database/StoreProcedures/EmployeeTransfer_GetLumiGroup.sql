CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiGroup] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Group
DB: NexHRM
Original: 

Revisions:
	- 20250627@Henry.Dinh: Update to use SystemSetting to get CompanyId
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	-- Declare variables
	DECLARE @SystemSettingValue INT
	DECLARE @MappedCompanyId INT

	-- Get SystemSetting value
	SELECT @SystemSettingValue = Value 
	FROM [dbo].[SystemSetting] WITH (NOLOCK) 
	WHERE Id = 4

	-- Map CompanyId based on SystemSetting value
	SET @MappedCompanyId = CASE 
		WHEN @SystemSettingValue = 1 THEN 2
		WHEN @SystemSettingValue = 2 THEN 1
		ELSE @SystemSettingValue
	END

	SELECT g.[GroupId] AS Id,
		g.[Code],
		g.[Name]
	FROM [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) 
	WHERE ISNULL(IsDeleted, 0) = 0 AND g.CompanyId = @MappedCompanyId
END
GO
