USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_GetSource]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetSource]

AS
/*
Created: 20240327@Lilac.Nguyen
Task: Get Source.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT
	ld.[Name]
	,CAST(ld.[Value] AS INT) AS [Value]
	FROM dbo.LookupData AS ld WITH (NOLOCK)
	LEFT JOIN dbo.Lookup AS lu WITH (NOLOCK) ON lu.Id = ld.LookupId
	WHERE lu.Code = 'AS'
	
END
GO
