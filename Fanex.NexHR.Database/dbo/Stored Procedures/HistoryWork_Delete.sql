CREATE PROCEDURE [dbo].[HistoryWork_Delete] @Id INT
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

