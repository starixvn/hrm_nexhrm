-- Drop temporary table if it exists
IF OBJECT_ID('tempdb..#GroupEmployeeMapping') IS NOT NULL
    DROP TABLE #GroupEmployeeMapping;
GO

IF OBJECT_ID('tempdb..#TempCompanyEmployee') IS NOT NULL
    DROP TABLE #TempCompanyEmployee;
GO

CREATE TABLE #GroupEmployeeMapping (
    CompanyId INT,
    OldGroupId INT,
    NewGroupTeamId INT,
    NewDeptCode VARCHAR(50),
    PICNickName VARCHAR(50),
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
    CompanyDate smalldatetime NULL,
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
SELECT @NexVisionCompanyId = CompanyId FROM [bodb_HRM].[dbo].[Company] WITH (NOLOCK) WHERE Name = 'NexVision';

INSERT INTO #GroupEmployeeMapping (CompanyId, OldGroupId, NewGroupTeamId, NewDeptCode, PICNickName)
VALUES 
    -- NexHRM Atomix
    (1, 1063, 300000500, 'NX27', 'Troy'),  -- NexOps
    (1, 1078, 300000501, 'NX27', 'Larry'),  -- NexPE

    -- NexHRM Expat
    (1, 40, 300000503, 'IH03', 'Sean'), -- Core Team - IH
    (1, 99999, 300000504, 'CT88', 'Yves'), -- Core Team - SRM

    -- NexHRM Core Team
    (1, 1081, 300000502, 'NX01', 'Thien'),  -- Core Team
    -- NexHRM GM Office
    (1, 1070, 300000506, 'NX66', 'Dan'),  -- Data
    (1, 1080, 300000507, 'NX66', 'Julien'),  -- NovaStar
    (1, 1082, 300000508, 'NX66', 'Eric'),  -- OptiGo
    -- NexHRM Omni Flow Network
    (1, 2, 300000510, 'NX03', 'Jacky'),  -- IT Network
    (1, 1083, 300000511, 'NX03', 'Dominic'),  -- Pontus
    (1, 59, 300000512, 'NX03', 'Paladin'), -- SAS
    -- NexHRM Pioneers
    (1, 1089, 300000513, 'NX18', 'Alvin'), -- Forge
    (1, 1087, 300000514, 'NX18', 'Jerry'), -- Zeal
    (1, 1099, 300000517, 'NX18', 'Kiwi'), -- Core Team - Pioneers
    (1, 1090, 300000515, 'NX18', 'Elodie'), -- Pionex
    (1, 1088, 300000516, 'NX18', 'Ethan'), -- Robustus
    -- NexHRM Subarashi
    (1, 1091, 300000518, 'NX06', 'Candy'), -- Americano
    (1, 1094, 300000519, 'NX06', 'Lena'), -- Cocktail
    (1, 1100, 300000520, 'NX06', 'Ben'), -- Core Team - Subarashi
    (1, 1093, 300000521, 'NX06', 'William'), -- Latte
    (1, 1092, 300000522, 'NX06', 'Candy'), -- Shake
    -- NexHRM Synergy
    (1, 1101, 300000523, 'NX99', 'Richard'), -- Core Team - Synergy
    (1, 1084, 300000524, 'NX99', ''), -- SPO
    (1, 1086, 300000525, 'NX99', 'Jacob'), -- R-Pulse
    (1, 1085, 300000526, 'NX99', 'Kaka'), -- T-Pulse
    (1, 1103, 300000527, 'NX99', 'Helen'), -- V-Pulse
    -- NexHRM Wabi Sabi
    (1, 1098, 300000529, 'NX45', 'Ronny'), -- D.A
    (1, 1102, 300000528, 'NX45', 'Ronny'), -- Core Team - Wabi Sabi
    (1, 1096, 300000530, 'NX45', 'Long'), -- S.A.I
    (1, 1097, 300000531, 'NX45', 'Ronny'), -- Sentinels
    (1, 1095, 300000532, 'NX45', 'Terry'), -- SWAT
    -- NexHRM HR&ADM
    (1, 1104, 300000509, 'SR36', 'Cindy'), -- HRA NEX
    -- NexHRM Finance
    (1, 1, 300000505, 'FI01', 'Steven'), -- Finance
    -- LumiHRM Pentagon
    (2, 2018, 300000534, 'NX69', 'Anders'),  -- AscentisTech
    (2, 1, 300000535, 'NX69', 'Anders'),  -- Core Team - Pentagon
    (2, 2014, 300000536, 'NX69', 'TonyNg'),  -- Paradigm
    (2, 2016, 300000537, 'NX69', 'Nancy'),  -- Paragon
    (2, 2019, 300000538, 'NX69', 'Anders'),  -- Pathfinder
    (2, 2017, 300000539, 'NX69', 'April'),  -- Pixel
    (2, 2015, 300000540, 'NX69', 'Liz'),  -- Polaris
    (2, 2013, 300000541, 'NX69', 'Amy'),  -- Prime
    -- LumiHRM Special 
    (2, 3, 300000509, 'SR36', 'Cindy'), -- HRA NEX
    (2, 2, 300000502, 'NX01', 'Thien'),  -- Core Team
    (2, 2021, 300000505, 'FI01', 'Steven'), -- Finance
    -- NexVisionHRM Marketing
    (@NexVisionCompanyId, 4, 300000544, 'MW11', 'Dylan'),  -- Momentum
    (@NexVisionCompanyId, 5, 300000542, 'MW11', 'Paul'),  -- Horizon
    (@NexVisionCompanyId, 6, 300000543, 'MW11', 'Kat'),  -- Magnetic
    -- NexVisionHRM Special 
    (@NexVisionCompanyId, 1, 300000509, 'SR36', 'Cindy'), -- HRA NEX
    (@NexVisionCompanyId, 2, 300000502, 'NX01', 'Thien'),  -- Core Team
    (@NexVisionCompanyId, 3, 300000505, 'FI01', 'Steven'); -- Finance

