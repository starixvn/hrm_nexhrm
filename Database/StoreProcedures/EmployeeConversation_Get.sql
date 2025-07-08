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

	SELECT e.OldId AS Id
        ,e.NickName 
		,ec.ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
END

GO
