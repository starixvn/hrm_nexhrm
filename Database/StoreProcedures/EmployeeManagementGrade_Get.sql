CREATE OR ALTER PROCEDURE [dbo].[EmployeeManagementGrade_Get] 
	 @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@IsOnlyCurrentUsed BIT = 1
AS
/*
Created: 20252602@Henry.Dinh
Task: Get Employee Management Grade
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
		,gra.ManagementGradeId
		,gra.Code AS ManagementGradeCode
		,gra.Name AS ManagementGradeName
		,gra.AffectedDate
		,gra.Description
		,gra.Note
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.ManagementGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as jg WITH (NOLOCK) on ejg.ManagementGradeId = jg.ManagementGradeId
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
