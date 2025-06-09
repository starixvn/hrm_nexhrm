CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_Get]
AS
/*
Created: 20230404@Brian.nguyen
Task: Get all conversation of employee
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:19TyupFcn7CJOya4b7d76pGpzpmppzjOzppdk8JttHuU' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		-- AND e.GroupId = 9
		-- AND e.Id = 125
		-- AND e.NickName = 'Frankie'
END

GO
