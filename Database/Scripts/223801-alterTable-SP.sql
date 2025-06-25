UPDATE HRM.dbo.JobTitle
SET ModifiedDate = '2025-01-01'
WHERE ModifiedDate IS NULL;

GO
CREATE OR ALTER   PROCEDURE [dbo].[Sync_Employee_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT e.Id AS EmployeeId, 
		e.DomainUsername, 
		e.Code, 
		e.FullName, 
		e.NickName, 
		e.GroupId, 
		e.[Enabled], 
		e.isDeleted, 
		CAST(e.JoinedDate AS DATE) AS JoinedDate, 
		CAST(e.ConfirmedDate AS DATE) 
        AS ConfirmedDate, 
		e.JobTitleId, 
		e.Email, 
		ec.ConversationId,
		e.ModifiedDate
	FROM dbo.Employee AS e WITH (NOLOCK) LEFT OUTER JOIN
        dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE e.ModifiedDate > @LastModifiedDate
END

GO

CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeAttendance_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee attendence
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

	SELECT Id,
		EmpId, 
		AttendanceDate, 
		InFixed, 
		LeaveDay, 
		OutFixed, 
		LateIn,
		EarlyOut, 
		LeaveTypeId, 
		LeaveTypeId2, 
		Absence, 
		TotalWorkHours,
		InsertDate
	FROM dbo.EmployeeAttendance WITH (NOLOCK)
	WHERE InsertDate > @LastModifiedDate
	
END

GO

CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeCalendar_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee calendar
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id,
		EmpId, 
		WorkDate, 
		IsOffDay, 
		IsHoliday,
		IsLeaveDay, 
		LeaveDay,
		ModifiedDate
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
END

GO

CREATE OR ALTER  PROCEDURE [dbo].[Sync_EmployeeOverTime_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee ot by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT ot.OvertimeId,
		otd.EmployeeId, 
		otd.[Date], 
		otd.[Hour],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.Overtime AS ot WITH (NOLOCK) 
	INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	WHERE ISNULL(ot.ModifiedDate, ot.CreatedDate) > @LastModifiedDate
END

GO


CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeTOEIC_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee toeic by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT EmpId, 
		MinimumScore, 
		CAST(InputDate AS DATE) AS InputDate, 
		CAST(LRExamDate AS DATE) AS LRExamDate, 
		Listening, 
		Reading,
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM  HRM.dbo.TOEIC WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_Group_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get group by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id AS GroupId, 
		[Name], 
		Code, 
		ParentId, 
		CASE 
			WHEN DepartmentId IS NULL OR DepartmentId = 0 
				THEN 999 
			WHEN DepartmentId IS NOT NULL 
				THEN DepartmentId 
		END AS DepartmentId, 
		ISNULL(Deleted, 0) AS IsDeleted, 
		[Rank],
		ParentId,
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.EmployeeGroup WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END

GO

CREATE OR ALTER   PROCEDURE [dbo].[Sync_JobTitle_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get job title by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT JobTitleId AS Id, 
		[Name],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM  HRM.dbo.JobTitle WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END

GO

CREATE OR ALTER   PROCEDURE [dbo].[Sync_WorkingTimeRequest_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get working time request by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id,
		EmpId, 
		RequestType, 
		FromDate, 
		ToDate, 
		TotalDays, 
		[Status],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END

GO

