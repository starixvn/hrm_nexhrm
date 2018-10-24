
CREATE PROCEDURE [dbo].[EmployeeSetting_Get] 
				@employeeId INT
AS
/*
Created: 20180618@Oliver.Dang
Task: Get employee setting by employee id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[EmployeeSetting]  WITH (NOLOCK) WHERE EmpId = @employeeId)
		BEGIN
			SELECT @employeeId AS [EmpId]
				,1 AS [DisplayInfoAndTodoList]
				,1 AS [DisplayTeamAttendence]
				,1 AS [DisplayTeamWorkLoad]
				,0 AS [DisplayRedmineTask]
				,0 AS [DisplayRedmineInputSpendTime]
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

