USE LumiHRM
GO

DECLARE @EmployeeId INT = 6131
        ,@AffectedDate DATE = '2025-07-11'
        ,@IsSpecialUser BIT = 1
        ,@FromDomain VARCHAR(20) = N'spvn\'
        ,@ToDomain VARCHAR(20) = N'lumiplus\'
        ,@FromDomainEmail VARCHAR(20) = N'nexcel.com'
        ,@ToDomainEmail VARCHAR(20) = N'lumiplus.vn'
        ,@ToCompanyId INT = 2
        ,@FromCompanyId INT = 1

IF EXISTS (SELECT 1 FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
BEGIN
    UPDATE a WITH (UPDLOCK, ROWLOCK)
        SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
        ,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
        ,a.IsSpecialUser = @IsSpecialUser
        ,a.IsDeleted = 0
        ,a.Enabled = 1
        ,a.GroupId = b.GroupId
        ,a.Roles = b.Roles
        ,a.CompanyDate = @AffectedDate
        ,a.ConfirmedDate = @AffectedDate
        ,a.SalaryGroupId = b.SalaryGroupId
		,a.CompanyId = @ToCompanyId
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
		,CompanyId
    )
    SELECT 
        @EmployeeId
        ,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
        ,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
        ,@IsSpecialUser
        ,0
        ,1
        ,GroupId
        ,Roles
        ,@AffectedDate
        ,@AffectedDate
        ,SalaryGroupId
		,@ToCompanyId
    FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
    WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
END

INSERT INTO LumiHRM.dbo.EmployeeGroup_Employee (GroupId, EmpID, isMainGroup)
SELECT GroupId, @EmployeeId, 1
FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
WHERE EmpId = @EmployeeId AND CompanyId = @FromCompanyId
