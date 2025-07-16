USE [NexHRM]
GO

CREATE OR ALTER VIEW [dbo].[GroupSupervisor]
AS
SELECT 
    [OwnerId] AS ApproverId,
    g.[GroupId],
    g.[ModifiedBy],
    g.[ModifiedDate],
    g.[IsDeleted],
    c.Name AS OuterCompanyName
FROM [bodb_HRM].[dbo].[GroupSupervisor] AS g WITH (NOLOCK)
INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS ce WITH (NOLOCK) 
    ON ce.EmpId = g.OwnerId AND ISNULL(ce.IsSpecialUser, 0) = 0 AND ISNULL(ce.IsDeleted, 0) = 0 AND ISNULL(ce.Enabled, 1) = 1
LEFT JOIN [bodb_HRM].[dbo].[Company] AS c WITH (NOLOCK) 
    ON c.CompanyId = ce.CompanyId 
    AND c.CompanyId != (SELECT [Value] FROM [dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
WHERE g.ActionType = 3 AND g.RequestType = 6
GO


CREATE OR ALTER VIEW [dbo].[EmployeeSupDetail]
AS
SELECT 
    esup.EmpId, 
    esup.NickName, 
    esup.FullName, 
    esup.ApproverId, 
    CASE 
        WHEN c.Name IS NOT NULL THEN sup.NickName + ' (' + c.Name + ')'
        ELSE sup.NickName
    END AS ApproverNickName,
    sup.FullName AS ApproverFullName, 
    esup.IsPrivateSetting
FROM 
(
    SELECT 
        e.Id AS EmpId, 
        e.NickName, 
        e.FullName, 
        CASE 
            WHEN es.ApproverId IS NULL THEN gs.ApproverId 
            ELSE es.ApproverId 
        END AS ApproverId, 
        CAST(
            CASE 
                WHEN es.ApproverId IS NOT NULL THEN 1 
                ELSE 0 
            END AS BIT
        ) AS IsPrivateSetting
    FROM dbo.Employee AS e WITH (NOLOCK)
    LEFT OUTER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK)
        ON e.GroupId = gs.GroupId 
        AND gs.IsDeleted = 0 
        AND e.Id <> gs.ApproverId
    LEFT OUTER JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK)
        ON e.Id = es.EmpId 
        AND es.IsDeleted = 0 
        AND e.Id <> es.ApproverId
    WHERE 
        e.isDeleted = 0 AND (
            gs.ApproverId IS NOT NULL OR 
            es.ApproverId IS NOT NULL
        )
) AS esup
INNER JOIN bodb_HRM.dbo.Employee AS sup WITH (NOLOCK) ON esup.ApproverId = sup.EmpId AND ISNULL(sup.isDeleted, 0) = 0
INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = sup.EmpId AND ISNULL(ce.IsSpecialUser, 0) = 0 AND ISNULL(ce.IsDeleted, 0) = 0 AND ISNULL(ce.Enabled, 1) = 1
LEFT JOIN bodb_HRM.dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = ce.CompanyId AND c.CompanyId != (SELECT [Value] FROM [dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
GO

CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Get]
AS
/*
Created: 20240423@Lilac.Nguyen
Task: Get group supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT
		eg.Id AS GroupId,
		eg.Code AS GroupCode,
		eg.[Name] AS GroupName,
		STRING_AGG(
			e.NickName + 
			CASE 
				WHEN gs.OuterCompanyName IS NOT NULL THEN ' (' + gs.OuterCompanyName + ')'
				ELSE ''
			END,
			', '
		) AS Approvers
	FROM dbo.EmployeeGroup AS eg WITH (NOLOCK)
	LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
		ON eg.Id = gs.GroupId AND ISNULL(gs.IsDeleted, 0) = 0
	LEFT JOIN bodb_HRM.dbo.Employee AS e 
		ON e.EmpId = gs.ApproverId
	WHERE ISNULL(eg.Deleted, 0) = 0
	AND eg.IsSpecialTeam = 0
	GROUP BY eg.Id, eg.Code, eg.[Name];
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Update]
	@GroupId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240423@Lilac.Nguyen
Task: update approver for group.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(),  @logData NVARCHAR(MAX);
	DECLARE @ActionType INT = 3;
	DECLARE @RequestType INT = 6;

	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.GroupSupervisor WITH (NOLOCK)
	WHERE GroupId = @GroupId AND IsDeleted <> 1

	CREATE TABLE #tblSupervisor (ApproverId INT)

	INSERT INTO #tblSupervisor(
            ApproverId
            )
    SELECT val
    FROM dbo.fnc_strSplit(@ApproverIds, ',')

	UPDATE su
	SET su.IsDeleted = 1
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM bodb_HRM.dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = su.OwnerId AND ISNULL(ce.IsSpecialUser, 0) = 0 AND ISNULL(ce.IsDeleted, 0) = 0 AND ISNULL(ce.Enabled, 1) = 1
	WHERE su.GroupId = @GroupId
		AND ISNULL(su.IsDeleted, 0) = 0
		AND ce.CompanyId = (SELECT [Value] FROM [dbo].[SystemSetting] WITH (NOLOCK) WHERE [Name] = 'CompanyId')
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.OwnerId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM bodb_HRM.dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.OwnerId
	WHERE su.GroupId = @GroupId
		AND su.ActionType = @ActionType
		AND su.RequestType = @RequestType
		AND su.IsDeleted = 1

	MERGE bodb_HRM.dbo.GroupSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].GroupId = @GroupId
				AND [target].OwnerId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			GroupId
			,OwnerId
			,ActionType
			,RequestType
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			,DisplayOrder
			)
		VALUES (
			@GroupId
			,[source].ApproverId
			,@ActionType
			,@RequestType
			,@ModifiedBy
			,@ModifiedDate
			,0
			,1
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES (10, dbo.fnc_GetClientDateTime(), 'Update Team Flow Setting', @logData, 0)

	DROP TABLE #tblSupervisor;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetById] 
    @id BIGINT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by id.
DB: NexEPA
Original: 

Revisions:  
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 1
    SELECT r.Id
        ,r.empID
        ,RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,ProcessedDate
        ,0.0 AS TotalDaysRequestInMonth
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
        ,@IsAllowUpdateStatus as IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
        JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
            AND c.WorkDate = convert(VARCHAR(10), r.FromDate, 111)
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.id = @id
END

GO

CREATE OR ALTER  PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByDate] @role INT
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
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
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
		,UnitCode VARCHAR(200)
		,DepartmentName NVARCHAR(200)
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
			,u.[Code] AS UnitCode
			,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
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
		,u.[Code] AS UnitCode
		,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
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
		,u.[Code] AS UnitCode
		,d.[Name] AS DepartmentName
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
		LEFT JOIN bodb_HRM.dbo.Department d WITH(NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN bodb_HRM.dbo.Unit u WITH (NOLOCK) ON d.UnitId = u.UnitId
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

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory]
    @requestid bigint
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexHRM
Original: 
Revisions:  
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 0
    SELECT
        r.Id,
        r.empID,
        RequestType,
        (CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s.Code END
            ) AS FromShift,
        s2.Code as ToShift,
        r.FromDate AS RequestedDate,
        r.FromDate,
        r.ToDate,
        r.TotalDays,
        lv.Id AS LeaveTypeId,
        lv.NAME AS LeaveTypeName,
        case when RequestType = 3 then LateInMins else EarlyOutMins end as TotalMins,    
        Remark,
        r.CreatedDate,
        Status,
        e.NickName AS Requester,
        e.FullName AS FullNameRequester,
        '' AS EmailRequester,
        g.Name AS Team,
        g.Code AS TeamCode,
        ap.NickName AS ProcessedBy,
        ap.FullName AS FullNameProcessedBy,
        '' AS EmailProcessedBy,
        ProcessedDate,
        0.0 AS TotalDaysRequestInMonth,
        @IsAllowUpdateStatus as IsAllowUpdateStatus,
		r.IsCrossDate
    FROM dbo.WorkingTimeRequest_history r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
        LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) 
        ON (
        (ap.EmpId = r.ProcessedBy AND r.ProcessedBy IS NOT NULL)
        OR (ap.EmpId = r.CreatedBy AND r.ProcessedBy IS NULL)
        )
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= r.ToShift
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.Id = @requestid
    ORDER BY keyid
END

GO
