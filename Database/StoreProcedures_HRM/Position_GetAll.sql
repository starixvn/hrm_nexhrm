USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Position_GetAll]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Position_GetAll] @Keyword NVARCHAR(100) = ''
AS
/*
Created: 20231116@Lilac.Nguyen
Task: Get all position.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT PositionId
	,PositionCode
	,PositionName
	,DisplayOrder
	,IsActive
	FROM dbo.Position WITH (NOLOCK)
    WHERE  PositionName LIKE '%' + @Keyword + '%' OR PositionCode LIKE '%' + @Keyword + '%' 
END
GO
