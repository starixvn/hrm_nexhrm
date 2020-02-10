
/*
--Report_RequestMonitor '2015/8/12', '2015/8/12',1, 3, 125
*/
CREATE PROCEDURE [dbo].[Report_GetHeadCountByYear]
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
			FROM HistoryWork as h
			WHERE h.GroupId = g.Id AND h.AffectedDate BETWEEN @startDateOfYear AND @endDateOfYear) AS [JoinTeam]
		 ,(SELECT COUNT(m.Id) 
			FROM (select Id, EmpId, AffectedDate, ModifiedBy, ModifiedDate, JobtitleId, JobtitleName, GroupId, GroupName
					from HistoryWork as h
					Where h.AffectedDate not in (select top 1 AffectedDate 
													from HistoryWork
													where EmpId = h.EmpId
													order by AffectedDate desc))
													 as m
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
