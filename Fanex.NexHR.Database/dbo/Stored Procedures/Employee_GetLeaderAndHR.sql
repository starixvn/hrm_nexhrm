
CREATE PROCEDURE [dbo].[Employee_GetLeaderAndHR] @employeeId INT
AS
/*
Created: QuinnVo@20160621
Task: get leader and assistant leader and HR for Nex request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT e.ID
		,e.NickName
		,e.FullName
		,e.roles
		,e.GroupID
		,e.Email
	FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeGroup AS g ON eg.Groupid = g.ID
		INNER JOIN Employee AS e WITH (NOLOCK) ON (
			e.ID = g.LeaderId
			AND g.LeaderId <> @employeeId
			)
		OR (
			e.ID = g.AssistantLeaderId
			AND g.AssistantLeaderId <> @employeeId
			)
	WHERE eg.EmpID = @employeeId
		AND ISNULL(HandleRequest, 0) = 1
		AND (ISNULL(LeaderId, 0) <> @employeeId)
		AND e.Id <> @employeeId
	
	UNION
	SELECT DISTINCT e.ID
		,e.NickName
		,e.FullName
		,e.roles
		,e.GroupID
		,e.Email
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.NickName IN ('Albert', 'Maria', 'Hoa')
END
