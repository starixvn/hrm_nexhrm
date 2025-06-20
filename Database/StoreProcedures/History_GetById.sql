CREATE OR ALTER PROCEDURE [dbo].[History_GetById]
    @Id INT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Get History by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,h.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,hd.GroupId
	,hd.GroupName
	,hd.JobTitleId
	,hd.JobGradeId
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
    WHERE h.Id = @Id
END
GO
