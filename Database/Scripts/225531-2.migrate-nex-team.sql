-- Insert Company if needed
IF NOT EXISTS (SELECT 1 FROM [bodb_HRM_bk].[dbo].[Company] WHERE Name = 'NexVision')
BEGIN
    INSERT INTO [bodb_HRM_bk].[dbo].[Company] ([Name], [CountryCode], [IsActive])
    VALUES ('NexVision', 'VN', 1);
END;

DECLARE @NexHRMCompanyId INT = 1;
DECLARE @LumiHRMCompanyId INT = 2;
DECLARE @NexVisionHRMCompanyId INT;

SELECT @NexVisionHRMCompanyId = CompanyId 
FROM [bodb_HRM_bk].[dbo].[Company] WITH (NOLOCK)
WHERE Name = 'NexVision';

-- ✅ Insert Groups by JOINing directly to Department table
INSERT INTO [bodb_HRM_bk].[dbo].[Group]
    ([Code], [Name], [CreatedDate], [CompanyId], [DepartmentId], [GroupTeamId], [AccountableArea], [OldGroupId])
SELECT
    s.Code,
    s.Name,
    GETDATE(),
    CASE s.Company WHEN 'NexHRM' THEN @NexHRMCompanyId
                   WHEN 'NexVisionHRM' THEN @NexVisionHRMCompanyId
                   WHEN 'LumiHRM' THEN @LumiHRMCompanyId END,
    d.DepartmentId,
    111111,
    s.AccountableArea,
    s.OldGroupId
FROM 
    (VALUES
        -- Code, Name, Company, DeptCode, AccountableArea, OldGroupId
        -- NexVisionHRM Marketing 
        ('', 'Momentum', 'NexVisionHRM', 'MW11', N'NX Marketing Momentum', 4),
        ('C77', 'Horizon', 'NexVisionHRM', 'MW11', N'NX Marketing Horizon', 5),
        ('C88', 'Magnetic', 'NexVisionHRM', 'MW11', N'NX Marketing Magnetic', 6),
        -- NexHRM Finance 
        ('FI1', 'Finance - HMA', 'NexHRM', 'FI01', N'Finance SU01', 1),
        -- NexHRM Atomix 
        ('O13', 'NEXOPS', 'NexHRM', 'NX27', N'NX NEXOPS', 1063),
        ('E13', 'NEXPE', 'NexHRM', 'NX27', N'NX NEXPE', 1078),
        -- NexHRM Core Team
        ('CT1', 'Core Team', 'NexHRM', 'NX01', N'NX CORE TEAM', 1081),
        -- NexHRM Expat
        ('E13', 'Expat', 'NexHRM', 'NX66', N'NX EXPAT', 40),
        -- NexHRM GM Office 
        ('D13', 'Data', 'NexHRM', 'NX66', N'NX DATA', 1070),
        ('S13', 'NovaStar', 'NexHRM', 'NX66', N'NX Novastar', 1080),
        ('HR3', 'OptiGo', 'NexHRM', 'NX66', N'NX OptiGo', 1082),
        -- NexHRM Omni Flow Network
        ('TS3', 'IT Network', 'NexHRM', 'NX03', N'NX IT Network', 2),
        ('EV3', 'Pontus', 'NexHRM', 'NX03', N'NX Pontus', 51),
        ('S13', 'SAS', 'NexHRM', 'NX03', N'NX SAS', 59),
        -- NexHRM Pioneers
        ('P13', 'Forge', 'NexHRM', 'NX18', N'NX FORGE', 1089),
        ('P13', 'PPO', 'NexHRM', 'NX18', N'NX PPO', 1087),
        ('TT3', 'HOD Pioneers', 'NexHRM', 'NX18', N'NX HOD PIONEERS', 1099),
        ('EL3', 'Pionex', 'NexHRM', 'NX18', N'NX PIONEX', 1090),
        ('R13', 'Robustus', 'NexHRM', 'NX18', N'NX ROBUTUS', 1088),
        -- NexHRM Subarashi
        ('B13', 'Americano', 'NexHRM', 'NX06', N'NX AMERICANO', 1091),
        ('S13', 'Cocktail', 'NexHRM', 'NX06', N'NX COCKTAIL', 1094),
        ('G13', 'HOD Subarashi', 'NexHRM', 'NX06', N'NX HOD SUBARASHI', 1100),
        ('P13', 'Latte', 'NexHRM', 'NX06', N'NX LATTE', 1093),
        ('T13', 'Shake', 'NexHRM', 'NX06', N'NX SHAKE', 1092),
        -- NexHRM Synergy
        ('S13', 'HOD Synergy', 'NexHRM', 'NX99', N'NX HOD SYNERGY', 1101), 
        ('S13', 'SPO', 'NexHRM', 'NX99', N'NX SPO', 1084),
        ('O13', 'R-Pulse', 'NexHRM', 'NX99', N'NX R-PULSE', 1086),
        ('I13', 'T-Pulse', 'NexHRM', 'NX99', N'NX T-PULSE', 1085),
        ('V13', 'V-Pulse', 'NexHRM', 'NX99', N'NX V-PULSE', 1103),
        -- NexHRM Wabi Sabi
        ('S13', 'D.A', 'NexHRM', 'NX45', N'NX D.A', 1098),
        ('W13', 'HOD Wabi Sabi', 'NexHRM', 'NX45', N'NX HOD WABI SABI', 1102),
        ('GV3', 'S.A.I', 'NexHRM', 'NX45', N'NX S.A.I', 1096),
        ('MG3', 'Sentinels', 'NexHRM', 'NX45', N'NX SENTINELS', 1097),
        ('T13', 'SWAT', 'NexHRM', 'NX45', N'NX SWAT', 1095),
        -- NexHRM HR&ADM
        ('', 'HRA NEX', 'NexHRM', 'SR36', N'NX HR&ADM', 1104),
        -- LumiHRM Pentagon
        ('AC1', 'AscentisTech', 'LumiHRM', 'NX69', N'NX AscentisTech', 2018),
        ('CT1', 'HOD Pentagon', 'LumiHRM', 'NX69', N'NX HOD Pentagon', 1),
        ('D23', 'Paradigm', 'LumiHRM', 'NX69', N'NX PARADIGM', 2014),
        ('P13', 'Paragon', 'LumiHRM', 'NX69', N'NX PARAGON', 2016),
        ('', 'Pathfinder', 'LumiHRM', 'NX69', N'NX PATHFINDER', 2019),
        ('', 'Pixel', 'LumiHRM', 'NX69', N'NX PIXEL', 2017),
        ('M13', 'Polaris', 'LumiHRM', 'NX69', N'NX POLARIS', 2015),
        ('V13', 'Prime', 'LumiHRM', 'NX69', N'NX PRIME', 2013),
        -- LumiHRM Rad
        ('NX5', 'RAID - NX', 'LumiHRM', 'XX55', N'RAID NX', NULL)
    ) AS s (Code, Name, Company, DeptCode, AccountableArea, OldGroupId)
JOIN 
    [bodb_HRM_bk].[dbo].[Department] d 
    ON d.Code = s.DeptCode;
GO
