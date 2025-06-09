USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetSupervisorForCheckInOut]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSupervisorForCheckInOut] 
	@employeeId INT
	,@IsStandbyAtWeekend BIT 
AS
/*
Created: 20240329@Quinn
Task: Get Supervisors for notification check in/out request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsSupReadOnly BIT = 0
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'

	CREATE TABLE #tempSupIds(
		SupervisorId INT,
		IsReadOnly BIT
	)

	IF (@IsStandbyAtWeekend = 1 AND 
		EXISTS (SELECT 1 FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id 
		WHERE e.Id = @employeeId
			AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ',')))
	)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT e.Id, 0
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.Id = @LeaderOfStandbyTeam
		AND e.isDeleted = 0
		SET @IsSupReadOnly = 1
	END
	ELSE
	BEGIN
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
END
GO
