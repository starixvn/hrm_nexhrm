USE [NexHRM]
--USE [LumiHRM]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[AnnualLeave_GetSummary] @year INT
    ,@employeeId INT = NULL
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@minAnnualBalance INT = 0
AS
/*
Created: 20160222@Quinn.Vo
Task: Get annual leave summary.
DB: NexHR
Original: 
Revisions:
    - 20250516@Henry.Dinh: add department id filter
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    SELECT e.Id AS EmpId
        ,SUM(ISNULL(CreditDays, 0)) CreditDays
        ,SUM(ISNULL(DeductDays, 0)) DeductDays
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 1 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jan
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 2 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Feb
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 3 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Mar
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 4 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Apr
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 5 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) May
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 6 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jun
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 7 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jul
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 8 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Aug
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 9 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Sep
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 10 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Oct
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 11 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Nov
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 12 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) DEC
    INTO #CurrenBalace
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT EmpId
            ,CreditDays
            ,DeductDays
            ,FromDate
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND YEAR(FromDate) = @year
        ) t ON e.Id = t.Empid
    WHERE e.IsSpecialUser = 0
		AND (
            @employeeId IS NULL
            OR e.id = @employeeId
            )
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR e.groupId IN (SELECT Id FROM EmployeeGroup WHERE DepartmentId = @departmentId)
            )
        AND (ISNULL(e.isDeleted, 0) = 0)
    GROUP BY e.ID

    SELECT 
        ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
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
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS LeaveBalance
        ,cb.EmpId
        ,e.Code
        ,FullName
        ,NickName
        ,d.DepartmentId
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
    FROM #CurrenBalace cb
        INNER JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        INNER JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
     WHERE 
		e.IsSpecialUser = 0
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    DROP TABLE #CurrenBalace
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Employee_GetEmployees] @query NVARCHAR(256)
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@jobTitleId INT = NULL
	,@IsDisable BIT = 0
    ,@isDelete BIT = 0
    ,@pageNumber INT = 1
    ,@pageSize INT = 2000
    ,@isSpecial BIT = NULL
    ,@totalRow INT OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Get employee by conditions.
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	20200505@Merlin.Le: Add isSpecial IN PARAM
	20250308@Henry.Dinh: Add management grade
    20250514@Henry.Dinh: Add departmentId IN PARAM
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)

    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT @totalRow = COUNT(1)
    FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.ID
    WHERE (
            @isDelete IS NULL
            OR isDeleted = @isDelete
            )
		AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
        AND (
            @query IS NULL
            OR NickName LIKE @q
            OR FullName LIKE @q
            OR DomainUsername LIKE @q
            OR e.Code LIKE @q
            )
        AND (
            @groupId IS NULL
            OR GroupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR DepartmentId = @departmentId
            )

    SELECT e.[Id]
		,e.[Id] as UserId
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[GroupDate]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,d.DepartmentId
		,d.[Name] AS DepartmentName
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.AffectedDate
		,gra.Note
		,mgra.Code AS ManagementGradeCode
		,mgra.Name AS ManagementGradeName
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy] CreatorId
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[isDeleted]
        ,[PermanentAddress]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,LRExamDate
        ,Listening
        ,Reading
        ,Speaking
        ,Writing
        ,[GroupId]
        ,g.Code
        ,g.NAME
        ,g.[Rank]
        ,e.JobTitleId
        ,jt.[Name]
    FROM (
        SELECT row_number() OVER (
                ORDER BY NickName ASC
                ) AS RowNumber
            ,em.[Id]
            ,em.[Code]
            ,[DomainUsername]
            ,[FullName]
            ,[NickName]
            ,[DOB]
            ,[Email]
            ,[Enabled]
            ,[Phone]
            ,[Gender]
            ,[Roles]
            ,GroupId
			,[GroupDate]
            ,JobTitleId
            ,[JoinedDate]
            ,[ConfirmedDate]
            ,[ResignedDate]
            ,[Skype]
            ,[Yahoo]
            ,[Marriage]
            ,em.[CreatedDate]
            ,em.[CreatedBy]
            ,em.[ModifiedDate]
            ,em.[ModifiedBy]
            ,[isDeleted]
            ,[PermanentAddress]
            ,[CurrentAddress]
            ,[IdCardNumber]
            ,[PassportNo]
            ,[PassportExp]
            ,[GeoMapLatitude]
            ,[GeoMapLongtitude]
            ,[DistanceToCompany]
            ,[EmployeeCard]
            ,[IssuedDate]
            ,[IssuedPlace]
			,[PlaceOfBirth]
			,[ContactName]
			,[UserRelationship]
			,[ContactPhone]
			,[ContactAddress]
			,[SocialInsuranceNo]
        FROM dbo.Employee AS em WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS emg WITH (NOLOCK) ON em.GroupId = emg.ID
        WHERE 
            (IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
            AND (
                @query IS NULL
                OR NickName LIKE @q
                OR FullName LIKE @q
                OR DomainUsername LIKE @q
                OR em.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @jobTitleId IS NULL
                OR JobTitleId = @jobTitleId
                )
            AND (
                @departmentId IS NULL
                OR emg.DepartmentId = @departmentId
                )
        ) AS e
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
		LEFT JOIN (
			SELECT emg.EmpId
			,emg.AffectedDate
			,emg.Note
			,mg.ManagementGradeId
			,mg.Code
			,mg.Name
			,mg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			  WHERE emg.IsCurrentUsed = 1
			  AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId
    WHERE RowNumber BETWEEN (@pageNumber - 1) * @pageSize
            AND @pageNumber * @pageSize
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[Employee_GetStaffInfo] @query NVARCHAR(256)
    ,@groupId INT = NULL
	,@isSpecial BIT = NULL
	,@departmentId INT = NULL
AS
/*
Created: 20230519@Quinn.Vo
Task: Get staff info by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
@departmentId - Optional parameter to filter by department
*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)
    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
        ,[DOB] AS DateOfBirth
        ,[Email]
        ,[Phone]
        ,[JoinedDate]
		,[GroupDate]
        ,[Skype]
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
        ,jt.NAME AS JobTitle
        ,d.Name AS DepartmentName
        FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
        WHERE 
            (e.IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND e.Enabled = 1
			AND e.isDeleted = 0
            AND (
                @query IS NULL
                OR e.NickName LIKE @q
                OR e.FullName LIKE @q
                OR e.DomainUsername LIKE @q
                OR e.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @departmentId IS NULL
                OR g.DepartmentId = @departmentId
                )
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[EmployeeAttendance_GetByType] @fromtime DATETIME
    ,@totime DATETIME
    ,@type SMALLINT
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@departmentid INT = NULL
    ,@excludeUserDelete BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 2012/03/17@Dan
Task: get attentance list of employees by type
DB: NexHR
Original: 

Revisions:    
    - 20250515@Henry.Dinh: Add DepartmentName
Param's Explanation:
    @type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @note VARCHAR(200)
        ,@today DATETIME
        ,@now DATETIME

    SET @now = dateadd(hh, 7, GETUTCDATE())
    SET @today = convert(VARCHAR(10), @now, 111) -- use UTC because the server use GMT-4

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END

    CREATE TABLE #EmployeeAttendance (
        AttendanceId INT
        ,Code VARCHAR(20)
        ,EmpId INT
        ,FullName NVARCHAR(50)
        ,NickName NVARCHAR(20)
        ,DepartmentName NVARCHAR(100)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,[Rank] INT
        ,Roles INT
        ,AttendanceDate DATETIME
        ,ShiftIf INT
        ,ShiftCode VARCHAR(10)
        ,InFixed DATETIME
        ,OutFixed DATETIME
        ,LateIn INT
        ,EarlyOut INT
        ,TotalWorkHours DECIMAL(18, 2)
        ,Absence BIT
        ,LeaveTypeId INT
        ,LeaveDay DECIMAL(18, 1)
        ,IsLateIn INT
        ,IsEarlyOut INT
        ,IsMissingCheckin INT
        ,IsMissingCheckOut INT
        ,Notes NVARCHAR(100)
		,IsWorkFromHome BIT
        )

    INSERT INTO #EmployeeAttendance
    SELECT T.Id AS AttendanceId 
        ,T.Code
        ,T.EmpId
        ,T.FullName
        ,T.NickName
        ,T.DepartmentName
        ,T.Team
        ,T.TeamCode
        ,T.[Rank]
        ,T.Roles
        ,T.AttendanceDate
        ,T.ShiftId
        ,T.ShiftCode
        ,T.InFixed
        ,T.OutFixed
        ,T.LateIn
        ,T.EarlyOut
        ,T.TotalWorkHours
        ,T.Absence
        ,T.LeaveTypeId
        ,T.LeaveDay
        ,T.isLateIn
        ,T.isEarlyOut
        ,T.isMissingCheckin
        ,T.isMissingCheckOut
        ,stuff(CASE 
                WHEN ISNULL(T.Notes, '') = '' THEN ''
                ELSE ', ' + T.Notes
                END + CASE 
                WHEN isLateIn = 1 THEN (', Late In')
                ELSE ''
                END + CASE 
                WHEN isEarlyOut = 1 THEN ', Early Out'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckin = 1 THEN ', Missing Check-in'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckOut = 1 THEN ', Missing Check-out'
                ELSE ''
                END + CASE 
                WHEN T.isHoliday = 1 THEN ', Public Holiday'
                ELSE ''
                END + CASE 
                WHEN isOffday = 1 THEN ', Off day'
                ELSE ''
                END + CASE 
                WHEN IsWorkFromHome = 1 THEN ', WFH'
                ELSE ''
                END + CASE 
                WHEN (
                        isNotWork = 1
                        AND T.isHoliday <> 1
                        AND T.isOffday <> 1
                        ) THEN ', Absent'
                ELSE CASE 
                        WHEN lv.NAME IS NULL THEN ''
                        ELSE (CASE 
                                WHEN (
                                        T.isHoliday <> 1
                                        AND T.isOffday <> 1
                                        ) THEN (', ' + lv.NAME)
                                ELSE '' END
                                ) + CASE 
                                WHEN LeaveDay = 0.5 THEN ' (Half day)'
                                ELSE ''
                                END
                        END
                END, 1, 2, '') AS Notes,
				IsWorkFromHome
    FROM (
        SELECT a.Id
            ,e.Code
            ,a.EmpId
            ,FullName
            ,NickName
            ,d.Name AS DepartmentName
            ,g.NAME [Team]
            ,g.Code [TeamCode]
            ,g.[Rank]
            ,e.Roles
            ,AttendanceDate
            ,a.ShiftId
            ,s.code AS ShiftCode
            ,a.InFixed
            ,a.OutFixed
            ,a.LateIn
            ,a.EarlyOut
            ,TotalWorkHours
            ,Absence
            ,a.LeaveTypeId
            ,a.LeaveDay
            ,a.Notes
			,c.IsWorkFromHome
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND ISNULL(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.LateIn, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.LateIn, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isLateIn
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.EarlyOut, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.EarlyOut, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isEarlyOut
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND InFixed IS NULL THEN 1
                ELSE 0
                END isMissingCheckin
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND OutFixed IS NULL
                    AND AttendanceDate < @today THEN 1
                ELSE 0
                END isMissingCheckOut
            ,CASE 
                WHEN (
                        e.Id NOT IN (88, 89, 123, 134, 153)
                        AND isnull(Absence, 0) = 1
                        AND c.IsOffDay = 0
                        AND c.IsHoliday = 0
                        AND ISNULL(a.LeaveDay, 0) <> 1
                        AND @now > (AttendanceDate + ' ' + s.EndCheckInTime)
                        ) THEN 1
                ELSE 0
                END isNotWork
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsOffDay = 1 THEN 1
                ELSE 0
                END isOffday
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsHoliday = 1 THEN 1
                ELSE 0
                END isHoliday
        FROM dbo.EmployeeAttendance a WITH (NOLOCK)
            LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.EmpId = a.EmpId
            AND c.Workdate = a.AttendanceDate
            INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
            LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id = a.ShiftId
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
            LEFT JOIN dbo.Department d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
        WHERE e.IsSpecialUser = 0
			AND attendancedate BETWEEN @fromtime
                AND @totime
            AND (
                (
                    e.isDeleted IS NULL
                    OR e.isDeleted = 0
                    )
                OR @excludeUserDelete = 0
                )
            AND (
                @empid IS NULL
                OR a.empid = @empid
                )
            AND (
                @groupid IS NULL
                OR g.ID = @groupid
                )
            AND (
                @departmentid IS NULL
                OR g.DepartmentId = @departmentid
                )
        ) T
        LEFT JOIN LeaveType lv WITH (NOLOCK) ON lv.ID = T.LeaveTypeId
    WHERE @type = 0
        OR (
            @type = 1
            AND isMissingCheckin = 1
            )
        OR (
            @type = 2
            AND isMissingCheckout = 1
            )
        OR (
            @type = 3
            AND isLateIn = 1
            )
        OR (
            @type = 4
            AND isEarlyOut = 1
            )
        OR (
            @type = 5
            AND isNotWork = 1
            )

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,Roles DESC
            ,Code OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @itemsPerPage ROWS ONLY
    END
    ELSE
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,Roles DESC
            ,Code
    END

    SELECT wk.Id
        ,wk.FromDate
        ,(CASE 
            WHEN wk.RequestType IN (1, 2) THEN wk.FromDate
            ELSE wk.ToDate END
            ) AS ToDate
        ,wk.TotalDays
        ,CASE 
            WHEN wk.RequestType = 3 THEN wk.LateInMins
            ELSE wk.EarlyOutMins
            END AS TotalMins
        ,wk.CreatedDate
        -- ,wk.[Status]
        ,wk.Remark
        ,wk.RequestType AS RequestTypeId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Id
            ELSE s3.Id END
            ), 0) AS FromShiftId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ), '') AS FromShiftName
        ,s2.Id AS ToShiftId
        ,s2.Code AS ToShiftName
        ,wk.EmpId AS RequesterId
        ,e.NickName
        ,e.FullName
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wk.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.empid = wk.empid
        AND DATEDIFF(D, cl.WorkDate, wk.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = wk.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = wk.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = cl.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = wk.LeaveTypeID
    WHERE wk.[Status] IN (1, 2)
        AND (
            (
                wk.ToDate IS NULL
                AND DATEDIFF(D, @fromtime, wk.FromDate) >= 0
                AND DATEDIFF(D, wk.FromDate, @totime) >= 0
                )
            OR (
                NOT (
                    DATEDIFF(D, @totime, wk.FromDate) > 0
                    OR DATEDIFF(D, wk.ToDate, @fromtime) > 0
                    )
                )
            )
        AND (
            (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
            OR @excludeUserDelete = 0
            )
        AND (
            @empid IS NULL
            OR wk.empid = @empid
            )
        AND (
            @groupid IS NULL
            OR e.GroupId = @groupid
            )

    SELECT @TotalItem = COUNT(*)
    FROM #EmployeeAttendance

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END
END


GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[EmployeeCalendar_GetLeaveSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
    ,@DepartmentId INT = NULL
AS
/*
Created: 20180515@Oliver.Dang
Task: Get employee leave schedule by date.
DB: NexEPA
Original: 

Revisions:  
    - 20250515@Henry.Dinh: Add department
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsHalfDayLeave
	   ,(CASE WHEN IsOffDay = 1 OR cl.IsHoliday = 1 THEN 0 ELSE cl.LeaveDay END) AS LeaveDay
	   ,cl.IsLeaveDay AS IsLeaveDaye
       ,(CASE 
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN LeaveDay = 0.5 THEN '½'
			WHEN LeaveDay = 1 THEN 'x'
            ELSE ''
            END) AS LeaveType
         ,cl.EmpId
        ,e.NickName
        ,e.FullName
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.Name AS TeamName
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
    INNER JOIN dbo.Department AS d ON d.DepartmentId = g.DepartmentId
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND cl.WorkDate BETWEEN @fromDate
            AND @toDate
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @DepartmentId IS NULL
            OR g.DepartmentId = @DepartmentId
            )
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
        ,cl.WorkDate
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWorkingSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
    ,@DepartmentId INT = NULL
AS
/*
Created: 20160219@Quinn.Vo
Task: Get employee working schedule by date.
DB: NexEPA
Original: 

Revisions:  
    - 20250515@Henry.Dinh: Add department
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsLeaveHalfDay
       ,(CASE 
            WHEN cl.isHoliday = 0
                AND IsOffDay = 0
                AND IsLeaveDay = 0 THEN s.code
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN isLeaveDay = 1 THEN lv.Code + CASE 
                    WHEN LeaveDay = 0.5 THEN '½'
                    WHEN LeaveTypeId2 IS NOT NULL THEN '/' + lv2.Code
                    WHEN LeaveDay = 1 THEN ''
                    ELSE ''
                    END
            END) AS Title
         ,cl.EmpId
        ,e.NickName
        ,e.FullName
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.Name AS TeamName
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
    INNER JOIN dbo.Department AS d ON d.DepartmentId = g.DepartmentId
    LEFT JOIN dbo.LeaveType AS lv ON lv.id = cl.LeaveTypeId
     LEFT JOIN dbo.LeaveType AS lv2 ON lv2.Id = cl.LeaveTypeId2
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND e.Enabled = 1
		AND e.isDeleted = 0
		AND cl.WorkDate BETWEEN @fromDate
            AND @toDate
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @DepartmentId IS NULL
            OR g.DepartmentId = @DepartmentId
            )
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,Roles DESC
        ,e.GroupDate
        ,cl.WorkDate
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroups] 
	@IncludeSpecialTeam BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SET @IncludeSpecialTeam = ISNULL(@IncludeSpecialTeam, 0)

    SELECT g.Id,
		g.Id as TeamId,
		g.Name, 
        d.DepartmentId,
        d.Name as DepartmentName,
		g.Code, 
		g.Rank, 
        COUNT(e.Id) AS NoOfMembers
    FROM dbo.EmployeeGroup g WITH (NOLOCK)
        LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.GroupId = g.Id AND e.isDeleted = 0 AND e.IsSpecialUser = 0
        LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
    WHERE ((@IncludeSpecialTeam = 0 AND g.IsSpecialTeam = 0) OR @IncludeSpecialTeam = 1)  
		AND ISNULL(Deleted,0)=0
    GROUP BY g.Id, g.Name, g.Code, g.Rank, d.DepartmentId, d.Name
    ORDER BY g.[Code]
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[Overtime_GetSummary] 
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
		AND e.GroupId NOT IN (1081, 40)
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[Report_WorkingHoursStatistic]
	 @fromdate DATETIME
	,@todate DATETIME
	,@FromDateLastMonth DATETIME
	,@ToDateLastMonth DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
	,@departmentid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:   
  20170411@Raymond.Le: create SP Report_WorkingHoursStatistic
  20191206@Merlin.Le: PreMonthSickLeave contains Maternity and Parternity
  20250515@Henry.Dinh: Add DepartmentId and DepartmentName
Param's Explanation:

*/

