USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetByModifiedDate]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByModifiedDate] @ModifiedDate DateTime
AS
/*
Created: 20230205@Quinn.Vo
Task: Get employee info by username.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.[Id]
	,e.[Code]
	,e.DomainUsername
	,e.[FullName]
	,e.[NickName] AS EnglishName
	,e.DOB AS DateOfBirth
	,e.PlaceOfBirth
	,e.[Email]
	,e.EmployeeCard
	,g.Code AS TeamCode
	,g.Name AS TeamName
	,jt.Name AS JobTitle
	,gra.Code AS JobGrade
	,e.GroupDate
	,e.JoinedDate
	,e.ConfirmedDate
	,e.ResignedDate
	,e.Phone
	,(CASE WHEN e.Gender = 0 THEN 'F'
		WHEN e.Gender = 1 THEN 'M'
		ELSE 'Unknown' END) AS Gender
	,e.IdCardNumber
	,e.IssuedDate
	,e.IssuedPlace
	,e.Phone
	,e.CurrentAddress
	,e.PermanentAddress
	,e.PassportNo
	,e.PassportExp
	,e.SocialInsuranceNo
	,e.Skype
	,e.Yahoo AS Telegram
	,e.[Enabled]
	,e.[IsDeleted]
	,e.Marriage
	,e.CreatedDate
	,e.ModifiedDate
	,e.Enabled
	,e.isDeleted
	,e.IsSpecialUser
	,e.ContactName AS EmergencyContact
	,e.UserRelationship AS EmergencyRelationship
	,e.ContactPhone AS EmergencyPhone
	,e.ContactAddress AS EmergencyAddress
	,t.LRExamDate AS ToeicExamDate
    ,t.Listening AS ToeicListening
	,t.Reading AS ToeicReading
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.NickName ELSE egs.NickName END AS SupervisorEnglishName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.FullName ELSE egs.FullName END AS SupervisorFullName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.Email ELSE egs.Email END AS SupervisorEmail
	FROM dbo.Employee AS e WITH (NOLOCK) 
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON gs.GroupId = e.GroupId
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON es.EmpId = e.Id
		LEFT JOIN dbo.Employee AS egs WITH (NOLOCK) ON gs.ApproverId = egs.Id
		LEFT JOIN dbo.Employee AS egs2 WITH (NOLOCK) ON es.ApproverId = egs2.Id
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
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
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
	WHERE e.ModifiedDate BETWEEN @ModifiedDate AND dbo.fnc_GetClientDateTime()
END
GO
