
CREATE PROCEDURE [dbo].[LeaveDeduction_Get] 
	@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20140810@Quinn.Vo
Task: Get Leave deduction.
DB: NexEPA
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
		ld.Id AS LeaveDeductionId,
		ld.[Date] AS DeductDate,
		ld.IsAnnual,
		ld.LeaveDays,
		ld.[Description],
		--ld.ModifiedBy,
		ld.ModifiedDate,
		ld.EmpId,
		e.NickName,
		e.FullName
		,e.GroupId
		,g.Code
		,g.Name
		,ld.ModifiedBy AS ModifiedById
		,modi.NickName
		,modi.FullName
	FROM dbo.LeaveConversion AS ld WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ld.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	LEFT JOIN dbo.Employee AS modi WITH (NOLOCK) ON ld.ModifiedBy = modi.Id
	WHERE [Type] = 1
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
