USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetLeaderForStanbyTeam]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Employee_GetLeaderForStanbyTeam]
AS
/*
Created: 20150814@Quinn.Vo
Task: Get leaders which approved check in/out request for Stanby.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.GroupID
        ,e.Email
		,con.ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE NickName = 'Viet'
END


GO
