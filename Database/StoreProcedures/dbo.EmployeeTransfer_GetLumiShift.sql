USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeTransfer_GetLumiShift]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
