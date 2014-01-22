USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_GetScore]    Script Date: 7/25/2013 4:53:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TOEIC_GetScore]
	@empId int
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
	
	SELECT TOP 1 EmpId,
		LRExamDate, Listening, OldListening, Reading, OldReading,
		SWExamDate, Speaking, OldSpeaking, Writing, OldWriting,
		InputDate
	FROM TOEIC (NOLOCK)
	WHERE EmpId = @empId
END