USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Sync_GO_UnitHead_GetInfo]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_GO_UnitHead_GetInfo]
AS
/*
Created: 20230307@Tim.Trinh
Task: get unit head info
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.Id AS EmpId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.[Enabled] AS IsActive
		,emp.IsDeleted
		,gr.Code AS DepartmentCode
		,gr.[Name] AS DepartmentName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON emp.GroupId = gr.Id
	WHERE emp.Code IN ('02197')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
END

GO
