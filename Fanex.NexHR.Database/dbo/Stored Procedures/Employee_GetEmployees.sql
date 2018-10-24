

CREATE PROCEDURE [dbo].[Employee_GetEmployees] @query NVARCHAR(256)
    ,@groupId INT = NULL
    ,@jobTitleId INT = NULL
    ,@enable BIT = NULL
    ,@isDelete BIT = NULL
    ,@pageNumber INT = 1
    ,@pageSize INT = 2000
    ,@totalRow INT OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Get employee by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @q NVARCHAR(256)

    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT @totalRow = COUNT(1)
    FROM dbo.Employee WITH (NOLOCK)
    WHERE (
            @isDelete IS NULL
            OR isDeleted = @isDelete
            )
        AND (
            @isDelete = 1
            OR (
                (
                    (
                        @isDelete = 0
                        OR @isDelete IS NULL
                        )
                    AND (
                        isDeleted = 0
                        OR isDeleted IS NULL
                        )
                    )
                AND (
                    @enable IS NULL
                    OR Enabled = @enable
                    )
                )
            )
        AND (
            @query IS NULL
            OR NickName LIKE @q
            OR FullName LIKE @q
            OR DomainUsername LIKE @q
            OR Code LIKE @q
            )
        AND (
            @groupId IS NULL
            OR GroupId = @groupId
            )

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[ManagerId]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Mariage]
        ,[LastLogin]
        ,e.[CreatedDate]
        ,e.[CreatedBy]
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
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
        ,(CASE e.Id
            WHEN g.LeaderId THEN 2 -- employee is leader
            WHEN g.AssistantLeaderId THEN 1 -- employee is assistant leader
            ELSE 0 END
            ) AS EmployeeRank
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
        ,lk.Code
        ,lk.NAME
    FROM (
        SELECT row_number() OVER (
                ORDER BY NickName ASC
                ) AS RowNumber
            ,[Id]
            ,[Code]
            ,[DomainUsername]
            ,[FullName]
            ,[NickName]
            ,[DOB]
            ,[Email]
            ,[Enabled]
            ,[Phone]
            ,[Gender]
            ,[Roles]
            ,[ManagerId]
            ,GroupId
            ,JobTitleId
            ,[JoinedDate]
            ,[ConfirmedDate]
            ,[ResignedDate]
            ,[Skype]
            ,[Yahoo]
            ,[Mariage]
            ,[LastLogin]
            ,[CreatedDate]
            ,[CreatedBy]
            ,[ModifiedDate]
            ,[ModifiedBy]
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
        FROM Employee WITH (NOLOCK)
        WHERE 
			IsSpecialUser = 0
			AND (
                @isDelete IS NULL
                OR isDeleted = @isDelete
                )
            AND (
                @isDelete = 1
                OR (
                    (
                        (
                            @isDelete = 0
                            OR @isDelete IS NULL
                            )
                        AND (
                            isDeleted = 0
                            OR isDeleted IS NULL
                            )
                        )
                    AND (
                        @enable IS NULL
                        OR Enabled = @enable
                        )
                    )
                )
            AND (
                @query IS NULL
                OR NickName LIKE @q
                OR FullName LIKE @q
                OR DomainUsername LIKE @q
                OR Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @jobTitleId IS NULL
                OR JobTitleId = @jobTitleId
                )
        ) AS e
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.LookupData AS LK WITH (NOLOCK) ON e.JobTitleId = lk.ID
        LEFT JOIN dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
    WHERE RowNumber BETWEEN (@pageNumber - 1) * @pageSize
            AND @pageNumber * @pageSize
    ORDER BY g.[Rank]
        ,g.Code
        ,EmployeeRank DESC
        ,e.JoinedDate
END


