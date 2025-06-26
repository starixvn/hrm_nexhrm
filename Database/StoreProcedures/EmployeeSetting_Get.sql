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
	@MGTId INT;

	SELECT @HRTeamId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END),
		@ITTeamId = MAX(CASE WHEN ss.Name = 'ITNetWorkTeamId' THEN g.Id END),
		@ExpId = MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END),
		@FinanceId = MAX(CASE WHEN ss.Name = 'FATeamId' THEN g.Id END),
		@MGTId = MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'ITNetWorkTeamId', 'EXPTeamId', 'FATeamId', 'CoreTeamId')

		
			IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
				AND (GroupId IN (@ExpId, @FinanceId, @HRTeamId, @ITTeamId, @MGTId) OR CompanyId = 2)
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
