USE [NexHRM]
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_GetTeamHr]
AS
/*
	Created: 2023@Brian.nguyen
	Task: Get group conversations team human resources
	DB: NexHRM
	Revisions: 
		20250507@Henry.Dinh: update group id from 38 to 1104
	Original: 
*/
BEGIN
	SET NOCOUNT ON; 

	DECLARE @HRTeamId INT = 0
	SELECT @HRTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK) 
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:1KmVXImsgANnLEyWFPGem8Y4K4fMFvYN-tE_QfuS0SfASNxMSmouUgyega3ebG6yZ' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		AND GroupId = @HRTeamId

END

GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Get] -- '2022-01-04 11:20:25.000'
    @FromDate DATETIME = NULL
AS
/*
Created: 20141002@Quinn.Vo
Task: Get employee calendar from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
      cl.[WorkDate]
      ,ISNULL(e.OldId, e.Id) AS UserId
      ,CASE WHEN ISNULL(IsWorkFromHome, 0) = 1 THEN NULL ELSE [ShiftId] END [ShiftId]
	  --,[ShiftId]
      ,cl.[IsHoliday]
      ,CAST(CASE WHEN cl.[IsOffDay] = 1 OR ISNULL(cl.IsWorkFromHome, 0) = 1 THEN 1 ELSE 0 END AS BIT) AS [IsOffDay]
       ,cl.[IsLeaveDay]
      ,cl.[LeaveTypeId]
      ,cl.[LeaveDay]
      ,cl.ModifiedDate
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = cl.EmpId
    WHERE @FromDate IS NULL
    OR cl.ModifiedDate > @FromDate
    ORDER BY cl.ModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetCurrentUserWorkedDates]
    @UserId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
	Created: 20180614@Oliver.Dang
	Task: Get employee working dates by date range.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250327@Henry: Update MGT Code -> MGT/COR code
		- 20250506@Henry: Add HR-ADM group code
		- 20250618@Assistant: Fix SystemSetting usage for comma-separated values
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsNotShow BIT = 0

	DECLARE @HRTeamId INT,
	@ITTeamId INT,
	@ExpId INT,
	@FinanceId INT,
	@MGTId INT,
	@CEOTeamId INT;

	SELECT @HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END), 0),
		@ITTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'ITNetWorkTeamId' THEN g.Id END), 0),
		@ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@FinanceId = ISNULL(MAX(CASE WHEN ss.Name = 'FATeamId' THEN g.Id END), 0),
		@MGTId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'ITNetWorkTeamId', 'EXPTeamId', 'FATeamId', 'CoreTeamId', 'CEOTeamId')

	IF(
		EXISTS (
			SELECT 1 FROM 
				dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
			WHERE e.Id = @UserId
				AND 
					(e.Roles LIKE '%2%'
						OR e.Roles LIKE '%3%'
						OR e.Roles LIKE '%4%'
						OR g.Id IN (@HRTeamId, @ITTeamId, @ExpId, @FinanceId, @MGTId, @CEOTeamId)
					)
		)
	)
	BEGIN
		SET @IsNotShow = 1
	END

	CREATE TABLE #EmployeeCalendar2HalfDay
	(
		WorkDate DATE
	)

	INSERT INTO #EmployeeCalendar2HalfDay(WorkDate)
	SELECT CAST(rq.FromDate AS DATE)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
	WHERE rq.EmpId = @UserId
		AND rq.[Status] IN (1, 2)
		AND rq.TotalDays = 0.5
	GROUP BY rq.EmpId
		,CAST(rq.FromDate AS DATE)
	HAVING SUM(TotalDays) = 1

    SELECT cl.WorkDate
    FROM  dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
		AND cl.WorkDate BETWEEN rq.FromDate AND rq.ToDate
		AND rq.[Status] = 1
		AND rq.TotalDays >= 1
	LEFT JOIN #EmployeeCalendar2HalfDay AS hd ON cl.WorkDate = hd.WorkDate
    WHERE cl.EmpId = @UserId
		AND (cl.WorkDate between @FromDate and @ToDate)
		AND (cl.IsOffDay <> 1 AND ISNULL(cl.LeaveDay, 0) < 1 AND cl.IsHoliday <> 1)
		AND rq.Id IS NULL
		AND hd.WorkDate IS NULL
		AND @IsNotShow = 0

	DROP TABLE #EmployeeCalendar2HalfDay
END

SET ANSI_NULLS ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_GetTeamHr]
AS
/*
	Created: 2023@Brian.nguyen
	Task: Get group conversations team human resources
	DB: NexHRM
	Revisions: 
		20250507@Henry.Dinh: update group id from 38 to 1104
	Original: 

*/
BEGIN
	SET NOCOUNT ON; 

	DECLARE @HRTeamId INT
	SELECT @HRTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:1KmVXImsgANnLEyWFPGem8Y4K4fMFvYN-tE_QfuS0SfASNxMSmouUgyega3ebG6yZ' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		AND GroupId = @HRTeamId

END

GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_ChangePassword] @Id INT
    ,@Password NVARCHAR(100)
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250507@Henry.Dinh
    Task: change password
    DB: NexHRM
    Original: 

    Revisions:    
        20250507@Henry.Dinh: change password
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WHERE EmpId = @Id)
    BEGIN
        SET @ErrCode = 1
        SET @ErrMsg = 'Employee not found'
        RETURN
    END

    UPDATE bodb_HRM.dbo.CompanyEmployee
    SET Password = @Password
    WHERE EmpId = @Id
END

SET ANSI_NULLS ON

GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_GetLoginEmployee] @DomainUsername VARCHAR(50)
AS
/*
    Created: 20250507@Henry.Dinh
    Task: log in
    DB: HRM
    Original: 
    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT EmpId AS Id, DomainUsername, Password, IsDeleted, Enabled
    FROM bodb_HRM.dbo.CompanyEmployee WHERE DomainUsername = @DomainUsername
END

GO

CREATE OR ALTER   PROCEDURE [dbo].[EmployeeGroup_Get] 
    @FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all groups from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
    g.Id AS TeamId,
    g.DepartmentId,
    g.Code,
    g.Name,
    ISNULL(g.Deleted, 0) AS IsDeleted,
    CASE WHEN g.ModifiedDate IS NULL THEN g.CreatedDate ELSE g.ModifiedDate END AS ModifiedDate
    FROM dbo.EmployeeGroup AS g WITH (NOLOCK)
    WHERE (@FromDate IS NULL
    OR ISNULL(g.ModifiedDate, g.CreatedDate) > @FromDate)
	AND g.IsSpecialTeam = 0
END
GO

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

	SELECT @HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END), 0),
		@ITTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'ITNetWorkTeamId' THEN g.Id END), 0),
		@ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@FinanceId = ISNULL(MAX(CASE WHEN ss.Name = 'FATeamId' THEN g.Id END), 0),
		@MGTId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
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

CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiGroup] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Group
DB: NexHRM
Original: 

Revisions:
	- 20250627@Henry.Dinh: Update to use SystemSetting to get CompanyId
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	-- Declare variables
	DECLARE @SystemSettingValue INT
	DECLARE @MappedCompanyId INT

	-- Get SystemSetting value
	SELECT @SystemSettingValue = Value 
	FROM [dbo].[SystemSetting] WITH (NOLOCK) 
	WHERE Id = 4

	-- Map CompanyId based on SystemSetting value
	SET @MappedCompanyId = CASE 
		WHEN @SystemSettingValue = 1 THEN 2
		WHEN @SystemSettingValue = 2 THEN 1
		ELSE @SystemSettingValue
	END

	SELECT g.[GroupId] AS Id,
		g.[Code],
		g.[Name]
	FROM [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) 
	WHERE ISNULL(IsDeleted, 0) = 0 AND g.CompanyId = @MappedCompanyId
END
GO

CREATE OR ALTER   PROCEDURE [dbo].[Employee_Delete] @employeeId INT
    ,@ResignedDate SMALLDATETIME
	,@ModifiedBy INT = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Delete employee
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    IF (@ResignedDate IS NULL)
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The user cannot be deleted without resigned date'

        RETURN
    END

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @CompanyId int = (SELECT [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId')

        UPDATE bodb_HRM.dbo.CompanyEmployee WITH (UPDLOCK, ROWLOCK)
        SET ResignedDate = @ResignedDate
            ,isDeleted = 1
        WHERE EmpId = @employeeId
		AND CompanyId = @CompanyId

		UPDATE bodb_HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE EmpId = @employeeId

        DELETE dbo.EmployeeCalendar
        WHERE WorkDate > @ResignedDate
            AND empid = @employeeId;

        DELETE dbo.EmployeeAttendance
        WHERE AttendanceDate > @ResignedDate
            AND empid = @employeeId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete Employee unsuccessfully.'
    END CATCH
END
GO

GO

CREATE OR ALTER   PROCEDURE [dbo].[Employee_GetByDomainName] @DomainUsername VARCHAR(50)
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by domain username.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.[Id]
	,e.[Code]
	,e.[FullName]
	,e.[NickName]
	,e.[Email]
	,e.[Enabled]
	,e.[IsDeleted]
	,e.[Roles]
	,ISNULL(g.Id, 0) AS TeamId
	,g.Code AS TeamCode
	,g.GroupTeamId
	,CAST(CASE WHEN e.Id = 125 OR e.Id = 142 THEN 1 ELSE 0 END AS BIT) AS IsLeaderHR
	FROM dbo.Employee AS e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE e.DomainUsername = @DomainUsername
	AND e.isDeleted = 0
END

GO

CREATE OR ALTER   PROCEDURE [dbo].[Employee_GetEmployeeByTeams] @employeeId INT
    ,@teamCodes VARCHAR(200) = ''
    ,@ignoreTeams VARCHAR(200) = ''
AS
/*  
Created: 20120810@Quinn.Vo
Task: Send mail for all Staff in PM team
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #TeamCodeList (GroupTeamId INT);

    CREATE TABLE #IgnoreTeamList (GroupTeamId INT);

    INSERT INTO #TeamCodeList (GroupTeamId)
    SELECT val
    FROM fnc_strSplit(@teamCodes, ',');

    INSERT INTO #IgnoreTeamList (GroupTeamId)
    SELECT val
    FROM fnc_strSplit(@ignoreTeams, ',');

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Employee AS e WITH (NOLOCK)
                    INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.Id = @employeeId
                    AND g.GroupTeamId NOT IN (
                        SELECT GroupTeamId
                        FROM #IgnoreTeamList
                        )
                )
            )
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
            INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON eg.GroupId = g.Id
            INNER JOIN dbo.Employee AS e ON e.Id = eg.EmpID
        WHERE g.GroupTeamId IN (
                SELECT GroupTeamId
                FROM #TeamCodeList
                )
            AND (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
	       AND e.[Enabled] = 1
		   AND (ISNULL(eg.HandleRequest, 0) = 0 OR isMainGroup = 1)
    END;
    ELSE
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.Employee AS e WITH (NOLOCK)
        WHERE 1 = 0;
    END;

    DROP TABLE #TeamCodeList;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetManagements]
AS
/*
  Created: 20150703@Tim.Trinh
  Task: get management that can handle unpaid leave request for employee. 
  DB: NexHR
  Original: 

  Revisions:  
    20250507@Henry.Dinh: update group id from 11 to 1081
  Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CoreTeamId INT = 0
	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    SELECT DISTINCT e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.Email
		,con.ConversationId
    FROM [dbo].[Employee] AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE e.GroupId = @CoreTeamId
	AND e.isDeleted = 0
END


GO

CREATE OR ALTER   PROCEDURE [dbo].[Employee_GetMissedLogTime]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
/*
Created: 20250228@Henry.dinh
Task: Insert/Update get missed logging 
DB: NexHRM
Original: Employee_GetMissedLogTime
Revisions:  
	- 20250307 - Added date range parameters and changed to use Date instead of Month/Year
				 Default date range is now first day to last day of current month
	- 20240319 - Removed RedmineNexcel dependencies as it's now handled by a separate API
Param's Explanation:
- @FromDate: Optional start date for filtering (defaults to first day of current month if NULL)
- @ToDate: Optional end date for filtering (defaults to last day of current month if NULL)
*/
BEGIN
    DECLARE @Yesterday DATE = 
        CASE 
            WHEN DATEPART(WEEKDAY, dbo.fnc_GetClientDateTime()) = 2 THEN
                DATEADD(DAY, -3, dbo.fnc_GetClientDateTime())
            ELSE
                DATEADD(DAY, -1, dbo.fnc_GetClientDateTime())
        END;
   
    DECLARE @FirstDayOfMonth DATE = DATEFROMPARTS(YEAR(dbo.fnc_GetClientDateTime()), MONTH(dbo.fnc_GetClientDateTime()), 1);
    DECLARE @LastDayOfMonth DATE = EOMONTH(dbo.fnc_GetClientDateTime());
    
    SET @FromDate = ISNULL(@FromDate, @FirstDayOfMonth);
    SET @ToDate = ISNULL(@ToDate, @LastDayOfMonth);
    
    WITH ExtractedDomainUsers AS (
        SELECT 
            Id,
            -- Extract username part after domain (e.g., "henry.dinh" from "spvn\henry.dinh")
            CASE 
                WHEN CHARINDEX('\', DomainUsername) > 0 
                THEN SUBSTRING(DomainUsername, CHARINDEX('\', DomainUsername) + 1, LEN(DomainUsername)) 
                ELSE DomainUsername 
            END AS ExtractedUsername,
            DomainUsername
        FROM [dbo].[Employee] WITH (NOLOCK)
        WHERE IsDeleted = 0 OR IsDeleted IS NULL
    ),
    ValidLeaders AS (
        -- Get leaders from GroupSupervisor
        SELECT DISTINCT gs.ApproverId
        FROM [dbo].[GroupSupervisor] gs WITH (NOLOCK)
        WHERE (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    ),
    EmployeesToExclude AS (
        -- Get emps is not Leader but exist in EmployeeSupervisor
        SELECT DISTINCT EmpId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.EmpId NOT IN (SELECT ApproverId FROM ValidLeaders)
        UNION
        SELECT DISTINCT ApproverId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.ApproverId NOT IN (SELECT ApproverId FROM ValidLeaders)
    ),
    MissedCounts AS (
        SELECT 
            EmpID,
            COUNT(*) AS MissedCount
        FROM [dbo].[EmployeeLogTimeMissed] WITH (NOLOCK)
        WHERE [Date] BETWEEN @FromDate AND @ToDate
        GROUP BY EmpID
    )
    SELECT DISTINCT
        eg.Code AS TeamCode,
		eg.GroupTeamId,
        e.Id AS EmpID,
        e.NickName,
		e.Code,
        CASE 
            WHEN e.Id = gs.ApproverId THEN 1 ELSE 0
        END AS IsLeader,
        ISNULL(mc.MissedCount, 0) AS MissedCount,
		jt.Name As JobTitle,
        ec.ConversationId,
        edu.DomainUsername
    FROM [dbo].[EmployeeGroup] eg WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] e WITH (NOLOCK) ON eg.Id = e.GroupId
    LEFT JOIN [dbo].[GroupSupervisor] gs WITH (NOLOCK) ON eg.Id = gs.GroupId AND (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    LEFT JOIN [dbo].[EmployeeSupervisor] es WITH (NOLOCK) ON e.Id = es.EmpId AND (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
	LEFT JOIN [HRM].[dbo].[JobTitle] jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
    LEFT JOIN ExtractedDomainUsers edu ON e.Id = edu.Id
    LEFT JOIN MissedCounts mc ON e.Id = mc.EmpID
    LEFT JOIN [dbo].[EmployeeConversation] ec WITH (NOLOCK) ON e.Id = ec.EmpId
    LEFT JOIN [dbo].[EmployeeAttendance] ea WITH (NOLOCK)
        ON e.Id = ea.EmpId 
        AND ea.AttendanceDate = @Yesterday
    WHERE 
        (eg.Deleted = 0 OR eg.Deleted IS NULL)
        AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
        AND e.Enabled = 1
        AND (
            e.Id = gs.ApproverId
            OR (ea.InFixed IS NOT NULL AND (ea.LeaveDay != 1 OR ea.LeaveDay IS NULL))
        )
        -- Exclude EmpId is not Leader but exist in EmployeeSupervisor
        AND e.Id NOT IN (SELECT EmpId FROM EmployeesToExclude)
    ORDER BY 
        eg.Code, 
        IsLeader DESC, 
        e.NickName;
END

GO

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
	DECLARE @CoreTeamId INT = 0
	
	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

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

CREATE OR ALTER PROCEDURE [dbo].[Employee_Insert_CheckExist] @Nickname NVARCHAR(20)
	,@Code VARCHAR(20)
AS
/*
Created: 20250306@Tim
Task: check code, nickname available
DB: HRM

Revisions:
    20250306@Tim: initial - [Redmine: #217976]
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CompanyId INT

	SELECT @CompanyId = [Value]
    FROM NexHRM.dbo.SystemSetting WITH (NOLOCK)
    WHERE [Name] = 'CompanyId'

	DECLARE @EmpId_exist INT
		,@Nickname_exist NVARCHAR(20)
		,@FullName_exist NVARCHAR(50)

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = Nickname
	FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = e.EmpId 
	WHERE Code = @Code
		AND ce.CompanyId = @CompanyId

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT 1 AS [Code]
			,N'Code ' + @Code + ' has been assigned ' + @FullName_exist AS [Message]

		RETURN
	END

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = Nickname
	FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = E.EmpId 
	WHERE NickName = @Nickname
		AND ce.CompanyId = @CompanyId
		AND (
			e.isDeleted = 0
			OR e.[Enabled] = 1
			)

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT 2 AS [Code]
			,N'Nickname  ' + @Nickname_exist + '  has been assigned to ' + @FullName_exist AS [Message]

		RETURN
	END

    SELECT 0 AS [Code], NULL AS [Message]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Transfer] @EmployeeId INT
	,@LastWorkingDate DATE
	,@AffectedDate DATE
	,@ToGroupId INT
	,@ShiftId INT
	,@IsTransferLateInEarlyOut BIT
	,@IsTransferAnnualLeaveBalance BIT
	,@IsTransferNexBalance BIT
	,@IsSpecialUser BIT = 0
	,@ModifiedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210901@Quinn.Vo
Task: transfer employee
DB: NexHRM
Original: 

Revisions:
     - 20250620@Henry: migrate to bodb_HRM
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @FromDomain VARCHAR(20)
		,@ToDomain VARCHAR(20)
		,@FromDomainEmail VARCHAR(20)
		,@ToDomainEmail VARCHAR(20)
		,@FromCompanyId INT
		,@ToCompanyId INT
		,@Todate DATETIME
		,@NexBalance INT = 0
		
	DECLARE @CreditDays NUMERIC(18, 1)
		,@LateInEarlyOut INT

	SELECT @FromDomain = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
	SELECT @FromDomainEmail = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'
	SELECT @FromCompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

	IF (@FromCompanyId = 1)
	BEGIN
		SET @ToCompanyId = 2
		SELECT @ToDomain = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.CompanyDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
				,a.SalaryGroupId = b.SalaryGroupId
			FROM bodb_HRM.dbo.CompanyEmployee AS a 
			INNER JOIN bodb_HRM.dbo.CompanyEmployee AS b ON a.EmpId = b.EmpId AND b.CompanyId = @FromCompanyId
			WHERE a.EmpId = @employeeId AND a.CompanyId = @ToCompanyId
		END
		ELSE
		BEGIN
			INSERT INTO bodb_HRM.dbo.CompanyEmployee (
				EmpId
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,CompanyDate
				,ConfirmedDate
				,SalaryGroupId
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
				,SalaryGroupId
			FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
			WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
		END

		INSERT INTO LumiHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO LumiHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE LumiHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO LumiHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO LumiHRM.dbo.LateInEarlyOutTrans(
					[EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO LumiHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
					WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
					ELSE 0
					END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO LumiHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO LumiHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END
	ELSE IF (@FromCompanyId = 2)
	BEGIN
		SET @ToCompanyId = 1
		SELECT @ToDomain = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.CompanyDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM bodb_HRM.dbo.CompanyEmployee AS a 
			INNER JOIN bodb_HRM.dbo.CompanyEmployee AS b ON a.EmpId = b.EmpId AND b.CompanyId = @FromCompanyId
			WHERE a.EmpId = @employeeId AND a.CompanyId = @ToCompanyId
		END
		ELSE
		BEGIN
			INSERT INTO bodb_HRM.dbo.CompanyEmployee (
				EmpId
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,CompanyDate
				,ConfirmedDate
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
			FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
			WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
		END

		INSERT INTO NexHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO NexHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE NexHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO NexHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO NexHRM.dbo.LateInEarlyOutTrans(
					[EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO NexHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
					WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
					ELSE 0
					END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO NexHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO NexHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END

	UPDATE bodb_HRM.dbo.CompanyEmployee WITH (ROWLOCK, UPDLOCK)
	SET [Enabled] = 0
		,ResignedDate = @LastWorkingDate
	WHERE EmpId = @EmployeeId

	UPDATE bodb_HRM.dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET ModifiedDate = dbo.fnc_GetClientDateTime()
		,ModifiedBy = @ModifiedBy
	WHERE EmpId = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[HRM_SyncData_GroupReport_GetEmployeeInfo] @LastModifiedDate DATETIME = NULL
AS
/*
Created: 20250220@Tim
Task: get employee info
DB: NexHRM
Original:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id AS EmpId
	    ,dept.Code AS DepartmentCode
	    ,dept.[Name] AS DepartmentName
	    ,team.Code AS TeamCode
	    ,team.[Name] AS TeamName
		,e.Code
		,e.NickName
		,e.FullName
        ,e.DOB
        ,e.Gender
        ,jl.[Name] AS JobLevel
	    ,jt.[Name] AS JobTitle
        ,e.GroupDate AS JoinedDate
		,e.JoinedDate AS CompanyDate
		,e.ConfirmedDate
		,e.ResignedDate
        ,CASE WHEN team.[Name] = 'Expat' THEN 0 ELSE 1 END AS IsLocal
		,e.[Enabled]
		,e.IsDeleted
		,e.IsSpecialUser
		,e.ModifiedDate 
	FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS team WITH (NOLOCK) ON e.GroupId = team.Id
        LEFT JOIN HRM.dbo.EmployeeJobGrade AS ejl WITH (NOLOCK) ON e.Id = ejl.EmpId
        AND ejl.IsCurrentUsed = 1
        LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON team.DepartmentId = dept.DepartmentId
        LEFT JOIN HRM.dbo.JobGrade AS jl WITH (NOLOCK) ON ejl.JobGradeId = jl.JobGradeId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE @LastModifiedDate IS NULL
        OR e.ModifiedDate > @LastModifiedDate
        OR team.ModifiedDate > @LastModifiedDate
        OR dept.ModifiedDate > @LastModifiedDate
        OR ejl.ModifiedDate > @LastModifiedDate
        OR jl.ModifiedDate > @LastModifiedDate
        OR jt.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[HRM_SyncData_GroupReport_GetEmployeeInfo] @LastModifiedDate DATETIME = NULL
AS
/*
Created: 20250220@Tim
Task: get employee info
DB: NexHRM
Original:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id AS EmpId
	    ,dept.Code AS DepartmentCode
	    ,dept.[Name] AS DepartmentName
	    ,team.Code AS TeamCode
	    ,team.[Name] AS TeamName
		,e.Code
		,e.NickName
		,e.FullName
        ,e.DOB
        ,e.Gender
        ,jl.[Name] AS JobLevel
	    ,jt.[Name] AS JobTitle
        ,e.GroupDate AS JoinedDate
		,e.JoinedDate AS CompanyDate
		,e.ConfirmedDate
		,e.ResignedDate
        ,CASE WHEN team.[Name] = 'Expat' THEN 0 ELSE 1 END AS IsLocal
		,e.[Enabled]
		,e.IsDeleted
		,e.IsSpecialUser
		,e.ModifiedDate 
	FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS team WITH (NOLOCK) ON e.GroupId = team.Id
        LEFT JOIN HRM.dbo.EmployeeJobGrade AS ejl WITH (NOLOCK) ON e.Id = ejl.EmpId
        AND ejl.IsCurrentUsed = 1
        LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON team.DepartmentId = dept.DepartmentId
        LEFT JOIN HRM.dbo.JobGrade AS jl WITH (NOLOCK) ON ejl.JobGradeId = jl.JobGradeId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE @LastModifiedDate IS NULL
        OR e.ModifiedDate > @LastModifiedDate
        OR team.ModifiedDate > @LastModifiedDate
        OR dept.ModifiedDate > @LastModifiedDate
        OR ejl.ModifiedDate > @LastModifiedDate
        OR jl.ModifiedDate > @LastModifiedDate
        OR jt.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Get] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20240723@Lilac.Nguyen
Task: Get history.
DB: NexHRM
Original: 
Revisions:  
	- 20250619@Henry.Dinh: Use GroupName from HistoryWorkDetail.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT h.Id
	,h.EmpId
	,e.NickName
	,e.FullName
	,hd.GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,h.AffectedDate
	FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
	WHERE (@GroupId IS NULL OR hd.GroupId = @GroupId)
		AND (@EmpId IS NULL OR h.EmpId = @EmpId)
		AND DATEDIFF(DAY, @FromDate, h.AffectedDate) >= 0
		AND DATEDIFF(DAY, @ToDate, h.AffectedDate) <= 0
       
	ORDER BY h.AffectedDate DESC
            
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_GetByEmpId]
    @EmpId INT
AS
/*
Created: 20240729@Lilac.Nguyen
Task: Get History by EmpId.
DB: NexHRM
Original: 
Revisions: 
	- 20250319@Henry.Dinh: Order by AffectedDate desc
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,hd.GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,mg.[Name] AS ManagementGradeName
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.ManagementGrade AS mg WITH (NOLOCK) ON mg.ManagementGradeId = hd.ManagementGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
    WHERE h.EmpId = @EmpId
	ORDER BY h.AffectedDate DESC  
END
GO


CREATE OR ALTER PROCEDURE [dbo].[History_GetById]
    @Id INT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Get History by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,h.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,hd.GroupId
	,hd.GroupName
	,hd.JobTitleId
	,hd.JobGradeId
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
    WHERE h.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Insert] 
    @EmpId INT
    ,@GroupId INT = NULL
	,@JobTitleId INT = NULL
	,@JobGradeId INT = NULL
    ,@AffectedDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Insert History
DB: NexHRM
Original: 

Revisions:
	- Remove check different from the current value for user init data
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewId INT;
	DECLARE @GroupName NVARCHAR(255);
	--DECLARE @CurrentGroupId INT, @CurrentJobTitleId INT, @CurrentJobGradeId INT;

	--SELECT 
	--	@CurrentGroupId = GroupId,
	--	@CurrentJobTitleId = JobTitleId
	--FROM dbo.Employee WITH (NOLOCK)
	--WHERE Id = @EmpId;

	--SELECT @CurrentJobGradeId = JobGradeId
	--FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
	--WHERE EmpId = @EmpId AND IsCurrentUsed = 1

	--IF @GroupId = @CurrentGroupId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Team must be different from the current Team!';
	--	RETURN;
	--END

	--IF @JobTitleId = @CurrentJobTitleId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Title must be different from the current Job Title!';
	--	RETURN;
	--END

	--IF @JobGradeId = @CurrentJobGradeId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Grade must be different from the current Job Grade!';
	--	RETURN;
	--END

    INSERT INTO dbo.HistoryWork(
		EmpId
		,AffectedDate
		,ModifiedBy
		,ModifiedDate
        )
    VALUES (
        @EmpId
		,@AffectedDate
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()
		)

	SET @NewId = SCOPE_IDENTITY();

	SELECT @GroupName = Name FROM dbo.EmployeeGroup WITH (NOLOCK) WHERE Id = @GroupId;

	INSERT INTO dbo.HistoryWorkDetail(
		HistoryWorkId
		,GroupId
		,GroupName
		,JobTitleId
		,JobGradeId
        )
    VALUES (
        @NewId
		,@GroupId
		,@GroupName
		,@JobTitleId
		,@JobGradeId
		)

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Insert history failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0 ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Update] 
	@Id INT
	,@EmpId	INT
    ,@GroupId INT = NULL
	,@JobTitleId INT = NULL
	,@JobGradeId INT = NULL
    ,@AffectedDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Update a history
DB: NexHRM
Original: 

Revisions:  
	- Remove check different from the current value for user init data
Param's Explanation:
� @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
� @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @GroupName NVARCHAR(255);
	--DECLARE @CurrentGroupId INT, @CurrentJobTitleId INT, @CurrentJobGradeId INT;

	--SELECT 
	--	@CurrentGroupId = GroupId,
	--	@CurrentJobTitleId = JobTitleId
	--FROM dbo.Employee WITH (NOLOCK)
	--WHERE Id = @EmpId;

	--SELECT @CurrentJobGradeId = JobGradeId
	--FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
	--WHERE EmpId = @EmpId AND IsCurrentUsed = 1

	--IF @GroupId = @CurrentGroupId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Team must be different from the current Team!';
	--	RETURN;
	--END

	--IF @JobTitleId = @CurrentJobTitleId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Title must be different from the current Job Title!';
	--	RETURN;
	--END

	--IF @JobGradeId = @CurrentJobGradeId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Grade must be different from the current Job Grade!';
	--	RETURN;
	--END

	SELECT @GroupName = Name FROM dbo.EmployeeGroup WITH (NOLOCK) WHERE Id = @GroupId;

    UPDATE dbo.HistoryWork WITH (ROWLOCK, UPDLOCK)
    SET 
		AffectedDate = @AffectedDate
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE Id = @Id

	UPDATE dbo.HistoryWorkDetail WITH (ROWLOCK, UPDLOCK)
    SET 
		GroupId = @GroupId
		,GroupName = @GroupName
        ,JobTitleId = @JobTitleId
        ,JobGradeId = @JobGradeId
    WHERE HistoryWorkId = @Id
  
    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update history failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0 ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutDisciplineViolate] 
	@FromDate DATE
    ,@ToDate DATE
AS
/*
    Created: 20200709@Merlin.Le
    Task: Get late in/ early out violate disciplines
    DB: NexHRM
    Revisions: 
		20250507@Henry.Dinh: update group id from 11 to 1081
    Original: 

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @CoreTeamId INT;
    DECLARE @ExpId INT;
    DECLARE @CEOTeamId INT;

    SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
           @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
           @CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
    FROM dbo.SystemSetting ss WITH (NOLOCK)
    INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
    WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

    IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutSummary;
    END;

    CREATE TABLE #LateInEarlyOutSummary (
        EmpId INT
        ,[Month] INT
        ,[Year] INT
        ,TotalTimes INT
        );

    INSERT INTO #LateInEarlyOutSummary
    SELECT DISTINCT tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
        ,COUNT(*)
    FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (@ExpId, @CoreTeamId, @CEOTeamId) -- expat, core
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND (CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate)
       
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM [dbo].Employee AS e WITH (NOLOCK)
        INNER JOIN [dbo].EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (@ExpId, @CoreTeamId, @CEOTeamId) -- expat, core
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND  (e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
                    )
				)
		
    GROUP BY e.Id 
        ,e.NickName
        ,e.FullName
        ,e.Code
		,e.Roles
		,e.GroupDate
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]

	HAVING COUNT(tr.TotalTimes) >=3
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate;

    SELECT su.EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary AS su WITH (NOLOCK)
	INNER JOIN (
		SELECT EmpId
		FROM #LateInEarlyOutSummary
		GROUP BY EmpId
		HAVING COUNT(EmpId) >=3) AS tmp ON tmp.EmpId = su.EmpId
    ORDER BY su.EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutOnMonth] 
	@Year INT
	,@Month INT
	
AS
/*
	Created: 20200708@Merlin.Le
	Task: Get late in/ early out on current month
	DB: NexHRM & NexBot
	Original: 

	Revisions:  
		20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CoreTeamId NVARCHAR(50);
	DECLARE @ExpId NVARCHAR(50);
	DECLARE @CEOTeamId NVARCHAR(50);

	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary
	END

	CREATE TABLE #LateInEarlyOutSummary(
		EmpId INT
		,MinuteOnMonth INT
		,TimeOnMonth INT)

	INSERT INTO #LateInEarlyOutSummary(EmpId, MinuteOnMonth, TimeOnMonth)

	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0
						THEN 1
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.GroupId NOT IN (@ExpId, @CoreTeamId, @CEOTeamId)
	 AND YEAR(WorkDate) = @Year AND Month(WorkDate) = @Month
	GROUP BY EmpId

	SELECT e.Id AS EmpId
		  ,e.FullName
		  ,e.NickName
		  ,ec.ConversationId
		  ,ls.MinuteOnMonth
		  ,ls.TimeOnMonth
	FROM #LateInEarlyOutSummary AS ls WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON ls.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
		--LEFT JOIN [NexBot].[dbo].[ConversationUser] AS c WITH (NOLOCK) ON e.NickName = c.NickName
		LEFT JOIN [dbo].[EmployeeConversation] AS ec WITH (NOLOCK) ON ec.EmpId = e.Id
	WHERE e.isDeleted = 0 AND e.[Enabled] = 1
		AND ls.TimeOnMonth >= 3
		
	DROP TABLE #LateInEarlyOutSummary	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutToDeduct] @Year INT
	
AS
/*
Created: 20200709@Merlin.Le
Task: Get late in/early out need to deduct
DB: NexHRM
Original: 
Revisions:
	- 20250328@Henry.Dinh: Update GroupID 11-40 -> GroupCode MGT/COR/EXP
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @CoreTeamId INT;
	DECLARE @ExpId INT;
	DECLARE @CEOTeamId INT;

	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#tblAnnualLeaveBalance') IS NOT NULL
	BEGIN
		DROP TABLE #tblAnnualLeaveBalance;
	END;

	CREATE TABLE #tblAnnualLeaveBalance (
		EmpId INT
		,AnnualLeaveBalance NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest;
	END;

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance;
	END;

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		);

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance;
	END;

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,Overtime INT
		);

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		);

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0)
			END
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,Overtime
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.IsSpecialUser = 0
		AND YEAR(WorkDate) = @Year
	GROUP BY EmpId;

	INSERT INTO #tblAnnualLeaveBalance(EmpId, AnnualLeaveBalance)
	SELECT lt.EmpId, SUM(lt.CreditDays) - SUM(lt.DeductDays)
	FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lt.EmpId = e.Id
	WHERE e.Enabled = 1 
	AND e.IsDeleted = 0
	AND YEAR(lt.FromDate) = @Year
	AND lt.Deleted = 0
	GROUP BY lt.EmpId

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT e.Id, SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
		AND YEAR(wk.FromDate) = @Year
	GROUP BY e.Id


	SELECT dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,al.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN [dbo].EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		INNER JOIN (
			SELECT EmpId
			,(SUM(CreditDays) - SUM(DeductDays)) AS AnnualLeaveBalance
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e ON lt.EmpId = e.Id AND e.isDeleted = 0
			WHERE YEAR(FromDate) = @Year
			AND ISNULL(lt.Deleted, 0) = 0
			GROUP BY EmpId
		) AS leasum ON e.Id = leasum.EmpId
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #tblAnnualLeaveBalance AS al ON al.EmpId = pv.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE eg.Id NOT IN (@CoreTeamId, @ExpId, @CEOTeamId)
		AND ISNULL(cu.[Minutes], 0) >= 240
		AND (leasum.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) > 0)
		
	ORDER BY e.NickName;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Notification_GetAllViolation] @EmpId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CoreTeamId INT;
	DECLARE @ExpId INT;
	DECLARE @CEOTeamId INT;
	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#LateinEarlyOutViolationInYear') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInYear;
	END;

	CREATE TABLE #LateinEarlyOutViolationInYear (
		EmpId INT
		,NickName VARCHAR(100)
		,Fullname NVARCHAR(max)
		,TimesWithinAYear INT
		);

	IF OBJECT_ID('#LateinEarlyOutViolationInMonth') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInMonth;
	END;

	CREATE TABLE #LateinEarlyOutViolationInMonth (
		EmpId INT
		,TimesWithinThisMonth INT
		);

	IF OBJECT_ID('#CheckInOutViolation') IS NOT NULL
	BEGIN
		DROP TABLE #CheckInOutViolation;
	END;

	CREATE TABLE #CheckInOutViolation (
		EmpId INT
		,TimesMissingThisMonth INT
		);

	IF OBJECT_ID('#ToeicExpired') IS NOT NULL
	BEGIN
		DROP TABLE #ToeicExpired;
	END;

	CREATE TABLE #ToeicExpired (
		EmpId INT
		,IsExpired BIT
		);
--------
IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,[Month] INT
		,[Year] INT
		,TotalTimes INT
		);

	INSERT INTO #LateInEarlyOutSummary
	SELECT DISTINCT tr.EmpId
		,MONTH(tr.WorkDate)
		,YEAR(tr.WorkDate)
		,COUNT(*)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
		AND e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId) -- management, expat
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
	WHERE tr.[Type] IN (1, 2)
		AND CAST(tr.WorkDate AS DATE) BETWEEN DATEADD(m, - 12, dbo.fnc_GetClientDateTime())
			AND dbo.fnc_GetClientDateTime()
		AND e.Id = @EmpId
	GROUP BY tr.EmpId
		,MONTH(tr.WorkDate)
		,YEAR(tr.WorkDate)
	HAVING COUNT(*) >= 4
---------
	--late in/early out violation--
	INSERT INTO #LateinEarlyOutViolationInYear (
		EmpId
		,NickName
		,Fullname
		,TimesWithinAYear
		)
	SELECT e.Id
		,e.NickName
		,e.FullName
		,COUNT(tr.TotalTimes) AS TotalTimes
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
	WHERE 
	   --e.GroupId NOT IN (11, 40) -- management, expat
		ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.Id = @EmpId
	GROUP BY e.Id
		,e.NickName
		,e.FullName
		,e.Code
		,e.Roles
		,e.GroupId
		,e.GroupDate
		,g.Code
		,g.[Name]
		,g.[Rank]
	ORDER BY g.[Rank]
		,g.Code
		,e.Roles DESC
		,e.GroupDate;

	---------------
	DECLARE @StartOfMonth DATETIME
		,@EndOfMonth DATETIME

	SET @StartOfMonth = DATEADD(month, DATEDIFF(month, 0, dbo.fnc_GetClientDateTime()), 0)
	SET @EndOfMonth = DATEADD(month, DATEDIFF(month, 0, dbo.fnc_GetClientDateTime()) + 1, - 1)

	INSERT INTO #LateinEarlyOutViolationInMonth (
		EmpId
		,TimesWithinThisMonth
		)
	SELECT e.Id
		,COUNT(lieot.WorkDate) AS 'TimesWithinThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM dbo.LateInEarlyOutTrans lieot
		WHERE lieot.Type IN (1, 2)
			AND lieot.WorkDate BETWEEN @StartOfMonth
				AND @EndOfMonth
		) lieot ON e.Id = lieot.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.id
		,e.NickName
		,e.FullName

	-----------------------
	DECLARE @missingCheckInOut TABLE (
		WorkDate DATETIME
		,isCurrent BIT
		,TypeId BIT
		,LeaveDay FLOAT
		,EmpId INT
		,Nickname VARCHAR(100)
		,Fullname NVARCHAR(max)
		)

	INSERT @missingCheckInOut
	EXEC [dbo].[Report_EmployeeSpaceDashBoard] @EmpId = @EmpId

	INSERT INTO #CheckInOutViolation (
		EmpId
		,TimesMissingThisMonth
		)
	SELECT e.Id
		,COUNT(mcio.WorkDate) 'TimesMissingInThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM @missingCheckInOut mcio
		WHERE mcio.isCurrent = 1
			AND mcio.TypeId = 3
		) mcio ON e.Id = mcio.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.Id
		,e.NickName
		,e.FullName



	SELECT leoviy.TimesWithinAYear AS 'LateInEarlyOutInYear'
		,leovim.TimesWithinThisMonth AS 'LateInEarlyOutInMonth'
		,ciov.TimesMissingThisMonth AS 'TimeMisssingCheckInOutThisMonth'
		,leoviy.EmpId
		,leoviy.NickName
		,leoviy.Fullname		
	FROM #LateinEarlyOutViolationInYear leoviy
		JOIN #LateinEarlyOutViolationInMonth leovim ON leoviy.EmpId = leovim.EmpId
		JOIN #CheckInOutViolation ciov ON leoviy.EmpId = ciov.EmpId
		JOIN #ToeicExpired te ON leoviy.EmpId = te.EmpId

	DROP TABLE #LateinEarlyOutViolationInYear;

	DROP TABLE #LateinEarlyOutViolationInMonth;

	DROP TABLE #CheckInOutViolation;

	DROP TABLE #ToeicExpired;
	
	DROP TABLE #LateInEarlyOutSummary;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ApproveRequest] 
	@Id             INT
    ,@ModifiedBy    INT
    ,@ErrCode       INT				= 0 OUTPUT
    ,@ErrMsg        VARCHAR(200)	= '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request  
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @CoreTeamId INT = 0;
	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    DECLARE
		@PendingStatus		INT = 1
		,@ApprovedStatus	INT	= 2;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] = @PendingStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can approve the pending NEX request';

		RETURN;
	END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
			WHERE e.[Id] = @ModifiedBy
				AND eg.[Id] = @CoreTeamId
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers have a permisson to aprove NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET
            [Status]        = @ApprovedStatus
            ,[ModifiedBy]   = @ModifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [OvertimeId] = @Id AND [Status]=1 -- pending;

		COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Approve NEX request failed';

		ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DeleteConversion]
    @modifiedBy     INT
    ,@id            INT
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 20200723@Dwight.Tran
Task: Enhance NEX request
DB: NexHRM
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
    - 20250507@Henry.Dinh: update HR code to HR-ADM
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CoreTeamId INT = 0;
    DECLARE @HRTeamId INT = 0;

	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END), 0),
		@HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'CoreTeamId')
    
    DECLARE
        @empId     INT
        ,@year          INT
        ,@oldMinutes    INT
		,@HRCode    VARCHAR (2) = 'HR-ADM'

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Id] = @HRTeamId
                OR eg.[Id] = @CoreTeamId
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to delete OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId     = [EmpId]
        ,@year          = Year([DeductDate])
        ,@oldMinutes    = [Deduct]
    FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [dbo].[OvertimeConversion]
        WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Delete OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DisapproveRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200903Dwight.Tran: Enhance NEX Request.
	- 20250507@Henry.Dinh: update group id from 11 to 1081
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
        @PendingStatus     INT = 1
        ,@ApprovedStatus	INT = 2
		,@CoreTeamId INT = 0;

	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] = @ApprovedStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can disapprove approved NEX request';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND (e.[GroupId] = @CoreTeamId OR 
					(e.Roles IN (3, 4))) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers or HR have a permisson to disapproved NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Disapprove NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;



GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetSummary] 
	@year			INT
	,@employeeId	INT = NULL
	,@groupId		INT = NULL
	,@departmentId	INT = NULL
AS
/*
	Created: 
	Task: 
	DB: NexHRM
	Original:
	Revisions:
		- 20200723Dwight.Tran: Enhance NEX request
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:
*/
BEGIN
	
	DECLARE @CoreTeamId INT = 0;
	DECLARE @ExpId INT = 0;
	DECLARE @CEOTeamId INT = 0;
	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance
	END

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		)

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance
	END

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,LateInEarlyOut INT
		)

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(oty.[Minutes], 0) - CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0)
			END
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,LateInEarlyOut
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @year
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)

	SELECT ott.EmpId AS EmployeeId
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 1
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jan'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 2
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Feb'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 3
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Mar'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 4
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Apr'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 5
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'May'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 6
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jun'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 7
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jul'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 8
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Aug'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 9
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Sep'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 10
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Oct'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 11
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Nov'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 12
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Dec'
		,SUM(CASE 
				WHEN ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'TotalHour'
		,SUM(CASE 
				WHEN ott.[Type] = 0 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'LeaveConversion'
	INTO #OvertimeSummary
	FROM [dbo].[OvertimeTrans] AS ott
	WHERE YEAR(ott.[Date]) = @year
	GROUP BY ott.EmpId

	IF OBJECT_ID('#TempOvertimeConversion') IS NOT NULL
	BEGIN
		DROP TABLE #TempOvertimeConversion;
	END;

	CREATE TABLE #TempOvertimeConversion (
		[EmpId]	INT
		,[Deduct]		INT
	);

	INSERT INTO #TempOvertimeConversion
	SELECT
		[EmpId]
		,SUM([Deduct])
	FROM [dbo].[OvertimeConversion]
	WHERE Year([DeductDate]) = @year
	GROUP BY [EmpId]

	SELECT ISNULL(os.Jan, 0) / 60.0 AS 'Jan'
		,ISNULL(os.Feb, 0) / 60.0 AS 'Feb'
		,ISNULL(os.Mar, 0) / 60.0 AS 'Mar'
		,ISNULL(os.Apr, 0) / 60.0 AS 'Apr'
		,ISNULL(os.May, 0) / 60.0 AS 'May'
		,ISNULL(os.Jun, 0) / 60.0 AS 'Jun'
		,ISNULL(os.Jul, 0) / 60.0 AS 'Jul'
		,ISNULL(os.Aug, 0) / 60.0 AS 'Aug'
		,ISNULL(os.Sep, 0) / 60.0 AS 'Sep'
		,ISNULL(os.Oct, 0) / 60.0 AS 'Oct'
		,ISNULL(os.Nov, 0) / 60.0 AS 'Nov'
		,ISNULL(os.DEC, 0) / 60.0 AS 'Dec'
		,ISNULL(os.TotalHour, 0) / 60.0 AS TotalHour
		,ISNULL(os.LeaveConversion, 0) / 60.0 AS LeaveConversion
		,ISNULL(prev.Minutes, 0) / 60.0 AS BalanceLastYear
		,ISNULL(cur.[Minutes], 0) / 60.0 AS BalanceCurrentYear
		,ISNULL(cur.LateInEarlyOut, 0) / 60.0 AS LateInEarlyOut
		,ISNULL(totc.[Deduct], 0) / 60.0 AS OvertimeDeduct
		,e.Id AS EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,eg.DepartmentId
		,d.Name AS DepartmentName
		,e.GroupId AS TeamId
		,eg.Code AS TeamCode
		,eg.NAME AS TeamName
	FROM dbo.Employee e
		INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON eg.DepartmentId = d.DepartmentId
		LEFT JOIN #OvertimeSummary AS os WITH (NOLOCK) ON e.Id = os.EmployeeId
		LEFT JOIN #CurrentYearBalance AS cur WITH (NOLOCK) ON e.Id = cur.EmpId
		LEFT JOIN #PreviousYearBalance AS prev WITH (NOLOCK) ON e.Id = prev.EmpId
		LEFT JOIN #TempOvertimeConversion AS totc WITH (NOLOCK) ON e.[Id] = totc.[EmpId]
	WHERE e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId)
	ORDER BY eg.[Rank]
		,eg.Code
		,e.Roles DESC
		,e.GroupDate

	DROP TABLE #OvertimeSummary

	DROP TABLE #PreviousYearBalance

	DROP TABLE #CurrentYearBalance

	DROP TABLE #TempOvertimeConversion
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_InsertConversion]
    @createdBy      INT
    ,@empId    INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
    Created: 20200723@Dwight.Tran
    Task: Enhance NEX request
    DB: NexHRM
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:
*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldMinutes INT
		,@year INT = YEAR(@deductDate)
        
	DECLARE @HRTeamId INT;
	DECLARE @CoreTeamId INT;

	SELECT @HRTeamId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END),
		@CoreTeamId = MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'CoreTeamId')


    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @createdBy
            AND (
                eg.[Id] = @HRTeamId
                OR eg.[Id] = @CoreTeamId
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to deduct NEX balance';

        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

	
    SELECT @oldMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly]
    WHERE [EmpId] = @empId
        AND [Year] = @year;

    IF (@oldMinutes < (@deduct * 60.0))
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[OvertimeConversion] (
            [EmpId]
            ,[Deduct]
            ,[DeductDate]
            ,[Description]
			,[ModifiedBy]
			,[ModifiedDate]
        )
        VALUES (
            @empId
            ,@deduct * 60.0
            ,@deductDate
            ,@description
			,@createdBy,
			dbo.fnc_GetClientDateTime()
        )

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] - (@deduct * 60.0)
            ,[ModifiedBy]   = @createdBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Deduct NEX balance fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_RejectRequest] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request.  
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CoreTeamId INT = 0;

	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    DECLARE
        @RejectStatus      INT = 3
        ,@PendingStatus		INT = 1;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE OvertimeId = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE OvertimeId = @Id
				AND [Status] = @PendingStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can reject pending NEX request';

		RETURN;
	END;

    IF (
        NOT EXISTS (
			SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
			WHERE e.[Id] = @ModifiedBy
				AND eg.[Id] = @CoreTeamId
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers have a permisson to reject NEX request'

		RETURN;
	END;

    BEGIN TRY
        UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET
            [Status]        = @RejectStatus
            ,[ModifiedBy]   = @ModifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Reject OT request failed';
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ResetRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
	Created: 20200903Dwight.Tran
	Task: Enhance NEX Request.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
        @PendingStatus		INT = 1;

	DECLARE @CoreTeamId INT = 0;
	SELECT @CoreTeamId = g.Id
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] BETWEEN 2 AND 5
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can reset NEX request when status is not pending';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND (e.[GroupId] = @CoreTeamId OR 
					(e.Roles IN (3, 4))) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to void NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

		EXECUTE dbo.[Overtime_DeleteTransaction] @Id, @ModifiedBy;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Reset NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;



GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_UpdateConversion]
    @modifiedBy     INT
    ,@id            INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate	DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
    Created: 20200723@Dwight.Tran
    Task: Enhance NEX request
    DB: NexHRM
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldYearlyMinutes   INT
        ,@empId				INT
        ,@year              INT = Year(@deductDate)
		,@oldYear			INT
        ,@oldMinutes        INT


    DECLARE @HRTeamId INT = 0;
	DECLARE @CoreTeamId INT = 0;
	SELECT @HRTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('HRTeamId', 'CoreTeamId')

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Id] = @HRTeamId
                OR eg.[Id] = @CoreTeamId
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to update OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId = [EmpId]
		,@oldYear = Year([DeductDate])
		,@oldMinutes = [Deduct]
    FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    SELECT @oldYearlyMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly] as oty WITH (NOLOCK)
    WHERE oty.[EmpId] = @empId
        AND oty.[Year] = @year;

    IF (@oldYearlyMinutes + @oldMinutes < @deduct * 60.0)
    BEGIN
        SELECT
            @errCode = 4
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		UPDATE [dbo].[OvertimeConversion] WITH (ROWLOCK, UPDLOCK)
		SET
			[Deduct]        = @deduct * 60
			,[DeductDate] = @deductDate
			,[Description]  = @description
			,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			,[ModifiedBy] = @modifiedBy
		WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes - (@deduct * 60.0)
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

		IF(@oldYear < @year)
		BEGIN
			UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]       = [Minutes] + @oldMinutes
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			WHERE [EmpId] = @empId
				AND [Year] >= @oldYear AND [Year] < @year;
		END;
		COMMIT TRANSACTION;
        
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Update OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_VoidRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
	Created: 20200903Dwight.Tran
	Task: Enhance NEX Request.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
        @VoidStatus		INT = 5
        ,@ApprovedStatus	INT = 2
		,@VerifiedStatus	INT = 4;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND ([Status] = @ApprovedStatus OR [Status] = @VerifiedStatus)
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can void approved NEX request or verified NEX request';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND e.Roles IN (3, 4) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to void NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @VoidStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

		EXECUTE dbo.[Overtime_DeleteTransaction] @Id, @ModifiedBy;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Void NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;



GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetLateInEarlyOut] @Year INT
	,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
	Created: 20150805@Quinn.Vo
	Task: Get late in/ early out summary report
	DB: NexHRM
	Original: 

	Revisions:
		- 20160129@Razor.Nguyen: OT Feature
		- 20250411@Henry.Dinh: Ignore CORE Team
	Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CoreTeamId INT;
	DECLARE @ExpId INT;
	DECLARE @CEOTeamId INT;
	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#tblAnnualLeaveBalance') IS NOT NULL
	BEGIN
		DROP TABLE #tblAnnualLeaveBalance;
	END;

	CREATE TABLE #tblAnnualLeaveBalance (
		EmpId INT
		,AnnualLeaveBalance NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest;
	END;

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance;
	END;

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		);

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance;
	END;

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,Overtime INT
		);

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,Jan INT
		,Feb INT
		,Mar INT
		,Apr INT
		,May INT
		,Jun INT
		,Jul INT
		,Aug INT
		,Sep INT
		,Oct INT
		,Nov INT
		,[Dec] INT
		,TimesInJan INT
		,TimesInFeb INT
		,TimesInMar INT
		,TimesInApr INT
		,TimesInMay INT
		,TimesInJun INT
		,TimesInJul INT
		,TimesInAug INT
		,TimesInSep INT
		,TimesInOct INT
		,TimesInNov INT
		,TimesInDec INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		);

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0)
			END
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			);

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,Overtime
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			);

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
		,Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,DEC
		,TimesInJan
		,TimesInFeb
		,TimesInMar
		,TimesInApr
		,TimesInMay
		,TimesInJun
		,TimesInJul
		,TimesInAug
		,TimesInSep
		,TimesInOct
		,TimesInNov
		,TimesInDec
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.IsSpecialUser = 0
		AND YEAR(WorkDate) = @Year
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
	GROUP BY EmpId;

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT rq.EmpId
		,SUM(rq.TotalDays)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
		LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
	WHERE [Status] = 1
		AND RequestType = 6
		AND ty.IsALDeduction = 1
	GROUP BY rq.EmpId;

	INSERT INTO #tblAnnualLeaveBalance(EmpId, AnnualLeaveBalance)
	SELECT lt.EmpId, SUM(lt.CreditDays) - SUM(lt.DeductDays)
	FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lt.EmpId = e.Id
	WHERE e.Enabled = 1 
	AND e.IsDeleted = 0
	AND YEAR(lt.FromDate) = @Year
	AND lt.Deleted = 0
	GROUP BY lt.EmpId

	SELECT dt.Jan
		,dt.Feb
		,dt.Mar
		,dt.Apr
		,dt.May
		,dt.Jun
		,dt.Jul
		,dt.Aug
		,dt.Sep
		,dt.Oct
		,dt.Nov
		,dt.[Dec]
		,dt.TimesInJan
		,dt.TimesInFeb
		,dt.TimesInMar
		,dt.TimesInApr
		,dt.TimesInMay
		,dt.TimesInJun
		,dt.TimesInJul
		,dt.TimesInAug
		,dt.TimesInSep
		,dt.TimesInOct
		,dt.TimesInNov
		,dt.TimesInDec
		,dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,pv.[Minutes] AS BalanceLastYear
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,al.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
		,g.Id AS TeamId
		,g.Code AS TeamCode
		,g.NAME AS TeamName
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #tblAnnualLeaveBalance AS al ON al.EmpId = pv.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId)
	ORDER BY g.[Rank]
		,g.Code
		,e.Roles DESC
		,e.GroupDate;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetLateInEarlyOutDetail] --'2015-08-01', '2015-08-31'
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
    ,@EmployeeId INT = NULL
    ,@IncludeLeaveDeduction BIT = 0
