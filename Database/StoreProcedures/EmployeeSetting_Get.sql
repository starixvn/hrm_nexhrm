CREATE OR ALTER PROCEDURE [dbo].[EmployeeSetting_Get]
				@employeeId INT
AS
/*
	Created: 20180618@Oliver.Dang
	Task: Get employee setting by employee id
	DB: NexHRM
	Original: 

	Revisions:  
		20250507@Henry.Dinh: update group id from 38 to 1104
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @HRTeamId INT,
	@ITTeamId INT,
	@ExpId INT,
	@FinanceId INT,
	@MGTId INT,
	@CEOTeamId INT;

	SELECT @HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.GroupId END), 0),
		@ITTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'ITNetWorkTeamId' THEN g.GroupId END), 0),
		@ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.GroupId END), 0),
		@FinanceId = ISNULL(MAX(CASE WHEN ss.Name = 'FATeamId' THEN g.GroupId END), 0),
		@MGTId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.GroupId END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.GroupId END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN bodb_HRM.dbo.[Group] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'ITNetWorkTeamId', 'EXPTeamId', 'FATeamId', 'CoreTeamId', 'CEOTeamId')

		IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
			AND (GroupId IN (@ExpId, @FinanceId, @HRTeamId, @ITTeamId, @MGTId, @CEOTeamId) OR CompanyId = 2)
			)
			BEGIN
				SELECT @employeeId AS [EmpId]
				,1 AS [DisplayInfoAndTodoList]
				,1 AS [DisplayTeamAttendence]
				,0 AS [DisplayTeamWorkLoad]
				,0 AS [DisplayRedmineTask]
				,0 AS [DisplayRedmineInputSpendTime]
				,0 AS [DisplayRemindLogtime]
			END
			ELSE
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM [dbo].[EmployeeSetting]  WITH (NOLOCK) WHERE EmpId = @employeeId)
				BEGIN
					SELECT @employeeId AS [EmpId]
						,1 AS [DisplayInfoAndTodoList]
						,1 AS [DisplayTeamAttendence]
						,1 AS [DisplayTeamWorkLoad]
						,1 AS [DisplayRedmineTask]
						,1 AS [DisplayRedmineInputSpendTime]
						,1 AS [DisplayRemindLogtime]
				END
				ELSE
				BEGIN
					SELECT [EmpId]
				  ,[DisplayInfoAndTodoList]
				  ,[DisplayTeamAttendence]
				  ,[DisplayTeamWorkLoad]
				  ,[DisplayRedmineTask]
				  ,[DisplayRedmineInputSpendTime]
				  ,[DisplayRemindLogtime] 
			  FROM dbo.EmployeeSetting WITH (NOLOCK) WHERE EmpId = @employeeId
				END
			END
END

GO
