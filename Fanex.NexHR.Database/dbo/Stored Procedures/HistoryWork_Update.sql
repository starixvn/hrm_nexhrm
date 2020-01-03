CREATE PROCEDURE [dbo].[HistoryWork_Update]
	 @Id INT
	,@ModifiedDate DATETIME = NULL
	,@JobtitleId INT = NULL
	,@JobtitleName VARCHAR(100) = NULL
	,@GroupId INT = NULL
	,@GroupName VARCHAR(100) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: Update History Work
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN	
	SET NOCOUNT ON;

	SELECT @JobtitleId = Id 
    FROM dbo.LookupData WITH (NOLOCK)
    WHERE Name = @JobtitleName

	SELECT @GroupId = Id 
    FROM dbo.EmployeeGroup WITH (NOLOCK)
    WHERE Name = @GroupName
	
	UPDATE [dbo].[HistoryWork]
	SET ModifiedDate = @ModifiedDate
	,JobtitleId = @JobtitleId
	,JobtitleName = @JobtitleName
	,GroupId = @GroupId
	,GroupName =@GroupName 
	WHERE Id = @Id
END