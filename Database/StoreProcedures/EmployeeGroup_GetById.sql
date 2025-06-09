CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetById] @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT g.[Id]
        ,g.[Code]
        ,g.[Name]
        ,g.[Rank]
        ,g.[Deleted]
        ,g.ParentId
        ,g.[DepartmentId]
    FROM dbo.EmployeeGroup g WITH (NOLOCK)
    WHERE g.Id = @id
END


GO
