USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LeaveConversion_Get]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[LeaveConversion_Get] 
	@FromDate DATETIME
	,@ToDate DATETIME
	,@Type TINYINT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20140810@Quinn.Vo
Task: Get Leave deduction.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	
	SELECT 
		ld.Id AS LeaveConversionId,
		ld.[Date],
		ld.IsAnnual,
		ld.LeaveDays,
		ld.[Description],
		ld.ModifiedDate,
		ld.EmpId,
		e.NickName,
		e.FullName
		,g.Code AS TeamCode
		,g.Name AS TeamName
	FROM dbo.LeaveConversion AS ld WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ld.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE ld.[Type] = @Type
	AND DATEDIFF(D, @FromDate, ld.[Date]) >= 0
	AND DATEDIFF(D, ld.[Date], @ToDate) >= 0
	AND (
			@EmployeeId IS NULL
			OR e.Id = @EmployeeId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
END
GO
