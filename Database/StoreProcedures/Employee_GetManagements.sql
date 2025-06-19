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

	DECLARE @CoreTeamId INT
	SELECT @CoreTeamId = GroupId 
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'CoreTeamCode'
	WHERE gs.OldGroupCode = ss.Value

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
