USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[MessageCategoryLog_CheckIfObjectNotChange]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[MessageCategoryLog_CheckIfObjectNotChange]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
AS
/*
Created: 20200708@Merlin.Le
Task: Check if the object have not change
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1 
	FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK) 
	WHERE l.MessageCategory = @categoryId AND l.JsonObject = @jsonObject;		
END

GO
