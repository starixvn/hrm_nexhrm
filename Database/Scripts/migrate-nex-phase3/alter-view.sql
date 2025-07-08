USE [NexHRM]
GO

CREATE OR ALTER VIEW [dbo].[GroupSupervisor]
AS
SELECT 
    [OwnerId] AS ApproverId,
    g.[GroupId],
    g.[ModifiedBy],
    g.[ModifiedDate],
    g.[IsDeleted],
    c.Name AS OuterCompanyName
FROM [bodb_HRM].[dbo].[GroupSupervisor] AS g WITH (NOLOCK)
INNER JOIN [bodb_HRM].[dbo].[CompanyEmployee] AS ce WITH (NOLOCK) 
    ON ce.EmpId = g.OwnerId AND ISNULL(ce.IsSpecialUser, 0) = 0 AND ISNULL(ce.IsDeleted, 0) = 0 AND ISNULL(ce.Enabled, 1) = 1
LEFT JOIN [bodb_HRM].[dbo].[Company] AS c WITH (NOLOCK) 
    ON c.CompanyId = ce.CompanyId 
    AND c.CompanyId != (SELECT [Value] FROM [NexHRM].[dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
WHERE g.ActionType = 3 AND g.RequestType = 6
GO

CREATE OR ALTER VIEW [dbo].[EmployeeSupDetail]
AS
SELECT 
    esup.EmpId, 
    esup.NickName, 
    esup.FullName, 
    esup.ApproverId, 
    CASE 
        WHEN c.Name IS NOT NULL THEN sup.NickName + ' (' + c.Name + ')'
        ELSE sup.NickName
    END AS ApproverNickName,
    sup.FullName AS ApproverFullName, 
    esup.IsPrivateSetting
FROM 
(
    SELECT 
        e.Id AS EmpId, 
        e.NickName, 
        e.FullName, 
        CASE 
            WHEN es.ApproverId IS NULL THEN gs.ApproverId 
            ELSE es.ApproverId 
        END AS ApproverId, 
        CAST(
            CASE 
                WHEN es.ApproverId IS NOT NULL THEN 1 
                ELSE 0 
            END AS BIT
        ) AS IsPrivateSetting
    FROM dbo.Employee AS e WITH (NOLOCK)
    LEFT OUTER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK)
        ON e.GroupId = gs.GroupId 
        AND gs.IsDeleted = 0 
        AND e.Id <> gs.ApproverId
    LEFT OUTER JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK)
        ON e.Id = es.EmpId 
        AND es.IsDeleted = 0 
        AND e.Id <> es.ApproverId
    WHERE 
        e.isDeleted = 0 AND (
            gs.ApproverId IS NOT NULL OR 
            es.ApproverId IS NOT NULL
        )
) AS esup
INNER JOIN bodb_HRM.dbo.Employee AS sup WITH (NOLOCK) ON esup.ApproverId = sup.EmpId AND ISNULL(sup.isDeleted, 0) = 0
INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ce WITH (NOLOCK) ON ce.EmpId = sup.EmpId AND ISNULL(ce.IsSpecialUser, 0) = 0 AND ISNULL(ce.IsDeleted, 0) = 0 AND ISNULL(ce.Enabled, 1) = 1
LEFT JOIN bodb_HRM.dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = ce.CompanyId AND c.CompanyId != (SELECT [Value] FROM [dbo].[SystemSetting] WHERE [Name] = 'CompanyId')
GO
