ALTER PROCEDURE [dbo].[NexHangfire_UpdateJsonObject]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
AS
/*
Created: 20200708Merlin.Le
Task: Update Json Object
DB: NexHRM & NexHangfire
*/
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS 
		(SELECT 1 
		FROM [NexHangfire].[dbo].[CustomLog] AS l WITH (NOLOCK) 
		WHERE l.MessageCategory = @categoryId)
		)
		BEGIN
			UPDATE [NexHangfire].[dbo].[CustomLog]
				SET JsonObject = @jsonObject
				WHERE MessageCategory = @categoryId
		END
	ELSE
		BEGIN
			INSERT INTO [NexHangfire].[dbo].[CustomLog] (MessageCategory, JsonObject)
			VALUES(@categoryId, @jsonObject)
		END
END