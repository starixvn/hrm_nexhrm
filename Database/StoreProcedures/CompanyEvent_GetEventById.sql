USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[CompanyEvent_GetEventById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_GetEventById] @Id NVARCHAR(150)
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
		,e.ToDate
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
		,e.CreatedBy
		,e.CreatedOn
	FROM dbo.CompanyEvent e WITH (NOLOCK)
	WHERE e.Id = @Id
END
GO
