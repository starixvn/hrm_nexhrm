USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[History_GetByEmpId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[History_GetByEmpId]
    @EmpId INT
AS
/*
Created: 20240729@Lilac.Nguyen
Task: Get History by EmpId.
DB: NexHRM
Original: 
Revisions: 
	- 20250319@Henry.Dinh: Order by AffectedDate desc
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,eg.[Name] AS GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,mg.[Name] AS ManagementGradeName
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = hd.GroupId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.ManagementGrade AS mg WITH (NOLOCK) ON mg.ManagementGradeId = hd.ManagementGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
    WHERE h.EmpId = @EmpId
	ORDER BY h.AffectedDate DESC  
END
GO
