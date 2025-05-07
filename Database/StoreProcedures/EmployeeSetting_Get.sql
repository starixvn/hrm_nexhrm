SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EmployeeSetting_Get]
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
		
			IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
				AND (GroupId IN (40, 1, 1104, 2, 11) OR CompanyId = 2)
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
			ELSE IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
				AND (GroupId IN (51, 63, 13, 44, 54)
				 OR Roles IN ('1', '2', '4'))
			)
			BEGIN
				
				IF NOT EXISTS (SELECT 1 FROM [dbo].[EmployeeSetting]  WITH (NOLOCK) WHERE EmpId = @employeeId)
				BEGIN
					SELECT @employeeId AS [EmpId]
					,1 AS [DisplayInfoAndTodoList]
					,1 AS [DisplayTeamAttendence]
					,1 AS [DisplayTeamWorkLoad]
					,1 AS [DisplayRedmineTask]
					,1 AS [DisplayRedmineInputSpendTime]
					,0 AS [DisplayRemindLogtime]
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
