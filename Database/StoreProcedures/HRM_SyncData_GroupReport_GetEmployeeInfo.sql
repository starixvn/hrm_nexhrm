CREATE OR ALTER PROCEDURE [dbo].[HRM_SyncData_GroupReport_GetEmployeeInfo] @LastModifiedDate DATETIME = NULL
AS
/*
Created: 20250220@Tim
Task: get employee info
DB: NexHRM
Original:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id AS EmpId
	    ,dept.Code AS DepartmentCode
	    ,dept.[Name] AS DepartmentName
	    ,team.Code AS TeamCode
	    ,team.[Name] AS TeamName
		,e.Code
		,e.NickName
		,e.FullName
        ,e.DOB
        ,e.Gender
        ,jl.[Name] AS JobLevel
	    ,jt.[Name] AS JobTitle
        ,e.GroupDate AS JoinedDate
		,e.JoinedDate AS CompanyDate
		,e.ConfirmedDate
		,e.ResignedDate
        ,CASE WHEN team.[Name] = 'Expat' THEN 0 ELSE 1 END AS IsLocal
		,e.[Enabled]
		,e.IsDeleted
		,e.IsSpecialUser
		,e.ModifiedDate 
	FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS team WITH (NOLOCK) ON e.GroupId = team.Id
        LEFT JOIN HRM.dbo.EmployeeJobGrade AS ejl WITH (NOLOCK) ON e.Id = ejl.EmpId
        AND ejl.IsCurrentUsed = 1
        LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON team.DepartmentId = dept.DepartmentId
        LEFT JOIN HRM.dbo.JobGrade AS jl WITH (NOLOCK) ON ejl.JobGradeId = jl.JobGradeId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE @LastModifiedDate IS NULL
        OR e.ModifiedDate > @LastModifiedDate
        OR team.ModifiedDate > @LastModifiedDate
        OR dept.ModifiedDate > @LastModifiedDate
        OR ejl.ModifiedDate > @LastModifiedDate
        OR jl.ModifiedDate > @LastModifiedDate
        OR jt.ModifiedDate > @LastModifiedDate
END
GO
