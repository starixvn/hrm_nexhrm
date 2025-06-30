-- Map SubCompany
DECLARE @NexcelCompanyId INT = 1;
SET IDENTITY_INSERT [bodb_HRM].[dbo].[SubCompany] ON;
INSERT INTO [bodb_HRM].[dbo].[SubCompany] (SubCompanyId, CompanyId, [Name], CountryCode)
VALUES
	(8, @NexcelCompanyId, 'DAR', 'VN'),
	(9, @NexcelCompanyId, 'SRM', 'VN');
SET IDENTITY_INSERT [bodb_HRM].[dbo].[SubCompany] OFF;

DECLARE @DARCompanyId INT = 8;
UPDATE ce
SET SubCompanyId = @DARCompanyId
FROM [bodb_HRM].[dbo].[CompanyEmployee] ce WITH (NOLOCK)
WHERE ce.DomainUsername like '%sean%';

DECLARE @SRMCompanyId INT = 9;
UPDATE ce
SET SubCompanyId = @SRMCompanyId
FROM [bodb_HRM].[dbo].[CompanyEmployee] ce WITH (NOLOCK)
WHERE ce.DomainUsername like '%yves%';

GO

-- Insert missing EGrade + MGrade
INSERT INTO [bodb_HRM].[dbo].[JobLevel] ([Name], [IsDeleted], [ModifiedBy], [ModifiedDate])
SELECT 
    jg.Code,
    1 AS IsDeleted,
    NULL AS ModifiedBy,
    GETDATE() AS ModifiedDate
FROM [HRM].[dbo].[JobGrade] jg WITH (NOLOCK)
LEFT JOIN [bodb_HRM].[dbo].[JobLevel] jl WITH (NOLOCK) ON jg.Code = jl.Name
WHERE jl.JobLevelId IS NULL
GO

INSERT INTO [bodb_HRM].[dbo].[ManagementLevel] ([Name], [IsDeleted], [ModifiedBy], [ModifiedDate])
SELECT 
    mg.Code,
    1 AS IsDeleted,
    NULL AS ModifiedBy,
    GETDATE() AS ModifiedDate
FROM [HRM].[dbo].[ManagementGrade] mg WITH (NOLOCK)
LEFT JOIN [bodb_HRM].[dbo].[ManagementLevel] ml WITH (NOLOCK) ON mg.Code = ml.Name
WHERE ml.ManagementLevelId IS NULL
GO

ALTER TABLE [HRM].[dbo].[JobGrade] ADD OldJobGradeId INT NULL, NewJobGradeId INT NULL;
ALTER TABLE [HRM].[dbo].[ManagementGrade] ADD OldManagementGradeId INT NULL, NewManagementGradeId INT NULL;

-- Map JobLevel → JobGrade
UPDATE jg
SET jg.NewJobGradeId = jl.JobLevelId
FROM [HRM].[dbo].[JobGrade] jg
INNER JOIN [bodb_HRM].[dbo].[JobLevel] jl ON jl.Name = jg.Code

-- Map ManagementLevel → ManagementGrade
UPDATE mg
SET mg.NewManagementGradeId = ml.ManagementLevelId
FROM [HRM].[dbo].[ManagementGrade] mg
INNER JOIN [bodb_HRM].[dbo].[ManagementLevel] ml ON ml.Name = mg.Code


ALTER TABLE [HRM].[dbo].[JobGrade] DROP CONSTRAINT FK_JobGrade_YourConstraint;
ALTER TABLE [HRM].[dbo].[JobGrade] ALTER COLUMN JobGradeId INT NULL;
GO

ALTER TABLE [HRM].[dbo].[ManagementGrade] DROP CONSTRAINT FK_ManagementGrade_YourConstraint;
ALTER TABLE [HRM].[dbo].[ManagementGrade] ALTER COLUMN ManagementGradeId INT NULL;
GO

