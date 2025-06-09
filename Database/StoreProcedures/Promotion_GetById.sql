USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Promotion_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Promotion_GetById]
    @Id INT
AS
/*
Created: 20240103@Lilac.Nguyen
Task: Get Promotion by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ej.Id
	,ej.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,ej.JobGradeId
	,ej.OldJobGradeId
	,ej.AffectedDate
	,ej.Note
    FROM HRM.dbo.EmployeeJobGrade AS ej WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ej.EmpId
    WHERE ej.Id = @Id

END

GO
