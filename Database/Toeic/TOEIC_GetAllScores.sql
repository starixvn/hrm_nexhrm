USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TOEIC_GetAllScores]    Script Date: 7/25/2013 4:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TOEIC_GetAllScores] 
	@query nvarchar(256),
	@groupId int = null,
	@titleId int = null,
	@score int = null,
	@enable bit = 1,
	@isDelete bit = 1,
	@pageNumber int = 1,
	@pageSize int = 50,
	@totalRow int = 0 OUTPUT
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
	
	DECLARE @q nvarchar(256)
	SET @q = '%' + @query + '%'
	IF (LEN(@query) = 0)
	BEGIN
		SET @query = NULL
	END
	
	SELECT @totalRow = COUNT(*)
	FROM Employee (NOLOCK)
	WHERE	(@isDelete IS NULL OR isDeleted = @isDelete)
	 AND (@isDelete = 1 OR (((@isDelete = 0 OR @isDelete IS NULL)
	 AND (isDeleted = 0 OR isDeleted IS NULL))
	 AND (@enable IS NULL OR Enabled = @enable)))
	 AND (@query IS NULL OR NickName LIKE @q)
	 AND (@groupId IS NULL OR GroupId = @groupId)

	-- Insert statements for procedure here
	SELECT *
	FROM
	 (SELECT row_number() OVER (ORDER BY NickName ASC) AS RowNumber
		, e.Id, e.NickName, e.FullName, e.JoinedDate
		, lk.Name AS [JobTitle]
		, g.ID AS [TeamID]
		, g.Code AS [Team]
		, g.Name AS [TeamName]
		, g.Rank
		, (CASE e.Id
		WHEN g.LeaderId THEN
		 2 -- employee is leader
		WHEN g.AssistantLeaderId THEN
		 1 -- employee is assistant leader
		ELSE
		 0
	   END) AS EmployeeRank
	    , ISNULL(t.Listening, 0) Listening, ISNULL(t.Reading, 0) Reading, LRExamDate
	    , ISNULL(t.Writing, 0) Writing, ISNULL(t.Speaking, 0) Speaking, t.InputDate, SWExamDate
	    , ISNULL(t.OldListening, 0) OldListening, ISNULL(t.OldReading, 0) OldReading
	    , ISNULL(t.OldWriting, 0) OldWriting, ISNULL(t.OldSpeaking, 0) OldSpeaking
	  FROM
	   Employee e WITH (NOLOCK)
	   LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK)
		ON e.GroupID = g.ID
	   LEFT JOIN dbo.LookupData LK
		ON e.JobTitleId = lk.ID
	   LEFT JOIN TOEIC t WITH (NOLOCK)
	    ON e.Id = t.EmpId
	  WHERE
	   (@isDelete IS NULL
	   OR isDeleted = @isDelete)
	   AND (@isDelete = 1
	   OR (((@isDelete = 0
	   OR @isDelete IS NULL)
	   AND (isDeleted = 0
	   OR isDeleted IS NULL))
	   AND (@enable IS NULL
	   OR Enabled = @enable)))
	   AND (@query IS NULL
	   OR NickName LIKE @q)
	   AND (@groupId IS NULL
	   OR GroupId = @groupId)
	   AND (@titleId IS NULL
	   OR e.JobTitleId = @titleId)) AS alias
	WHERE
	 RowNumber BETWEEN (@pageNumber - 1) * @pageSize AND @pageNumber * @pageSize
	ORDER BY
	 [Rank]
	  , [Team]
	  , EmployeeRank DESC
	  , JoinedDate
END