AS
/*
    Created: 20150805@Quinn.Vo
    Task: Get late in/ early out detail report
    DB: NexEPA
    Original: 

    Revisions:  
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @CoreTeamId INT = 0;
    DECLARE @ExpId INT = 0;
    DECLARE @CEOTeamId INT = 0;
    SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
        @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
        @CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
    FROM dbo.SystemSetting ss WITH (NOLOCK)
    INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
    WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

    SELECT 
        la.WorkDate
        ,la.[Minutes]
        ,la.[Type] AS TypeId
        ,la.LeaveDeductionId
        ,la.ModifiedDate
        ,la.EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.Name
    FROM dbo.LateInEarlyOutTrans AS la WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON la.EmpId = e.Id
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
    WHERE 
        e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId)
        AND DATEDIFF(D, @FromDate, la.WorkDate) >= 0
        AND DATEDIFF(D, la.WorkDate, @ToDate) >= 0
        AND (@IncludeLeaveDeduction = 1 OR la.[Type] <> 0)
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
END

GO

CREATE OR ALTER PROCEDURE [dbo].[Report_LateInEarlyOutSummary] @FromDate DATE
    ,@ToDate DATE
    ,@GroupId INT = NULL
    ,@EmpId INT = NULL
    ,@IsUserWarning BIT = NULL
AS
/*
    Created: 20160219@Tim.Trinh
    Task: Get late in/ early out summary report
    DB: NexHRM
    Revisions: 
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Original: 

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @CoreTeamId INT = 0;
	DECLARE @ExpId INT = 0;
	DECLARE @CEOTeamId INT = 0;
	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

    IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutSummary;
    END;

    CREATE TABLE #LateInEarlyOutSummary (
        EmpId INT
        ,[Month] INT
        ,[Year] INT
        ,TotalTimes INT
        );

    INSERT INTO #LateInEarlyOutSummary
    SELECT DISTINCT tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
        ,COUNT(*)
    FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (@CoreTeamId, @ExpId, @CEOTeamId) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @IsUserWarning IS NULL
            OR @IsUserWarning = 0
            OR (
                @IsUserWarning = 1
                AND e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
                    )
                )
            )
    GROUP BY e.Id
        ,e.NickName
        ,e.FullName
        ,e.Code
		,e.Roles
        ,e.GroupDate
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    SELECT EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary
    ORDER BY EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;

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

	SELECT att.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		att.AttendanceDate, 
		att.InFixed, 
		att.LeaveDay, 
		att.OutFixed, 
		att.LateIn,
		att.EarlyOut, 
		att.LeaveTypeId, 
		att.LeaveTypeId2, 
		att.Absence, 
		att.TotalWorkHours,
		att.InsertDate
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = att.EmpId
	WHERE att.InsertDate > @LastModifiedDate
	
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
	
	SELECT cl.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		cl.WorkDate, 
		cl.IsOffDay, 
		cl.IsHoliday,
		cl.IsLeaveDay, 
		cl.LeaveDay,
		cl.ModifiedDate
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = cl.EmpId
	WHERE cl.ModifiedDate > @LastModifiedDate
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
		ISNULL(e.OldId, e.Id) AS EmployeeId, 
		otd.[Date], 
		otd.[Hour],
		ISNULL(ot.ModifiedDate, ot.CreatedDate) AS ModifiedDate
	FROM dbo.Overtime AS ot WITH (NOLOCK) 
	INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId
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
	
	SELECT ISNULL(e.OldId, e.Id) AS EmpId, 
		toeic.MinimumScore, 
		CAST(toeic.InputDate AS DATE) AS InputDate, 
		CAST(toeic.LRExamDate AS DATE) AS LRExamDate, 
		toeic.Listening, 
		toeic.Reading,
		ISNULL(toeic.ModifiedDate, toeic.CreatedDate) AS ModifiedDate
	FROM  HRM.dbo.TOEIC AS toeic WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = toeic.EmpId
	WHERE ISNULL(toeic.ModifiedDate, toeic.CreatedDate) > @LastModifiedDate
END
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
	
	SELECT ISNULL(e.OldId, e.Id) AS EmployeeId, 
		e.DomainUsername, 
		e.Code, 
		e.FullName, 
		e.NickName, 
		ISNULL(eg.OldId, e.GroupId) AS GroupId, 
		e.[Enabled], 
		e.isDeleted, 
		CAST(e.JoinedDate AS DATE) AS JoinedDate, 
		CAST(e.ConfirmedDate AS DATE) 
        AS ConfirmedDate,
		e.JobTitleId,
		e.Email,
		ec.ConversationId,
		e.ModifiedDate
	FROM dbo.Employee AS e WITH (NOLOCK) 
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT OUTER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE e.ModifiedDate > @LastModifiedDate
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
	
	SELECT ISNULL(Id, OldId) AS GroupId, 
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
	
	SELECT wtr.Id,
		ISNULL(e.OldId, e.Id) AS EmpId, 
		wtr.RequestType, 
		wtr.FromDate, 
		wtr.ToDate, 
		wtr.TotalDays, 
		wtr.[Status],
		ISNULL(wtr.ModifiedDate, wtr.CreatedDate) AS ModifiedDate
	FROM dbo.WorkingTimeRequest AS wtr WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wtr.EmpId
	WHERE ISNULL(wtr.ModifiedDate, wtr.CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetUpdatingScoreRelatedPeople] @id INT
AS
/*
    Created: 20150403@Tim.Trinh
    Task: Get related people to send mail when update toeic.
    DB: NexEPA
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
    Param's Explanation:
*/
BEGIN
    DECLARE @CoreTeamId INT = 0;
	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
    WHERE e.ID = @id
    
    UNION
    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON e.GroupId = gr.Id
    WHERE gr.Id = @CoreTeamId
	AND e.isDeleted = 0
    
    UNION
    SELECT DISTINCT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM dbo.Employee e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON e.Id = sup.ApproverId
    WHERE e.isDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetPendingRequest] 
