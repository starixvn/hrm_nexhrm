CREATE OR ALTER   PROCEDURE [dbo].[Employee_GetEmployeeByTeams] @employeeId INT
    ,@teamCodes VARCHAR(200) = ''
    ,@ignoreTeams VARCHAR(200) = ''
AS
/*  
Created: 20120810@Quinn.Vo
Task: Send mail for all Staff in PM team
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #TeamCodeList (GroupTeamId INT);

    CREATE TABLE #IgnoreTeamList (GroupTeamId INT);

    INSERT INTO #TeamCodeList (GroupTeamId)
    SELECT val
    FROM fnc_strSplit(@teamCodes, ',');

    INSERT INTO #IgnoreTeamList (GroupTeamId)
    SELECT val
    FROM fnc_strSplit(@ignoreTeams, ',');

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Employee AS e WITH (NOLOCK)
                    INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.Id = @employeeId
                    AND g.GroupTeamId NOT IN (
                        SELECT GroupTeamId
                        FROM #IgnoreTeamList
                        )
                )
            )
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
            INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON eg.GroupId = g.Id
            INNER JOIN dbo.Employee AS e ON e.Id = eg.EmpID
        WHERE g.GroupTeamId IN (
                SELECT GroupTeamId
                FROM #TeamCodeList
                )
            AND (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
	       AND e.[Enabled] = 1
		   AND (ISNULL(eg.HandleRequest, 0) = 0 OR isMainGroup = 1)
    END;
    ELSE
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.Employee AS e WITH (NOLOCK)
        WHERE 1 = 0;
    END;

    DROP TABLE #TeamCodeList;
END;
