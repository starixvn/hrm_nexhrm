/****** Object:  Stored Procedure dbo.Employee_GetBirthdayAndFirstWorkingday    Script Date: 3/15/2016 10:01:02 AM ******/

CREATE PROCEDURE [dbo].[Employee_GetBirthdayAndFirstWorkingday] @OnDate DATETIME
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
		,e.Phone
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.ManagerId
		,e.JoinedDate
		,e.ConfirmedDate
		,e.ResignedDate
		,e.Skype
		,e.Yahoo
		,e.Mariage
		,e.LastLogin
		,e.AnnualLeaveBalance
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
		,e.PermanentAddress AS Address
		,e.CurrentAddress
		,e.IdCardNumber AS CardNo
		,e.PassportNo
		,e.PassportExp
	--,isnull(g.ID, -1) as [TeamID], isnull(g.Name,'') as [Team]
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
			OR DATEDIFF(DD, GETDATE(), ResignedDate) >= 0
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
		,e.Phone
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.ManagerId
		,e.JoinedDate
		,e.ConfirmedDate
		,e.ResignedDate
		,e.Skype
		,e.Yahoo
		,e.Mariage
		,e.LastLogin
		,e.AnnualLeaveBalance
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
		,e.PermanentAddress AS Address
		,e.CurrentAddress
		,e.IdCardNumber AS CardNo
		,e.PassportNo
		,e.PassportExp
	-- ,isnull(g.ID, -1) as [TeamID], isnull(g.Name,'') as [Team]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND MONTH(JoinedDate) = MONTH(@OnDate)
		AND DAY(JoinedDate) = DAY(@OnDate)
		AND JoinedDate < convert(VARCHAR(10), getdate(), 111)
		AND (
			ResignedDate IS NULL
			OR DATEDIFF(DD, GETDATE(), ResignedDate) >= 0
			)
		AND e.Enabled = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
		
		EXEC [CompanyEvent_GetEventInWeek]
END