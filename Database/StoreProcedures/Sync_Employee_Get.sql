CREATE OR ALTER   PROCEDURE [dbo].[Sync_Employee_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT ISNULL(e.OldId, e.Id) AS EmployeeId, 
		e.DomainUsername, 
		e.Code, 
		e.FullName, 
		e.NickName, 
		ISNULL(eg.OldId, e.GroupId) AS GroupId, 
		e.[Enabled], 
		e.isDeleted, 
		CAST(e.JoinedDate AS DATE) AS JoinedDate, 
		CAST(e.ConfirmedDate AS DATE) 
        AS ConfirmedDate,
		e.JobTitleId,
		e.Email,
		ec.ConversationId,
		e.ModifiedDate
	FROM dbo.Employee AS e WITH (NOLOCK) 
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT OUTER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE e.ModifiedDate > @LastModifiedDate
END