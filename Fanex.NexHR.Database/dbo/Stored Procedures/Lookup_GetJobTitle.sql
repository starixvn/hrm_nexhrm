
CREATE PROCEDURE [dbo].[Lookup_GetJobTitle] @JobTitle NVARCHAR(100) = ''
AS
/*
Created: 20160224@Quinn.Vo
Task: Get job title data.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF @JobTitle IS NULL
        SET @JobTitle = ''

    SELECT lkd.Id
        ,lkd.LookupId
        ,lkd.Code
        ,lkd.[Name]
        --,lkd.Value
        ,COUNT(e.Id) AS NumberOfEmployee
    FROM LookupData AS lkd WITH (NOLOCK)
        INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
        AND lk.Code = 'JT'
        LEFT JOIN Employee AS e WITH (NOLOCK) ON lkd.Id = e.JobTitleId
    WHERE lkd.[Name] LIKE '%' + @JobTitle + '%'
        AND ISNULL(e.isDeleted, 0) = 0
        --AND e.Enabled = 1
    GROUP BY lkd.LookupId
        ,lkd.id
        ,lkd.Code
        ,lkd.[Name]
    ORDER BY lkd.[Name]
END