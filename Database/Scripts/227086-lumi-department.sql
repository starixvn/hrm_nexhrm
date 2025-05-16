USE [LumiHRM]
GO

UPDATE [dbo].[Department]
SET [IsDisabled] = 1

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
    ,[ModifiedDate])
VALUES
    ('Pentagon', 'Pentagon', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE()),
    ('Core Team', 'Core Team', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE()),
    ('HR & Admin', 'HR & Admin', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE()),
    ('Marketing BU', 'Marketing BU', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE()),
    ('FI BU', 'FI BU', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE())
GO

UPDATE g
SET g.DepartmentId = d.DepartmentId
FROM [dbo].[EmployeeGroup] g
INNER JOIN [dbo].[Department] d ON 
    CASE g.Code
        WHEN 'HOD-PE' THEN 'Pentagon'
        WHEN 'MGT' THEN 'Core Team'
        WHEN 'HR' THEN 'HR & Admin'
        WHEN 'HOR' THEN 'Marketing BU'
        WHEN 'MAG' THEN 'Marketing BU'
        WHEN 'MOM' THEN 'Marketing BU'
        WHEN 'PRI' THEN 'Pentagon'
        WHEN 'PAD' THEN 'Pentagon'
        WHEN 'POL' THEN 'Pentagon'
        WHEN 'PAG' THEN 'Pentagon'
        WHEN 'PIX' THEN 'Pentagon'
        WHEN 'AT' THEN 'Pentagon'
        WHEN 'PAT' THEN 'Pentagon'
        WHEN 'FA' THEN 'FI BU'
    END = d.Name
WHERE d.IsDisabled = 0
GO





