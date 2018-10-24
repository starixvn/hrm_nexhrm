
CREATE PROCEDURE [dbo].[Department_GetById]
    @DepartmentId INT
AS
/*
Created: 20140819@Josh.Phan
Task: Get by departmentId.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT dep.[DepartmentId]
        ,dep.[Code]
        ,dep.[Name]
        ,dep.[ParentId]
        ,dep2.[Name] AS ParentName
        ,dep.[ModifiedDate]
        ,dep.[CreatedDate]
        ,dep.[IsDisabled]
        ,dep.[DepartmentLevel]
        ,dep.[DisplayOrder]
        ,dep.[CreatedBy]
        ,cr.FullName
        ,cr.NickName
        ,dep.[ManagerId]
        ,mg.FullName
        ,mg.NickName
        ,dep.[ModifiedBy]
        ,md.FullName
        ,md.NickName
    FROM dbo.Department AS dep WITH (NOLOCK)
    LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.Id
    LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.Id
    LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.Id
    LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
    WHERE @DepartmentId = dep.DepartmentId

END

