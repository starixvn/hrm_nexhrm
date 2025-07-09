USE [HRM]
GO

EXEC sp_rename 'JobGrade', 'JobGrade_bk';
GO

EXEC sp_rename 'ManagerGrade', 'ManagerGrade_bk';
GO

CREATE OR ALTER VIEW [dbo].[JobGrade]
AS
SELECT 
    JobLevelId AS JobGradeId,
    [Name] AS Code,
    [Name]
    NULL AS [Description],
    [IsDeleted],
    [ModifiedBy],
    [ModifiedDate]
FROM [bodb_HRM].[dbo].[JobLevel] WITH (NOLOCK)
GO

CREATE OR ALTER VIEW [dbo].[ManagementGrade]
AS
SELECT 
    ManagementLevelId AS ManagementGradeId,
    [Name] AS Code,
    [Name]
    NULL AS [Description],
    [IsDeleted],
    [ModifiedBy],
    [ModifiedDate]
FROM [bodb_HRM].[dbo].[ManagementLevel] WITH (NOLOCK)
GO