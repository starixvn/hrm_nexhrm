USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[CompanyEvent_GetEventInWeek]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_GetEventInWeek]
AS
/*
Created: 20160512@Harvey.Nguyen
Task: Show event on Dashboard
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @now DATE = GETDATE()
		,@startWeek DATE
		,@endWeek DATE

	SELECT @startWeek = DATEADD(dd, - (DATEPART(dw, @now) - 2), @now)

	SELECT @endWeek = DATEADD(dd, 15 - (DATEPART(dw, @now)), @now)

	SELECT e.Id
		,e.Title
		,e.FromDate
		,e.ToDate
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
	FROM dbo.CompanyEvent e WITH (NOLOCK)
	WHERE ((
			e.FromDate >= @startWeek
			AND e.FromDate <= @endWeek
			)
		OR (
			@startWeek >= e.FromDate
			AND @startWeek <= e.ToDate
			))
		AND (
			e.ToDate >= @now
		)
END
GO
