CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetUpdatingScoreRelatedPeople] @id INT
AS
/*
    Created: 20150403@Tim.Trinh
    Task: Get related people to send mail when update toeic.
    DB: NexEPA
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
    Param's Explanation:
*/
BEGIN
    DECLARE @CoreTeamId INT = 0;
	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
    WHERE e.ID = @id
    
    UNION
    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON e.GroupId = gr.Id
    WHERE gr.Id = @CoreTeamId
	AND e.isDeleted = 0
    
    UNION
    SELECT DISTINCT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM dbo.Employee e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON e.Id = sup.ApproverId
    WHERE e.isDeleted = 0
END
GO
