CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiShift] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Shift
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
	FROM LumiHRM.dbo.[Shift] WITH (NOLOCK) 
	WHERE IsDisabled = 0
END
GO
