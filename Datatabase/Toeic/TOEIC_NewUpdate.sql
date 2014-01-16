USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_NewUpdate]    Script Date: 7/25/2013 4:54:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TOEIC_NewUpdate]
	@empId int,
	@listening int = null,
	@reading int = null,
	@lrExamDate datetime = null,
	@speaking int = null,
	@writing int = null,
	@swExamDate datetime = null,
	@inputDate datetime = null,
	@modifiedBy int = null,
	@modifiedDate datetime = null,
	@createdBy int = null,
	@createdDate datetime = null
AS
/*
Created: 
Task: only update new TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
		IF EXISTS(SELECT 1 FROM TOEIC (NOLOCK) WHERE EmpId = @empId)
		BEGIN -- update
			IF ((@listening + @reading) > 0 AND (@speaking + @writing) > 0) -- LR & SW
			BEGIN
				UPDATE TOEIC WITH (UPDLOCK,ROWLOCK)
				SET Listening = @listening, Reading = @reading, LRExamDate = @lrExamDate,
					Speaking = @speaking, Writing = @writing, SWExamDate = @swExamDate, 
					InputDate = @inputDate, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate
				WHERE EmpId = @empId
			END
			IF (@listening + @reading > 0) -- LR
			BEGIN
				UPDATE TOEIC WITH (UPDLOCK,ROWLOCK)
				SET Listening = @listening, Reading = @reading, LRExamDate = @lrExamDate,
					InputDate = @inputDate, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate
				WHERE EmpId = @empId
			END
			ELSE IF (@speaking + @writing > 0) -- SW
			BEGIN
				UPDATE TOEIC WITH (UPDLOCK,ROWLOCK)
				SET Speaking = @speaking, Writing = @writing, SWExamDate = @swExamDate, 
					InputDate = @inputDate, ModifiedBy = @modifiedBy, ModifiedDate = @modifiedDate
				WHERE EmpId = @empId
			END
		END
		ELSE
		BEGIN -- insert
			EXEC TOEIC_Insert @empId, @listening, @reading, @lrExamDate, null, null,
				@speaking, @writing, @swExamDate, null, null, @inputDate,
				@modifiedBy, @modifiedDate, @createdBy, @createdDate
		END
END