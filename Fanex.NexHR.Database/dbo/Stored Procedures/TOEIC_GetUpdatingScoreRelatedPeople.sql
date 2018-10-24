
CREATE PROCEDURE [dbo].[TOEIC_GetUpdatingScoreRelatedPeople] @id INT
AS
/*
Created: 20150403@Tim.Trinh
Task: Get related people to send mail when update toeic.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
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
    WHERE e.GroupId = 11
	AND ISNULL(e.IsSpecialUser, 0) = 0
    
    UNION
    SELECT DISTINCT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
        INNER JOIN [dbo].[EmployeeGroup] g WITH (NOLOCK) ON e.ID = g.LeaderId
        OR e.ID = g.AssistantLeaderId
        INNER JOIN [dbo].[EmployeeGroup_Employee] eg WITH (NOLOCK) ON eg.GroupId = g.ID
    WHERE ISNULL(HandleRequest, 0) = 1
	AND ISNULL(g.Deleted, 0) = 0
	AND ISNULL(e.IsSpecialUser, 0) = 0
END

