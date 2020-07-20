ALTER PROCEDURE [dbo].[MessageCategoryLog_UpdateJsonObject]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
	,@errCode INT = 0
	,@errMsg VARCHAR (50) = ''
AS
/*
Created: 20200708Merlin.Le
Task: Update Json Object
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS 
		(SELECT 1 
		FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK) 
		WHERE l.MessageCategory = @categoryId)
		)
		BEGIN
			UPDATE [dbo].[MessageCategoryLog]
				SET JsonObject = @jsonObject
				WHERE MessageCategory = @categoryId
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[MessageCategoryLog] (MessageCategory, JsonObject)
			VALUES(@categoryId, @jsonObject)
		END
END