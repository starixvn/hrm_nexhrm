-- EmployeeGroup_GetGroup 45
CREATE PROCEDURE [dbo].[EmployeeGroup_GetGroup]
	-- Add the parameters for the stored procedure here
	@id int = null,
	@code varchar(20) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select g.[Id],
		g.[Code],
		g.[Name],
		g.[Rank],
		g.[CreatedDate],
		g.[CreatedBy],
		g.[ModifiedDate],
		g.[ModifiedBy],
		g.[Deleted],
		g1.[Id] AS ParentId,
		g1.[Name] AS ParentName,
		g.[DepartmentId],
		g.[LeaderId],
		leader.NickName AS LeaderNickName,  
		leader.FullName AS LeaderFullName,  
		g.[AssistantLeaderId],
		astleader.NickName AS AssistantLeaderNickName,  
		astleader.FullName AS AssistantLeaderFullName
		
	from EmployeeGroup g 
		LEFT JOIN dbo.Employee leader WITH (NOLOCK) ON leader.ID = g.LeaderId
		LEFT JOIN dbo.Employee astLeader WITH (NOLOCK) ON astLeader.ID = g.AssistantLeaderId
		LEFT JOIN dbo.EmployeeGroup g1 WITH (NOLOCK) ON g1.Id = g.ParentId
	where g.Id=@id or g.Code=@code
END
