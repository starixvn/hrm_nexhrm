
CREATE PROCEDURE [dbo].[Employee_GetLeaderForStanbyTeam]
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
    FROM dbo.Employee AS e WITH (NOLOCK)
    WHERE NickName = 'Viet'
        OR NickName = 'Kiwi'
END

