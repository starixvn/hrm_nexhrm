SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Get]
	 @GroupId INT = NULL
	,@EmpId INT = NULL
	,@CurrentUserId INT
	,@TypeId INT = NULL
	,@Name NVARCHAR(200) = NULL
	,@FromDate DATETIME = NULL
	,@ToDate DATETIME = NULL
AS
/*
Created: 20230417@Brian.Nguyen
Task: Get Training And Development
DB: NexHR
Original: 

Revisions:    
- 20241202@Lilac: Add certificate upload
- 20250520@Henry: Add default value for FromDate and ToDate
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
			@UserId INT
			,@UserRoles NVARCHAR(20)
			,@UserGroupId INT

	SELECT @UserId = Id, @UserRoles = Roles, @UserGroupId = GroupId
	FROM dbo.Employee WITH (NOLOCK) 
	WHERE Id = @CurrentUserId

	SELECT 
		cc.Id,
		cc.EmpId,
		e.NickName, 
		e.FullName,
		e.GroupId,
		cc.ConductedBy,
		cc.Description,
		eg.Name AS TeamName,
		eg.Code AS TeamCode,
		cc.TypeId, 
		tt.Name AS TypeName,
		cc.Name, 
		cc.StartDate, 
		cc.EndDate, 
		cc.ExamDate, 
		cc.ExpirationDate, 
		cc.Organization,
		cc.Publish,
		cc.Certificate
	FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
			ON cc.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
			ON e.GroupId = eg.Id
		INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
			ON cc.TypeId = tt.Id
	WHERE  e.Enabled = 1
		AND e.isDeleted = 0
		AND e.IsSpecialUser = 0
		AND (@GroupId IS NULL
				OR e.GroupId = @GroupId
            )
			AND (
				@EmpId IS NULL
				OR e.Id = @EmpId
            )
			AND (
				@TypeId IS NULL
				OR @TypeId = cc.TypeId
            )
			AND (
				@Name IS NULL
				OR cc.Name Like ('%' + @Name + '%')
            )
			AND (
				@FromDate IS NULL
				OR cc.CreatedDate >= @FromDate
            )
			AND (
				@ToDate IS NULL
				OR cc.CreatedDate <= @ToDate
            )
			AND(
				@UserRoles IN ('3','4')
				OR cc.Publish LIKE 'Public'
				OR (cc.Publish LIKE 'Protected' AND 
				(cc.EmpId = @UserId 
					OR e.GroupId = @UserGroupId 
					OR EXISTS(SELECT 1 FROM dbo.EmployeeSupDetail WITH (NOLOCK)
					WHERE EmpId = cc.EmpId AND ApproverId = @UserId) 
				))
				OR (cc.Publish LIKE 'Private' AND cc.EmpId = @UserId)
			)

	ORDER BY cc.CreatedDate DESC

END
GO
