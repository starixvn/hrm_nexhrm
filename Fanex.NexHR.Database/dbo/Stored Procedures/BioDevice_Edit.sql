


CREATE PROCEDURE [dbo].[BioDevice_Edit]
	@DeviceId INT
	,@ModifiedBy INT
	,@StatusId VARCHAR(5) = NULL
	,@DisplayName VARCHAR(50) = NULL
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180621@Elton.Ton
Task: Get Bio Statuses.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM dbo.BioDevice bd WHERE bd.DeviceId = @DeviceId)
	BEGIN
		INSERT INTO [dbo].[BioDevice]
			   ([DeviceId]
			   ,[DisplayName]
			   ,[StatusId]
			   ,[ModifiedBy]
			   ,[ModifiedDate])
		 VALUES
			   (@DeviceId
			   ,@DisplayName
			   ,@StatusId
			   ,@ModifiedBy
			   ,GETDATE())
	END
	ELSE
	BEGIN
		UPDATE [dbo].[BioDevice] WITH (ROWLOCK, UPDLOCK)
		SET [DisplayName] = @DisplayName
		  ,[StatusId] = @StatusId
		  ,[ModifiedBy] = @ModifiedBy
		  ,[ModifiedDate] = GETDATE()
		WHERE DeviceId = @DeviceId
	END
END
