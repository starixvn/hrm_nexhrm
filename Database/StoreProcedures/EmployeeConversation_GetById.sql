CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_GetById]
	@EmpId INT
AS
/*
Created: 20230404@Brian.nguyen
Task: Get conversation of employee from employee id
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.Id AS EmpId
        ,e.NickName 
		,ec.ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.isDeleted = 0
		AND e.Enabled = 1
		AND e.Id = @EmpId

END

GO
