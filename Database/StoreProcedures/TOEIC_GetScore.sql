USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_GetScore]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




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
