CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSupervisorForLeave] 
	@employeeId INT
	,@IsManagementApprove BIT 
AS
/*
	Created: 20240329@Quinn
	Task: Get Supervisors for notification check leave request.
	DB: NexHR
	Original: 

	Revisions:
		- 20250327@Henry: Update MGT Code -> MGT/COR code
	Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsSupReadOnly BIT = 0
	DECLARE @CoreTeamId NVARCHAR(50)
	
	SELECT @CoreTeamId = GroupId 
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'CoreTeamCode'
	WHERE gs.OldGroupCode = ss.Value

	CREATE TABLE #tempSupIds(
		SupervisorId INT,
		IsReadOnly BIT
	)

	IF (@IsManagementApprove = 1)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT e.Id, 0
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.GroupId = @CoreTeamId
		AND e.isDeleted = 0
		SET @IsSupReadOnly = 1
	END

	IF EXISTS (SELECT 1 FROM dbo.EmployeeSupervisor WITH (NOLOCK) WHERE EmpId = @employeeId)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT DISTINCT sup.Id, @IsSupReadOnly
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
			ON sup.Id = es.ApproverId AND sup.isDeleted = 0 AND sup.Enabled = 1
		WHERE es.EmpId = @employeeId 
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ApproverId <> @employeeId
		AND NOT EXISTS (
				SELECT 1 FROM #tempSupIds WHERE SupervisorId = es.ApproverId
		)
	END
	ELSE
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT DISTINCT gs.ApproverId, @IsSupReadOnly
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
			ON e.GroupId = gs.GroupId
		INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
			ON sup.Id = gs.ApproverId 
			AND sup.isDeleted = 0 
			AND sup.Enabled = 1
			AND ISNULL(gs.IsDeleted, 0) = 0
		WHERE e.Id = @employeeId
			AND gs.ApproverId <> @employeeId
			AND NOT EXISTS (
					SELECT 1 FROM #tempSupIds WHERE SupervisorId = gs.ApproverId
			)
	END

	SELECT DISTINCT 
		e.EmpId
		,e.NickName
		,e.FullName
		,e.Code
		,sup.SupervisorId
        ,sup.SupervisorNickName
        ,sup.SupervisorFullName
        ,sup.SupervisorEmail
		,sup.SupervisorConversationId
		,sup.IsReadOnly
    FROM (
			SELECT Id AS EmpId
			,NickName
			,FullName
			,Code 
			FROM dbo.Employee WITH (NOLOCK)
			WHERE Id = @employeeId
		) AS e
		INNER JOIN
		(
			SELECT a.Id AS SupervisorId
				,NickName AS SupervisorNickName
				,FullName AS SupervisorFullName
				,Email AS SupervisorEmail 
				,con.ConversationId AS SupervisorConversationId
				,tsup.IsReadOnly
			FROM dbo.Employee AS a WITH (NOLOCK)
			INNER JOIN #tempSupIds tsup ON a.Id = tsup.SupervisorId
			LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON a.Id = con.EmpId
		) AS sup ON 1 = 1
		WHERE e.EmpId <> sup.SupervisorId

	DROP TABLE #tempSupIds
END

SET ANSI_NULLS ON
GO
