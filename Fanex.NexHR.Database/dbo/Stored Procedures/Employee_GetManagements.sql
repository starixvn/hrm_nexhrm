

CREATE PROCEDURE [dbo].[Employee_GetManagements]
AS
/*
Created: 20150703@Tim.Trinh
Task: get management that can handle unpaid leave request for employee. 
DB: NexHR
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT e.Id
        ,e.Code
        ,e.NickName
        ,e.FullName
        ,e.Email
        ,e.DomainUsername
		,con.ConversationId
    FROM [dbo].[Employee] AS e WITH (NOLOCK)
        INNER JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.[GroupId] = g.[Id]
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE g.[ID] = 11
END

