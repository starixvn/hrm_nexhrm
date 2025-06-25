USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[RepeatType_GetAll]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[RepeatType_GetAll]
AS
/*
Created: 
Task: Get Repeat type.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Value] AS Id, [Name] AS [Description]
	FROM dbo.LookupData WITH (NOLOCK)
	WHERE LookupId = 5
END

GO
