USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Supervisor_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--[Supervisor_Get] ''
CREATE OR ALTER PROCEDURE [dbo].[Supervisor_Get] 
    @keyword NVARCHAR(256),
    @groupId INT = NULL,
    @departmentId INT = NULL
AS
/*
Created: 20240516@Lilac.Nguyen
Task: Get Supervisor detail.
DB: NexHRM
Original: 

Revisions:  
    20250516@Henry.Dinh: Add filter by department ID
Param's Explanation:
    @keyword: Search keyword for employee name
    @groupId: Filter by group ID
    @departmentId: Filter by department ID
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @q NVARCHAR(256)

    SET @q = '%' + @keyword + '%'

    IF LEN(@keyword) = 0
        SET @keyword = NULL

	SELECT
		e.Id AS EmpId,
		e.NickName,
		e.FullName,
		d.[Name] AS DepartmentName,
		g.[Name] AS GroupName,
		STRING_AGG(CONVERT(NVARCHAR(MAX), sup.ApproverNickName), ', ') AS Approvers
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON sup.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g ON e.GroupId = g.Id
		LEFT JOIN dbo.Department AS d ON g.DepartmentId = d.DepartmentId
	WHERE 
		ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] <> 0 
		AND e.IsSpecialUser = 0
		AND (
			@keyword IS NULL
			OR e.NickName LIKE @q
			OR e.FullName LIKE @q
		)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
		)
		AND (
			@departmentId IS NULL
			OR g.DepartmentId = @departmentId
		)
	GROUP BY 
		e.Id,
		e.NickName,
		e.FullName,
		d.Name,
		g.[Name],
		g.[Rank],
		g.Code,
		e.Roles,
		e.GroupDate
	ORDER BY 
		d.[Name],
		g.[Rank],
		g.Code,
		e.Roles DESC,
		e.GroupDate
END
GO
