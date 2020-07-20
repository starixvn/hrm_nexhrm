ALTER PROCEDURE [dbo].[MessageCategoryLog_GetJsonObjectByCategoryId]
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