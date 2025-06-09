USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[MessageCategoryLog_GetJsonObjectByCategoryId]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[MessageCategoryLog_GetJsonObjectByCategoryId]
	@categoryId INT
AS
/*
Created: 20200708Merlin.Le
Task: Get Json object by category id
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT l.JsonObject
	FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK)
	WHERE l.MessageCategory = @categoryId
END
GO
