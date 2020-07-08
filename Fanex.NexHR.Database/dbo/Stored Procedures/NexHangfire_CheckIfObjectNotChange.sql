ALTER PROCEDURE [dbo].[NexHangfire_CheckIfObjectNotChange]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
AS
/*
Created: 20200708@Merlin.Le
Task: Check if the object have not change
DB: NexHRM & NexHangfire
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1 
	FROM [NexHangfire].[dbo].[CustomLog] AS l WITH (NOLOCK) 
	WHERE l.MessageCategory = @categoryId AND l.JsonObject = @jsonObject;		
END