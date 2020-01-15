CREATE PROCEDURE [dbo].[Employee_GetHR]
AS
/*
Created: 
Task: get HR for Nex request.
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
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.NickName IN ('Albert', 'Maria', 'Hoa')
END