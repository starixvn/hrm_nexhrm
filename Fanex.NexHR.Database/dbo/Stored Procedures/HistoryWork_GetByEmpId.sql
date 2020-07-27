CREATE PROCEDURE [dbo].[HistoryWork_GetByEmpId]
	@EmpId INT
AS
/*
Created: 20191212@Lilac.Nguyen
Task: Get History Work by Employee Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.EmpId
		,sm.AffectedDate
		,sm.ModifiedBy
		,sm.ModifiedDate
		,sm.JobtitleName
		,sm.GroupName
		,sm.Id
	FROM dbo.HistoryWork sm WITH (NOLOCK)		
	WHERE sm.EmpId = @EmpId
END
