USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[BioDevice_Edit]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[BioDevice_Edit]
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
			   ,dbo.fnc_GetClientDateTime())
	END
	ELSE
	BEGIN
		UPDATE [dbo].[BioDevice] WITH (ROWLOCK, UPDLOCK)
		SET [DisplayName] = @DisplayName
		  ,[StatusId] = @StatusId
		  ,[ModifiedBy] = @ModifiedBy
		  ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
		WHERE DeviceId = @DeviceId
	END
END
GO
