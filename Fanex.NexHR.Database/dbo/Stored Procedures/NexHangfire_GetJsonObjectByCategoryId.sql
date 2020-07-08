ALTER PROCEDURE [dbo].[NexHangfire_GetJsonObjectByCategoryId]
	@categoryId INT
AS
/*
Created: 20200708Merlin.Le
Task: Get Json object by category id
DB: NexHRM & NexHangfire
*/
BEGIN
	SET NOCOUNT ON;

	SELECT l.JsonObject
	FROM [NexHangfire].[dbo].[CustomLog] AS l WITH (NOLOCK)
	WHERE l.MessageCategory = @categoryId
END