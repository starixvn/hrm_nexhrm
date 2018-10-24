
CREATE PROCEDURE [dbo].[Employee_GetMembesByManager] @managerID INT
    ,@role INT
    ,@excludedManager BIT
    ,@groupID INT = NULL
    ,@keyword NVARCHAR(100)
AS
/*
Created: 2012/05/24@Dan
Task: Get list of employees for a leader/manager
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @searchText NVARCHAR(100)

    IF (ISNULL(@keyword, '') <> '')
        SET @searchText = '%' + @keyword + '%'

    PRINT @searchText

    SELECT e.Id
        ,e.Code
        ,e.NickName
        ,e.FullName
        ,e.Email
        ,e.DomainUsername
    FROM dbo.Employee e WITH (NOLOCK)
    WHERE (
            ISNULL(@excludedManager, 0) = 0
            OR id <> @managerID
            )
        AND (
            (
                @groupid IS NULL
                AND @role >= 3
                )
            OR (
                @groupid IS NULL
                AND @role IN (1, 2)
                AND ID IN (
                    SELECT empid
                    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
                    WHERE eg.GroupId IN (
                            SELECT Id
                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                            WHERE LeaderId = @managerID
                                OR AssistantLeaderId = @managerID
                            )
                        AND ISNULL(HandleRequest, 0) = 1
                    )
                )
            OR (
                @groupid IS NOT NULL
                AND ID IN (
                    SELECT empid
                    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
                    WHERE eg.GroupId = @groupid
                        AND ISNULL(HandleRequest, 0) = 1
                    )
                )
            )
        AND (
            @searchText IS NULL
            OR FullName LIKE @searchText
            OR NickName LIKE @searchText
            )
    ORDER BY [FullName]
END

