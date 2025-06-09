USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[History_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[History_Get] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20240723@Lilac.Nguyen
Task: Get history.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT h.Id
	,h.EmpId
	,e.NickName
	,e.FullName
	,eg.[Name] AS GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,h.AffectedDate
	FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = hd.GroupId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
	WHERE (@GroupId IS NULL OR hd.GroupId = @GroupId)
		AND (@EmpId IS NULL OR h.EmpId = @EmpId)
		AND DATEDIFF(DAY, @FromDate, h.AffectedDate) >= 0
		AND DATEDIFF(DAY, @ToDate, h.AffectedDate) <= 0
       
	ORDER BY h.AffectedDate DESC
            
END
GO
