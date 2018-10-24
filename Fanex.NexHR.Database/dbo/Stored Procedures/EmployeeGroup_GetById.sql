
/****** Object:  Stored Procedure dbo.EmployeeGroup_GetById    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE PROCEDURE [dbo].[EmployeeGroup_GetById] @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT g.[Id]
        ,g.[Code]
        ,g.[Name]
        ,g.[Rank]
        ,g.[CreatedDate]
        ,g.[CreatedBy]
        ,g.[ModifiedDate]
        ,g.[ModifiedBy]
        ,g.[Deleted]
        ,g1.[Id] AS ParentId
        ,g1.Code
        ,g1.[Name]
        ,g.[DepartmentId]
        ,de.Code
        ,de.Name
        ,g.[LeaderId]
        ,leader.NickName
        ,leader.FullName
        ,g.[AssistantLeaderId]
        ,astleader.NickName
        ,astleader.FullName
    FROM EmployeeGroup g WITH (NOLOCK)
        LEFT JOIN dbo.Employee leader WITH (NOLOCK) ON leader.ID = g.LeaderId
        LEFT JOIN dbo.Employee astLeader WITH (NOLOCK) ON astLeader.ID = g.AssistantLeaderId
        LEFT JOIN dbo.EmployeeGroup g1 WITH (NOLOCK) ON g1.Id = g.ParentId
        LEFT JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
    WHERE g.Id = @id
END

