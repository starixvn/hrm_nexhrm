

/****** Object:  Stored Procedure dbo.TOEIC_GetAllScores    Script Date: 3/13/2016 3:21:43 PM ******/
CREATE PROCEDURE [dbo].[TOEIC_GetAllScores] @query NVARCHAR(256) = NULL
	,@groupId INT = NULL
	,@pageNumber INT = 1
	,@pageSize INT = 50
	,@totalRow INT = 0 OUTPUT
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

	DECLARE @q NVARCHAR(256)

	SET @q = '%' + @query + '%'

	IF (LEN(@query) = 0)
	BEGIN
		SET @query = NULL
	END

	SELECT @totalRow = COUNT(1)
	FROM Employee(NOLOCK)
	WHERE IsSpecialUser = 0
		AND (
			@query IS NULL
			OR NickName LIKE @q
			)
		AND (
			@groupId IS NULL
			OR GroupId = @groupId
			)

	-- Insert statements for procedure here
	SELECT e.Id
		,e.NickName
		,e.FullName
		,e.JoinedDate
		,g.ID AS [TeamID]
		,g.Code
		,g.[Name]
		,g.[Rank]
		,lk.Id AS [JobTitleId]
		,lk.[Name]
		,(CASE e.Id
			WHEN g.LeaderId THEN 2 -- employee is leader
			WHEN g.AssistantLeaderId THEN 1 -- employee is assistant leader
			ELSE 0 END
			) AS EmployeeRank
		,ISNULL(LRExamDate, '1900-01-01') AS LRExamDate
		,ISNULL(t.Listening, 0) AS Listening
		,ISNULL(t.Reading, 0) AS Reading
		,ISNULL(t.Writing, 0) AS Writing
		,ISNULL(t.Speaking, 0) AS Speaking
		,t.InputDate
		,SWExamDate
		,ISNULL(t.OldListening, 0) AS OldListening
		,ISNULL(t.OldReading, 0) AS OldReading
		,ISNULL(t.OldWriting, 0) AS OldWriting
		,ISNULL(t.OldSpeaking, 0) AS OldSpeaking
		,ISNULL(t.MinimumScore, 0) AS MinimumScore
	FROM (
		SELECT *
		FROM dbo.Employee WITH (NOLOCK)
		WHERE IsSpecialUser = 0
			AND ISNULL(Enabled, 1) = 1
			AND ISNULL(isDeleted, 0) = 0
			AND (
				@query IS NULL
				OR NickName LIKE @q
				)
			AND (
				@groupId IS NULL
				OR GroupId = @groupId
				)
		) AS e
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		LEFT JOIN dbo.LookupData AS LK ON e.JobTitleId = lk.ID
		LEFT JOIN dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
	ORDER BY [Rank]
		,g.Code
		,EmployeeRank DESC
		,JoinedDate
END

