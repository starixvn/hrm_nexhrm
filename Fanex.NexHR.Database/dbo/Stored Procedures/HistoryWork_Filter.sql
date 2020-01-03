CREATE PROCEDURE [dbo].[HistoryWork_Filter]
	@EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
AS
/*
Created: 
Task: Get History Work filter date time
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT 
		sm.Id
		,sm.EmpId
		,sm.AffectedDate
		,sm.ModifiedBy		
		,sm.ModifiedDate
		,sm.JobtitleName
		,sm.GroupName
		,e.NickName
		
	FROM dbo.HistoryWork sm WITH (NOLOCK)	
		LEFT JOIN dbo.Employee e WITH (NOLOCK) ON sm.ModifiedBy = e.Id
	WHERE sm.EmpId = @EmpId and @FromDate <= sm.ModifiedDate and @ToDate  >= sm.ModifiedDate
END
