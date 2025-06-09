USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopment_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_GetById]
	 @Id INT
AS
/*
Created: 20230418@Brian.Nguyen
Task: get Training And Development by id
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

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
		cc.[Certificate]
	FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
			ON cc.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
			ON e.GroupId = eg.Id
		INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
			ON cc.TypeId = tt.Id
	WHERE  @Id = cc.Id
END


GO
