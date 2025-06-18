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
    ([Code], [Name], [CreatedDate], [CompanyId], [DepartmentId], [GroupTeamId], [AccountableArea])
SELECT
    s.Code,
    s.Name,
    GETDATE(),
    CASE s.Company WHEN 'NexHRM' THEN @NexHRMCompanyId
                   WHEN 'NexVisionHRM' THEN @NexVisionHRMCompanyId
                   WHEN 'LumiHRM' THEN @LumiHRMCompanyId END,
    d.DepartmentId,
    111111,
    s.AccountableArea
FROM 
    (VALUES
        -- Code, Name, Company, DeptCode, AccountableArea
        -- NexVisionHRM Marketing 
        ('', 'Momentum', 'NexVisionHRM', 'MW11', N'NX Marketing Momentum'),
        ('C77', 'Horizon', 'NexVisionHRM', 'MW11', N'NX Marketing Horizon'),
        ('C88', 'Magnetic', 'NexVisionHRM', 'MW11', N'NX Marketing Magnetic'),
        -- NexHRM Finance 
        --('FI1', 'Finance - HMA', 'NexHRM', 'FI01', N'Finance SU01'),
        -- NexHRM Atomix 
        ('O13', 'NEXOPS', 'NexHRM', 'NX27', N'NX NEXOPS'),
        ('E13', 'NEXPE', 'NexHRM', 'NX27', N'NX NEXPE'),
        -- NexHRM Core Team
        ('CT1', 'Core Team', 'NexHRM', 'NX01', N'NX CORE TEAM'),
        -- NexHRM Expat
        ('E13', 'Expat', 'NexHRM', 'NX66', N'NX EXPAT'),
        -- NexHRM GM Office 
        ('D13', 'Data', 'NexHRM', 'NX66', N'NX DATA'),
        ('S13', 'NovaStar', 'NexHRM', 'NX66', N'NX Novastar'),
        ('HR3', 'OptiGo', 'NexHRM', 'NX66', N'NX OptiGo'),
        -- NexHRM Omni Flow Network
        ('TS3', 'IT Network', 'NexHRM', 'NX03', N'NX IT Network'),
        ('EV3', 'Pontus', 'NexHRM', 'NX03', N'NX Pontus'),
        ('S13', 'SAS', 'NexHRM', 'NX03', N'NX SAS'),
        -- NexHRM Pioneers
        ('P13', 'Forge', 'NexHRM', 'NX18', N'NX FORGE'),
        ('P13', 'PPO', 'NexHRM', 'NX18', N'NX PPO'),
        ('TT3', 'HOD Pioneers', 'NexHRM', 'NX18', N'NX HOD PIONEERS'),
        ('EL3', 'Pionex', 'NexHRM', 'NX18', N'NX PIONEX'),
        ('R13', 'Robustus', 'NexHRM', 'NX18', N'NX ROBUTUS'),
        -- NexHRM Subarashi
        ('B13', 'Americano', 'NexHRM', 'NX06', N'NX AMERICANO'),
        ('S13', 'Cocktail', 'NexHRM', 'NX06', N'NX COCKTAIL'),
        ('G13', 'HOD Subarashi', 'NexHRM', 'NX06', N'NX HOD SUBARASHI'),
        ('P13', 'Latte', 'NexHRM', 'NX06', N'NX LATTE'),
        ('T13', 'Shake', 'NexHRM', 'NX06', N'NX SHAKE'),
        -- NexHRM Synergy
        ('S13', 'HOD Synergy', 'NexHRM', 'NX99', N'NX HOD SYNERGY'), 
        ('S13', 'SPO', 'NexHRM', 'NX99', N'NX SPO'),
        ('O13', 'R-Pulse', 'NexHRM', 'NX99', N'NX R-PULSE'),
        ('I13', 'T-Pulse', 'NexHRM', 'NX99', N'NX T-PULSE'),
        ('V13', 'V-Pulse', 'NexHRM', 'NX99', N'NX V-PULSE'),
        -- NexHRM Wabi Sabi
        ('S13', 'D.A', 'NexHRM', 'NX45', N'NX D.A'),
        ('W13', 'HOD Wabi Sabi', 'NexHRM', 'NX45', N'NX HOD WABI SABI'),
        ('GV3', 'S.A.I', 'NexHRM', 'NX45', N'NX S.A.I'),
        ('MG3', 'Sentinels', 'NexHRM', 'NX45', N'NX SENTINELS'),
        ('T13', 'SWAT', 'NexHRM', 'NX45', N'NX SWAT'),
        -- NexHRM HR&ADM
        ('HM7', 'HRA NEX', 'NexHRM', 'SR36', N'NX HR&ADM'),
        -- LumiHRM Pentagon
        ('AC1', 'AscentisTech', 'LumiHRM', 'NX69', N'NX AscentisTech'),
        ('CT1', 'HOD Pentagon', 'LumiHRM', 'NX69', N'NX HOD Pentagon'),
        ('D23', 'Paradigm', 'LumiHRM', 'NX69', N'NX PARADIGM'),
        ('P13', 'Paragon', 'LumiHRM', 'NX69', N'NX PARAGON'),
        ('', 'Pathfinder', 'LumiHRM', 'NX69', N'NX PATHFINDER'),
        ('', 'Pixel', 'LumiHRM', 'NX69', N'NX PIXEL'),
        ('M13', 'Polaris', 'LumiHRM', 'NX69', N'NX POLARIS'),
        ('V13', 'Prime', 'LumiHRM', 'NX69', N'NX PRIME'),
        -- LumiHRM Rad
        ('NX5', 'RAID - NX', 'LumiHRM', 'RAID', N'RAID NX')
    ) AS s (Code, Name, Company, DeptCode, AccountableArea)
JOIN 
    [bodb_HRM_bk].[dbo].[Department] d 
    ON d.Code = s.DeptCode;
GO
