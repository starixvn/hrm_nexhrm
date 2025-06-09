CREATE OR ALTER PROCEDURE [dbo].[Promotion_Get] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20240103@Lilac.Nguyen
Task: Get promotions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT ej.Id
	,e.FullName
	,e.NickName
	,eg.Code AS TeamCode
	,eg.Name AS TeamName
	,jg.Name AS CurrentGrade
	,jg2.Name AS PromotedGrade
	,ej.AffectedDate
	,ej.Note

	FROM HRM.dbo.EmployeeJobGrade AS ej WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ej.EmpId
		INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		INNER JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = ej.OldJobGradeId
		INNER JOIN HRM.dbo.JobGrade AS jg2 WITH (NOLOCK) ON jg2.JobGradeId = ej.JobGradeId
	WHERE (@GroupId IS NULL OR e.GroupId = @GroupId)
		AND (@EmpId IS NULL OR ej.EmpId = @EmpId)
		AND DATEDIFF(DAY, @FromDate, ej.AffectedDate) >= 0
		AND DATEDIFF(DAY, @ToDate, ej.AffectedDate) <= 0
		AND (ej.IsDeleted = 0 OR ej.IsDeleted IS NULL)
		AND (ej.IsPromotion = 1)
       
	ORDER BY ej.AffectedDate DESC
            
END
GO
