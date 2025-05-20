USE [NexHRM]
GO

ALTER TABLE [dbo].[Department]
ADD [IsSpecial] BIT NOT NULL DEFAULT 0
GO

UPDATE [dbo].[Department]
SET [IsDisabled] = 1
GO

INSERT INTO [dbo].[Department]
    ([Code]
    ,[Name]
    ,[ParentId]
    ,[ManagerId]
    ,[DepartmentLevel]
    ,[DisplayOrder]
    ,[IsDisabled]
    ,[CreatedBy]
    ,[CreatedDate]
    ,[ModifiedBy]
    ,[ModifiedDate]
    ,[IsSpecial])
VALUES
    ('FI BU', 'FI BU', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('OmniFlow', 'OmniFlow', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Expat', 'Expat', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('DEV Pool', 'DEV Pool', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Atomix', 'Atomix', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('GM Office', 'GM Office', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Core Team', 'Core Team', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Synergy', 'Synergy', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Pioneers', 'Pioneers', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Subarashi', 'Subarashi', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('Wabi Sabi', 'Wabi Sabi', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0),
    ('HR & Admin', 'HR & Admin', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0)
GO

UPDATE g
SET g.DepartmentId = d.DepartmentId
FROM [dbo].[EmployeeGroup] g
INNER JOIN [dbo].[Department] d ON 
    CASE 
        WHEN g.Code = 'FA' THEN 'FI BU'
        WHEN g.Code = 'ITNW' THEN 'OmniFlow'
        WHEN g.Code = 'EXP' THEN 'Expat'
        WHEN g.Code = 'DEV' THEN 'DEV Pool'
        WHEN g.Code = 'SAS' THEN 'OmniFlow'
        WHEN g.Code = 'NEO' THEN 'Atomix'
        WHEN g.Code = 'DAT' THEN 'GM Office'
        WHEN g.Code = 'NPE' THEN 'Atomix'
        WHEN g.Code = 'NOS' THEN 'GM Office'
        WHEN g.Code = 'COR' THEN 'Core Team'
        WHEN g.Code = 'OPT' THEN 'GM Office'
        WHEN g.Code = 'PON' THEN 'OmniFlow'
        WHEN g.Code = 'SPO' THEN 'Synergy'
        WHEN g.Code = 'TPU' THEN 'Synergy'
        WHEN g.Code = 'RPU' THEN 'Synergy'
        WHEN g.Code = 'PPO' THEN 'Pioneers'
        WHEN g.Code = 'ROB' THEN 'Pioneers'
        WHEN g.Code = 'FOR' THEN 'Pioneers'
        WHEN g.Code = 'PIO' THEN 'Pioneers'
        WHEN g.Code = 'AME' THEN 'Subarashi'
        WHEN g.Code = 'SHA' THEN 'Subarashi'
        WHEN g.Code = 'LAT' THEN 'Subarashi'
        WHEN g.Code = 'COC' THEN 'Subarashi'
        WHEN g.Code = 'SWA' THEN 'Wabi Sabi'
        WHEN g.Code = 'SAI' THEN 'Wabi Sabi'
        WHEN g.Code = 'SEN' THEN 'Wabi Sabi'
        WHEN g.Code = 'D.A' THEN 'Wabi Sabi'
        WHEN g.Code = 'HOD-PI' THEN 'Pioneers'
        WHEN g.Code = 'HOD-SU' THEN 'Subarashi'
        WHEN g.Code = 'HOD-SY' THEN 'Synergy'
        WHEN g.Code = 'HOD-WASA' THEN 'Wabi Sabi'
        WHEN g.Code = 'VPU' THEN 'Synergy'
        WHEN g.Code = 'HR-ADM' THEN 'HR & Admin'
    END = d.Name
WHERE d.IsDisabled = 0
GO




