USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_UpdateTeamAndGrade]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[Employee_UpdateTeamAndGrade]
	@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240223@Lilac.Nguyen
Task: Update Team And Grade
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Employee WITH (UPDLOCK, ROWLOCK)
	SET GroupId = h.GroupId
	FROM Employee AS e WITH (UPDLOCK, ROWLOCK)
	JOIN HistoryWork AS h WITH (UPDLOCK, ROWLOCK) ON e.Id = h.EmpId
	WHERE CONVERT(DATE, h.AffectedDate) = CONVERT(DATE, GETDATE())
	AND h.OldGroupId IS NOT NULL;

	UPDATE dbo.EmployeeJobGrade WITH (UPDLOCK, ROWLOCK)
	SET IsCurrentUsed = 1
	WHERE CONVERT(DATE, AffectedDate) = CONVERT(DATE, GETDATE())
	AND IsCurrentUsed <> 1
	
END

GO
