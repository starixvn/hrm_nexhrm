UPDATE hwd
SET hwd.GroupName = g.Name
FROM [NexHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [NexHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId

UPDATE hwd
SET hwd.GroupName = g.Name
FROM [LumiHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [LumiHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId

UPDATE hwd
SET hwd.GroupName = g.Name
FROM [NexVisionHRM].[dbo].[HistoryWorkDetail] hwd WITH (NOLOCK)
INNER JOIN [NexVisionHRM].[dbo].[EmployeeGroup] g WITH (NOLOCK) ON g.Id = hwd.GroupId