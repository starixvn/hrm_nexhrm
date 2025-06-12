CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeTOEIC_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee toeic by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT EmpId, 
		MinimumScore, 
		CAST(InputDate AS DATE) AS InputDate, 
		CAST(LRExamDate AS DATE) AS LRExamDate, 
		Listening, 
		Reading,
		ModifiedDate
	FROM  dbo.TOEIC_ WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
END