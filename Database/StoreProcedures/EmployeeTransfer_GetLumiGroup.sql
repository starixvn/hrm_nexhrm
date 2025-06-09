CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiGroup] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Group
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT [Id],
		[Code],
		[Name]
	FROM LumiHRM.dbo.EmployeeGroup WITH (NOLOCK) 
	WHERE ISNULL(Deleted, 0) = 0
END
GO
