USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_Update]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TOEIC_Update] @empId INT
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
	,@oldDeadlineDate DATETIME = NULL
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

DECLARE @monthEvaluation1H TINYINT = 5,
@monthEvaluation2H TINYINT = 11,
@toDay DATETIME = dbo.fnc_GetClientDateTime()

BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			AND ( Listening + Reading ) >= @MinimumScore
			AND InputDate > @toDay
			AND (MONTH(InputDate) != @monthEvaluation1H OR MONTH(InputDate) != @monthEvaluation2H)
			AND YEAR(InputDate) != YEAR(@toDay)
			AND MONTH(@toDay) IN ( @monthEvaluation1H, @monthEvaluation2H)
			AND (MONTH(@lrExamDate) = @monthEvaluation1H OR MONTH(@lrExamDate) = @monthEvaluation2H)
			AND YEAR(@lrExamDate) = YEAR(@toDay)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Toeic score still effect. Please do not update scores this time.'

			RETURN
	END;
	ELSE
	BEGIN
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
		INSERT INTO dbo.TOEIC (
		EmpId
		,Listening
		,Reading
		,LRExamDate
		,OldListening
		,OldReading
		,Speaking
		,Writing
		,SWExamDate
		,OldSpeaking
		,OldWriting
		,MinimumScore
		,InputDate
		,ModifiedBy
		,ModifiedDate
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@empId
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
		,@createdDate
		)
	END;
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

GO
