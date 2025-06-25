CREATE OR ALTER PROCEDURE [dbo].[Employee_GetHR]
AS
	/*
	Created: 
	Task: get HR for Nex request.
	DB: NexHR
	Original: 

	Revisions:    
		20250507@Henry.Dinh: update nickname
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
	WHERE e.NickName IN ('Maria', 'Hoa')
END
GO
