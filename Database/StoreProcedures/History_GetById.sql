USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[History_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[History_GetById]
    @Id INT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Get History by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,h.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,hd.GroupId
	,hd.JobTitleId
	,hd.JobGradeId
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
    WHERE h.Id = @Id
END

GO
