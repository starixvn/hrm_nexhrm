USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[WorkFromHomeRequest_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_GetById] 
	@Id INT
AS
/*
Created: 20211223@Teddy.le
Task: Get Work From Home Request By Id.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON;

	SELECT wfhr.Id
		,wfhr.FromDate
		,wfhr.ToDate
		,wfhr.Remark
		,wfhr.HRRemark
		,wfhr.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wfhr.EmpId = e.Id
	WHERE wfhr.Id = @Id
END
GO
