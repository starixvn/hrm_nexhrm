SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroups] 
	@IncludeSpecialTeam BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SET @IncludeSpecialTeam = ISNULL(@IncludeSpecialTeam, 0)

    SELECT g.Id,
		g.Id as TeamId,
		g.Name, 
        d.DepartmentId,
        d.Name as DepartmentName,
		g.Code, 
		g.Rank, 
        COUNT(e.Id) AS NoOfMembers
    FROM dbo.EmployeeGroup g WITH (NOLOCK)
        LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.GroupId = g.Id AND e.isDeleted = 0 AND e.IsSpecialUser = 0
        LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
    WHERE ((@IncludeSpecialTeam = 0 AND g.IsSpecialTeam = 0) OR @IncludeSpecialTeam = 1)  
		AND ISNULL(Deleted,0)=0
    GROUP BY g.Id, g.Name, g.Code, g.Rank, d.DepartmentId, d.Name
    ORDER BY g.[Code]
END

GO