DECLARE @DeletedGroupNexcel INT;
DECLARE @DeletedGroupLumiplus INT;
SELECT @DeletedGroupNexcel = GroupId FROM [bodb_HRM].[dbo].[Group] WITH (NOLOCK) WHERE Name = 'Other' AND CompanyId = 1;
SELECT @DeletedGroupLumiplus = GroupId FROM [bodb_HRM].[dbo].[Group] WITH (NOLOCK) WHERE Name = 'Other' AND CompanyId = 2;

-- Insert data from NexHRM (CompanyId = 1)
INSERT INTO #TempCompanyEmployee
SELECT 
    ee.Id,
    1 as CompanyId, -- NexHRM
    ee.DomainUsername,
    ee.Email,
    CASE 
        WHEN g.GroupId IS NULL THEN @DeletedGroupNexcel 
        ELSE g.GroupId 
    END,
    ee.Roles,
    ee.JoinedDate AS CompanyDate,
    ee.ConfirmedDate,
    ee.ResignedDate,
    ee.NickName2,
    ee.IsSpecialUser,
    ISNULL(ee.IsDeleted, 0),
    ee.Enabled,
    ee.SalaryGroupId,
    ee.Password
FROM [NexHRM].[dbo].[EmployeeExt] ee WITH (NOLOCK)
LEFT JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gem.CompanyId = 1 AND gem.OldGroupId = ee.GroupId
LEFT JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
LEFT JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId IN (1, 2, @NexVisionCompanyId) AND g.IsDeleted = 0;

-- Insert data from LumiHRM (CompanyId = 2)
INSERT INTO #TempCompanyEmployee
SELECT 
    ee.Id,
    2 as CompanyId, -- LumiHRM
    ee.DomainUsername,
    ee.Email,
    CASE 
        WHEN g.GroupId IS NULL THEN @DeletedGroupLumiplus 
        ELSE g.GroupId 
    END,
    ee.Roles,
    ee.JoinedDate AS CompanyDate,
    ee.ConfirmedDate,
    ee.ResignedDate,
    ee.NickName2,
    ee.IsSpecialUser,
    ISNULL(ee.IsDeleted, 0),
    ee.Enabled,
    ee.SalaryGroupId,
    ee.Password
