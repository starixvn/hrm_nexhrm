/****** Object:  Stored Procedure dbo.TimeRecord_Get    Script Date: 3/13/2016 9:45:22 AM ******/

CREATE PROCEDURE [dbo].[TimeRecord_Get] @EmpID INT = NULL
	,@fromDate DATETIME
	,@toDate DATETIME
	,@groupIds VARCHAR(MAX) = ''
	,@PageIndex INT = NULL
	,@ItemsPerPage INT = NULL
	,@TotalItem INT = NULL OUT
AS
/*
Created: Quinn.Vo@2014-01-24
Task: Get Time Record
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	IF OBJECT_ID('tempdb..#TimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #TimeRecord
	END

	CREATE TABLE #TimeRecord (
		RecordTimeId INT
		,RecordTime DATETIME
		,InOutType TINYINT
		,EmpId INT
		,Code VARCHAR(20)
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupId INT
		,GroupCode VARCHAR(20)
		,GroupName NVARCHAR(50)
		,ReaderName NVARCHAR(64)
		,EventName NVARCHAR (64)
		)

	DECLARE @GroupsList TABLE (GroupId TINYINT)

	INSERT INTO @GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@groupids, ',')

	INSERT INTO #TimeRecord
	(
		RecordTimeId
		,RecordTime
		,InOutType
		,ReaderName
		,EventName
		,EmpId 
		,Code
		,NickName
		,FullName
		,GroupId 
		,GroupCode
		,GroupName
		
	)
	SELECT t.[Id]
		,t.RecordTime
		,ISNULL(t.InOutType, 2)
		,CASE WHEN bio.DeviceId IS NULL THEN t.ReaderName ELSE bio.DisplayName END as ReaderName
		,t.EventName as EventName
		,e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,g.Id
		,g.Code
		,g.[Name]
	FROM dbo.TimeRecord AS t WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON t.EmpID = e.Id
		INNER JOIN dbo.[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.BioDevice AS bio WITH (NOLOCK) ON t.ReaderId = bio.DeviceId
	WHERE CONVERT(VARCHAR(10), RecordTime, 111) BETWEEN CONVERT(VARCHAR(10), @fromDate, 111)
			AND CONVERT(VARCHAR(10), @toDate, 111)
		AND (
			@EmpID IS NULL
			OR e.Id = @EmpID
			)
		AND (
			ISNULL(@groupIds, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM @GroupsList
				)
			)
	ORDER BY t.RecordTime DESC

	IF @PageIndex IS NOT NULL
		AND @ItemsPerPage IS NOT NULL
	BEGIN
		SELECT t.RecordTimeId
		,t.RecordTime
		,ISNULL(t.InOutType, 2) as InOutType
		,t.ReaderName as ReaderName
		,t.EventName as EventName
		,t.EmpId
		,t.Code
		,t.NickName
		,t.FullName
		,t.GroupId
		,t.GroupCode
		,t.GroupName
		
		FROM #TimeRecord AS t
		ORDER BY t.RecordTime DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

		FETCH NEXT @ItemsPerPage ROWS ONLY;
	END
	ELSE
	BEGIN
		SELECT t.RecordTimeId
		,t.RecordTime
		,ISNULL(t.InOutType, 2) as InOutType
		,t.ReaderName as ReaderName
		,t.EventName as EventName
		,t.EmpId
		,t.Code
		,t.NickName
		,t.FullName
		,t.GroupId
		,t.GroupCode
		,t.GroupName
		
		FROM #TimeRecord AS t
		ORDER BY t.RecordTime DESC
	END

	SELECT @TotalItem = COUNT(*)
	FROM #TimeRecord

	IF OBJECT_ID('tempdb..#TimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #TimeRecord
	END
END