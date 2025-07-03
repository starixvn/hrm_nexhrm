USE [bodb_HRM]
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Auto_AnnualLeaveBalance_Get]
(
	@CompanyId SMALLINT
    ,@year INT
)
AS
/*
    Created: Tim.Trinh@25-07-2022
    Task: annual leave balance
    DB: bodb_HRM

    Revisions:
        - 20250409@Tim: remove section [Redmine: #222886]
		- 20250619@Henry: fix ambiguous column error migrate nex
    Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME = CAST(@year AS VARCHAR(10)) + '/01/01'
		,@todate DATETIME

	SET @todate = DATEADD(yy, 1, @fromdate)

	SELECT e.EmpId
		,ISNULL(CreditDays, 0) AS CreditDays
		,SUM(ISNULL(DeductDays, 0)) AS DeductDays
	INTO #tmpCurrenBalance
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS c WITH (NOLOCK) ON c.EmpId = e.EmpId AND c.CompanyId = @CompanyId
		LEFT JOIN (
			SELECT EmpId
				,DeductDays
				,FromDate
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE YEAR(FromDate) = @year
				AND ISNULL(Deleted, 0) = 0
			) AS ded ON e.EmpId = ded.EmpId
		LEFT JOIN (
			SELECT lt.EmpId
				,SUM(CreditDays) CreditDays
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			WHERE lt.FromDate >= @fromdate
				AND lt.FromDate < @todate
				AND ISNULL(lt.Deleted, 0) = 0
				AND lt.CreditDays > 0
				AND ISNULL(lt.Deleted, 0) = 0
			GROUP BY lt.EmpId
		) cred ON e.EmpId = cred.Empid
	WHERE e.[Enabled] = 1
		AND (
			e.isDeleted = 0
			OR YEAR(e.ResignedDate) >= year(@todate)
			)
	GROUP BY e.EmpId
		,ISNULL(cred.CreditDays, 0)

	SELECT ISNULL(CreditDays, 0) AS AnnualLeaveCurrentYear
		,ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) AS LeaveBalance
		,cb.EmpId
		,e.Code
		,Nickname
		,FullName
		,e.Email
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,e.LetsTalk
		,esm.IsLocal
	INTO #tmpEmployeeLeaveBalance
	FROM #tmpCurrenBalance AS cb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = cb.EmpId
		INNER JOIN dbo.CompanyEmployee AS c WITH (NOLOCK) ON c.EmpId = e.EmpId AND c.CompanyId = @CompanyId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId AND g.CompanyId = @CompanyId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = g.DepartmentId
		INNER JOIN dbo.EmployeeSummary AS esm WITH (NOLOCK) ON esm.EmpId = e.EmpId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
		AND ISNULL(CreditDays, 0) > 0
		AND ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) > 0
		AND esm.IsTopManager = 0
	ORDER BY e.Nickname

	SELECT EmpId
		,Code
		,Nickname
		,FullName
		,Email
		,AnnualLeaveCurrentYear
		,LeaveBalance
		,DepartmentId
		,DepartmentName
		,LetsTalk
		,IsLocal
	FROM #tmpEmployeeLeaveBalance WITH (NOLOCK)

	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,suemp.LetsTalk
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS c WITH (NOLOCK) ON c.EmpId = su.EmpId AND c.CompanyId = @CompanyId
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON su.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.CompanyEmployee AS cu WITH (NOLOCK) ON cu.EmpId = suemp.EmpId AND cu.CompanyId = @CompanyId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = suemp.GroupId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = g.DepartmentId
		AND suemp.[Enabled] = 1
		AND suemp.isDeleted = 0
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND su.IsDeleted = 0
	
	UNION ALL
	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,suemp.LetsTalk
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		INNER JOIN dbo.CompanyEmployee AS c WITH (NOLOCK) ON c.EmpId = e.EmpId AND c.CompanyId = @CompanyId
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON e.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = suemp.GroupId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = g.DepartmentId
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND su.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS empsu WITH (NOLOCK)
			WHERE e.EmpId = empsu.EmpId
				AND empsu.IsDeleted = 0
				AND empsu.RequestType = 6
				AND empsu.ActionType IN (2, 3)
			)

	SELECT dep.DepartmentId
		,dep.[Name] AS DepartmentName
		,COUNT(*) AS TotalEmployee
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS c WITH (NOLOCK) ON c.EmpId = e.EmpId AND c.CompanyId = @CompanyId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = g.DepartmentId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
	GROUP BY dep.DepartmentId
		,dep.[Name]
	ORDER BY dep.[Name]

	DROP TABLE #tmpCurrenBalance

	DROP TABLE #tmpEmployeeLeaveBalance
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Employee_GetByRole] @CompanyId SMALLINT
    ,@RoleIds VARCHAR(20)
	,@query NVARCHAR(256)
	,@ExcludeIds VARCHAR(2000)
AS
/*
Created: 2013/10/22@Quinn
Task: Get list employee by role (for supervisor list)
DB: HRM
Original:

Revisions:
    - 20250619@Henry: fix ambiguous column error migrate nex
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @q NVARCHAR(258)

	SET @q = '%' + @query + '%'

	IF LEN(@query) = 0
		OR @query = ';'
	BEGIN
		SET @query = NULL
	END

	CREATE TABLE #tmpExcludedEmployee (EmpId INT)

	CREATE TABLE #tmpRole (RoleId INT)

	CREATE TABLE #tmpEmployee (EmpId INT)

	INSERT INTO #tmpExcludedEmployee (EmpId)
	SELECT value
	FROM STRING_SPLIT(@ExcludeIds, ',')

	INSERT INTO #tmpRole (RoleId)
	SELECT value
	FROM STRING_SPLIT(@RoleIds, ',')

	INSERT INTO #tmpEmployee
	SELECT e.EmpId AS Id
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON e.EmpId = ce.EmpId
        INNER JOIN dbo.EmployeeRole AS r WITH (NOLOCK) ON r.EmployeeId = e.EmpId
	WHERE ce.CompanyId = @CompanyId
        AND r.RoleId IN (
			SELECT RoleId
			FROM #tmpRole
			)
		AND e.EmpId NOT IN (
			SELECT EmpId
			FROM #tmpExcludedEmployee
			)
		AND (
			E.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND e.[Enabled] = 1
		AND (
			@query IS NULL
			OR NickName LIKE @q
			OR FullName LIKE @q
			OR e.DomainUsername LIKE @q
			OR e.Code LIKE @q
			)

	SELECT e.EmpId AS Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB
		,e.Email
		,e.JobTitleId
		,e.[Enabled]
		,e.Phone
		,e.Gender
		,e.ManagerId
		,e.JoinedDate
		,e.ConfirmedDate
		,e.ResignedDate
		,e.Skype
		,e.LetsTalk
		,e.Marriage
		,e.LastLogin
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
		,e.PermanentAddress AS [Address]
		,e.CurrentAddress
		,e.IdCardNumber AS CardNo
		,e.PassportNo
		,e.PassportExp
		,e.[Password]
		,ISNULL(e.GroupID, - 1) AS [TeamID]
		,ISNULL(g.[Name], '') AS [Team]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tmpEmployee AS e1 ON e1.EmpId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId

	SELECT emRo.EmployeeId
		,emRo.RoleId
		,loDa.Code
		,loDa.[Name]
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData AS loDa WITH (NOLOCK) ON emRo.RoleId = loDa.[Value]
        AND loda.CompanyId = @CompanyId
		INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
		AND look.Code = 'SR' -- System Role
		INNER JOIN #tmpEmployee AS e ON e.EmpId = emRo.EmployeeId

	DROP TABLE #tmpExcludedEmployee

	DROP TABLE #tmpRole

	DROP TABLE #tmpEmployee
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Employee_GetEmployees] 
(	
	@query NVARCHAR(256) = NULL
	,@teamIds NVARCHAR(MAX) = ''
	,@enable BIT = NULL
	,@isDelete BIT = NULL
	,@isLocal BIT = NULL
	,@pageNumber INT = 1
	,@pageSize INT = 2000
	,@totalRow INT = NULL OUTPUT
	,@titleId INT = NULL
	,@companyId INT
)
AS
/*
	Created: Tom.Vo@2013-10-23
	Task: Insert default working calendar for employee
	DB: HRM_SMS
	Original:

	Revisions:
	Param's Explanation:
		-Otis - 03062023: get field ResignedReason
		-Jay  - 04042025: remove Section
		-Henry- 10042025: map Company
		-Henry- 19062025: fix ambiguous column error
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupsList (GroupId INT)

	INSERT INTO #GroupsList (GroupId)
	SELECT value
	FROM STRING_SPLIT(@teamIds, ',')

	IF LEN(@query) = 0
		OR @query LIKE '% or %'
		OR @query LIKE '%--%'
	BEGIN
		SET @query = NULL
	END

	SET @query = REPLACE(@query, '''', '''''')

	DECLARE @q NVARCHAR(256) = @query + '%'''

	SELECT @pageNumber = ISNULL(@pageNumber, 1)
		,@pageSize = ISNULL(@pageSize, 2000)

	DECLARE @SQL NVARCHAR(4000)
		,@sWhere NVARCHAR(4000)
		,@bWhere BIT

	SELECT @SQL = 'SELECT @totalRow = count(1) FROM  dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.CompanyEmployee ce WITH (NOLOCK) ON ce.EmpId = e.EmpId AND ce.CompanyId = ' + CAST(@companyId AS VARCHAR(10))  + CHAR(13)
				+ 'INNER JOIN dbo.EmployeeSummary AS s WITH (NOLOCK) ON s.EmpId = e.EmpId ' + CHAR(13)

	SELECT @sWhere = ''
		,@bWhere = 0

	IF @enable IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + 'WHERE ISNULL(e.isDeleted, 0) = 0 and e.Enabled = ' + CONVERT(NVARCHAR(10), @enable) + CHAR(13)
			,@bWhere = 1
	END
	ELSE IF @isDelete IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' e.isDeleted = ' + CONVERT(NVARCHAR(10), @isDelete) + CHAR(13)
			,@bWhere = 1
	END

	IF @q IS NOT NULL
		AND @query IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' (e.NickName LIKE ''' + @q + ' OR e.FullName LIKE N''%' + @q + ' OR e.Code LIKE ''%' + @q + ')' + CHAR(13)
			,@bWhere = 1
	END

	IF ISNULL( @teamIds, '') <> ''
	BEGIN
	SELECT @sWhere = @sWhere + CASE 
			WHEN @bWhere = 1 THEN ' AND '
			ELSE ' WHERE '
			END + ' ( e.GroupId IN (SELECT GroupId FROM #GroupsList) )' + CHAR(13)
		,@bWhere = 1
	END

	IF @titleId IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' JobTitleId = ' + CONVERT(NVARCHAR(10), @titleId) + CHAR(13)
			,@bWhere = 1
	END

	IF @isLocal IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' s.IsLocal = ' + CONVERT(NVARCHAR(10), @isLocal) + CHAR(13)
			,@bWhere = 1
	END

	SELECT @SQL = @SQL + @sWhere

	PRINT @SQL

	EXEC sp_executesql @SQL
		,N'@totalRow INT OUTPUT'
		,@totalRow = @totalRow OUTPUT

	SELECT @SQL = 'SELECT e.[EmpId]
      ,e.[Code]
      ,e.[DomainUsername]
      ,[FullName]
      ,[NickName]
      ,[DOB]
      ,e.[Email]
      ,[JobTitleId]
      ,e.[Enabled]
      ,[Phone]
      ,[Gender]
      ,[JoinedDate]
      ,e.[ConfirmedDate]
      ,e.[ResignedDate]
	  ,[ResignedReason]
      ,[Skype]
      ,[LetsTalk]
      ,[Marriage]
      ,[LastLogin]
      ,e.[CreatedDate]
      ,e.[CreatedBy]
      ,e.[ModifiedDate]
      ,e.[ModifiedBy]
      ,e.[isDeleted]
      ,[PermanentAddress]
      ,[CurrentAddress]
      ,[IdCardNumber]
      ,[PassportNo]
      ,[PassportExp]
      ,e.[Password]
      ,[ChineseName]
      ,[PlaceOfbirth]
      ,[Ethnic]
      ,[Native]
      ,[Religion]
      ,[DateOfIDCard]
      ,[PlaceOfIDCard]
      ,[HomePhone]
      ,[Qualification]
      ,[Major]
      ,[PasswordReset]
      ,[EmployeeCard]
	  ,sc.[Name] AS SubCompanyName' + CHAR(13)
	  + ',s.[AnnualLeaveBalance]' + CHAR(13)
	  + ',dbo.fnc_GetKIVBalance(e.EmpId,null) AS KeepInViewBalance' + CHAR(13)
	  + ',s.[CanOT]' + CHAR(13)
	  + ',s.[IsLocal]' + CHAR(13)
	  + ',s.[WorkingDays]' + CHAR(13)
	  + ',s.[WorkingHours]' + CHAR(13)
	  + ',s.[IsDefaultSchedule]' + CHAR(13)
	  + ',s.[CompanyDate]' + CHAR(13)
	  + ',s.[IsTopManager]' + CHAR(13)
	  + ',s.[OffDaysbyWeek]' + CHAR(13)
	  + ',s.CountryCode' + CHAR(13)
	  + ',s.IsExpatUseHoliday' + CHAR(13)
	  + ',ISNULL(ctry.Nationality, ctry.CountryName) CountryName' + CHAR(13)
	  + ',s.MilitaryFromDate' + CHAR(13)
	  + ',s.MilitaryToDate' + CHAR(13)
	  + ',jt.Name AS [JobTitle]' + CHAR(13)
	  + ',c.CompanyId' + CHAR(13)
	  + ',c.[Name] AS CompanyName' + CHAR(13)
	  + ',unit.[Name] AS UnitName' + CHAR(13)
	  + ',unit.[Code] AS UnitCode' + CHAR(13)
	  + ',g.GroupId AS [TeamId]' + CHAR(13)
	  + ',g.Code AS [TeamCode]' + CHAR(13)
	  + ',g.Name AS [TeamName]' + CHAR(13)
	  + ',g.Rank' + CHAR(13)
	  + ',dep.DepartmentId' + CHAR(13)
	  + ',dep.Code AS DepartmentCode' + CHAR(13)
	  + ',dep.[Name] AS DepartmentName' + CHAR(13)
	  + 'FROM dbo.Employee AS e WITH (NOLOCK)' + CHAR(13)
	  + 'INNER JOIN dbo.CompanyEmployee ce WITH (NOLOCK) ON ce.EmpId = e.EmpId AND ce.CompanyId = ' + CAST(@companyId AS VARCHAR(10))  + CHAR(13)
	  + 'LEFT JOIN dbo.SubCompany sc WITH (NOLOCK) ON ce.SubCompanyId = sc.SubCompanyId' + CHAR(13)
	  + 'INNER JOIN dbo.EmployeeSummary AS s WITH (NOLOCK) ON s.EmpId = e.EmpId' + CHAR(13)
	  + 'LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID' + CHAR(13)
	  + 'INNER JOIN dbo.[Department] AS dep WITH (NOLOCK) ON dep.DepartmentId = g.DepartmentId' + CHAR(13)
	  + 'INNER JOIN dbo.[Unit] AS unit WITH (NOLOCK) ON dep.UnitId = unit.UnitId' + CHAR(13)
	  + 'LEFT JOIN [dbo].[Country] AS ctry WITH(NOLOCK) ON ctry.CountryCode = s.CountryCode' + CHAR(13)
	  + 'LEFT JOIN [dbo].[Lookup] AS l WITH(NOLOCK) ON l.Code = ''JT''' + CHAR(13)
	  + 'LEFT JOIN [dbo].[LookupData] AS jt WITH(NOLOCK) ON l.Id = jt.LookupId AND e.JobTitleId = jt.Id' + CHAR(13)
	  + 'LEFT JOIN [dbo].Company c WITH (NOLOCK) ON c.CompanyId =' + CAST(@companyId AS VARCHAR(10))  + CHAR(13)
	  + @sWhere + 'ORDER BY ' + CHAR(13)

	IF (@enable = 0)
	BEGIN
		SELECT @SQL = @SQL + ' e.ResignedDate DESC,'
	END

	SELECT @SQL = @SQL + 'JoinedDate DESC' + CHAR(13)
		+ ',[Code] DESC' + CHAR(13)
		+ ',[NickName]' + CHAR(13)
		+ 'OFFSET ' + CONVERT(NVARCHAR(10), (@pageNumber - 1) * @pageSize) + ' ROWS FETCH NEXT ' + CONVERT(NVARCHAR(10), @pageSize) + ' ROWS ONLY;'

	EXEC sp_executesql @SQL

	SELECT EmployeeId
		,RoleId
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)

	PRINT @SQL

	DROP TABLE #GroupsList
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Employee_GetNewEmployeeCode]
	@CompanyId SMALLINT
AS
/*
Created: Tom.Vo@2013-10-21
Task: get employee by code
DB: HRM
Original:

Revisions:
    - 20250619@Henry: fix ambiguous column error migrate nex
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxCodeNumber VARCHAR(20)

	DECLARE @LastestPrefix VARCHAR(20)

	SELECT @MaxCodeNumber = MAX(CAST(
		RIGHT(Code, COALESCE(NULLIF(PATINDEX('%[^0-9]%', REVERSE(Code)), 0) - 1, LEN(Code)))
		AS BIGINT))
	FROM dbo.Employee AS e WITH(NOLOCK)
		INNER JOIN CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = e.EmpId 
	WHERE e.[Enabled] = 1 
		AND ISNULL(Code, '') != ''
		AND ce.CompanyId = @CompanyId

	IF @MaxCodeNumber IS NOT NULL
		SELECT TOP 1 @LastestPrefix = LEFT(Code, PATINDEX('%[0-9]%', Code) - 1)
		FROM dbo.Employee AS e WITH(NOLOCK)
		INNER JOIN CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = e.EmpId 
		WHERE e.[Enabled] = 1 
			AND ISNULL(Code, '') != ''
			AND ce.CompanyId = @CompanyId
		ORDER BY e.EmpId DESC


		SELECT @LastestPrefix + CAST((@MaxCodeNumber + 1) AS VARCHAR(20)) AS Code
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Employee_Information_update] @EmpId INT
	,@Code VARCHAR(20)
	,@DomainUsername VARCHAR(50) = ''
	,@FullName NVARCHAR(50) = ''
	,@Nickname NVARCHAR(20) = ''
	,@DOB DATE = NULL
	,@Email VARCHAR(50) = NULL
	,@JobTitleId INT = NULL
	,@JobLevelId INT = NULL
    ,@ManagementLevelId SMALLINT = NULL
	,@Enabled BIT
	,@Phone VARCHAR(50) = NULL
	,@Gender TINYINT = NULL
	,@TeamId INT
	,@Roles VARCHAR(20)
	,@JoinedDate DATE = NULL
	,@ConfirmedDate DATE = NULL
	,@ResignedDate DATE = NULL
	,@ResignedReason NVARCHAR(200) = NULL
	,@Skype VARCHAR(100) = NULL
	,@LetsTalk VARCHAR(100) = NULL
	,@Marriage TINYINT = 1
	,@ModifiedBy INT
	,@PermanentAddress NVARCHAR(250) = ''
	,@CurrentAddress NVARCHAR(250) = ''
	,@IdCardNumber VARCHAR(20) = ''
	,@PassportNo VARCHAR(35) = ''
	,@PassportExp DATE = NULL
	,@ChineseName NVARCHAR(200) = NULL
	,@CanOT BIT
	,@SpecialOT DECIMAL(3, 1) = NULL
	,@IsLocal BIT
	,@WorkingDays INT
	,@WorkingHours INT
	,@WorkingHoursByWeek TINYINT = NULL
	,@CompanyDate DATE = NULL
	,@CountryCode VARCHAR(5) = NULL
	,@IsTopManager BIT = 0
	,@OffDaysbyWeek SMALLINT = 0
	,@IsExpatUseHoliday BIT = 0
	,@PlaceOfbirth NVARCHAR(250) = ''
	,@Ethnic NVARCHAR(250) = ''
	,@Native NVARCHAR(250) = ''
	,@Religion NVARCHAR(250) = ''
	,@DateOfIDCard DATE = NULL
	,@PlaceOfIDCard NVARCHAR(250) = ''
	,@HomePhone NVARCHAR(250) = ''
	,@Qualification NVARCHAR(300) = ''
	,@Major NVARCHAR(300) = ''
	,@EmployeeCard VARCHAR(10) = ''
	,@PickupRemark NVARCHAR(200) = NULL
	,@PromotionDate DATE = NULL
	,@TeamDate DATE = NULL
	,@MilitaryFromDate DATE = NULL
	,@MilitaryToDate DATE = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@CompanyId SMALLINT
	,@SubCompanyId INT = NULL
AS
/*
Created: TOM
Task: UPDATE an employee Information 
DB: HRM

Revisions:
	-Henry- 19062025: fix ambiguous column error
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId_exist INT
		,@Nickname_exist NVARCHAR(20)
		,@FullName_exist NVARCHAR(50)
		,@Code_exist NVARCHAR(50)
		,@Roles_old VARCHAR(100)
		,@Enabled_old BIT
		,@IsDeleted_old BIT
		,@IsDefaultSchedule_old BIT
		,@AnnualLeaveBalance_old NUMERIC(18, 1) = 0
		,@KeepInViewBalance_old NUMERIC(18, 1) = 0
		,@xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = e.FullName
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee c WITH(NOLOCK) ON c.EmpId = e.EmpId
	WHERE e.EmpId <> @EmpId
		AND Code = @code
		AND c.CompanyId = @CompanyId


	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code ' + @Code + ' has been assigned to ' + @FullName_exist

		RETURN
	END

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@Nickname_exist = Nickname
		,@FullName_exist = FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee c WITH(NOLOCK) ON c.EmpId = e.EmpId
	WHERE e.EmpId <> @EmpId
		AND Nickname = @nickname
		AND c.CompanyId = @CompanyId
		AND (
			e.isDeleted = 0
			OR e.[Enabled] = 1
			)

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Nickname  ' + @Nickname_exist + '  has been assigned to ' + @FullName_exist

		RETURN
	END

	SELECT @Enabled_old = [Enabled]
		,@IsDeleted_old = isDeleted
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @EmpId
	
	SELECT @IsDefaultSchedule_old = IsDefaultSchedule
		,@AnnualLeaveBalance_old = AnnualLeaveBalance
		,@KeepInViewBalance_old = KeepInViewBalance
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	SELECT @Roles_old = COALESCE(@Roles_old + ',', '') + CONVERT(NVARCHAR(2), RoleId)
	FROM dbo.EmployeeRole WITH (NOLOCK)
	WHERE EmployeeId = @EmpId
	Order by RoleID 

	SET @xml = (
			SELECT Code AS '@Code'
				,emp.DomainUsername AS '@DomainUsername'
				,FullName AS '@FullName'
				,Nickname AS '@Nickname'
				,DOB AS '@DOB'
				,emp.Email AS '@Email'
				,JobTitleId AS '@JobTitleId'
				,JobLevelId AS '@JobLevelId'
				,ManagementLevelId AS'@ManagementLevelId'
				,emp.[Enabled] AS '@Enabled'
				,emp.isDeleted AS '@IsDeleted'
				,Phone AS '@Phone'
				,Gender AS '@Gender'
				,emp.GroupId AS '@TeamId'
				,JoinedDate AS '@JoinedDate'
				,emp.ConfirmedDate AS '@ConfirmedDate'
				,emp.ResignedDate AS '@ResignedDate'
				,ResignedReason AS '@ResignedReason'
				,Skype AS '@Skype'
				,LetsTalk AS '@LetsTalk'
				,Marriage AS '@Marriage'
				,PermanentAddress AS '@PermanentAddress'
				,CurrentAddress AS '@CurrentAddress'
				,IdCardNumber AS '@IdCardNumber'
				,PassportNo AS '@PassportNo'
				,PassportExp AS '@PassportExp'
				,ChineseName AS '@ChineseName'
				,PlaceOfbirth AS '@PlaceOfbirth'
				,Ethnic AS '@Ethnic'
				,[Native] AS '@Native'
				,Religion AS '@Religion'
				,DateOfIDCard AS '@DateOfIDCard'
				,PlaceOfIDCard AS '@PlaceOfIDCard'
				,HomePhone AS '@HomePhone'
				,Qualification AS '@Qualification'
				,Major AS '@Major'
				,EmployeeCard AS '@EmployeeCard'
				,PickupRemark AS '@PickupRemark'
				,PromotionDate AS '@PromotionDate'
				,TeamDate AS '@TeamDate'
				,@Roles_old AS '@RoleIds'
				,CanOT AS '@CanOT'
				,SpecialOT AS '@SpecialOT'
				,IsLocal AS '@IsLocal'
				,WorkingDays AS '@WorkingDays'
				,WorkingHours AS '@WorkingHours'
				,WorkingHoursByWeek AS '@WorkingHoursByWeek'
				,IsTopManager AS '@IsTopManager'
				,empsum.CompanyDate AS '@CompanyDate'
				,CountryCode AS '@CountryCode'
				,IsExpatUseHoliday AS '@IsExpatUseHoliday'
				,OffDaysbyWeek AS '@OffDaysbyWeek'
				,MilitaryFromDate AS '@MilitaryFromDate'
				,MilitaryToDate AS '@MilitaryToDate'
				,ce.SubCompanyId AS '@SubCompanyId'
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
				INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON emp.EmpId = ce.EmpId
			WHERE emp.EmpId = @EmpId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	IF @Enabled_old = 0 AND @Enabled = 1 AND @IsDeleted_old = 1
	BEGIN
		UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
		SET isDeleted = 0
		WHERE EmpId = @EmpId
	END

	UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET Code = @Code
		,DomainUsername = @DomainUsername
		,FullName = @FullName
		,Nickname = @Nickname
		,DOB = @DOB
		,Email = REPLACE(@Email, ' ', '')
		,JobTitleId = @JobTitleId
		,JobLevelId = @JobLevelId
		,ManagementLevelId = @ManagementLevelId
		,[Enabled] = @Enabled
		,Phone = @Phone
		,Gender = @Gender
		,GroupId = @TeamId
		,JoinedDate = @JoinedDate
		,ConfirmedDate = @ConfirmedDate
		,ResignedDate = @ResignedDate
		,ResignedReason = @ResignedReason
		,Skype = @Skype
		,LetsTalk = @LetsTalk
		,Marriage = @Marriage
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
		,PermanentAddress = @PermanentAddress
		,CurrentAddress = @CurrentAddress
		,IdCardNumber = @IdCardNumber
		,PassportNo = @PassportNo
		,PassportExp = @PassportExp
		,ChineseName = @ChineseName
		,PlaceOfbirth = @PlaceOfbirth
		,Ethnic = @Ethnic
		,[Native] = @Native
		,Religion = @Religion
		,DateOfIDCard = @DateOfIDCard
		,PlaceOfIDCard = @PlaceOfIDCard
		,HomePhone = @HomePhone
		,Qualification = @Qualification
		,Major = @Major
		,EmployeeCard = @EmployeeCard
		,PickupRemark = @PickupRemark
		,PromotionDate = @PromotionDate
		,TeamDate = @TeamDate
	WHERE EmpId = @EmpId

	UPDATE CompanyEmployee  WITH (ROWLOCK, UPDLOCK)
	SET SubCompanyId = @SubCompanyId
	WHERE EmpId = @EmpId

	EXEC dbo.EmployeeRole_Update @EmployeeId = @EmpId
		,@RoleIds = @Roles
		,@ModifiedBy = @ModifiedBy

	EXEC dbo.Employee_Summary_Update @EmpId = @EmpId
		,@AnnualLeaveBalance = @AnnualLeaveBalance_old
		,@KeepInViewBalance = @KeepInViewBalance_old
		,@CanOT = @CanOT
		,@SpecialOT = @SpecialOT
		,@IsLocal = @IsLocal
		,@WorkingDays = @WorkingDays
		,@WorkingHours = @WorkingHours
		,@WorkingHoursByWeek = @WorkingHoursByWeek
		,@CompanyDate = @CompanyDate
		,@CountryCode = @CountryCode
		,@IsDefaultSchedule = @IsDefaultSchedule_old
		,@IsTopManager = @IsTopManager
		,@OffDaysbyWeek = @OffDaysbyWeek
		,@IsExpatUseHoliday = @IsExpatUseHoliday
		,@MilitaryFromDate = @MilitaryFromDate
		,@MilitaryToDate = @MilitaryToDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	SET @xml = (
			SELECT Code AS '@Code'
				,emp.DomainUsername AS '@DomainUsername'
				,FullName AS '@FullName'
				,Nickname AS '@Nickname'
				,DOB AS '@DOB'
				,emp.Email AS '@Email'
				,JobTitleId AS '@JobTitleId'
				,JobLevelId AS '@JobLevelId'
				,ManagementLevelId AS '@ManagementLevelId'
				,emp.[Enabled] AS '@Enabled'
				,emp.isDeleted AS '@IsDeleted'
				,Phone AS '@Phone'
				,Gender AS '@Gender'
				,emp.GroupId AS '@TeamId'
				,JoinedDate AS '@JoinedDate'
				,emp.ConfirmedDate AS '@ConfirmedDate'
				,emp.ResignedDate AS '@ResignedDate'
				,ResignedReason AS '@ResignedReason'
				,Skype AS '@Skype'
				,LetsTalk AS '@LetsTalk'
				,Marriage AS '@Marriage'
				,PermanentAddress AS '@PermanentAddress'
				,CurrentAddress AS '@CurrentAddress'
				,IdCardNumber AS '@IdCardNumber'
				,PassportNo AS '@PassportNo'
				,PassportExp AS '@PassportExp'
				,ChineseName AS '@ChineseName'
				,PlaceOfbirth AS '@PlaceOfbirth'
				,Ethnic AS '@Ethnic'
				,[Native] AS '@Native'
				,Religion AS '@Religion'
				,DateOfIDCard AS '@DateOfIDCard'
				,PlaceOfIDCard AS '@PlaceOfIDCard'
				,HomePhone AS '@HomePhone'
				,Qualification AS '@Qualification'
				,Major AS '@Major'
				,EmployeeCard AS '@EmployeeCard'
				,PickupRemark AS '@PickupRemark'
				,PromotionDate AS '@PromotionDate'
				,TeamDate AS '@TeamDate'
				,@Roles AS '@RoleIds'
				,CanOT AS '@CanOT'
				,SpecialOT AS '@SpecialOT'
				,IsLocal AS '@IsLocal'
				,WorkingDays AS '@WorkingDays'
				,WorkingHours AS '@WorkingHours'
				,WorkingHoursByWeek AS '@WorkingHoursByWeek'
				,IsTopManager AS '@IsTopManager'
				,empsum.CompanyDate AS '@CompanyDate'
				,CountryCode AS '@CountryCode'
				,IsExpatUseHoliday AS '@IsExpatUseHoliday'
				,OffDaysbyWeek AS '@OffDaysbyWeek'
				,MilitaryFromDate AS '@MilitaryFromDate'
				,MilitaryToDate AS '@MilitaryToDate'
				,ce.SubCompanyId AS '@SubCompanyId'
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
				INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON emp.EmpId = ce.EmpId
			WHERE emp.EmpId = @EmpId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.ActivityLog_Insert @UserId = @ModifiedBy
		,@ObjectTypeId = 1
		,@ObjectId = @EmpId
		,@OldData = @logOldData
		,@Data = @logData
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Employee_Insert_CheckExist] @Nickname NVARCHAR(20)
	,@Code VARCHAR(20)
	,@CompanyId SMALLINT
AS
/*
Created: 20250306@Tim
Task: check code, nickname available
DB: HRM
Original:

Revisions:
    20250306@Tim: initial - [Redmine: #217976]
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId_exist INT
		,@Nickname_exist NVARCHAR(20)
		,@FullName_exist NVARCHAR(50)

	SELECT TOP 1 @EmpId_exist = e.EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = Nickname
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = e.EmpId 
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
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = E.EmpId 
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

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/
ALTER PROCEDURE [dbo].[Report_ShiftStatistic] @SupervisorId INT
	,@fromDate DATE
	,@toDate DATE
	,@TeamIds VARCHAR(MAX) = ''
	,@isShowWholeCompany BIT = 0
	,@isLocal BIT = NULL
	,@companyId INT
AS
/*
Created: Tim.Trinh@2019-11-12
Task: shift statistic report
DB: HRM_SMS
Original:
Revisions:
	- 20250619@Henry: fix ambiguous column error migrate nex
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)

	IF ISNULL(@TeamIds, '') <> ''
		INSERT INTO @tblGroupId (GroupId)
		SELECT value
		FROM STRING_SPLIT(@TeamIds, ',')
	--ELSE
	--BEGIN
	--	INSERT INTO @tblGroupId (GroupId)
	--	SELECT GroupId
	--	FROM dbo.[Group] WITH (NOLOCK)
	--	WHERE IsDeleted = 0
	--END

	IF @isShowWholeCompany = 0
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT es.OwnerId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.OwnerId = emp.EmpId
			INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @companyId
		WHERE es.ActionType = 4
			AND es.EmpId = @SupervisorId
			AND emp.[Enabled] = 1
			AND ISNULL(es.IsDeleted, 0) = 0
		
		UNION
		SELECT emp.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
			INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @companyId
		WHERE gs.OwnerId = @SupervisorId
			AND gs.ActionType = 4
			AND emp.[Enabled] = 1
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				WHERE emp.EmpId = es.OwnerId
					AND es.ActionType = 4
					AND ISNULL(es.IsDeleted, 0) = 0
				)
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT emp.EmpId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @companyId
		WHERE emp.[Enabled] = 1
	END

	SELECT shift.ShiftId
		,shift.Code
		,empca.WorkDate
		,COUNT(1) AS Total
	FROM dbo.Shift AS shift WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON shift.ShiftId = empca.ShiftId
		AND ISNULL(empca.ShiftId, 0) <> 0
		AND empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND empca.IsLeaveDay = 0
		INNER JOIN @tblEmployeeId AS tblemp ON empca.EmpId = tblemp.EmpId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tblemp.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON tblemp.EmpId = empsum.EmpId
	WHERE (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND EXISTS (
			SELECT 1
			FROM @tblGroupId AS tblgr
			WHERE tblgr.GroupId = emp.GroupId
			)
	GROUP BY shift.ShiftId
		,shift.Code
		,empca.WorkDate
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER PROCEDURE [dbo].[Report_ShiftStatisticDetail] @WorkDate DATE
	,@ShiftId INT = NULL
	,@EmployeeId INT = NULL
	,@TeamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@SupervisorId INT = NULL
	,@IsShowWholeCompany BIT = 0
	,@CompanyId INT
AS
/*
Created: Tim.Trinh@2019-11-13
Task: shift statistic detail
DB: HRM_SMS
Original:
Revisions:
	- 20250619@Henry: fix ambiguous column error migrate nex
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupId (GroupId)
		SELECT value
		FROM STRING_SPLIT(@TeamIds, ',')
	END

	IF @isShowWholeCompany = 0
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT es.OwnerId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.OwnerId = emp.EmpId
			INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @CompanyId
		WHERE es.ActionType = 4
			AND es.EmpId = @SupervisorId
			AND emp.[Enabled] = 1
			AND ISNULL(es.IsDeleted, 0) = 0
		
		UNION
		SELECT emp.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
			INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @CompanyId
		WHERE gs.OwnerId = @SupervisorId
			AND gs.ActionType = 4
			AND emp.[Enabled] = 1
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				WHERE emp.EmpId = es.OwnerId
					AND es.ActionType = 4
					AND ISNULL(es.IsDeleted, 0) = 0
				)
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT emp.EmpId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = emp.EmpId AND ce.CompanyId = @CompanyId
		WHERE emp.[Enabled] = 1
	END

	SELECT empca.WorkDate
		,emp.EmpId
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Team
		,shift.[Name] AS [Shift]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN @tblEmployeeId AS tblemp ON emp.EmpId = tblemp.EmpId
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND empca.WorkDate = @WorkDate
		AND ISNULL(empca.ShiftId, 0) <> 0
		AND empca.IsLeaveDay = 0
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.[Shift] AS shift WITH (NOLOCK) ON empca.ShiftId = shift.ShiftId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE (
			@employeeId IS NULL
			OR emp.EmpId = @employeeId
			)
		AND (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND (
			@ShiftId IS NULL
			OR ISNULL(empca.ShiftId, 0) = @ShiftId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblGroupId AS tblgr
			WHERE tblgr.GroupId = emp.GroupId
			)
END
GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/


CREATE OR ALTER PROCEDURE [dbo].[SyncEPA_EmployeeSupervisor_Get] @FromDate DATETIME = NULL
	,@CompanyId INT 
AS
/*
Created: 20150921@Quinn.Vo
Task: Get all employee supervisor from change date
DB: NexEPA
Original: 

Revisions:  
20160428@Razor.Nguyen: new way to syn data
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF(@FromDate IS NOT NULL)
	BEGIN
		SELECT @FromDate = DATEADD(HH, -1, @FromDate)
	END

	SELECT DISTINCT CASE WHEN own.OldEmpId IS NOT NULL THEN own.OldEmpId ELSE own.EmpId END AS OwnerId
		,CASE WHEN emp.OldEmpId IS NOT NULL THEN emp.OldEmpId ELSE emp.EmpId END AS EmpId 
		,ISNULL(r.[IsDeleted], 0) AS IsDeleted
		,m.ModifiedDate AS ModifiedDate
	FROM (
		SELECT [OwnerId]
			,es.[EmpId]
			,ISNULL(es.[IsDeleted], 0) AS IsDeleted
			,MAX(ModifiedDate) AS ModifiedDate
		FROM dbo.[EmployeeSupervisor] AS es WITH (NOLOCK)
		INNER JOIN dbo.CompanyEmployee AS ec WITH (NOLOCK) ON ec.EmpId = es.EmpId AND ec.CompanyId = @CompanyId
		WHERE ActionType IN (2, 3)
			AND (
				@FromDate IS NULL
				OR ModifiedDate > @FromDate
				)
		GROUP BY [OwnerId]
			,es.[EmpId]
			,es.[IsDeleted]
		) AS m
		INNER JOIN dbo.[EmployeeSupervisor] AS r WITH (NOLOCK) ON r.OwnerId = m.OwnerId
		AND r.EmpId = m.EmpId
		AND r.ModifiedDate = m.ModifiedDate
		AND r.IsDeleted = m.IsDeleted
		LEFT JOIN dbo.Employee AS own WITH (NOLOCK) ON own.EmpId = r.OwnerId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
		ORDER BY EmpId, OwnerId
END
GO



/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

ALTER   PROCEDURE [dbo].[Group_UnitScope_Get]
	@keyWord varchar(20) = null,
	@TeamId INT = NULL
AS
/*
Created: Tom.Vo@2013-10-21
Task: get group
DB: HRM

Revisions:
	20250310@Mira: filter by CompanyId [Redmine: #217976]
    20250403@Tim: remove section [Redmine: #222886]
	20250515@Jay: add years of service for extra leave [Redmine: #227166]
	20250701@Tim: Show Nexcel, Lumiplus, NexVision [Redmine: #225531]
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tmpGroup (
		GroupId INT PRIMARY KEY
		,NoOfMembers INT
		)

	CREATE TABLE #tmpGroupEmployees (
		GroupId INT,
		EmpId INT,
		PRIMARY KEY (GroupId, EmpId)
	)

	INSERT INTO #tmpGroupEmployees (GroupId, EmpId)
	SELECT DISTINCT emp.GroupId, emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON esum.EmpId = emp.EmpId
	WHERE emp.GroupId IS NOT NULL
		AND emp.isDeleted = 0 
		AND emp.[Enabled] = 1
		AND esum.IsLocal IS NOT NULL

	INSERT INTO #tmpGroupEmployees (GroupId, EmpId)
	SELECT DISTINCT ce.GroupId, ce.EmpId
	FROM dbo.CompanyEmployee AS ce WITH (NOLOCK)
	WHERE ce.GroupId IS NOT NULL
		AND ce.IsDeleted = 0 
		AND ce.[Enabled] = 1
		AND ce.IsSpecialUser = 0

	INSERT INTO #tmpGroup (
		GroupId
		,NoOfMembers
		)
	SELECT g.GroupId
		,ISNULL(COUNT(DISTINCT tge.EmpId), 0) AS NoOfMembers
	FROM dbo.[Group] AS g WITH (NOLOCK)
		LEFT JOIN #tmpGroupEmployees AS tge ON g.GroupId = tge.GroupId
	WHERE (
			 @TeamId IS NULL
			OR g.GroupId =  @TeamId
			)
		AND g.isDeleted != 1
	GROUP BY g.GroupId

	SELECT g.GroupId AS Id
		,GroupTeamId AS GroupTeamId
		,g.[Name] AS TeamName
		,g.Code AS TeamCode
		,g.[Rank]
		,tbl.NoOfMembers
		,g.PICEmpId
		,e.NickName as TeamPIC
		,cpic.[CompanyId] AS TeamPICCompanyId
		,cpic.[Name] AS TeamPICCompany
		,d.DepartmentId
        ,d.Code AS DepartmentCode
        ,d.[Name] AS DepartmentName
		,d.PICEmpId AS DepartmentPICId
		,ed.NickName AS DepartmentPIC
		,u.Code AS UnitCode
		,g.AccountableArea
		,g.CompanyId
		,c.[Name] AS CompanyName
		,g.MailGroup
		,g.LocalBasicAnnualLeaveBalance
		,g.LocalWorkingDays
		,g.LocalWorkingHours
		,g.LocalOffDaysbyWeek
		,g.LocalWorkingHoursByWeek
		,LocalYearsOfServiceForExtraLeave
		,g.ExpatBasicAnnualLeaveBalance
		,g.ExpatWorkingDays
		,g.ExpatWorkingHours
		,g.ExpatOffDaysbyWeek
		,g.ExpatWorkingHoursByWeek
		,ExpatYearsOfServiceForExtraLeave
		,g.IsOutsource
	FROM dbo.[Group] AS g WITH (NOLOCK)
		INNER JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = g.PICEmpId
		LEFT JOIN dbo.CompanyEmployee AS gpic WITH (NOLOCK) ON gpic.EmpId = e.EmpId AND ISNULL(gpic.isDeleted, 0) = 0 AND ISNULL(gpic.[Enabled], 1) = 1 AND ISNULL(gpic.IsSpecialUser, 0) = 0
		INNER JOIN dbo.Unit AS u WITH (NOLOCK) ON u.UnitId = d.UnitId
		LEFT JOIN dbo.Employee AS ed WITH (NOLOCK) ON ed.EmpId = d.PICEmpId
		LEFT JOIN dbo.CompanyEmployee AS cepic WITH (NOLOCK) ON cepic.EmpId = g.PICEmpId AND ISNULL(cepic.isDeleted, 0) = 0 AND ISNULL(cepic.[Enabled], 1) = 1 AND ISNULL(cepic.IsSpecialUser, 0) = 0
		LEFT JOIN dbo.Company AS cpic WITH (NOLOCK) ON cepic.CompanyId = cpic.CompanyId
		LEFT JOIN #tmpGroup AS tbl ON g.GroupId = tbl.GroupId
		LEFT JOIN dbo.Company AS c ON g.CompanyId = c.CompanyId
	WHERE (
			 @TeamId IS NULL
			OR g.GroupId =  @TeamId
			)
		AND g.isDeleted != 1
		AND (
			 @keyWord IS NULL
			 OR LOWER(g.[Name]) like LOWER('%' + @keyWord + '%') 
			 OR LOWER(g.Code) like LOWER('%' + @keyWord + '%') 
			)
	ORDER BY d.Code

    DROP TABLE #tmpGroup
    DROP TABLE #tmpGroupEmployees
END

GO

/*<info serverAlias="bodb_HRM" executers="wsv_hrm" isFunction="0"></info>*/

