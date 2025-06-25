CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_GetById] 
	@EmployeeWorkFromHomeId INT
AS
/*
Created: 2021213@Teddy.le
Task: Get Employee Work From Home by Id.
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
		,ew.Remark
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)  ON wfhr.Id = ew.WorkFromHomeRequestId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.Id
	WHERE ew.EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
END

GO
