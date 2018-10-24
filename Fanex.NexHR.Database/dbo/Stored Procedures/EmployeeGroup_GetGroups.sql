
/****** Object:  Stored Procedure dbo.EmployeeGroup_GetGroups    Script Date: 3/13/2016 3:21:40 PM ******/
CREATE PROCEDURE [dbo].[EmployeeGroup_GetGroups] 
	@IncludeSpecialTeam BIT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
	SET @IncludeSpecialTeam = ISNULL(@IncludeSpecialTeam, 0)
    -- Insert statements for procedure here
    SELECT g.Id, g.Name, g.Code, g.Rank, 
        leaderID, 
        leader.NickName AS LeaderNickName,  
        leader.FullName AS LeaderFullName,  
        AssistantLeaderId,
        astleader.NickName AS AssistantLeaderNickName,  
        astleader.FullName AS AssistantLeaderFullName,
        COUNT(eg.EmpID) AS NoOfMembers
    FROM EmployeeGroup g LEFT JOIN dbo.Employee leader WITH (NOLOCK) ON leader.ID = g.LeaderId
        LEFT JOIN dbo.Employee astLeader WITH (NOLOCK) ON astLeader.ID = g.AssistantLeaderId
        LEFT JOIN dbo.EmployeeGroup_Employee eg WITH (NOLOCK) ON eg.groupId=g.Id
    WHERE ((@IncludeSpecialTeam = 0 AND g.IsSpecialTeam = 0) OR @IncludeSpecialTeam = 1)  
		AND ISNULL(Deleted,0)=0
    GROUP BY g.Id, g.Name, g.Code, g.Rank, leaderID, AssistantLeaderId,leader.NickName, leader.FullName, astleader.NickName, astleader.FullName
    ORDER BY g.[Code]
END
