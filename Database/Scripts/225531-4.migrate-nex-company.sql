-- Drop temporary table if it exists
IF OBJECT_ID('tempdb..#GroupEmployeeMapping') IS NOT NULL
    DROP TABLE #GroupEmployeeMapping;
GO

CREATE TABLE #GroupEmployeeMapping (
    CompanyId INT,
    OldGroupId INT,
    NewGroupName VARCHAR(50),
    NewDeptCode VARCHAR(50)
);
GO

-- Create temp table to consolidate data from all three databases
CREATE TABLE #TempCompanyEmployee (
    EmpId int NOT NULL,
    CompanyId int NOT NULL,
    DomainUsername varchar(50) NULL,
    Email varchar(50) NULL,
    GroupId int NULL,
    Roles varchar(20) NULL,
    JoinedDate smalldatetime NULL,
    ConfirmedDate smalldatetime NULL,
    ResignedDate smalldatetime NULL,
    NickName2 nvarchar(20) NULL,
    IsSpecialUser bit NULL,
    IsDeleted bit NULL,
    Enabled bit NULL,
    SalaryGroupId smallint NULL,
    Password nvarchar(100) NULL
)
GO

DECLARE @NexVisionCompanyId INT;
SELECT @NexVisionCompanyId = CompanyId FROM [bodb_HRM_bk].[dbo].[Company] WITH (NOLOCK) WHERE Name = 'NexVision';

INSERT INTO #GroupEmployeeMapping (CompanyId, OldGroupId, NewGroupName, NewDeptCode)
VALUES 
    -- NexHRM Atomix
    (1, 1063, 'NEXOPS', 'NX27'),  -- NexOps
    (1, 1078, 'NEXPE', 'NX27'),  -- NexPE
    -- NexHRM Expat
    (1, 40, 'Expat', 'NX66'), -- Expat
    -- NexHRM Core Team
    (1, 1081, 'Core Team', 'NX01'),  -- Core Team
    -- NexHRM GM Office
    (1, 1070, 'Data', 'NX66'),  -- Data
    (1, 1080, 'NovaStar', 'NX66'),  -- NovaStar
    (1, 1082, 'OptiGo', 'NX66'),  -- OptiGo
    -- NexHRM Omni Flow Network
    (1, 2, 'IT Network', 'NX03'),  -- IT Network
    (1, 51, 'Pontus', 'NX03'),  -- Pontus
    (1, 59, 'SAS', 'NX03'), -- SAS
    -- NexHRM Pioneers
    (1, 1089, 'Forge', 'NX18'), -- Forge
    (1, 1087, 'PPO', 'NX18'), -- PPO
    (1, 1099, 'HOD Pioneers', 'NX18'), -- HOD Pioneers
    (1, 1090, 'Pionex', 'NX18'), -- Pionex
    (1, 1088, 'Robustus', 'NX18'), -- Robustus
    -- NexHRM Subarashi
    (1, 1091, 'Americano', 'NX06'), -- Americano
    (1, 1094, 'Cocktail', 'NX06'), -- Cocktail
    (1, 1100, 'HOD Subarashi', 'NX06'), -- HOD Subarashi
    (1, 1093, 'Latte', 'NX06'), -- Latte
    (1, 1092, 'Shake', 'NX06'), -- Shake
    -- NexHRM Synergy
    (1, 1101, 'HOD Synergy', 'NX99'), -- HOD Synergy
    (1, 1084, 'SPO', 'NX99'), -- SPO
    (1, 1086, 'R-Pulse', 'NX99'), -- R-Pulse
    (1, 1085, 'T-Pulse', 'NX99'), -- T-Pulse
    (1, 1103, 'V-Pulse', 'NX99'), -- V-Pulse
    -- NexHRM Wabi Sabi
    (1, 1098, 'D.A', 'NX45'), -- D.A
    (1, 1102, 'HOD Wabi Sabi', 'NX45'), -- HOD Wabi Sabi
    (1, 1096, 'S.A.I', 'NX45'), -- S.A.I
    (1, 1097, 'Sentinels', 'NX45'), -- Sentinels
    (1, 1095, 'SWAT', 'NX45'), -- SWAT
    -- NexHRM HR&ADM
    (1, 1104, 'HRA NEX', 'SR36'), -- HR&ADM
    -- NexHRM Finance
    (1, 1, 'Finance - HMA', 'FI01'), -- Finance
    -- LumiHRM Pentagon
    (2, 2018, 'AscentisTech', 'NX69'),  -- AscentisTech
    (2, 1, 'HOD Pentagon', 'NX69'),  -- HOD Pentagon
    (2, 2014, 'Paradigm', 'NX69'),  -- Paradigm
    (2, 2016, 'Paragon', 'NX69'),  -- Paragon
    (2, 2019, 'Pathfinder', 'NX69'),  -- Pathfinder
    (2, 2017, 'Pixel', 'NX69'),  -- Pixel
    (2, 2015, 'Polaris', 'NX69'),  -- Polaris
    (2, 2013, 'Prime', 'NX69'),  -- Prime
    -- LumiHRM Special 
    (2, 3, 'HRA NEX', 'SR36'), -- HR&ADM
    (2, 2, 'Core Team', 'NX01'),  -- Core Team
    (2, 2021, 'Finance - HMA', 'FI01'), -- Finance
    -- NexVisionHRM Marketing
    (@NexVisionCompanyId, 4, 'Momentum', 'MW11'),  -- Momentum
    (@NexVisionCompanyId, 5, 'Horizon', 'MW11'),  -- Horizon
    (@NexVisionCompanyId, 6, 'Magnetic', 'MW11'),  -- Magnetic
    -- NexVisionHRM Special 
    (@NexVisionCompanyId, 1, 'HRA NEX', 'SR36'), -- HR&ADM
    (@NexVisionCompanyId, 2, 'Core Team', 'NX01'),  -- Core Team
    (@NexVisionCompanyId, 3, 'Finance - HMA', 'FI01') -- Finance