/* Script run
USE [NexHRM]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[Report_WorkingHoursStatistic]
		@fromdate = N'8/1/2016 12:00:00 AM',
		@todate = N'8/25/2016 12:00:00 AM',
		@FromDateLastMonth = N'7/26/2016 12:00:00 AM',
		@ToDateLastMonth = N'7/31/2016 12:00:00 AM'

SELECT	'Return Value' = @return_value

GO
*/

/* Result
PreMonthSickLeave of Julien is 2
*/

DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@MaternityLeave INT
	,@PaternityLeave INT
	,@BusinessLeave INT


SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@MaternityLeave = 13
	,@PaternityLeave = 27
	,@BusinessLeave = 32

CREATE TABLE #tmpWorkingHoursStatistic(
	TotalWorkingDay INT
	,ActualWorkingDay FLOAT
	,AnnualLeave FLOAT
	,PaidLeave FLOAT
	,SickLeave FLOAT
	,InsuranceLeave FLOAT
	,UnpaidLeave FLOAT
	,BusinessLeave FLOAT
	,WorkFromHome FLOAT
	,EmpId INT
	,Code VARCHAR(20)
	,NickName NVARCHAR(20)
	,FullName NVARCHAR(50)
	,GroupId INT
	,GroupCode VARCHAR(20)
	,GroupName NVARCHAR(50)
	,DepartmentId INT
	,DepartmentName NVARCHAR(50)
)

