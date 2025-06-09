USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TOEIC_Insert] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@oldListening INT = NULL
	,@oldReading INT = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@swExamDate DATETIME = NULL
	,@oldSpeaking INT = NULL
	,@oldWriting INT = NULL
	,@MinimumScore INT = NULL
	,@inputDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
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

	INSERT INTO HRM.dbo.TOEIC (
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
END

GO
