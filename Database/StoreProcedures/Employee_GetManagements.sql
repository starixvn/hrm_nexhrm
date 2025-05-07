SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[Employee_GetManagements]
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

    SELECT DISTINCT e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.Email
		,con.ConversationId
    FROM [dbo].[Employee] AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE e.GroupId = 1081
	AND e.isDeleted = 0
END


GO
