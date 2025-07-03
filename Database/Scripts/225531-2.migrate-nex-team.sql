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
    s.GroupTeamId,
    s.AccountableArea,
    s.OldGroupId,
    s.IsDeleted
FROM 
    (VALUES
        -- GroupTeamId, Code, Name, Company, DeptCode, AccountableArea, OldGroupId, IsDeleted
        -- NexHRM Teams
        -- Atomix
        (300000500, 'O13', 'NEXOPS', 'NexHRM', 'NX27', N'NX NEXOPS', 1063, 0),
        (300000501, 'E13', 'NEXPE', 'NexHRM', 'NX27', N'NX NEXPE', 1078, 0), 

        -- Core Team
        (300000502, 'CT1', 'Core Team', 'NexHRM', 'NX01', N'NX CORE TEAM', 1081, 0),

        -- Expat
        (300000503, 'CT1', 'Core Team - IH', 'NexHRM', 'IH03', N'Core Team SU03', 40, 0), 
        (300000504, 'CT1', 'Core Team - SRM', 'NexHRM', 'CT88', N'Core Team - SRM', NULL, 0), 

        -- FI
        (300000505, 'FI1', 'Finance', 'NexHRM', 'FI01', N'Finance SU01', 1, 0),

        -- GM Office
        (300000506, 'D13', 'Data', 'NexHRM', 'NX66', N'NX DATA', 1070, 0),
        (300000507, 'S13', 'NovaStar', 'NexHRM', 'NX66', N'NX Novastar', 1080, 0), 
        (300000508, 'HR3', 'OptiGo', 'NexHRM', 'NX66', N'NX OptiGo', 1082, 0), 

        -- HR
        (300000509, 'HM7', 'HRA NEX', 'NexHRM', 'SR36', N'SU04 HRA NEX', NULL, 0),

        -- OmniFlow
        (300000510, 'TS3', 'IT Network', 'NexHRM', 'NX03', N'NX IT Network', 2, 0),
        (300000511, 'EV3', 'Pontus', 'NexHRM', 'NX03', N'NX Production DBA', 1083, 0), 
        (300000512, 'S13', 'SAS', 'NexHRM', 'NX03', N'NX SAS', 59, 0), 
    
        -- Pioneers
        (300000513, 'P13', 'Forge', 'NexHRM', 'NX18', N'NX FORGE', 1089, 0),
        (300000514, 'P13', 'Zeal', 'NexHRM', 'NX18', N'NX PPO', 1087, 0), 
        (300000515, 'EL3', 'Pionex', 'NexHRM', 'NX18', N'NX PIONEX', 1090, 0), 
        (300000516, 'R13', 'Robustus', 'NexHRM', 'NX18', N'NX ROBUTUS', 1088, 0),
        (300000517, 'CT1', 'Core Team - Pioneers', 'NexHRM', 'NX18', N'NX HOD PIONEERS', 1099, 0), 

        -- Subarashi
        (300000518, 'B13', 'Americano', 'NexHRM', 'NX06', N'NX AMERICANO', 1091, 0),
        (300000519, 'S13', 'Cocktail', 'NexHRM', 'NX06', N'NX COCKTAIL', 1094, 0), 
        (300000520, 'CT1', 'Core Team - Subarashi', 'NexHRM', 'NX06', N'NX HOD SUBARASHI', 1100, 0), 
        (300000521, 'P13', 'Latte', 'NexHRM', 'NX06', N'NX LATTE', 1093, 0), 
        (300000522, 'T13', 'Shake', 'NexHRM', 'NX06', N'NX SHAKE', 1092, 0), 

        -- Synergy
        (300000523, 'CT1', 'Core Team - Synergy', 'NexHRM', 'NX99', N'NX HOD SYNERGY', 1101, 0),
        (300000524, 'S13', 'SPO', 'NexHRM', 'NX99', N'NX SPO', 1084, 0),
        (300000525, 'O13', 'R-Pulse', 'NexHRM', 'NX99', N'NX R-PULSE', 1086, 0),
        (300000526, 'I13', 'T-Pulse', 'NexHRM', 'NX99', N'NX T-PULSE', 1085, 0), 
        (300000527, 'V13', 'V-Pulse', 'NexHRM', 'NX99', N'NX V-PULSE', 1103, 0), 

        -- Wabi Sabi
        (300000528, 'CT1', 'Core Team - Wabi Sabi', 'NexHRM', 'NX45', N'NX HOD WABI SABI', 1102, 0),
        (300000529, 'S13', 'D.A', 'NexHRM', 'NX45', N'NX D.A', 1098, 0),
        (300000530, 'GV3', 'S.A.I', 'NexHRM', 'NX45', N'NX S.A.I', 1096, 0),
        (300000531, 'MG3', 'Sentinels', 'NexHRM', 'NX45', N'NX SENTINELS', 1097, 0),
        (300000532, 'T13', 'SWAT', 'NexHRM', 'NX45', N'NX SWAT', 1095, 0),

        -- RAID
        (300000533, 'NX5', 'RAID - NX', 'NexHRM', 'XX55', N'RAID NX', NULL, 0),
        
        -- LumiHRM Teams: Pentagon
        (300000534, 'AC1', 'AscentisTech', 'LumiHRM', 'NX69', N'NX AscentisTech', 2018, 0),
        (300000535, 'CT1', 'Core Team - Pentagon', 'LumiHRM', 'NX69', N'NX HOD Pentagon', 1, 0),
        (300000536, 'D23', 'Paradigm', 'LumiHRM', 'NX69', N'NX PARADIGM', 2014, 0),
        (300000537, 'P13', 'Paragon', 'LumiHRM', 'NX69', N'NX PARAGON', 2016, 0),
        (300000538, 'F13', 'Pathfinder', 'LumiHRM', 'NX69', N'NX PATHFINDER', 2019, 0),
        (300000539, 'X13', 'Pixel', 'LumiHRM', 'NX69', N'NX PIXEL', 2017, 0),
        (300000540, 'M13', 'Polaris', 'LumiHRM', 'NX69', N'NX POLARIS', 2015, 0),
        (300000541, 'V13', 'Prime', 'LumiHRM', 'NX69', N'NX PRIME', 2013, 0),
        
        -- NexVisionHRM Teams: Marketing
        (300000542, 'C77', 'Horizon', 'NexVisionHRM', 'MW11', N'NX Marketing Horizon', 5, 0),
        (300000543, 'C88', 'Magnetic', 'NexVisionHRM', 'MW11', N'NX Marketing Magnetic', 6, 0),
        (300000544, 'C89', 'Momentum', 'NexVisionHRM', 'MW11', N'NX Marketing Momentum', 4, 0),

         -- Deleted Group
        (NULL, 'Other', 'Other', 'NexHRM', 'Other', N'NX Other', NULL, 1),
        (NULL, 'Other', 'Other', 'LumiHRM', 'Other', N'NX Other', NULL, 1)
    ) AS s (GroupTeamId, Code, Name, Company, DeptCode, AccountableArea, OldGroupId, IsDeleted)
JOIN 
    [bodb_HRM].[dbo].[Department] d 
    ON d.Code = s.DeptCode AND d.IsDeleted = 0;
GO
