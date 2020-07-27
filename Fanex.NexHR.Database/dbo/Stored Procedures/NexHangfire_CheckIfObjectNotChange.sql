CREATE PROCEDURE [dbo].[NexHangfire_CheckIfObjectNotChange]
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