USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[JobGrade_GetAll]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[JobGrade_GetAll]
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Get jobGrade.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT JobGradeId AS Id
	,Code
	,Name
	FROM dbo.JobGrade WITH (NOLOCK)
    WHERE (IsDeleted = 0 OR IsDeleted IS NULL)  
END
GO
