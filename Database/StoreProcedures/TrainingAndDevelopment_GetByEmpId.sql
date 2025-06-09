USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopment_GetByEmpId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_GetByEmpId]
	 @EmpId INT
	,@CurrentUserId INT
AS
/*
Created: 20230418@Brian.Nguyen
Task: Get Training And Development by employee id
DB: NexHR
Original: 

Revisions:    
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
		cc.Publish
	FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
			ON cc.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
			ON e.GroupId = eg.Id
		INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
			ON cc.TypeId = tt.Id
	WHERE  @EmpId = cc.EmpId
			AND(
				@UserRoles IN ('3','4')
				OR cc.Publish LIKE 'Public'
				OR (cc.Publish LIKE 'Protected' AND 
				(cc.EmpId = @UserId 
					OR e.GroupId = @UserGroupId 
					OR EXISTS(SELECT 1 FROM dbo.EmployeeSupDetail WITH (NOLOCK)
					WHERE EmpId = cc.EmpId AND ApproverId = @UserId) 
				))
				OR (cc.Publish LIKE 'Private' AND cc.EmpId = @UserId )
			)
END


GO
