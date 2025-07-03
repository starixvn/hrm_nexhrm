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
	
	SELECT ISNULL(e.OldId, e.Id) AS EmpId, 
		toeic.MinimumScore, 
		CAST(toeic.InputDate AS DATE) AS InputDate, 
		CAST(toeic.LRExamDate AS DATE) AS LRExamDate, 
		toeic.Listening, 
		toeic.Reading,
		ISNULL(toeic.ModifiedDate, toeic.CreatedDate) AS ModifiedDate
	FROM  HRM.dbo.TOEIC AS toeic WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = toeic.EmpId
	WHERE ISNULL(toeic.ModifiedDate, toeic.CreatedDate) > @LastModifiedDate
END