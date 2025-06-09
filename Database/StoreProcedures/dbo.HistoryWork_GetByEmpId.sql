USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[HistoryWork_GetByEmpId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[HistoryWork_GetByEmpId]
	@EmpId INT
AS
/*
Created: 20191212@Lilac.Nguyen
Task: Get History Work by Employee Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.EmpId
		,sm.AffectedDate
		,sm.ModifiedBy
		,sm.ModifiedDate
		,sm.JobtitleName
		,sm.GroupName
		,sm.Id
	FROM dbo.HistoryWork sm WITH (NOLOCK)		
	WHERE sm.EmpId = @EmpId
END

GO
