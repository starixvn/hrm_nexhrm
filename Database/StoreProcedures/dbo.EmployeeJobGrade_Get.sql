USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeJobGrade_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[EmployeeJobGrade_Get] 
	 @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@IsOnlyCurrentUsed BIT = 1
AS
/*
Created: 20240104@Quinn.Vo
Task: Get Employee Job Grade
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,e.GroupId
		,eg.Code AS TeamCode
		,eg.Name AS TeamName
		,gra.JobGradeId
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.AffectedDate
		,gra.Description
		,gra.Note
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ((@IsOnlyCurrentUsed = 1 AND ejg.IsCurrentUsed = 1) OR (@IsOnlyCurrentUsed = 0))
				AND ISNULL(ejg.IsDeleted,0) = 0 
		) AS gra ON e.Id = gra.EmpId
	WHERE e.isDeleted = 0
	AND e.IsSpecialUser = 0
	AND  (@GroupId IS NULL
				OR e.GroupId = @GroupId
            )
			AND (
				@EmployeeId IS NULL
				OR e.Id = @EmployeeId
            )
	ORDER BY eg.[Rank]
	,eg.Code
	,e.Roles DESC
	,e.GroupDate
	,gra.AffectedDate
END


GO
