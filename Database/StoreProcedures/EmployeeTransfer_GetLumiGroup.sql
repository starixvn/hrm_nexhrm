USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeTransfer_GetLumiGroup]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
