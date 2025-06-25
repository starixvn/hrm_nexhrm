CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Get] 
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

	SELECT ew.EmployeeWorkFromHomeId
		,ew.FromDate
		,ew.ToDate
		,ew.EmpId
		,e.NickName
		,e.FullName
		,ew.Remark AS HRRemark
		,wfhr.Id AS WorkFromHomeRequestId
		,wfhr.Remark
		,wfhr.Status AS StatusId
		,wfhr.CreatedBy as CreatedById
		,creator.NickName as CreatedBy
		,wfhr.CreatedDate
		,Processor.NickName AS ProcessedBy
		,wfhr.ProcessedDate
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON wfhr.CreatedBy = creator.Id
		LEFT JOIN dbo.Employee AS Processor WITH (NOLOCK) ON wfhr.ProcessedBy = Processor.Id
	WHERE ((ew.FromDate BETWEEN @FromDate AND @ToDate)
		OR (ew.ToDate BETWEEN @FromDate AND @ToDate)
		OR (@FromDate BETWEEN ew.FromDate AND ew.ToDate)
		OR (@ToDate BETWEEN ew.FromDate AND ew.ToDate)) 
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
