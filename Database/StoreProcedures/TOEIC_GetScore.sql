CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetScore] @empId INT
AS
/*
Created: 
Task: get employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1 EmpId
		,LRExamDate
		,Listening
		,OldListening
		,Reading
		,OldReading
		,SWExamDate
		,Speaking
		,OldSpeaking
		,Writing
		,OldWriting
		,InputDate
		,MinimumScore
	FROM HRM.dbo.TOEIC WITH(NOLOCK)
	WHERE EmpId = @empId
END

GO
