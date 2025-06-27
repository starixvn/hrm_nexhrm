CREATE OR ALTER PROCEDURE [dbo].[Employee_GetManagements]
AS
/*
  Created: 20150703@Tim.Trinh
  Task: get management that can handle unpaid leave request for employee. 
  DB: NexHR
  Original: 

  Revisions:  
    20250507@Henry.Dinh: update group id from 11 to 1081
  Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CoreTeamId INT = 0
	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    SELECT DISTINCT e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.Email
		,con.ConversationId
    FROM [dbo].[Employee] AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE e.GroupId = @CoreTeamId
	AND e.isDeleted = 0
END


GO
