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

	SELECT @HRTeamId = MAX(CASE WHEN ss.Name = 'HRTeamCode' THEN gs.GroupId END),
		@ITTeamId = MAX(CASE WHEN ss.Name = 'ITNetWorkCode' THEN gs.GroupId END),
		@ExpId = MAX(CASE WHEN ss.Name = 'EXPCode' THEN gs.GroupId END),
		@FinanceId = MAX(CASE WHEN ss.Name = 'FACode' THEN gs.GroupId END),
		@MGTId = MAX(CASE WHEN ss.Name = 'CoreTeamCode' THEN gs.GroupId END)
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name IN ('HRTeamCode', 'ITNetWorkCode', 'EXPCode', 'FACode', 'CoreTeamCode')
	WHERE gs.OldGroupCode = ss.Value

		
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