CREATE TABLE #tmpPreMonthUPL (
	EmpId INT
	,PreMonthTotalWorkingDay INT
	,PreMonthSickLeave FLOAT 
	,PreMonthUnpaidLeave FLOAT
	,PreMonthBusinessLeave FLOAT
	,PreWorkFromHome FLOAT
	,PreInsuranceLeave FLOAT
	,PrePaidLeave FLOAT
	,PreAnnualLeave FLOAT
)

INSERT INTO #tmpPreMonthUPL(
	EmpId
	,PreMonthTotalWorkingDay 
	,PreMonthSickLeave  
	,PreMonthUnpaidLeave 
	,PreMonthBusinessLeave 
	,PreWorkFromHome
	,PreInsuranceLeave
	,PrePaidLeave
	,PreAnnualLeave
)
SELECT pm.EmpId
	,pm.TotalWorkingDay 
	,pm.SickLeave 
	,pm.UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,InsuranceLeave
	,PaidLeave
	,AnnualLeave
FROM (
	SELECT c.EmpId
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave
					OR c.LeaveTypeId = @MaternityLeave
					OR c.LeaveTypeId2 = @MaternityLeave
					OR c.LeaveTypeId = @PaternityLeave
					OR c.LeaveTypeId2 = @PaternityLeave) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS SickLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @BusinessLeave
					OR c.LeaveTypeId2 = @BusinessLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @FromDateLastMonth
			AND @ToDateLastMonth
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupid
			)
	GROUP BY c.empid
	) AS pm

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	(TotalWorkingDay + ISNULL(Holiday, 0)) AS TotalWorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0) + ISNULL(Holiday, 0)) AS ActualWorkingDay
	,AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,tpu.PreMonthTotalWorkingDay
	,tpu.PreMonthSickLeave
	,tpu.PreMonthUnpaidLeave
	,tpu.PreMonthBusinessLeave
	,tpu.PreWorkFromHome
	,tpu.PreInsuranceLeave
	,tpu.PrePaidLeave
	,tpu.PreAnnualLeave
	,A.EmpId
	,A.Code
	,NickName
	,FullName
	,A.DepartmentId
	,d.Name AS DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,g.DepartmentId
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
		,SUM(CASE 
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 0 THEN 1
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 1
					AND c.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) [ActualWorkingDay]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessTrip
						OR lv2.Id = @BusinessTrip
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessTrip
								OR lv2.Id <> @BusinessTrip
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [BusinessTrip]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessLeave
						OR lv2.Id = @BusinessLeave
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessLeave
								OR lv2.Id <> @BusinessLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [SickLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		
		,SUM(CASE WHEN c.IsHoliday = 1 THEN 1 ELSE 0 END) Holiday
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @fromdate
			AND @todate
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,g.DepartmentId
	) A
	LEFT JOIN #tmpPreMonthUPL AS tpu
	ON tpu.EmpId = A.EmpId
	LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = A.DepartmentId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate


