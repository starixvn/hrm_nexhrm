CREATE PROCEDURE [dbo].[Employee_GetHR]
AS
/*
Created: 20200909Dwight.Tran
Task: Enhance NEX Request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @HRCode	VARCHAR (3) = 'HR';

	SELECT e.ID
		,e.NickName
		,e.FullName
		,e.roles
		,e.GroupID
		,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
	INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
	WHERE eg.[Code] = @HRCode
END