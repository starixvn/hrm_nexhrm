USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[HistoryWork_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[HistoryWork_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: soft delete a history work 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

	DELETE FROM HistoryWork 
	WHERE Id = @id;

END


GO
