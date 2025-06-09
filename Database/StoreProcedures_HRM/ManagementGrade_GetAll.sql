USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[ManagementGrade_GetAll]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[ManagementGrade_GetAll]
AS
/*
Created: 20252602@Henry.Dinh
Task: Get Management Grade
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT ManagementGradeId AS Id
	,Code
	,Name
	FROM dbo.ManagementGrade WITH (NOLOCK)
    WHERE (IsDeleted = 0 OR IsDeleted IS NULL)  
END
GO