CREATE OR ALTER PROCEDURE [dbo].[Employee_GroupScope_Get] 
	@query NVARCHAR(256) = NULL,
	@teamIds VARCHAR(MAX) = '',
	@enable BIT = NULL,
	@isDelete BIT = NULL,
	@pageNumber INT = 1,
	@pageSize INT = NULL,
	@totalRow INT = NULL OUTPUT,
	@titleId INT = NULL,
	@companyIds VARCHAR(MAX) = ''
AS
/*
	Created: 20250429@Jay.Do
	Task: get employees in group space 
	DB: bodb_HRM

	Revisions:
        20250507@Jay.Do: remove dynamic sql
        20250627@Hank.Nguyen: check GroupId in both Employee and CompanyEmployee tables
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupsList TABLE (GroupId INT)

	INSERT INTO @GroupsList (GroupId)
	SELECT value
	FROM STRING_SPLIT(@teamIds, ',')

	DECLARE @CompaniesList TABLE (CompanyId INT)

	INSERT INTO @CompaniesList (CompanyId)
	SELECT value
	FROM STRING_SPLIT(@companyIds, ',')

	DECLARE @Result TABLE (
		CtrId INT,
		EmpId INT,
		EmployeeCode NVARCHAR(50),
		NickName NVARCHAR(100),
		JobTitleId INT,
		[Enabled] BIT,
		JoinedDate VARCHAR(50),
		ResignedDate VARCHAR(50),
		isDeleted BIT,
		FullName NVARCHAR(200),
		IsLocal BIT,
		IsSpecial BIT,
		CompanyDate VARCHAR(50),
		JobTitle NVARCHAR(100),
		CompanyId INT,
		Company NVARCHAR(200),
		UnitId INT,
		UnitCode NVARCHAR(50),
		UnitPIC NVARCHAR(100),
		TeamId INT,
		TeamCode NVARCHAR(50),
		TeamName NVARCHAR(200),
		TeamPIC NVARCHAR(100),
		DepartmentId INT,
		DepartmentCode NVARCHAR(50),
		DepartmentName NVARCHAR(200),
		DepartmentPIC NVARCHAR(100),
		ManagementLevel NVARCHAR(100),
		JobLevel NVARCHAR(100)
	)

	INSERT INTO @Result
	SELECT 
		e.CtrId,
		e.EmpId,
		e.Code AS EmployeeCode,
		e.NickName,
		e.JobTitleId,
		ISNULL(ce.[Enabled], e.[Enabled]) AS [Enabled],
		e.JoinedDate,
		ISNULL(ce.ResignedDate, e.ResignedDate) AS ResignedDate,
		ISNULL(ce.isDeleted, e.isDeleted) AS isDeleted,
		e.FullName,
		s.IsLocal,
		s.IsTopManager AS IsSpecial,
		ISNULL(ce.CompanyDate, s.CompanyDate) AS CompanyDate,
		COALESCE(ejt.[Name], jt.[Name]) AS JobTitle,
		c.CompanyId,
		c.[Name] AS Company,
		u.GroupUnitId AS UnitId,
		u.Code AS UnitCode,
		eu.NickName AS UnitPIC,
		g.GroupId AS TeamId,
		g.Code AS TeamCode,
		g.[Name] AS TeamName,
		eg.NickName AS TeamPIC,
		d.DepartmentId,
		d.Code AS DepartmentCode,
		d.[Name] AS DepartmentName,
		ed.NickName AS DepartmentPIC,
		COALESCE(mg.[Name], ml.[Name]) AS ManagementLevel,
		jg.[Name] AS JobLevel
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.CompanyEmployee ce WITH (NOLOCK) ON ce.EmpId = e.EmpId AND (ce.IsSpecialUser IS NULL OR ce.IsSpecialUser = 0)
		LEFT JOIN dbo.EmployeeSummary AS s WITH (NOLOCK) ON s.EmpId = e.EmpId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON COALESCE(e.GroupID, ce.GroupId) = g.GroupID
		LEFT JOIN dbo.[Department] AS d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN dbo.[Unit] AS u WITH (NOLOCK) ON d.UnitId = u.UnitId
		LEFT JOIN dbo.[Lookup] l WITH (NOLOCK) ON l.Code = 'JT'
		LEFT JOIN dbo.LookupData jt WITH (NOLOCK) ON l.Id = jt.LookupId AND e.JobTitleId = jt.Id
		LEFT JOIN HRM.dbo.JobTitle ejt WITH (NOLOCK) ON ejt.JobTitleId = e.JobTitleId AND ejt.IsDeleted = 0
		LEFT JOIN dbo.Company c WITH (NOLOCK) ON c.CompanyId = ce.CompanyId
		LEFT JOIN Employee eu WITH (NOLOCK) ON eu.EmpId = u.PICEmpId
		LEFT JOIN Employee ed WITH (NOLOCK) ON ed.EmpId = d.PICEmpId
		LEFT JOIN Employee eg WITH (NOLOCK) ON eg.EmpId = g.PICEmpId
		LEFT JOIN HRM.dbo.EmployeeManagementGrade em WITH (NOLOCK) ON em.EmpId = e.EmpId AND em.IsCurrentUsed = 1 AND ISNULL(em.IsDeleted, 0) = 0
		LEFT JOIN HRM.dbo.ManagementGrade mg WITH (NOLOCK) ON mg.ManagementGradeId = em.ManagementGradeId AND ISNULL(mg.IsDeleted, 0) = 0
		LEFT JOIN ManagementLevel ml WITH (NOLOCK) ON e.ManagementLevelId = ml.ManagementLevelId
		LEFT JOIN HRM.dbo.EmployeeJobGrade ejg WITH (NOLOCK) ON ejg.EmpId = e.EmpId AND ejg.IsCurrentUsed = 1 AND ISNULL(ejg.IsDeleted, 0) = 0
		LEFT JOIN JobLevel jg WITH (NOLOCK) ON COALESCE(e.JobLevelId, ejg.JobGradeId) = jg.JobLevelId 
	WHERE 
		(@enable IS NULL OR (ISNULL(ISNULL(ce.isDeleted, e.isDeleted), 0) = 0 AND COALESCE(ce.Enabled, e.Enabled) = @enable))
		AND (@isDelete IS NULL OR ISNULL(ISNULL(ce.isDeleted, e.isDeleted), 0) = @isDelete)
		AND (@query IS NULL OR e.NickName LIKE '%' + @query + '%')
		AND (ISNULL( @teamIds, '') = ''  OR COALESCE(ce.GroupId, e.GroupId) IN (SELECT GroupId FROM @GroupsList))
		AND (ISNULL( @companyIds, '') = '' OR ce.CompanyId IN (SELECT CompanyId FROM @CompaniesList))

	

	SELECT @totalRow = COUNT(*) FROM @Result

	-- Final result
	IF @pageSize IS NULL OR @pageSize <= 0
	BEGIN
		SELECT * FROM @Result
		ORDER BY UnitCode DESC, DepartmentCode DESC, TeamCode DESC
	END
	ELSE
	BEGIN
		SELECT * FROM @Result
		ORDER BY UnitCode DESC, DepartmentCode DESC, TeamCode DESC
		OFFSET (@pageNumber - 1) * @pageSize ROWS
		FETCH NEXT @pageSize ROWS ONLY
	END
END
GO