AS
/*
Created: 20240621@Oliver.Dang
Task: Get all pending requests.
DB: NexHRM
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
		,@CoreTeamId INT = 0;

	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'

    IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END

	CREATE TABLE #ManagementApprover(
		ApproverId INT
		,Approver NVARCHAR(20)
		,ApproverFullName NVARCHAR(50)
		,ApproverConversationId VARCHAR(200)
	)

	CREATE TABLE #PendingRequest(
		RequestId BIGINT
		,EmpId INT
		,GroupId INT
		,TeamName NVARCHAR(50)
		,Code VARCHAR(20)
		,RequestType SMALLINT
		,CreatedDate DATETIME
		,Requester NVARCHAR(20)
		,FullNameRequester NVARCHAR(50)
		,FromDate DATETIME
		,ToDate DATETIME
		,TotalDays NUMERIC(5, 1)
		,FromShift VARCHAR(10)
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(100)
		,Remark NVARCHAR(2000)
		,IsManagementApprove BIT
		,IsStandbyAtWeekend BIT
		)

	INSERT INTO #PendingRequest
	SELECT r.Id as RequestId
		  ,r.EmpId
		  ,e.GroupId
		  ,g.Code
		  ,e.Code
		  ,r.RequestType
		  ,r.CreatedDate
  		  ,e.NickName AS Requester
		  ,e.FullName AS FullNameRequester
		  ,r.FromDate
		  ,r.ToDate
  		  ,r.TotalDays
		  ,s.Code
		  ,lv.Id AS LeaveTypeId
		  ,lv.[Name] AS LeaveTypeName
		  ,r.Remark
		  ,(CASE WHEN 
				(r.RequestType = 6) AND -- Leave type
					(r.LeaveTypeID IN (2, 30, 31) AND r.TotalDays >= 5) -- Annual Leave >= 5 days
					OR 
					r.LeaveTypeID = 26 -- Unpaid leave
			THEN 1
			ELSE 0
			END) as IsManagementApprove
			,(CASE WHEN 
				(r.RequestType IN (1,2) AND DATEPART(DW, r.FromDate) IN (1, 7) AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ',')))
			THEN 1
			ELSE 0
			END) as IsStandbyAtWeekend
	FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
	INNER JOIN dbo.Employee e with (nolock) on r.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup g with (nolock) on e.GroupId = g.Id
	LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
	LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
		AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
	LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.id = c.ShiftID
	WHERE e.[Enabled] = 1 AND ISNULL(e.isDeleted, 0 ) = 0 
		AND r.RequestType IN (1,2,6)
		AND r.[Status] = 1
	ORDER BY r.CreatedDate DESC


	IF EXISTS(SELECT 1
	FROM #PendingRequest 
	WHERE IsManagementApprove = 1)
	BEGIN
		INSERT INTO #ManagementApprover
		SELECT e.Id
			  ,e.NickName
			  ,e.FullName
			  ,con.ConversationId
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.[Enabled] = 1 AND eg.Id = @CoreTeamId AND con.ConversationId <> ''
	END

	SELECT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,ap.ApproverId
		 ,ap.Approver
		 ,ap.ApproverFullName
		 ,ap.ApproverConversationId
	FROM #PendingRequest pr
	CROSS JOIN  #ManagementApprover ap
	WHERE IsManagementApprove = 1

	UNION
	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,e.Id as ApproverId
		 ,e.NickName as Approver
		 ,e.FullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = @LeaderOfStandbyTeam
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsStandbyAtWeekend = 1

	UNION

	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,sup.ApproverId as ApproverId
		 ,sup.ApproverNickName as Approver
		 ,sup.ApproverFullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON pr.EmpId = sup.EmpId
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON sup.ApproverId = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsManagementApprove = 0
		AND pr.IsStandbyAtWeekend = 0

	IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByDate] @role INT
    ,@fromdate DATETIME
    ,@todate DATETIME
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@leaveTypeId INT = NULL
    ,@status TINYINT = NULL
	,@actorId INT = NULL
    ,@excludedEmpID INT = NULL
    ,@requestType VARCHAR(10) = ''
    ,@isFilterByEffectedDate BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexEPA
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    DECLARE @IsManagement BIT
        ,@IsLeaderOfStandByTeam BIT
    DECLARE @RequestTypeId TABLE (RequestType INT)
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
		,@CoreTeamId INT = 0,
        @CEOTeamId INT = 0;

	SELECT @CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
        @CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('CoreTeamId', 'CEOTeamId')

	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'


    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END

    CREATE TABLE #WorkingTimeRequest (
        Id BIGINT
        ,EmpId INT
        ,RequestType SMALLINT
        ,RequestedDate DATETIME
        ,CreatedDate DATETIME
        ,[Status] TINYINT
        ,Requester NVARCHAR(20)
        ,FullNameRequester NVARCHAR(50)
        ,EmailRequester VARCHAR(50)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,ProcessedBy NVARCHAR(20)
        ,FullNameProcessedBy NVARCHAR(50)
        ,EmailProcessedBy VARCHAR(50)
        ,ProcessedDate DATETIME
        ,TotalDaysRequestInMonth DECIMAL
        ,FromShift VARCHAR(10)
        ,ToShift VARCHAR(10)
        ,TotalMins INT
        ,FromDate DATETIME
        ,ToDate DATETIME
        ,TotalDays NUMERIC(5, 1)
        ,LeaveTypeId INT
        ,LeaveTypeName NVARCHAR(100)
        ,Remark NVARCHAR(2000)
        ,IsAllowUpdateStatus TINYINT
		,IsCrossDate BIT
        )

    IF (
            EXISTS (
                SELECT 1
                FROM [dbo].[Employee] AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.ID = @excludedEmpID
                    AND g.Id IN (@CoreTeamId, @CEOTeamId)
                )
            )
    BEGIN
        SET @IsManagement = 1
    END
    ELSE
    BEGIN
        SET @IsManagement = 0
    END

	IF (@excludedEmpID = @LeaderOfStandbyTeam)
	BEGIN
		SET @IsLeaderOfStandByTeam = 1
	END
	ELSE
    BEGIN
        SET @IsLeaderOfStandByTeam = 0
    END

	IF ISNULL(@requestType, '') = ''
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		VALUES (1)
			,(2)
			,(3)
			,(4)
			,(5)
			,(6)
	END
	ELSE
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@requestType, ',')
	END

    INSERT INTO #WorkingTimeRequest
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,(CASE 
            WHEN (
                    @IsLeaderOfStandByTeam = 0
                    AND r.RequestType IN (1, 2)
                    AND DATEPART(DW, c.WorkDate) IN (1, 7)
					AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
                    ) THEN 0
            ELSE (CASE 
                    WHEN (
                            @IsManagement = 0
                            AND r.RequestType = 6
                            AND (
                                r.LeaveTypeID = 26
                                OR (
                                    r.LeaveTypeID IN (2, 30, 31)
                                    AND r.TotalDays >= 5
                                    )
                                )
                            ) THEN 0
                    ELSE 1 END
                    ) END
            ) AS IsAllowUpdateStatus
			,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN dbo.Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
                    @role NOT IN (1, 2)
                    OR r.empID IN (
						SELECT EmpId FROM dbo.EmployeeSupDetail WITH (NOLOCK) WHERE ApproverId = @excludedEmpID
                        )
            )
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    --ORDER BY r.FromDate DESC
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE @IsManagement = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType = 6
            AND (
                r.LeaveTypeID = 26
                OR (
                    r.LeaveTypeID IN (2, 30, 31)
                    AND r.TotalDays >= 5
                    )
                )
            )
        AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE @IsLeaderOfStandByTeam = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType IN (1, 2)
            AND DATEPART(DW, c.WorkDate) IN (1, 7)
			AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    ORDER BY r.CreatedDate DESC
        ,r.FromDate DESC

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @ItemsPerPage ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC
    END

    SELECT @TotalItem = COUNT(*)
    FROM #WorkingTimeRequest

    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Insert] @id BIGINT OUT
	,@EmpId INT
	,@RequestType SMALLINT
	,@IsPlanned BIT
	,@Remark NVARCHAR(2000) = NULL
	,@FromDate DATETIME
	,@FromShift INT = NULL
	,@ToDate DATETIME = NULL
	,@ToShift INT = NULL
	,@LeaveTypeID INT = NULL
	,@TotalDays NUMERIC(5, 1) = NULL
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@IsCrossDate BIT = 0
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
	Created: 2012/04/18@Dan
	Task: insert a request form
	DB: NexHR
	Original: 

	Revisions:    
		- 20250507@Henry.Dinh: update HR code to HR-ADM
	Param's Explanation:
*/
BEGIN
	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT

	DECLARE @HRGroupId INT;
	DECLARE @HeadId INT;

	SELECT @HeadId = e.Id
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.SystemSetting AS ss WITH (NOLOCK) ON ss.Name = 'MGTHeadCode'
    WHERE e.Code = ss.Value

	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'

	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

	IF (@RequestType IN (3, 4))
	BEGIN
		IF (DATEDIFF(d, @FromDate, '2015-07-31') < 0)
		BEGIN
			SELECT @ErrCode = 104
				,@ErrMsg = 'You can not request Late in/Early out later than 31 July, 2015.'

			RETURN;
		END
	END

	IF (@RequestType IN (1, 2))
	BEGIN
		IF (@FromDate IS NULL)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = 'Missing check in/out request has the invalid parameter'

			RETURN
		END

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.WorkingTimeRequest WITH (NOLOCK)
					WHERE empid = @empid
						AND RequestType = @RequestType
						AND STATUS IN (1, 2)
						AND DATEDIFF(d, fromdate, @FromDate) = 0
					)
				)
		BEGIN
			SELECT @ErrCode = 106
				,@ErrMsg = 'There is another request in the same date.'

			RETURN
		END

		-- IF (NOT EXISTS (SELECT 1 FROM dbo.Employee WHERE Id = @CreatedBy AND (Roles = '4' OR (Roles like '%3%'))))
		-- BEGIN
		IF (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						--AND (cl.ShiftId = 14 OR cl.ShiftId IS NULL)
						--AND DATEPART(DW, @FromDate) IN (1, 7)
						AND IsHoliday = 0
						AND IsOffDay = 0
					)
				)
		BEGIN
			IF (
					EXISTS (
						SELECT COUNT(1)
						FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
							AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
						WHERE wk.EmpId = @empid
							AND wk.RequestType IN (1, 2)
							AND wk.[Status] IN (1, 2)
							AND DATEDIFF(M, wk.FromDate, @FromDate) = 0
							AND cl.IsHoliday <> 1
							AND cl.IsOffDay <> 1
						HAVING COUNT(1) >= 2
						)
					)
			BEGIN
				IF (
						NOT EXISTS (
							SELECT 1
							FROM dbo.Employee WITH (NOLOCK)
							WHERE Id = @CreatedBy
								AND GroupId = @HRGroupId
							)
						) -- HR
				BEGIN
					SELECT @ErrCode = 106
						,@ErrMsg = 'The maximum request in a month has reached. You can contact HR for more information.'

					RETURN
				END
				ELSE
				BEGIN
					IF (
							NOT EXISTS (
								SELECT 1
								FROM dbo.Employee WITH (NOLOCK)
								WHERE Id = @EmpId
									AND GroupId = @HRGroupId
								)
							)
					BEGIN
						SELECT @ErrCode = - 1
					END
				END
			END
		END

		-- END
		
		IF(@EmpId = @HeadId) -- user Thien is auto approve check in/out request.
		BEGIN
			SELECT @ErrCode = - 1
		END
	END
	ELSE IF (@RequestType = 6)
	BEGIN
		IF (
				@FromDate IS NULL
				OR @ToDate IS NULL
				OR @LeaveTypeID IS NULL
				OR ISNULL(@TotalDays, 0) <= 0
				)
		BEGIN
			SELECT @ErrCode = 105
				,@ErrMsg = 'Leave request has the invalid parameter'

			RETURN
		END

		-- check range year
		IF (
				(
					@LeaveTypeID = @annualLeaveType
					OR @LeaveTypeID = @callInLeaveType
					)
				AND YEAR(@FromDate) <> YEAR(@ToDate)
				)
		BEGIN
			SELECT @ErrCode = 112
				,@ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

			RETURN;
		END

		IF (
				(@TotalDays = 0.5 AND
				(
				EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(MI, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
								AND RequestType = @RequestType
								AND TotalDays <> 0.5
								AND STATUS IN (1, 2)
								AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(DD, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(DD, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(DD, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(DD, ToDate, @ToDate) >= 0
									)
						)
					)
				)				
			)
			OR (
				@TotalDays <> 0.5
				AND EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND RequestType = @RequestType
							AND STATUS IN (1, 2)
							AND (
								(
									DATEDIFF(DD, FromDate, @FromDate) >= 0
									AND DATEDIFF(DD, @FromDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, FromDate, @ToDate) >= 0
									AND DATEDIFF(DD, @ToDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, FromDate) >= 0
									AND DATEDIFF(DD, FromDate, @ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, ToDate) >= 0
									AND DATEDIFF(DD, ToDate, @ToDate) >= 0
								)
						)
					)
				)
			)
		BEGIN
			SELECT @ErrCode = 107
				,@ErrMsg = 'There is another request in the same range'

			RETURN
		END

		-- check the total day is valid
		IF (
				@TotalDays < 1
				AND DATEDIFF(d, @FromDate, @ToDate) <> 0
				)
		BEGIN
			SELECT @ErrCode = 109
				,@ErrMsg = 'The total days is not correct'

			RETURN
		END
		ELSE
		BEGIN
			IF (
					(@TotalDays >= 1
					AND @TotalDays <> (
						SELECT COUNT(1)
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate BETWEEN @FromDate
								AND @ToDate
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND IsLeaveDay = 0
						))
					OR (@TotalDays = 0.5 AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate = CONVERT(DATE, @FromDate) AND WorkDate = CONVERT(DATE, @ToDate)
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND ISNULL(LeaveDay, 0) <> 1
					))
				)
			BEGIN
				SELECT @ErrCode = 110
					,@ErrMsg = 'The total days is not correct'

				RETURN
			END
		END


		
		--IF(@CreatedBy = @EmpId 
		--	OR NOT EXISTS (
		--	SELECT Id 
		--	FROM dbo.Employee WITH (NOLOCK) 
		--	WHERE Id = @CreatedBy AND GroupId = @HRGroupId)
		--)
		--BEGIN
		--	IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		--	BEGIN
		--		SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, null)

		--		IF(@IsValidLeaveDay = 0)
		--		BEGIN
		--			SELECT @ErrCode = 111
		--			,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

		--			RETURN
		--		END
		--	END
		--END

		-- check the total day is not greater than annual leave balance
		SELECT @TotalPedingDays = SUM(TotalDays)
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND [Status] = 1
			AND RequestType = 6
			AND (
				LeaveTypeID = 2
				OR LeaveTypeID = 30
				)
			AND YEAR(FromDate) = YEAR(@FromDate)

		SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND ISNULL(Deleted, 0) = 0
			AND YEAR(FromDate) = YEAR(@FromDate)

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.LeaveType
					WHERE Id = @LeaveTypeID
						AND IsALDeduction = 1
					)
				AND (@TotalDays + ISNULL(@TotalPedingDays, 0)) > ISNULL(@AnnualLeaveBalance, 0)
				)
		BEGIN
			SELECT @ErrCode = 111
				,@ErrMsg = 'You have not enough annual leave days to request'

			RETURN
		END

		-- Annual leave 2
		IF (@LeaveTypeID = @annualLeaveType)
		BEGIN
			IF (
					(
						@TotalDays = 0.5
						AND DATEDIFF(D, @FromDate, dbo.fnc_GetClientDateTime()) > 0
						)
					OR (
						@TotalDays = 1
						AND 1 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 2
						AND @TotalDays < 5
						AND 2 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 5
						AND 5 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					)
			BEGIN
				SELECT @ErrCode = 112
					,@ErrMsg = 'Request must be made in advance, in accordance with the policy established.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @callInLeaveType)
		BEGIN
			IF (
					NOT EXISTS (
						SELECT 1
						FROM dbo.Employee WITH (NOLOCK)
						WHERE id = @CreatedBy
							AND (
								Roles = '4'
								OR (Roles LIKE '%1%')
								OR (Roles LIKE '%2%')
								)
						)
					)
			BEGIN
				SELECT @ErrCode = 114
					,@ErrMsg = 'This leave type must be submitted by Leader on behalf of the employee.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @NEXLeaveType)
		BEGIN
			DECLARE @NEXBalance INT
				,@NEXLeavePending NUMERIC(5, 1)

			SELECT @NEXLeavePending = SUM(TotalDays)
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND [Status] = 1
				AND RequestType = 6
				AND LeaveTypeID = @NEXLeaveType

			SELECT @NEXBalance = (oty.[Minutes] - ISNULL(la.[Minutes], 0))
			FROM dbo.OvertimeYearly AS oty WITH (NOLOCK)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON oty.EmpId = la.EmpId
				AND oty.[Year] = la.[Year]
			WHERE oty.EmpId = @EmpId
				AND oty.[Year] = YEAR(@FromDate)

			IF (ISNULL(@NEXBalance, 0) - (ISNULL(@NEXLeavePending, 0) * 480) < (@TotalDays * 480))
			BEGIN
				SELECT @ErrCode = 115
					,@ErrMsg = 'You have not enough NEX days to request.'

				RETURN
			END
		END
	END

	INSERT INTO dbo.WorkingTimeRequest (
		STATUS
		,EmpId
		,RequestType
		,IsPlanned
		,Remark
		,CreatedDate
		,CreatedBy
		,ModifiedDate
		,ModifiedBy
		,FromDate
		,FromShift
		,ToDate
		,ToShift
		,LeaveTypeID
		,TotalDays
		,LateInMins
		,EarlyOutMins
		,IsCrossDate
		)
	SELECT 
		1
		,-- pending status as default
		@EmpId
		,@RequestType
		,@IsPlanned
		,@Remark
		,@CreatedDate
		,@CreatedBy
		,@CreatedDate
		,@CreatedBy
		,@FromDate
		,@FromShift
		,@ToDate
		,@ToShift
		,@LeaveTypeID
		,@TotalDays
		,@LateInMins
		,@EarlyOutMins
		,@IsCrossDate
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.WorkingTimeRequest WITH (NOLOCK) 
		WHERE EmpId = @EmpId
			AND Status = 1
			AND RequestType = @RequestType
			AND FromDate = @FromDate
			AND ToDate = @ToDate
	)

	SELECT @id = cast(SCOPE_IDENTITY() AS BIGINT)

	EXEC [WorkingTimeRequest_WriteHistory] @id

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@CreatedBy

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
	END

	IF (@LeaveTypeID = @BusinessLeave AND @EmpId = @HeadId)
	BEGIN
		SELECT @ErrCode = - 1
	END

	IF (
			NOT EXISTS (
				SELECT 
					CASE WHEN es.ApproverId IS NULL THEN gs.ApproverId ELSE es.ApproverId END
				FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.GroupId = gs.GroupId AND gs.IsDeleted = 0 AND e.Id <> gs.ApproverId
				LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON e.Id = es.EmpId AND es.IsDeleted = 0 AND e.Id <> es.ApproverId
				WHERE e.Id = @empId
					AND (gs.ApproverId IS NOT NULL OR es.ApproverId IS NOT NULL))
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @EmpId
			,@ApprovedDate = @CreatedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE IF (@ErrCode = - 1)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @CreatedBy
			,@ApprovedDate = @CreatedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Update]
    @Id bigint,
    @IsPlanned bit,
    @Remark nvarchar(2000)= null,
    @FromDate datetime, 
    @FromShift int = null, 
    @ToDate datetime = null, 
    @ToShift int = null,
    @LeaveTypeID int = null, 
    @TotalDays numeric(5,1) = null, 
    @LateInMins int = null,
    @EarlyOutMins int = null,
	@IsCrossDate BIT = 0,
    @ModifiedDate datetime, 
    @ModifiedBy int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
    Created: 2012/05/08@Dan
    Task: update a request form
    DB: NexHR
    Original: 

    Revisions:
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:
*/

