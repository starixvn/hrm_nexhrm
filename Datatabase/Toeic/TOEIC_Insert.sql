USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_Insert]    Script Date: 7/25/2013 4:53:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TOEIC_Insert] 
	@empId int,
	@listening int = null,
	@reading int = null,
	@lrExamDate datetime = null,
	@oldListening int = null,
	@oldReading int = null,
	@speaking int = null,
	@writing int = null,
	@swExamDate datetime = null,
	@oldSpeaking int = null,
	@oldWriting int = null,
	@inputDate datetime = null,
	@modifiedBy int = null,
	@modifiedDate datetime = null,
	@createdBy int = null,
	@createdDate datetime = null
AS
/*
Created: 
Task: create new employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO TOEIC(EmpId, Listening, Reading, LRExamDate, OldListening, OldReading,
		Speaking, Writing, SWExamDate, OldSpeaking, OldWriting, InputDate,
		ModifiedBy, ModifiedDate, CreatedBy, CreatedDate)
	VALUES(@empId, @listening, @reading, @lrExamDate, @oldListening, @oldReading,
		@speaking, @writing, @swExamDate, @oldSpeaking, @oldWriting, @inputDate,
		@modifiedBy, @modifiedDate, @createdBy, @createdDate)
END