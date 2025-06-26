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
		,@CoreTeamId INT;

	SELECT @CoreTeamId = MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name = 'CoreTeamId'

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
                    AND g.Id = @CoreTeamId
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
