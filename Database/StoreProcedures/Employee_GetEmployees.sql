USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetEmployees]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetEmployees] @query NVARCHAR(256)
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@jobTitleId INT = NULL
	,@IsDisable BIT = 0
    ,@isDelete BIT = 0
    ,@pageNumber INT = 1
    ,@pageSize INT = 2000
    ,@isSpecial BIT = NULL
    ,@totalRow INT OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Get employee by conditions.
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	20200505@Merlin.Le: Add isSpecial IN PARAM
	20250308@Henry.Dinh: Add management grade
    20250514@Henry.Dinh: Add departmentId IN PARAM
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)

    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT @totalRow = COUNT(1)
    FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.ID
    WHERE (
            @isDelete IS NULL
            OR isDeleted = @isDelete
            )
		AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
        AND (
            @query IS NULL
            OR NickName LIKE @q
            OR FullName LIKE @q
            OR DomainUsername LIKE @q
            OR e.Code LIKE @q
            )
        AND (
            @groupId IS NULL
            OR GroupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR DepartmentId = @departmentId
            )

    SELECT e.[Id]
		,e.[Id] as UserId
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[GroupDate]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,d.DepartmentId
		,d.[Name] AS DepartmentName
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.AffectedDate
		,gra.Note
		,mgra.Code AS ManagementGradeCode
		,mgra.Name AS ManagementGradeName
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy] CreatorId
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[isDeleted]
        ,[PermanentAddress]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,LRExamDate
        ,Listening
        ,Reading
        ,Speaking
        ,Writing
        ,[GroupId]
        ,g.Code
        ,g.NAME
        ,g.[Rank]
        ,e.JobTitleId
        ,jt.[Name]
    FROM (
        SELECT row_number() OVER (
                ORDER BY NickName ASC
                ) AS RowNumber
            ,em.[Id]
            ,em.[Code]
            ,[DomainUsername]
            ,[FullName]
            ,[NickName]
            ,[DOB]
            ,[Email]
            ,[Enabled]
            ,[Phone]
            ,[Gender]
            ,[Roles]
            ,GroupId
			,[GroupDate]
            ,JobTitleId
            ,[JoinedDate]
            ,[ConfirmedDate]
            ,[ResignedDate]
            ,[Skype]
            ,[Yahoo]
            ,[Marriage]
            ,em.[CreatedDate]
            ,em.[CreatedBy]
            ,em.[ModifiedDate]
            ,em.[ModifiedBy]
            ,[isDeleted]
            ,[PermanentAddress]
            ,[CurrentAddress]
            ,[IdCardNumber]
            ,[PassportNo]
            ,[PassportExp]
            ,[GeoMapLatitude]
            ,[GeoMapLongtitude]
            ,[DistanceToCompany]
            ,[EmployeeCard]
            ,[IssuedDate]
            ,[IssuedPlace]
			,[PlaceOfBirth]
			,[ContactName]
			,[UserRelationship]
			,[ContactPhone]
			,[ContactAddress]
			,[SocialInsuranceNo]
        FROM dbo.Employee AS em WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS emg WITH (NOLOCK) ON em.GroupId = emg.ID
        WHERE 
            (IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
            AND (
                @query IS NULL
                OR NickName LIKE @q
                OR FullName LIKE @q
                OR DomainUsername LIKE @q
                OR em.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @jobTitleId IS NULL
                OR JobTitleId = @jobTitleId
                )
            AND (
                @departmentId IS NULL
                OR emg.DepartmentId = @departmentId
                )
        ) AS e
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
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
		LEFT JOIN (
			SELECT emg.EmpId
			,emg.AffectedDate
			,emg.Note
			,mg.ManagementGradeId
			,mg.Code
			,mg.Name
			,mg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			  WHERE emg.IsCurrentUsed = 1
			  AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId
    WHERE RowNumber BETWEEN (@pageNumber - 1) * @pageSize
            AND @pageNumber * @pageSize
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

END
GO