-- Insert data from NexHRM (CompanyId = 1)
-- INSERT INTO #TempCompanyEmployee
-- SELECT 
--     ee.Id,
--     1 as CompanyId, -- NexHRM
--     ee.DomainUsername,
--     ee.Email,
--     g.GroupId,
--     ee.Roles,
--     ee.JoinedDate,
--     ee.ConfirmedDate,
--     ee.ResignedDate,
--     ee.NickName2,
--     ee.IsSpecialUser,
--     ISNULL(ee.IsDeleted, 0),
--     ee.Enabled,
--     ee.SalaryGroupId,
--     ee.Password
-- FROM [NexHRM_bk].[dbo].[EmployeeExt] ee WITH (NOLOCK)
-- LEFT JOIN #GroupEmployeeMapping ON #GroupEmployeeMapping.CompanyId = 1 AND #GroupEmployeeMapping.OldGroupId = ee.GroupId
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Department] d WITH (NOLOCK) ON d.Code = #GroupEmployeeMapping.NewDeptCode
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Group] g WITH (NOLOCK) ON g.Name = #GroupEmployeeMapping.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 1

-- -- Insert data from LumiHRM (CompanyId = 2)
-- INSERT INTO #TempCompanyEmployee
-- SELECT 
--     ee.Id,
--     2 as CompanyId, -- LumiHRM
--     ee.DomainUsername,
--     ee.Email,
--     g.GroupId,
--     ee.Roles,
--     ee.JoinedDate,
--     ee.ConfirmedDate,
--     ee.ResignedDate,
--     ee.NickName2,
--     ee.IsSpecialUser,
--     ISNULL(ee.IsDeleted, 0),
--     ee.Enabled,
--     ee.SalaryGroupId,
--     ee.Password
-- FROM [LumiHRM_bk].[dbo].[EmployeeExt] ee WITH (NOLOCK)
-- LEFT JOIN #GroupEmployeeMapping ON #GroupEmployeeMapping.CompanyId = 2 AND #GroupEmployeeMapping.OldGroupId = ee.GroupId
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Department] d WITH (NOLOCK) ON d.Code = #GroupEmployeeMapping.NewDeptCode
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Group] g WITH (NOLOCK) ON g.Name = #GroupEmployeeMapping.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 2

-- -- Insert data from NexVisionHRM (CompanyId = 3)
-- INSERT INTO #TempCompanyEmployee
-- SELECT 
--     ee.Id,
--     @NexVisionCompanyId as CompanyId, -- NexVisionHRM
--     ee.DomainUsername,
--     ee.Email,
--     g.GroupId,
--     ee.Roles,
--     ee.JoinedDate,
--     ee.ConfirmedDate,
--     ee.ResignedDate,
--     ee.NickName2,
--     ee.IsSpecialUser,
--     ISNULL(ee.IsDeleted, 0),
--     ee.Enabled,
--     ee.SalaryGroupId,
--     ee.Password
-- FROM [NexVisionHRM_bk].[dbo].[EmployeeExt] ee WITH (NOLOCK)
-- LEFT JOIN #GroupEmployeeMapping ON #GroupEmployeeMapping.CompanyId = @NexVisionCompanyId AND #GroupEmployeeMapping.OldGroupId = ee.GroupId
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Department] d WITH (NOLOCK) ON d.Code = #GroupEmployeeMapping.NewDeptCode
-- LEFT JOIN [bodb_HRM_bk].[dbo].[Group] g WITH (NOLOCK) ON g.Name = #GroupEmployeeMapping.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = @NexVisionCompanyId