FROM [LumiHRM].[dbo].[EmployeeExt] ee WITH (NOLOCK)
LEFT JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gem.CompanyId = 2 AND gem.OldGroupId = ee.GroupId
LEFT JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
LEFT JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId IN (1, 2, @NexVisionCompanyId) AND g.IsDeleted = 0;

-- Insert data from NexVisionHRM (CompanyId = 3)
INSERT INTO #TempCompanyEmployee
SELECT 
    ee.Id,
    @NexVisionCompanyId as CompanyId, -- NexVisionHRM
    ee.DomainUsername,
    ee.Email,
    g.GroupId,
    ee.Roles,
    ee.JoinedDate AS CompanyDate,
    ee.ConfirmedDate,
    ee.ResignedDate,
    ee.NickName2,
    ee.IsSpecialUser,
    ISNULL(ee.IsDeleted, 0),
    ee.Enabled,
    ee.SalaryGroupId,
    ee.Password
FROM [NexVisionHRM].[dbo].[EmployeeExt] ee WITH (NOLOCK)
LEFT JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gem.CompanyId = @NexVisionCompanyId AND gem.OldGroupId = ee.GroupId
LEFT JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
LEFT JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId IN (1, 2, @NexVisionCompanyId) AND g.IsDeleted = 0;

-- Insert into final table
INSERT INTO [bodb_HRM].[dbo].[CompanyEmployee]
(
   [EmpId],
   [CompanyId],
   [DomainUsername],
   [Email],
   [GroupId],
   [Roles],
   [CompanyDate],
   [ConfirmedDate],
   [ResignedDate],
   [NickName2],
   [IsSpecialUser],
   [IsDeleted],
   [Enabled],
   [SalaryGroupId],
   [Password]
)
SELECT 
   EmpId,
   CompanyId,
   DomainUsername,
   Email,
   GroupId,
   Roles,
   CompanyDate,
   ConfirmedDate,
   ResignedDate,
   NickName2,
   IsSpecialUser,
   IsDeleted,
   Enabled,
   SalaryGroupId,
   Password
FROM #TempCompanyEmployee
WHERE EmpId <> 0;

-- Update NexHRM employee group mappings
UPDATE ege
SET ege.GroupId = g.GroupId
FROM [NexHRM].[dbo].[EmployeeGroup_Employee] ege WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON ege.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
WHERE gem.CompanyId = 1;

-- Update LumiHRM employee group mappings
UPDATE ege
SET ege.GroupId = g.GroupId
FROM [LumiHRM].[dbo].[EmployeeGroup_Employee] ege WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON ege.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
WHERE gem.CompanyId = 2;

-- Update NexVisionHRM employee group mappings
UPDATE ege
SET ege.GroupId = g.GroupId
FROM [NexVisionHRM].[dbo].[EmployeeGroup_Employee] ege WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON ege.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
WHERE gem.CompanyId = @NexVisionCompanyId;

-- Delete unmapped records
DELETE FROM [NexHRM].[dbo].[EmployeeGroup_Employee]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 1
);

DELETE FROM [LumiHRM].[dbo].[EmployeeGroup_Employee]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 2
);

DELETE FROM [NexVisionHRM].[dbo].[EmployeeGroup_Employee]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = @NexVisionCompanyId
);

-- Update NexHRM GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [NexHRM].[dbo].[GroupSupervisor] gs WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 1
WHERE gem.CompanyId = 1;

-- Update LumiHRM GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [LumiHRM].[dbo].[GroupSupervisor] gs WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 2
WHERE gem.CompanyId = 2;

