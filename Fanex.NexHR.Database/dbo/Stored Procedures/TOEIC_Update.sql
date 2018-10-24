

CREATE PROCEDURE [dbo].[TOEIC_Update] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@oldListening INT = NULL
	,@oldReading INT = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@MinimumScore INT = NULL
	,@swExamDate DATETIME = NULL
	,@oldSpeaking INT = NULL
	,@oldWriting INT = NULL
	,@inputDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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

	IF EXISTS (
			SELECT 1
			FROM dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			)
	BEGIN -- update
		UPDATE dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
		SET Listening = @listening
			,Reading = @reading
			,LRExamDate = @lrExamDate
			,OldListening = @oldListening
			,OldReading = @oldReading
			,Speaking = @speaking
			,Writing = @writing
			,SWExamDate = @swExamDate
			,OldSpeaking = @oldSpeaking
			,OldWriting = @oldWriting
			,MinimumScore = @MinimumScore
			,InputDate = @inputDate
			,ModifiedBy = @modifiedBy
			,ModifiedDate = @modifiedDate
		WHERE EmpId = @empId;
	END;
	ELSE
	BEGIN -- insert
		EXEC dbo.TOEIC_Insert @empId
			,@listening
			,@reading
			,@lrExamDate
			,@oldListening
			,@oldReading
			,@speaking
			,@writing
			,@swExamDate
			,@oldSpeaking
			,@oldWriting
			,@MinimumScore
			,@inputDate
			,@modifiedBy
			,@modifiedDate
			,@createdBy
			,@createdDate;
	END;

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Toeic score failed.';
	END;
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = '';
	END;
END;