DROP TABLE #tmpPreMonthUPL
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Report_WorkingHoursSummary    Script Date: 3/13/2016 3:21:43 PM ******/
/*
declare @fromdate datetime,
        @todate datetime,
        @empid int,
        @groupid int
set @fromdate = '2012/07/01'
set @todate = '2012/07/30'
set @empid=128
--set @groupid =3
EXEC Report_WorkingHoursSummary '2015-08-01' , '2015-08-24' ,    165 , null 
*/
ALTER PROCEDURE [dbo].[Report_WorkingHoursSummary] @fromdate DATETIME
	,@todate DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
    ,@departmentid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@BusinessLeave INT

SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@BusinessLeave = 32

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	WorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0)) AS ActualWorkingDay
	,(AnnualLeave + ISNULL(upl.TotalAnnualLeave, 0)) AS AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,(UnpaidLeave + ISNULL(upl.TotalUnpaidLeave, 0)) AS UnpaidLeave
	,LateIn
	,EarlyOut
	,TotalHours
	,AverageHours
	,A.EmpId
	,Code
	,NickName
	,FullName
	,DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate		
		,d.Name AS DepartmentName
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 THEN 1
				ELSE 0
				END) AS WorkingDay
		,SUM(CASE 
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 0 THEN 1
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 1
					AND c.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) [ActualWorkingDay]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessTrip
						OR lv2.Id = @BusinessTrip
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessTrip
								OR lv2.Id <> @BusinessTrip
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [BusinessTrip]
		,SUM(CASE 
			WHEN isOffday = 0
				AND c.isHoliday = 0
				AND IsLeaveDay = 1
				AND (
					lv1.Id = @BusinessLeave
					OR lv2.Id = @BusinessLeave
					) THEN CASE 
					WHEN ISNULL(lv2.Id, 0) <> 0
						AND (
							lv1.Id <> @BusinessLeave
							OR lv2.Id <> @BusinessLeave
							) THEN 0.5
					ELSE c.LeaveDay
					END
			ELSE 0
			END) AS [BusinessLeave]
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [SickLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		,SUM(a.LateIn) LateIn
		,SUM(a.EarlyOut) EarlyOut
		,SUM(ISNULL(a.TotalWorkHours, 0)) TotalHours
		,SUM(ISNULL(a.TotalWorkHours, 0)) / CASE 
			WHEN SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END) = 0 THEN 1
			ELSE SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END)
			END AS AverageHours
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @fromdate
			AND @todate
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,d.Name
	) A
	LEFT JOIN (
	SELECT EmpId
		,SUM(CASE 
				WHEN IsAnnual = 0 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalUnpaidLeave
		,SUM(CASE 
				WHEN IsAnnual = 1 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalAnnualLeave
	FROM dbo.LeaveConversion AS lv WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lv.EmpId = e.Id
	WHERE lv.[Type] = 1
		AND (
			@empid IS NULL
			OR empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND [Date] BETWEEN @fromdate
			AND @todate
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY EmpId
	) AS upl ON A.EmpId = upl.EmpId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--[Supervisor_Get] ''
ALTER PROCEDURE [dbo].[Supervisor_Get] 
    @keyword NVARCHAR(256),
    @groupId INT = NULL,
    @departmentId INT = NULL
AS
/*
Created: 20240516@Lilac.Nguyen
Task: Get Supervisor detail.
DB: NexHRM
Original: 

Revisions:  
    20250516@Henry.Dinh: Add filter by department ID
Param's Explanation:
    @keyword: Search keyword for employee name
    @groupId: Filter by group ID
    @departmentId: Filter by department ID
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @q NVARCHAR(256)

    SET @q = '%' + @keyword + '%'

    IF LEN(@keyword) = 0
        SET @keyword = NULL

	SELECT
		e.Id AS EmpId,
		e.NickName,
		e.FullName,
		d.[Name] AS DepartmentName,
		g.[Name] AS GroupName,
		STRING_AGG(CONVERT(NVARCHAR(MAX), sup.ApproverNickName), ', ') AS Approvers
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON sup.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g ON e.GroupId = g.Id
		LEFT JOIN dbo.Department AS d ON g.DepartmentId = d.DepartmentId
	WHERE 
		ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] <> 0 
		AND e.IsSpecialUser = 0
		AND (
			@keyword IS NULL
			OR e.NickName LIKE @q
			OR e.FullName LIKE @q
		)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
		)
		AND (
			@departmentId IS NULL
			OR g.DepartmentId = @departmentId
		)
	GROUP BY 
		e.Id,
		e.NickName,
		e.FullName,
		d.Name,
		g.[Name],
		g.[Rank],
		g.Code,
		e.Roles,
		e.GroupDate
	ORDER BY 
		d.[Name],
		g.[Rank],
		g.Code,
		e.Roles DESC,
		e.GroupDate
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Department_Get]
	@ParentId INT = NULL
	,@IsDisabled BIT = NULL
	,@IsSpecial BIT = NULL
AS
/*
Created: 20140818@Josh.Phan
Task: Get departments by departmentId.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT dep.[DepartmentId] AS Id
		,dep.[Code]
		,dep.[Name]
		,dep.[ParentId]
		,dep2.[Name] AS ParentName
		,dep.[ModifiedDate]
		,dep.[CreatedDate]
		,dep.[IsDisabled]
		,dep.[DepartmentLevel]
		,dep.[DisplayOrder]
		,dep.[CreatedBy]
		,cr.FullName
		,cr.NickName
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
		,dep.[ModifiedBy]
		,md.FullName
		,md.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.Id
	LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.Id
	LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.Id
	LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE (
			@ParentId IS NULL
			OR dep.ParentId = @ParentId
			)
		AND (
			@IsDisabled IS NULL
			OR dep.IsDisabled = @IsDisabled
			)
		AND (
			@IsSpecial IS NULL
			OR dep.IsSpecial = @IsSpecial
			)
    ORDER BY dep.DisplayOrder, dep.Name
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- [Employee_GetByUsername] 'quinn.vo'

ALTER PROCEDURE [dbo].[Employee_GetByUsername] @Username VARCHAR(50)
AS
/*
Created: 20230205@Quinn.Vo
Task: Get employee info by username.
DB: NexHRM
Original: 

Revisions:  
    20250520@Henry.Dinh: Add DepartmentName.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @DomainName VARCHAR(20)

	SELECT @DomainName = Value
	FROM dbo.SystemSetting WITH (NOLOCK)
	WHERE [Name] = 'DomainName'

	SELECT e.[Id]
	,e.[Code]
	,e.DomainUsername
	,e.[FullName]
	,e.[NickName] AS EnglishName
	,e.DOB AS DateOfBirth
	,e.PlaceOfBirth
	,e.[Email]
	,e.EmployeeCard
    ,d.Name AS DepartmentName
	,g.Code AS TeamCode
	,g.Name AS TeamName
	,jt.Name AS JobTitle
	,gra.Code AS JobGrade
	,e.GroupDate
	,e.JoinedDate
	,e.ConfirmedDate
	,e.ResignedDate
	,e.Phone
	,(CASE WHEN e.Gender = 0 THEN 'F'
		WHEN e.Gender = 1 THEN 'M'
		ELSE 'Unknown' END) AS Gender
	,e.IdCardNumber
	,e.IssuedDate
	,e.IssuedPlace
	,e.Phone
	,e.CurrentAddress
	,e.PermanentAddress
	,e.PassportNo
	,e.PassportExp
	,e.SocialInsuranceNo
	,e.Skype
	,e.Yahoo AS Telegram
	,e.[Enabled]
	,e.[IsDeleted]
	,e.Marriage
	,e.CreatedDate
	,e.ModifiedDate
	,e.Enabled
	,e.isDeleted
	,e.IsSpecialUser
	,e.ContactName AS EmergencyContact
	,e.UserRelationship AS EmergencyRelationship
	,e.ContactPhone AS EmergencyPhone
	,e.ContactAddress AS EmergencyAddress
	,t.LRExamDate AS ToeicExamDate
    ,t.Listening AS ToeicListening
	,t.Reading AS ToeicReading
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.NickName ELSE egs.NickName END AS SupervisorEnglishName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.FullName ELSE egs.FullName END AS SupervisorFullName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.Email ELSE egs.Email END AS SupervisorEmail
	FROM dbo.Employee AS e WITH (NOLOCK) 
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
		LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON gs.GroupId = e.GroupId
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON es.EmpId = e.Id
		LEFT JOIN dbo.Employee AS egs WITH (NOLOCK) ON gs.ApproverId = egs.Id
		LEFT JOIN dbo.Employee AS egs2 WITH (NOLOCK) ON es.ApproverId = egs2.Id
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
	WHERE e.DomainUsername = (@DomainName + @Username)
END
GO
