CREATE OR ALTER PROCEDURE [dbo].[API_Redmine_GetTimeEntries]
    @Date DATE
AS
/*
Created: 20240319
Task: Get time entries from RedmineNexcel for a specific date
DB: RedmineNexcel
*/
BEGIN
    SELECT 
        u.Id AS UserId,
        u.Login,
        te.SpentOn,
        te.Hours
    FROM [RedmineNexcel].[dbo].[Users] u WITH (NOLOCK)
    INNER JOIN [RedmineNexcel].[dbo].[TimeEntries] te WITH (NOLOCK)
        ON u.Id = te.UserId 
        AND CAST(te.SpentOn AS DATE) = @Date
END
GO 