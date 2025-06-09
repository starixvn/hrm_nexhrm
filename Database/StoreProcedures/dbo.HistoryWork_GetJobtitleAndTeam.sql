USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[HistoryWork_GetJobtitleAndTeam]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[HistoryWork_GetJobtitleAndTeam] @Id INT
AS
/*
Created: 
Task: get employee job title and team info
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT Id
		,EmpId
		,AffectedDate
		,GroupId
		,JobTitleId
				
	FROM HistoryWork(NOLOCK)
	WHERE Id = @Id
END
GO