ALTER TABLE [HRM].[dbo].[JobGrade] DROP CONSTRAINT PK_JobGrade;
GO

ALTER TABLE [HRM].[dbo].[JobGrade] DROP COLUMN JobGradeId;
GO

ALTER TABLE [HRM].[dbo].[ManagementGrade] DROP CONSTRAINT PK_ManagementGrade;
GO

ALTER TABLE [HRM].[dbo].[ManagementGrade] DROP COLUMN ManagementGradeId;
GO

EXEC sp_rename 'JobGrade.NewJobGradeId', 'JobGradeId', 'COLUMN';
GO

EXEC sp_rename 'ManagementGrade.NewManagementGradeId', 'ManagementGradeId', 'COLUMN';
GO

ALTER TABLE [HRM].[dbo].[JobGrade] ALTER COLUMN JobGradeId INT NOT NULL;
GO

ALTER TABLE [HRM].[dbo].[ManagementGrade] ALTER COLUMN ManagementGradeId INT NOT NULL;
GO

ALTER TABLE [HRM].[dbo].[JobGrade] ADD CONSTRAINT PK_JobGrade PRIMARY KEY (JobGradeId);
GO

ALTER TABLE [HRM].[dbo].[ManagementGrade] ADD CONSTRAINT PK_ManagementGrade PRIMARY KEY (ManagementGradeId);
GO

-- Update EmployeeJobGrade
UPDATE ejg
SET ejg.JobGradeId = jg.JobGradeId
FROM [HRM].[dbo].[EmployeeJobGrade] ejg
JOIN [HRM].[dbo].[JobGrade] jg ON ejg.JobGradeId = jg.OldJobGradeId

UPDATE ejg
SET ejg.OldJobGradeId = jg.JobGradeId
FROM [HRM].[dbo].[EmployeeJobGrade] ejg
JOIN [HRM].[dbo].[JobGrade] jg ON ejg.OldJobGradeId = jg.OldJobGradeId

-- Update ManagementGrade
UPDATE emg
SET emg.ManagementGradeId = mg.ManagementGradeId
FROM [HRM].[dbo].[EmployeeManagementGrade] emg
JOIN [HRM].[dbo].[ManagementGrade] mg ON emg.ManagementGradeId = mg.OldManagementGradeId

UPDATE emg
SET emg.OldManagementGradeId = mg.ManagementGradeId
FROM [HRM].[dbo].[EmployeeManagementGrade] emg
JOIN [HRM].[dbo].[ManagementGrade] mg ON emg.OldManagementGradeId = mg.OldManagementGradeId

-- Update HistoryWorkDetail
-- NexHRM
UPDATE hwd
SET hwd.JobGradeId = jg.JobGradeId
FROM [NexHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[JobGrade] jg ON hwd.JobGradeId = jg.OldJobGradeId

UPDATE hwd
SET hwd.ManagementGradeId = mg.ManagementGradeId
FROM [NexHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[ManagementGrade] mg ON hwd.ManagementGradeId = mg.OldManagementGradeId

-- LumiHRM
UPDATE hwd
SET hwd.JobGradeId = jg.JobGradeId
FROM [LumiHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[JobGrade] jg ON hwd.JobGradeId = jg.OldJobGradeId

UPDATE hwd
SET hwd.ManagementGradeId = mg.ManagementGradeId
FROM [LumiHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[ManagementGrade] mg ON hwd.ManagementGradeId = mg.OldManagementGradeId

-- NexVisionHRM
UPDATE hwd
SET hwd.JobGradeId = jg.JobGradeId
FROM [NexvisionHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[JobGrade] jg ON hwd.JobGradeId = jg.OldJobGradeId

UPDATE hwd
SET hwd.ManagementGradeId = mg.ManagementGradeId
FROM [NexvisionHRM].[dbo].[HistoryWorkDetail] hwd
JOIN [HRM].[dbo].[ManagementGrade] mg ON hwd.ManagementGradeId = mg.OldManagementGradeId
