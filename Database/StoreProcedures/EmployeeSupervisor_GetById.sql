CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_GetById]
	@EmpId INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240422@Lilac.Nguyen
Task: get approver by id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
	  
	SELECT em.Id AS EmpId,
			em.NickName,
			em.FullName,
			STRING_AGG(CONVERT(NVARCHAR(MAX), es.ApproverId), ', ') AS ApproverIds
    FROM dbo.Employee AS em WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON em.Id = es.EmpId
    WHERE ISNULL(es.IsDeleted, 0) = 0 AND em.Id = @EmpId
	GROUP BY em.Id, em.NickName, em.FullName

END
GO
