USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_Update]    Script Date: 7/25/2013 4:54:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TOEIC_Update] 
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
Task: update employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT 1 FROM TOEIC (NOLOCK) WHERE EmpId = @empId)
	BEGIN -- update
		UPDATE TOEIC WITH (UPDLOCK,ROWLOCK)
		SET Listening = @listening, Reading = @reading, LRExamDate = @lrExamDate, OldListening = @oldListening, OldReading = @oldReading,
			Speaking = @speaking, Writing = @writing, SWExamDate = @swExamDate, OldSpeaking = @oldSpeaking, OldWriting = @oldWriting,
			InputDate = @inputDate, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate
		WHERE EmpId = @empId
	END
	ELSE
	BEGIN -- insert
		EXEC TOEIC_Insert @empId, @listening, @reading, @lrExamDate, @oldListening, @oldReading,
			@speaking, @writing, @swExamDate, @oldSpeaking, @oldWriting, @inputDate,
			@modifiedBy, @modifiedDate, @createdBy, @createdDate
	END
END