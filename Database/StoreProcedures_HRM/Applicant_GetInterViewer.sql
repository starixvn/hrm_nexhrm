USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_GetInterViewer]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetInterViewer]
AS
/*
Created: 20240819@Quinn.Vo
Task: Get interviewer .
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
   SET NOCOUNT ON;

   SELECT Id
	,NickName
   FROM dbo.Employee WITH (NOLOCK) WHERE IsDeleted = 0

END

GO
