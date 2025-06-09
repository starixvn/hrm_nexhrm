USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LateInEarlyOut_GetAllEmployee]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetAllEmployee] @Date DATETIME
AS
/*
Created: 20160321@HarveyNguyen
Task: Show dashboard
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id 'EmpId'
		,e.NickName
		,CASE 
			WHEN lieo.Type = 1 THEN 'Late in'
			ELSE 'Early out'
			END AS 'Types'
		,lieo.Minutes
	FROM dbo.LateInEarlyOut lieo WITH (NOLOCK)
		JOIN dbo.Employee e WITH (NOLOCK) ON e.Id = lieo.EmpId
	WHERE lieo.WorkDate = @Date
	ORDER BY e.NickName
END
GO
