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
        (300000151, 'O13', 'NEXOPS', 'NexHRM', 'NX27', N'NX NEXOPS', 1063, 0),
        (300000152, 'E13', 'NEXPE', 'NexHRM', 'NX27', N'NX NEXPE', 1078, 0), 

        -- Core Team
        (300000137, 'CT1', 'Core Team', 'NexHRM', 'NX01', N'NX CORE TEAM', 1081, 0),

        -- Expat
        (300000078, 'CT1', 'Core Team - IH', 'NexHRM', 'IH03', N'Core Team SU03', 40, 0), 
        (300000043, 'CT1', 'Core Team - SRM', 'NexHRM', 'CT88', N'Core Team - SRM', NULL, 0), 

        -- FI
        (300000050, 'FI1', 'Finance', 'NexHRM', 'FI01', N'Finance SU01', 1, 0),

        -- GM Office
        (300000140, 'D13', 'Data', 'NexHRM', 'NX66', N'NX DATA', 1070, 0),
        (300000163, 'S13', 'NovaStar', 'NexHRM', 'NX66', N'NX Novastar', 1080, 0), 
        (300000147, 'HR3', 'OptiGo', 'NexHRM', 'NX66', N'NX OptiGo', 1082, 0), 

        -- HR
        (300000279, 'HM7', 'HRA NEX', 'NexHRM', 'SR36', N'SU04 HRA NEX', NULL, 0),

        -- OmniFlow
        (300000169, 'TS3', 'IT Network', 'NexHRM', 'NX03', N'NX IT Network', 2, 0),
        (300000144, 'EV3', 'Pontus', 'NexHRM', 'NX03', N'NX Production DBA', 1083, 0), 
        (300000164, 'S13', 'SAS', 'NexHRM', 'NX03', N'NX SAS', 59, 0), 
    
        -- Pioneers
        (300000157, 'P13', 'Forge', 'NexHRM', 'NX18', N'NX FORGE', 1089, 0),
        (300000156, 'P13', 'Zeal', 'NexHRM', 'NX18', N'NX PPO', 1087, 0), 
        (300000143, 'EL3', 'Pionex', 'NexHRM', 'NX18', N'NX PIONEX', 1090, 0), 
        (300000161, 'R13', 'Robustus', 'NexHRM', 'NX18', N'NX ROBUTUS', 1088, 0),
        (300000171, 'CT1', 'Core Team - Pioneers', 'NexHRM', 'NX18', N'NX HOD PIONEERS', 1099, 0), 

        -- Subarashi
        (300000136, 'B13', 'Americano', 'NexHRM', 'NX06', N'NX AMERICANO', 1091, 0),
        (300000167, 'S13', 'Cocktail', 'NexHRM', 'NX06', N'NX COCKTAIL', 1094, 0), 
        (300000145, 'CT1', 'Core Team - Subarashi', 'NexHRM', 'NX06', N'NX HOD SUBARASHI', 1100, 0), 
        (300000159, 'P13', 'Latte', 'NexHRM', 'NX06', N'NX LATTE', 1093, 0), 
        (300000168, 'T13', 'Shake', 'NexHRM', 'NX06', N'NX SHAKE', 1092, 0), 

        -- Synergy
        (300000166, 'CT1', 'Core Team - Synergy', 'NexHRM', 'NX99', N'NX HOD SYNERGY', 1101, 0),
        (300000165, 'S13', 'SPO', 'NexHRM', 'NX99', N'NX SPO', 1084, 0),
        (300000154, 'O13', 'R-Pulse', 'NexHRM', 'NX99', N'NX R-PULSE', 1086, 0),
        (300000148, 'I13', 'T-Pulse', 'NexHRM', 'NX99', N'NX T-PULSE', 1085, 0), 
        (300000325, 'V13', 'V-Pulse', 'NexHRM', 'NX99', N'NX V-PULSE', 1103, 0), 

        -- Wabi Sabi
        (300000173, 'CT1', 'Core Team - Wabi Sabi', 'NexHRM', 'NX45', N'NX HOD WABI SABI', 1102, 0),
        (300000162, 'S13', 'D.A', 'NexHRM', 'NX45', N'NX D.A', 1098, 0),
        (300000146, 'GV3', 'S.A.I', 'NexHRM', 'NX45', N'NX S.A.I', 1096, 0),
        (300000150, 'MG3', 'Sentinels', 'NexHRM', 'NX45', N'NX SENTINELS', 1097, 0),
        (300000170, 'T13', 'SWAT', 'NexHRM', 'NX45', N'NX SWAT', 1095, 0),

        -- RAID
        (300000393, 'NX5', 'RAID - NX', 'NexHRM', 'XX55', N'RAID NX', NULL, 0),
        
        -- LumiHRM Teams: Pentagon
        (300000138, 'AC1', 'AscentisTech', 'LumiHRM', 'NX69', N'NX AscentisTech', 2018, 0),
        (300000139, 'CT1', 'Core Team - Pentagon', 'LumiHRM', 'NX69', N'NX HOD Pentagon', 1, 0),
        (300000142, 'D23', 'Paradigm', 'LumiHRM', 'NX69', N'NX PARADIGM', 2014, 0),
        (300000155, 'P13', 'Paragon', 'LumiHRM', 'NX69', N'NX PARAGON', 2016, 0),
        (300000395, 'F13', 'Pathfinder', 'LumiHRM', 'NX69', N'NX PATHFINDER', 2019, 0),
        (300000396, 'X13', 'Pixel', 'LumiHRM', 'NX69', N'NX PIXEL', 2017, 0),
        (300000158, 'M13', 'Polaris', 'LumiHRM', 'NX69', N'NX POLARIS', 2015, 0),
        (300000172, 'V13', 'Prime', 'LumiHRM', 'NX69', N'NX PRIME', 2013, 0),
        
        -- NexVisionHRM Teams: Marketing
        (300000299, 'C77', 'Horizon', 'NexVisionHRM', 'MW11', N'NX Marketing Horizon', 5, 0),
        (300000300, 'C88', 'Magnetic', 'NexVisionHRM', 'MW11', N'NX Marketing Magnetic', 6, 0),
        (300000394, 'C89', 'Momentum', 'NexVisionHRM', 'MW11', N'NX Marketing Momentum', 4, 0),

         -- Deleted Group
        (NULL, 'Other', 'Other', 'NexHRM', 'NX66', N'NX Other', NULL, 0),
        (NULL, 'Other', 'Other', 'LumiHRM', 'NX69', N'NX Other', NULL, 0)
    ) AS s (GroupTeamId, Code, Name, Company, DeptCode, AccountableArea, OldGroupId, IsDeleted)
JOIN 
    [bodb_HRM].[dbo].[Department] d 
    ON d.Code = s.DeptCode;
GO
