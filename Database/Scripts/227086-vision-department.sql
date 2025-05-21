USE [NexVisionHRM]
GO

ALTER TABLE [dbo].[Department]
ADD [IsSpecial] BIT NOT NULL DEFAULT 0
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
    ('Core Team', 'Core Team', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 1),
    ('HR & Admin', 'HR & Admin', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 1),
    ('FI BU', 'FI BU', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 1),
    ('Marketing BU', 'Marketing BU', NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), NULL, GETDATE(), 0)
GO

INSERT [dbo].[EmployeeGroup] ([Code], [Name], [DepartmentId], [Rank], [Deleted], [ParentId], [IsSpecialTeam])
VALUES 
    ('MGT', 'Management', NULL, 99, 0, NULL, 1),
    ('FA', 'Finance & Accounting', NULL, 99, 0, NULL, 1),
    ('MOM', 'Momentum', NULL, 99, 0, NULL, 0),
    ('HOR', 'Horizon', NULL, 99, 0, NULL, 0),
    ('MAG', 'Magnetic', NULL, 99, 0, NULL, 0)
GO

UPDATE g
SET g.DepartmentId = d.DepartmentId
FROM [dbo].[EmployeeGroup] g
INNER JOIN [dbo].[Department] d ON 
    CASE g.Code
        WHEN 'MGT' THEN 'Core Team'
        WHEN 'HR-ADM' THEN 'HR & Admin'
        WHEN 'FA' THEN 'FI BU'
        WHEN 'HOR' THEN 'Marketing BU'
        WHEN 'MAG' THEN 'Marketing BU'
        WHEN 'MOM' THEN 'Marketing BU'
    END = d.Name
WHERE d.IsDisabled = 0
GO