-- Insert into final table
-- INSERT INTO [bodb_HRM_bk].[dbo].[CompanyEmployee]
-- (
--    [EmpId],
--    [CompanyId],
--    [DomainUsername],
--    [Email],
--    [GroupId],
--    [Roles],
--    [JoinedDate],
--    [ConfirmedDate],
--    [ResignedDate],
--    [NickName2],
--    [IsSpecialUser],
--    [IsDeleted],
--    [Enabled],
--    [SalaryGroupId],
--    [Password]
-- )
-- SELECT 
--    EmpId,
--    CompanyId,
--    DomainUsername,
--    Email,
--    GroupId,
--    Roles,
--    JoinedDate,
--    ConfirmedDate,
--    ResignedDate,
--    NickName2,
--    IsSpecialUser,
--    IsDeleted,
--    Enabled,
--    SalaryGroupId,
--    Password
-- FROM #TempCompanyEmployee

-- -- Update NexHRM_bk employee group mappings
-- UPDATE ege
-- SET ege.GroupId = g.GroupId
-- FROM [NexHRM_bk].[dbo].[EmployeeGroup_Employee] ege
-- INNER JOIN #GroupEmployeeMapping gem ON ege.GroupId = gem.OldGroupId
-- INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
-- INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
-- WHERE gem.CompanyId = 1;

-- -- Update LumiHRM_bk employee group mappings
-- UPDATE ege
-- SET ege.GroupId = g.GroupId
-- FROM [LumiHRM_bk].[dbo].[EmployeeGroup_Employee] ege
-- INNER JOIN #GroupEmployeeMapping gem ON ege.GroupId = gem.OldGroupId
-- INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
-- INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
-- WHERE gem.CompanyId = 2;

-- -- Update NexVisionHRM employee group mappings
-- UPDATE ege
-- SET ege.GroupId = g.GroupId
-- FROM [NexVisionHRM_bk].[dbo].[EmployeeGroup_Employee] ege
-- INNER JOIN #GroupEmployeeMapping gem ON ege.GroupId = gem.OldGroupId
-- INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
-- INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
-- WHERE gem.CompanyId = @NexVisionCompanyId;

-- Delete unmapped records
-- DELETE FROM [NexHRM_bk].[dbo].[EmployeeGroup_Employee]
-- WHERE GroupId NOT IN (
--     SELECT g.GroupId
--     FROM #GroupEmployeeMapping gem
--     INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
--     INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
--     WHERE gem.CompanyId = 1
-- );

-- DELETE FROM [LumiHRM_bk].[dbo].[EmployeeGroup_Employee]
-- WHERE GroupId NOT IN (
--     SELECT g.GroupId
--     FROM #GroupEmployeeMapping gem
--     INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
--     INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
--     WHERE gem.CompanyId = 2
-- );

-- DELETE FROM [NexVisionHRM_bk].[dbo].[EmployeeGroup_Employee]
-- WHERE GroupId NOT IN (
--     SELECT g.GroupId
--     FROM #GroupEmployeeMapping gem
--     INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
--     INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
--     WHERE gem.CompanyId = @NexVisionCompanyId
-- );


-- Update NexHRM_bk GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [NexHRM_bk].[dbo].[GroupSupervisor] gs
INNER JOIN #GroupEmployeeMapping gem ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 1
WHERE gem.CompanyId = 1;

-- Update LumiHRM_bk GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [LumiHRM_bk].[dbo].[GroupSupervisor] gs
INNER JOIN #GroupEmployeeMapping gem ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 2
WHERE gem.CompanyId = 2;

-- Update NexVisionHRM_bk GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [NexVisionHRM_bk].[dbo].[GroupSupervisor] gs
INNER JOIN #GroupEmployeeMapping gem ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId AND g.CompanyId = @NexVisionCompanyId
WHERE gem.CompanyId = @NexVisionCompanyId;

-- Delete unmapped GroupSupervisor records from NexHRM_bk
DELETE FROM [NexHRM_bk].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem
    INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 1
);

-- Delete unmapped GroupSupervisor records from LumiHRM_bk
DELETE FROM [LumiHRM_bk].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem
    INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 2
);

-- Delete unmapped GroupSupervisor records from NexVisionHRM_bk
DELETE FROM [NexVisionHRM_bk].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem
    INNER JOIN [bodb_HRM_bk].[dbo].[Department] d ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM_bk].[dbo].[Group] g ON g.Name = gem.NewGroupName AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = @NexVisionCompanyId
);

-- Clean up
DROP TABLE #TempCompanyEmployee
GO