

CREATE PROCEDURE [dbo].[TOEIC_NewUpdate] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@MinimumScore INT = NULL
	,@swExamDate DATETIME = NULL
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
Task: only update new TOEIC score
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
		IF (
				(@listening + @reading) > 0
				AND (@speaking + @writing) > 0
				) -- LR & SW
		BEGIN
			UPDATE dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Listening = @listening
				,Reading = @reading
				,LRExamDate = @lrExamDate
				,Speaking = @speaking
				,Writing = @writing
				,MinimumScore = @MinimumScore
				,SWExamDate = @swExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END

		IF (@listening + @reading > 0) -- LR
		BEGIN
			UPDATE dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Listening = @listening
				,Reading = @reading
				,MinimumScore = @MinimumScore
				,LRExamDate = @lrExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END
		ELSE IF (@speaking + @writing > 0) -- SW
		BEGIN
			UPDATE dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Speaking = @speaking
				,Writing = @writing
				,MinimumScore = @MinimumScore
				,SWExamDate = @swExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END
	END
	ELSE
	BEGIN -- insert
		EXEC dbo.TOEIC_Insert @empId
			,@listening
			,@reading
			,@lrExamDate
			,NULL
			,NULL
			,@speaking
			,@writing
			,@swExamDate
			,NULL
			,NULL
			,@MinimumScore
			,@inputDate
			,@modifiedBy
			,@modifiedDate
			,@createdBy
			,@createdDate
	END
END

