-- Insert Company if needed
IF NOT EXISTS (SELECT 1 FROM [bodb_HRM].[dbo].[Company] WHERE Name = 'NexVision')
BEGIN
    INSERT INTO [bodb_HRM].[dbo].[Company] ([Name], [CountryCode], [IsActive])
    VALUES ('NexVision', 'VN', 1);
END;

DECLARE @NexHRMCompanyId INT = 1;
DECLARE @LumiHRMCompanyId INT = 2;
DECLARE @NexVisionHRMCompanyId INT;

SELECT @NexVisionHRMCompanyId = CompanyId 
FROM [bodb_HRM].[dbo].[Company] WITH (NOLOCK)
WHERE Name = 'NexVision';

-- ✅ Insert Groups by JOINing directly to Department table
INSERT INTO [bodb_HRM].[dbo].[Group]
    ([Code], [Name], [CreatedDate], [CompanyId], [DepartmentId], [GroupTeamId], [AccountableArea], [OldGroupId], [IsDeleted])
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
    s.OldGroupId,
    s.IsDeleted
FROM 
    (VALUES
        -- Code, Name, Company, DeptCode, AccountableArea, OldGroupId, IsDeleted
        -- NexVisionHRM Marketing 
        ('', 'Momentum', 'NexVisionHRM', 'MW11', N'NX Marketing Momentum', 4, 0),
        ('C77', 'Horizon', 'NexVisionHRM', 'MW11', N'NX Marketing Horizon', 5, 0),
        ('C88', 'Magnetic', 'NexVisionHRM', 'MW11', N'NX Marketing Magnetic', 6, 0),
        -- NexHRM Finance 
        ('FI1', 'Finance - HMA', 'NexHRM', 'FI01', N'Finance SU01', 1, 0),
        -- NexHRM Atomix 
        ('O13', 'NEXOPS', 'NexHRM', 'NX27', N'NX NEXOPS', 1063, 0),
        ('E13', 'NEXPE', 'NexHRM', 'NX27', N'NX NEXPE', 1078, 0),
        -- NexHRM Core Team
        ('CT1', 'Core Team', 'NexHRM', 'NX01', N'NX CORE TEAM', 1081, 0),
        -- NexHRM Expat
        ('E13', 'Expat', 'NexHRM', 'NX66', N'NX EXPAT', 40, 0),
        -- NexHRM GM Office 
        ('D13', 'Data', 'NexHRM', 'NX66', N'NX DATA', 1070, 0),
        ('S13', 'NovaStar', 'NexHRM', 'NX66', N'NX Novastar', 1080, 0),
        ('HR3', 'OptiGo', 'NexHRM', 'NX66', N'NX OptiGo', 1082, 0),
        -- NexHRM Omni Flow Network
        ('TS3', 'IT Network', 'NexHRM', 'NX03', N'NX IT Network', 2, 0),
        ('EV3', 'Pontus', 'NexHRM', 'NX03', N'NX Pontus', 1083, 0),
        ('S13', 'SAS', 'NexHRM', 'NX03', N'NX SAS', 59, 0),
        -- NexHRM Pioneers
        ('P13', 'Forge', 'NexHRM', 'NX18', N'NX FORGE', 1089, 0),
        ('P13', 'PPO', 'NexHRM', 'NX18', N'NX PPO', 1087, 0),
        ('TT3', 'HOD Pioneers', 'NexHRM', 'NX18', N'NX HOD PIONEERS', 1099, 0),
        ('EL3', 'Pionex', 'NexHRM', 'NX18', N'NX PIONEX', 1090, 0),
        ('R13', 'Robustus', 'NexHRM', 'NX18', N'NX ROBUTUS', 1088, 0),
        -- NexHRM Subarashi
        ('B13', 'Americano', 'NexHRM', 'NX06', N'NX AMERICANO', 1091, 0),
        ('S13', 'Cocktail', 'NexHRM', 'NX06', N'NX COCKTAIL', 1094, 0),
        ('G13', 'HOD Subarashi', 'NexHRM', 'NX06', N'NX HOD SUBARASHI', 1100, 0),
        ('P13', 'Latte', 'NexHRM', 'NX06', N'NX LATTE', 1093, 0),
        ('T13', 'Shake', 'NexHRM', 'NX06', N'NX SHAKE', 1092, 0),
        -- NexHRM Synergy
        ('S13', 'HOD Synergy', 'NexHRM', 'NX99', N'NX HOD SYNERGY', 1101, 0), 
        ('S13', 'SPO', 'NexHRM', 'NX99', N'NX SPO', 1084, 0),
        ('O13', 'R-Pulse', 'NexHRM', 'NX99', N'NX R-PULSE', 1086, 0),
        ('I13', 'T-Pulse', 'NexHRM', 'NX99', N'NX T-PULSE', 1085, 0),
        ('V13', 'V-Pulse', 'NexHRM', 'NX99', N'NX V-PULSE', 1103, 0),
        -- NexHRM Wabi Sabi
        ('S13', 'D.A', 'NexHRM', 'NX45', N'NX D.A', 1098, 0),
        ('W13', 'HOD Wabi Sabi', 'NexHRM', 'NX45', N'NX HOD WABI SABI', 1102, 0),
        ('GV3', 'S.A.I', 'NexHRM', 'NX45', N'NX S.A.I', 1096, 0),
        ('MG3', 'Sentinels', 'NexHRM', 'NX45', N'NX SENTINELS', 1097, 0),
        ('T13', 'SWAT', 'NexHRM', 'NX45', N'NX SWAT', 1095, 0),
        -- NexHRM HR&ADM
        ('', 'HRA NEX', 'NexHRM', 'SR36', N'NX HR&ADM', 1104, 0),
        -- LumiHRM Pentagon
        ('AC1', 'AscentisTech', 'LumiHRM', 'NX69', N'NX AscentisTech', 2018, 0),
        ('CT1', 'HOD Pentagon', 'LumiHRM', 'NX69', N'NX HOD Pentagon', 1, 0),
        ('D23', 'Paradigm', 'LumiHRM', 'NX69', N'NX PARADIGM', 2014, 0),
        ('P13', 'Paragon', 'LumiHRM', 'NX69', N'NX PARAGON', 2016, 0),
        ('', 'Pathfinder', 'LumiHRM', 'NX69', N'NX PATHFINDER', 2019, 0),
        ('', 'Pixel', 'LumiHRM', 'NX69', N'NX PIXEL', 2017, 0),
        ('M13', 'Polaris', 'LumiHRM', 'NX69', N'NX POLARIS', 2015, 0),
        ('V13', 'Prime', 'LumiHRM', 'NX69', N'NX PRIME', 2013, 0),
        -- LumiHRM Rad
        ('NX5', 'RAID - NX', 'LumiHRM', 'XX55', N'RAID NX', NULL, 0),
        -- Deleted Group
        ('DL1', 'Deleted Group Nexcel', 'NexHRM', 'NX66', N'NX Deleted Group Nexcel', NULL, 1),
        ('DL2', 'Deleted Group Lumiplus', 'LumiHRM', 'NX69', N'NX Deleted Group Lumiplus', NULL, 1),
    ) AS s (Code, Name, Company, DeptCode, AccountableArea, OldGroupId, IsDeleted)
JOIN 
    [bodb_HRM].[dbo].[Department] d 
    ON d.Code = s.DeptCode;
GO
