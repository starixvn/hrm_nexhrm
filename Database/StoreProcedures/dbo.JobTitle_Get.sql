USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[JobTitle_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Get] @JobTitle NVARCHAR(100) = ''
AS
/*
Created: 20160224@Quinn.Vo
Task: Get job title data.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF @JobTitle IS NULL
        SET @JobTitle = ''

	SELECT jt.JobTitleId AS Id
		,jt.Name
		,COUNT(e.Id) AS NumberOfEmployee
	FROM HRM.dbo.JobTitle AS jt WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON jt.JobTitleId = e.JobTitleId
	WHERE jt.Name LIKE '%' + @JobTitle + '%'
	AND ISNULL(e.IsDeleted, 0) = 0
	GROUP BY jt.JobTitleId
        ,jt.[Name]
    ORDER BY jt.[Name]
END
GO