-- Update NexVisionHRM GroupSupervisor mappings
UPDATE gs
SET gs.GroupId = g.GroupId
FROM [NexVisionHRM].[dbo].[GroupSupervisor] gs WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON gs.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = @NexVisionCompanyId
WHERE gem.CompanyId = @NexVisionCompanyId;

-- Delete unmapped GroupSupervisor records from NexHRM
DELETE FROM [NexHRM].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 1
);

-- Delete unmapped GroupSupervisor records from LumiHRM
DELETE FROM [LumiHRM].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = 2
);

-- Delete unmapped GroupSupervisor records from NexVisionHRM
DELETE FROM [NexVisionHRM].[dbo].[GroupSupervisor]
WHERE GroupId NOT IN (
    SELECT g.GroupId
    FROM #GroupEmployeeMapping gem WITH (NOLOCK)
    INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
    INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId
    WHERE gem.CompanyId = @NexVisionCompanyId
);

-- Update NexHRM HistoryWorkDetail
UPDATE hwd
SET hwd.GroupName = g.Name
FROM [NexHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [NexHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId;

UPDATE hwd
SET hwd.GroupName = g.Name
FROM [LumiHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [LumiHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId;

UPDATE hwd
SET hwd.GroupName = g.Name
FROM [NexVisionHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [NexVisionHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId;

UPDATE hwd
SET hwd.GroupId = g.GroupId
FROM [NexHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON hwd.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 1
WHERE gem.CompanyId = 1;

UPDATE hwd
SET hwd.GroupId = g.GroupId
FROM [LumiHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON hwd.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = 2
WHERE gem.CompanyId = 2;

UPDATE hwd
SET hwd.GroupId = g.GroupId
FROM [NexVisionHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON hwd.GroupId = gem.OldGroupId
INNER JOIN [bodb_HRM].[dbo].[Department] d WITH (NOLOCK) ON d.Code = gem.NewDeptCode
INNER JOIN [bodb_HRM].[dbo].[Group] g WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId AND g.DepartmentId = d.DepartmentId AND g.CompanyId = @NexVisionCompanyId
WHERE gem.CompanyId = @NexVisionCompanyId;

-- Update PIC
UPDATE g
SET g.PICEmpId = e.Id
FROM [bodb_HRM].[dbo].[Group] g WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId
INNER JOIN [NexHRM].[dbo].[Employee] e WITH (NOLOCK) ON e.NickName = gem.PICNickName AND e.IsDeleted = 0 AND e.Enabled = 1
WHERE gem.CompanyId = 1 AND g.CompanyId IN (1, 2, @NexVisionCompanyId);

UPDATE g
SET g.PICEmpId = e.Id
FROM [bodb_HRM].[dbo].[Group] g WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId
INNER JOIN [LumiHRM].[dbo].[Employee] e WITH (NOLOCK) ON e.NickName = gem.PICNickName AND e.IsDeleted = 0 AND e.Enabled = 1
WHERE gem.CompanyId = 2 AND g.CompanyId IN (1, 2, @NexVisionCompanyId);

UPDATE g
SET g.PICEmpId = e.Id
FROM [bodb_HRM].[dbo].[Group] g WITH (NOLOCK)
INNER JOIN #GroupEmployeeMapping gem WITH (NOLOCK) ON g.GroupTeamId = gem.NewGroupTeamId
INNER JOIN [NexVisionHRM].[dbo].[Employee] e WITH (NOLOCK) ON e.NickName = gem.PICNickName AND e.IsDeleted = 0 AND e.Enabled = 1
WHERE gem.CompanyId = @NexVisionCompanyId AND g.CompanyId IN (1, 2, @NexVisionCompanyId);

UPDATE [bodb_HRM].[dbo].[CompanyEmployee] 
SET Roles = '4'
WHERE CompanyId = 1 AND DomainUsername like '%henry.dinh%';

-- Clean up
DROP TABLE #TempCompanyEmployee;
GO

DROP TABLE #GroupEmployeeMapping;
GO