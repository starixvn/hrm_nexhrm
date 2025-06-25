CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Get] 
	 @FromDate DATETIME
	,@ToDate DATETIME
	,@EmpId INT = NULL
    ,@GroupId INT = NULL
	,@StatusId INT = NULL
AS
/*
Created: 20211208@Teddy.le
Task: Get Employee Work From Home by date.
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
		,wfhr.EmpId
		,e.NickName
		,e.FullName
		,wfhr.HRRemark
		,wfhr.Remark
		,wfhr.Status AS StatusId
		,lk.Name AS [Status]
		,wfhr.CreatedBy as CreatedById
		,creator.NickName as CreatedBy
		,wfhr.CreatedDate
		,Processor.NickName AS ProcessedBy
		,wfhr.ProcessedDate
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wfhr.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON wfhr.CreatedBy = creator.Id
		LEFT JOIN dbo.Employee AS Processor WITH (NOLOCK) ON wfhr.ProcessedBy = Processor.Id
		LEFT JOIN HRM.dbo.LookupData AS lk WITH (NOLOCK) ON wfhr.Status = lk.Value AND lk.LookupId = 4
	WHERE ((wfhr.FromDate BETWEEN @FromDate AND @ToDate)
		OR (wfhr.ToDate BETWEEN @FromDate AND @ToDate)
		OR (@FromDate BETWEEN wfhr.FromDate AND wfhr.ToDate)
		OR (@ToDate BETWEEN wfhr.FromDate AND wfhr.ToDate)) 
		AND (
            @GroupId IS NULL
            OR e.GroupId = @Groupid
            )
		AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
		AND (
            @StatusId IS NULL
            OR wfhr.Status = @StatusId
            )
	ORDER BY wfhr.CreatedDate DESC
END

GO
