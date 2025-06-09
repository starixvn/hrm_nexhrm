USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetBirthdayAndFirstWorkingday]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Employee_GetBirthdayAndFirstWorkingday] @OnDate DATETIME
AS
/*
Created: 20160218@Quinn.Vo
Task: get employee has birthday or first working day on date.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SELECT e.Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB
		,e.DOB AS DateOfBirth
		,e.Email
		,e.JobTitleId
		,e.Enabled
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.GroupDate AS JoinedDate
		,e.ConfirmedDate
		,e.Skype
		,e.Yahoo
		,e.Marriage
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND MONTH(DOB) = MONTH(@OnDate)
		AND DAY(DOB) = DAY(@OnDate)
		AND (
			ResignedDate IS NULL
			OR DATEDIFF(DD, dbo.fnc_GetClientDateTime(), ResignedDate) >= 0
			)
		AND e.Enabled = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0

	SELECT e.Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB AS DateOfBirth
		,e.Email
		,e.JobTitleId
		,e.Enabled
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.GroupDate AS JoinedDate
		,e.ConfirmedDate
		,e.Skype
		,e.Yahoo
		,e.Marriage
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND MONTH(GroupDate) = MONTH(@OnDate)
		AND DAY(GroupDate) = DAY(@OnDate)
		AND GroupDate < convert(VARCHAR(10), dbo.fnc_GetClientDateTime(), 111)
		AND (
			ResignedDate IS NULL
			OR DATEDIFF(DD, dbo.fnc_GetClientDateTime(), ResignedDate) >= 0
			)
		AND e.Enabled = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
		
		EXEC [CompanyEvent_GetEventInWeek]
END
GO
