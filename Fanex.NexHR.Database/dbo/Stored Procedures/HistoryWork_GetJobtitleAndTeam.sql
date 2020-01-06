CREATE PROCEDURE [dbo].[HistoryWork_GetJobtitleAndTeam] @Id INT
AS
/*
Created: 
Task: get employee jobtitle anf team info
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT AffectedDate
		,GroupId
		,GroupName
		,JobtitleId
		,JobtitleName
		,EmpId
				
	FROM HistoryWork(NOLOCK)
	WHERE Id = @Id
END