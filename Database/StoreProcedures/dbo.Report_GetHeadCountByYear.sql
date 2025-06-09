USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Report_GetHeadCountByYear]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/*
--Report_RequestMonitor '2015/8/12', '2015/8/12',1, 3, 125
*/
CREATE OR ALTER PROCEDURE [dbo].[Report_GetHeadCountByYear]
	 @startDateOfYear DATETIME
    ,@endDateOfYear DATETIME
AS
/*
Created: 2018/05/22@Oliver.Dang
Task: get head count of year
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SELECT g.Id AS GroupId
		  ,g.Code AS GroupCode
		  ,g.Name AS GroupName
		  ,SUM( CASE
				WHEN (e.JoinedDate <= @startDateOfYear) AND ((e.[Enabled] = 1 AND e.isDeleted = 0) OR e.ResignedDate >= @startDateOfYear) 
				THEN 1
				ELSE 0
				END) AS EmployeeAtJanuary
		  ,SUM( CASE
				WHEN e.JoinedDate BETWEEN @startDateOfYear AND @endDateOfYear
				THEN 1
				ELSE 0
				END) AS NewHire
		  ,SUM( CASE
				WHEN e.ResignedDate BETWEEN @startDateOfYear AND @endDateOfYear
				THEN 1
				ELSE 0
				END) AS Resigned
		 ,SUM( CASE
				WHEN (e.JoinedDate <= @endDateOfYear) AND ((e.[Enabled] = 1 AND e.isDeleted = 0) OR e.ResignedDate > @endDateOfYear)
				THEN 1
				ELSE 0
				END) AS EmployeeAtDecember
		 ,(SELECT COUNT(h.Id) 
			FROM dbo.HistoryWork as h
				LEFT JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
			WHERE hd.GroupId = g.Id AND h.AffectedDate BETWEEN @startDateOfYear AND @endDateOfYear) AS [JoinTeam]
		 ,(SELECT COUNT(m.Id) 
			FROM (SELECT h.Id, h.EmpId, h.AffectedDate, hd.*
					FROM dbo.HistoryWork as h
					LEFT JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
					WHERE h.AffectedDate NOT IN (SELECT TOP 1 AffectedDate 
													FROM dbo.HistoryWork
													WHERE EmpId = h.EmpId
													ORDER BY AffectedDate DESC)) AS m
			WHERE m.GroupId = g.Id AND m.AffectedDate BETWEEN @startDateOfYear AND @endDateOfYear)	AS [MoveTeam]

	FROM [dbo].EmployeeGroup As g WITH (NOLOCK)		
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE ISNULL(g.Deleted, 0) = 0
		AND e.IsSpecialUser = 0
	GROUP BY g.Id
			,g.Name
			,g.Code
			,g.[Rank]
	ORDER BY g.[Rank]
			,g.Code
			,g.Name
END
GO
