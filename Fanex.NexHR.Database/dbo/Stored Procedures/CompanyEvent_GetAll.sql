
CREATE PROCEDURE [dbo].[CompanyEvent_GetAll] @FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Get all events
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Title
		,e.FromDate
		,(CASE 
			WHEN (e.FromDate = e.ToDate) THEN e.ToDate
			ELSE CASE 
					WHEN (
							e.FromTime = ''
							AND e.ToTime = ''
							) THEN DATEADD(day, 1, e.ToDate)
					ELSE e.ToDate
					END END
			) AS 'ToDate'
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
		,e2.NickName AS 'CreatedBy'
		,e.CreatedOn
	FROM dbo.CompanyEvent e WITH (NOLOCK)
		INNER JOIN dbo.Employee e2 WITH (NOLOCK) ON e.CreatedBy = e2.Id
	WHERE (
			@FromDate IS NULL
			OR e.FromDate >= @FromDate
			)
		AND (
			@ToDate IS NULL
			OR e.ToDate <= @ToDate
			)
END
