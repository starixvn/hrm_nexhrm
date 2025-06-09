USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetStaffInfo]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Employee_GetStaffInfo] @query NVARCHAR(256)
    ,@groupId INT = NULL
	,@isSpecial BIT = NULL
	,@departmentId INT = NULL
AS
/*
Created: 20230519@Quinn.Vo
Task: Get staff info by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
@departmentId - Optional parameter to filter by department
*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)
    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
        ,[DOB] AS DateOfBirth
        ,[Email]
        ,[Phone]
        ,[JoinedDate]
		,[GroupDate]
        ,[Skype]
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
        ,jt.NAME AS JobTitle
        ,d.Name AS DepartmentName
        FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
        WHERE 
            (e.IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND e.Enabled = 1
			AND e.isDeleted = 0
            AND (
                @query IS NULL
                OR e.NickName LIKE @q
                OR e.FullName LIKE @q
                OR e.DomainUsername LIKE @q
                OR e.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @departmentId IS NULL
                OR g.DepartmentId = @departmentId
                )
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
END

GO