BEGIN
    DECLARE @empId INT, @requestType INT,@Status INT

	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT
		,@OldLeaveTypeId INT
		,@OldFromDate DATETIME
		,@OldToDate DATETIME
		,@OldTotalDays NUMERIC(5, 1)

	DECLARE @HRGroupId INT;
	DECLARE @HeadId INT;
	
	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamId' THEN g.Id END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'HRTeamId'

	SELECT @HeadId = e.Id
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'MGTHeadCode'
	WHERE e.Code = ss.Value

	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

    SELECT @empId = EmpId
		,@requestType = RequestType
		,@Status = [Status]
		,@OldFromDate = FromDate
		,@OldToDate = ToDate
		,@OldLeaveTypeId = LeaveTypeID
		,@OldTotalDays = TotalDays
    FROM dbo.WorkingTimeRequest WITH (NOLOCK)
    WHERE Id = @Id

	IF (@RequestType IN (1, 2))
	BEGIN
		IF (@FromDate IS NULL)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = 'Missing check in/out request has the invalid parameter'

			RETURN
		END

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.WorkingTimeRequest WITH (NOLOCK)
					WHERE Id <> @Id
						AND empid = @empid
						AND RequestType = @RequestType
						AND STATUS IN (1, 2)
						AND DATEDIFF(d, fromdate, @FromDate) = 0
					)
				)
		BEGIN
			SELECT @ErrCode = 106
				,@ErrMsg = 'There is another request in the same date.'

			RETURN
		END

		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						AND IsHoliday = 0
						AND IsOffDay = 0
					)
				)
		BEGIN
			IF (
					EXISTS (
						SELECT COUNT(1)
						FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
							AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
						WHERE wk.Id <> @Id
							AND wk.EmpId = @empid
							AND wk.RequestType IN (1, 2)
							AND wk.[Status] IN (1, 2)
							AND DATEDIFF(M, wk.FromDate, @FromDate) = 0
							AND cl.IsHoliday <> 1
							AND cl.IsOffDay <> 1
						HAVING COUNT(1) >= 2
						)
					)
			BEGIN
				IF (
						NOT EXISTS (
							SELECT 1
							FROM dbo.Employee WITH (NOLOCK)
							WHERE Id = @ModifiedBy
								AND GroupId = @HRGroupId
							)
						) -- HR
				BEGIN
					SELECT @ErrCode = 106
						,@ErrMsg = 'The maximum request in a month has reached. You can contact HR for more information.'

					RETURN
				END
				ELSE
				BEGIN
					IF (
							NOT EXISTS (
								SELECT 1
								FROM dbo.Employee WITH (NOLOCK)
								WHERE Id = @EmpId
									AND GroupId = @HRGroupId
								)
							)
					BEGIN
						SELECT @ErrCode = - 1
					END
				END
			END
		END

		-- END
		
		IF(@EmpId = @HeadId) -- user Thien is auto approve check in/out request.
		BEGIN
			SELECT @ErrCode = - 1
		END
	END
    ELSE IF (@RequestType = 6)
    BEGIN
		IF (
				@FromDate IS NULL
				OR @ToDate IS NULL
				OR @LeaveTypeID IS NULL
				OR ISNULL(@TotalDays, 0) <= 0
				)
		BEGIN
			SELECT @ErrCode = 105
				,@ErrMsg = 'Leave request has the invalid parameter'

			RETURN
		END

		-- check range year
		IF (
				(
					@LeaveTypeID = @annualLeaveType
					OR @LeaveTypeID = @callInLeaveType
					)
				AND YEAR(@FromDate) <> YEAR(@ToDate)
				)
		BEGIN
			SELECT @ErrCode = 112
				,@ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

			RETURN;
		END

		IF (
				(@TotalDays = 0.5 AND
				(
				EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
								AND Id <> @Id
								AND RequestType = @RequestType
								AND TotalDays <> 0.5
								AND STATUS IN (1, 2)
								AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(DD, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(DD, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(DD, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(DD, ToDate, @ToDate) >= 0
									)
						)
					)
				)
			)
			OR (
				@TotalDays <> 0.5
				AND EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND STATUS IN (1, 2)
							AND (
								(
									DATEDIFF(DD, FromDate, @FromDate) >= 0
									AND DATEDIFF(DD, @FromDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, FromDate, @ToDate) >= 0
									AND DATEDIFF(DD, @ToDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, FromDate) >= 0
									AND DATEDIFF(DD, FromDate, @ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, ToDate) >= 0
									AND DATEDIFF(DD, ToDate, @ToDate) >= 0
								)
						)
					)
				)
			)
		BEGIN
			SELECT @ErrCode = 107
				,@ErrMsg = 'There is another request in the same range'

			RETURN
		END

		-- check the total day is valid
		IF (
				@TotalDays < 1
				AND DATEDIFF(d, @FromDate, @ToDate) <> 0
				)
		BEGIN
			SELECT @ErrCode = 109
				,@ErrMsg = 'The total days is not correct'

			RETURN
		END
		--ELSE
		--BEGIN
		--	IF (
		--			@TotalDays >= 1
		--			AND @TotalDays <> (
		--				SELECT COUNT(1)
		--				FROM dbo.EmployeeCalendar
		--				WHERE EmpID = @EmpId
		--					AND WorkDate BETWEEN @FromDate
		--						AND @ToDate
		--					AND IsHoliday = 0
		--					AND IsOffDay = 0
		--					AND IsLeaveDay = 0
		--				)
		--			)
		--	BEGIN
		--		SELECT @ErrCode = 110
		--			,@ErrMsg = 'The total days is not correct'

		--		RETURN
		--	END
		--END

		--SELECT @TotalPedingDays = SUM(TotalDays)
		--FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		--WHERE id <> @Id 
		--	AND EmpId = @EmpId
		--	AND [Status] = 1
		--	AND RequestType = 6
		--	AND (
		--		LeaveTypeID = @annualLeaveType
		--		OR LeaveTypeID = @callInLeaveType
		--		)

		--SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		--FROM dbo.LeaveTrans WITH (NOLOCK)
		--WHERE EmpId = @EmpId
		--	AND ISNULL(Deleted, 0) = 0
		--	AND YEAR(FromDate) = YEAR(@FromDate)

		IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		BEGIN
			SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, @Id)

			IF(@IsValidLeaveDay = 0)
			BEGIN
				SELECT @ErrCode = 111
				,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

				RETURN
			END
		END

		-- Annual leave 2
		IF (@LeaveTypeID = @annualLeaveType)
		BEGIN
			IF (
					(
						@TotalDays = 0.5
						AND DATEDIFF(D, @FromDate, dbo.fnc_GetClientDateTime()) > 0
						)
					OR (
						@TotalDays = 1
						AND 1 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 2
						AND @TotalDays < 5
						AND 2 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 5
						AND 5 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					)
			BEGIN
				SELECT @ErrCode = 112
					,@ErrMsg = 'Request must be made in advance, in accordance with the policy established.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @callInLeaveType)
		BEGIN
			IF (
					NOT EXISTS (
						SELECT 1
						FROM dbo.Employee WITH (NOLOCK)
						WHERE id = @ModifiedBy
							AND (
								Roles = '4'
								OR (Roles LIKE '%1%')
								OR (Roles LIKE '%2%')
								)
						)
					)
			BEGIN
				SELECT @ErrCode = 114
					,@ErrMsg = 'This leave type must be submitted by Leader on behalf of the employee.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @NEXLeaveType)
		BEGIN
			DECLARE @NEXBalance INT
				,@NEXLeavePending NUMERIC(5, 1)

			SELECT @NEXLeavePending = SUM(TotalDays)
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE Id <> @id
				AND EmpId = @EmpId
				AND [Status] = 1
				AND RequestType = 6
				AND LeaveTypeID = @NEXLeaveType

			SELECT @NEXBalance = (oty.[Minutes] - ISNULL(la.[Minutes], 0))
			FROM dbo.OvertimeYearly AS oty WITH (NOLOCK)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON oty.EmpId = la.EmpId
				AND oty.[Year] = la.[Year]
			WHERE oty.EmpId = @EmpId
				AND oty.[Year] = YEAR(@FromDate)

			IF (ISNULL(@NEXBalance, 0) - (ISNULL(@NEXLeavePending, 0) * 480) < (@TotalDays * 480))
			BEGIN
				SELECT @ErrCode = 115
					,@ErrMsg = 'You have not enough NEX days to request.'

				RETURN
			END
		END
    END

    IF(@Status <> 1 OR @empId <> @ModifiedBy)
    BEGIN
       SELECT @ErrCode = 111
                ,@ErrMsg = 'You can not update this request. Please contact HR for more detail.'

            RETURN
    END

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_EditLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@OldLeaveTypeId
		,@OldFromDate
		,@OldToDate
		,@OldTotalDays
		,@ModifiedBy
	END

    UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
    SET     
            IsPlanned = @IsPlanned,
            Remark = @Remark ,
            FromDate = @FromDate, 
            FromShift = @FromShift, 
            ToDate = @ToDate, 
            ToShift = @ToShift, 
            LeaveTypeID = @LeaveTypeID, 
            TotalDays = @TotalDays, 
            LateInMins = @LateInMins,
            EarlyOutMins = @EarlyOutMins,
            ModifiedDate = @ModifiedDate,
            ModifiedBy = @ModifiedBy,
			IsCrossDate = @IsCrossDate 
     WHERE Id=@Id

    EXEC [WorkingTimeRequest_WriteHistory] @Id

	IF (@LeaveTypeID = @BusinessLeave AND @EmpId = @HeadId)
	BEGIN
		SELECT @ErrCode = - 1
	END

	IF (
			NOT EXISTS (
				SELECT 
					CASE WHEN es.ApproverId IS NULL THEN gs.ApproverId ELSE es.ApproverId END
				FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.GroupId = gs.GroupId AND gs.IsDeleted = 0 AND e.Id <> gs.ApproverId
				LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON e.Id = es.EmpId AND es.IsDeleted = 0 AND e.Id <> es.ApproverId
				WHERE e.Id = @empId
					AND (gs.ApproverId IS NOT NULL OR es.ApproverId IS NOT NULL))
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @EmpId
			,@ApprovedDate = @ModifiedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE IF (@ErrCode = - 1)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @ModifiedBy
			,@ApprovedDate = @ModifiedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END

GO


ALTER PROCEDURE [dbo].[Employee_Insert]
(
    @UserId INT
    ,@FullName NVARCHAR(50)
    ,@Nickname NVARCHAR(20)
    ,@DomainUsername VARCHAR(50)
    ,@code VARCHAR(20)
    ,@password NVARCHAR(100)
    ,@monShift INT = NULL
    ,@tueShift INT = NULL
    ,@wedShift INT = NULL
    ,@thuShift INT = NULL
    ,@friShift INT = NULL
    ,@satShift INT = NULL
    ,@sunShift INT = NULL
    ,@GroupId INT
    ,@createBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
)
AS
/*
    Created: 20160218@Quinn.Vo
    Task: insert an employee
    DB: NexHRM

    Revisions:    
        20250507@Henry.Dinh: insert password
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CompanyId INT
        ,@OldEmpId INT

	SELECT @CompanyId = [Value]
    FROM dbo.SystemSetting WITH (NOLOCK)
    WHERE [Name] = 'CompanyId'

    SELECT @OldEmpId = MAX(e.OldEmpId)
    FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN bodb_HRM.dbo.CompanyEmployee AS c WITH (NOLOCK) ON e.EmpId = c.EmpId
    WHERE c.CompanyId = @CompanyId

    SET @OldEmpId = ISNULL(@OldEmpId, 0) + 1

    IF EXISTS (
            SELECT 1
            FROM dbo.Employee WITH (NOLOCK)
            WHERE Code = @code
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This user has already existed !'

        RETURN
    END

    INSERT INTO bodb_HRM.dbo.Employee (
        Code
        ,FullName
        ,Nickname
        ,DOB
        ,CreatedDate
        ,CreatedBy
        ,OldEmpId
		,EmpId
		,[Enabled]
        )
    VALUES (
        @Code
        ,@FullName
        ,@NickName
        ,CAST('1-1-1990' AS SMALLDATETIME)
        ,dbo.fnc_GetClientDateTime()
        ,@createBy
        ,@OldEmpId
		,@UserId
		,1
        )

	INSERT INTO bodb_HRM.dbo.CompanyEmployee(CompanyId, EmpId, DomainUsername, [Password], GroupId, [Enabled], IsDeleted, IsSpecialUser)
	VALUES (@CompanyId, @UserId, @DomainUsername, @Password, @GroupId, 0, 0, 0)

    INSERT INTO dbo.EmployeeShift (
        [EmpId]
        ,[MonShift]
        ,[TueShift]
        ,[WedShift]
        ,[ThuShift]
        ,[FriShift]
        ,[SatShift]
        ,[SunShift]
        )
    VALUES (
        @UserId
        ,@monShift
        ,@tueShift
        ,@wedShift
        ,@thuShift
        ,@friShift
        ,@satShift
        ,@sunShift
        )

    INSERT INTO dbo.EmployeeGroup_Employee (GroupId, EmpId, isMainGroup)
    VALUES (@groupId, @UserId, 1)

    DECLARE @fromdate DATETIME
        ,@todate DATETIME

    SET @fromdate = dbo.fnc_GetClientDateTime()
	SELECT @todate = MAX(WorkDate)
    FROM dbo.EmployeeCalendar WITH (NOLOCK)

    EXECUTE dbo.EmployeeCalendar_Insert @UserId
        ,@fromdate
        ,@todate

    SELECT @ErrCode = 0
        ,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_UpdateEmployee] @employeeId INT
    ,@code VARCHAR(20) = NULL
    ,@domainUsername VARCHAR(50) = NULL
    ,@fullName NVARCHAR(50) = NULL
    ,@nickName NVARCHAR(20) = NULL
    ,@EducationBackground NVARCHAR(500) = NULL
    ,@EmployeeCard VARCHAR(11) = ''
    ,@dob DATETIME = NULL
    ,@email VARCHAR(50) = NULL
    ,@jobTitleId INT = NULL
    ,@enabled BIT = NULL
	,@isSpecialUser BIT = NULL
    ,@phone VARCHAR(50) = NULL
    ,@gender TINYINT = NULL
    ,@mainGroupID INT = NULL
    ,@subGroups VARCHAR(50) = NULL
    ,@Roles VARCHAR(20) = NULL
    ,@joinedDate DATETIME = NULL
    ,@confirmedDate DATETIME = NULL
    ,@resignedDate DATETIME = NULL
    ,@skype VARCHAR(100) = NULL
    ,@yahoo VARCHAR(100) = NULL
    ,@marriage BIT = NULL
    ,@address NVARCHAR(250) = NULL
    ,@currAddress NVARCHAR(250) = NULL
    ,@idCardNo NVARCHAR(20) = NULL
    ,@issuedDate DATETIME = NULL
    ,@issuedPlace NVARCHAR(250) = NULL
    ,@passportNo NVARCHAR(35) = NULL
    ,@passportExp DATETIME = NULL
	,@socialInsuranceNo VARCHAR(50) = NULL
    ,@geoMapLatitude VARCHAR(50) = ''
    ,@geoMapLongtitude VARCHAR(50) = ''
    ,@distanceToCompany VARCHAR(11) = ''
    ,@modifiedBy INT = NULL
	,@placeOfBirth NVARCHAR(20) = NULL
	,@contactName NVARCHAR(50) = NULL
	,@userRelationship NVARCHAR(11) = NULL
	,@contactPhone VARCHAR(50) = NULL
	,@contactAddress NVARCHAR(250) = NULL
	,@groupDate DATE = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: update employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE 
		@logData NVARCHAR(MAX)
		,@xml XML
		,@Groups VARCHAR(50)

    BEGIN TRY
        BEGIN TRANSACTION;

		SELECT @Groups = 
			COALESCE(@Groups + ', ' + CAST(GroupId AS VARCHAR(10)), CAST(GroupId AS VARCHAR(10)))
		FROM dbo.EmployeeGroup_Employee 
		WHERE EmpID = @employeeId

		SELECT @logData = '<Employee_Old Code="'+ COALESCE(CAST(Code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(DomainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(FullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(NickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(EducationBackground,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(DOB AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(Email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(JobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST([Enabled] AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(IsSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(Phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(Gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(GroupId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Roles="'+ COALESCE([Roles],SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(JoinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(GroupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(ConfirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(ResignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(Skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(Yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(Marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(PermanentAddress,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(CurrentAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(IdCardNumber,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(IssuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(IssuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(PassportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(PassportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(SocialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(PlaceOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(ContactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(UserRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(ContactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(ContactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(ModifiedBy AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Groups="'+ COALESCE(@Groups,SPACE(1)) + '" />' + 

		'<Employee_new Code="'+ COALESCE(CAST(@code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(@domainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(@fullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(@nickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(@EducationBackground,SPACE(1)) + '" ' +
		'EmployeeCard="'+ COALESCE(@EmployeeCard,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(@dob AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(@email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(@jobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST(@enabled AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(@isSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(@phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(@gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(@mainGroupID AS VARCHAR(50)),SPACE(1)) + '" ' +

		'Roles="'+ COALESCE(@Roles,SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(@joinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(@groupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(@confirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(@resignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(@skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(@yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(@marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(@address,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(@currAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(@idCardNo,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(@issuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(@issuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(@passportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(@passportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(@socialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(@placeOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(@contactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(@userRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(@contactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(@contactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(@modifiedBy AS VARCHAR(11)),SPACE(1)) + '" />'

		FROM dbo.Employee WITH (NOLOCK)
		WHERE Id = @employeeId

        UPDATE bodb_HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET Code = @code
            ,FullName = @fullName
            ,NickName = @nickName
            ,EmployeeCard = @EmployeeCard
            ,DOB = @dob
            ,JobTitleId = @jobTitleId
            ,Phone = @phone
            ,Gender = @gender
            ,Skype = @skype
            ,Yahoo = @yahoo
            ,Marriage = @marriage
            ,PermanentAddress = @address
            ,CurrentAddress = @currAddress
            ,IdCardNumber = @idCardNo
            ,DateOfIDCard = @issuedDate
            ,PlaceOfIDCard = @issuedPlace
            ,PassportNo = @passportNo
            ,PassportExp = @passportExp
			,SocialInsuranceNo = @socialInsuranceNo
            ,GeoMapLatitude = @geoMapLatitude
            ,GeoMapLongtitude = @geoMapLongtitude
            ,DistanceToCompany = @distanceToCompany
			,PlaceOfBirth = @placeOfBirth
			,ContactName = @contactName
			,UserRelationship = @userRelationship
			,ContactPhone = @contactPhone
			,ContactAddress = @contactAddress
			,JoinedDate = @groupDate
            ,ModifiedDate = GETDATE()
            ,ModifiedBy = @modifiedBy
			,EducationBackground = @EducationBackground
        WHERE EmpId = @employeeId

		DECLARE @CompanyId int = (SELECT [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId')

		UPDATE bodb_HRM.dbo.CompanyEmployee
		SET DomainUsername = @domainUsername
            ,Email = @email
			,IsSpecialUser = @isSpecialUser
			,[Enabled] = @enabled
            ,GroupId = @mainGroupID
            ,Roles = @roles
            ,CompanyDate = @joinedDate
            ,ConfirmedDate = @confirmedDate
            ,ResignedDate = @resignedDate
		WHERE EmpId = @employeeId
		AND CompanyId = @CompanyId

		-- UPDATE dbo.EmployeeExt WITH (UPDLOCK, ROWLOCK)
        -- SET DomainUsername = @domainUsername
        --     ,Email = @email
		-- 	,IsSpecialUser = @isSpecialUser
		-- 	,Enabled = @enabled
        --     ,GroupId = @mainGroupID
        --     ,Roles = @roles
        --     ,JoinedDate = @joinedDate
        --     ,ConfirmedDate = @confirmedDate
        --     ,ResignedDate = @resignedDate
        -- WHERE Id = @employeeId

		INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
		VALUES(5, GETDATE(), 'Update Employee', @logData, @employeeId)

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Employee unsuccessfully.'
    END CATCH;
END
GO


CREATE OR ALTER   PROCEDURE [dbo].[Sync_JobTitle_Get] 
	@LastModifiedDate DATETIME = NULL
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
	WHERE @LastModifiedDate IS NULL OR ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO