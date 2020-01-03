CREATE PROCEDURE [dbo].[HistoryWork_Insert] @EmpId INT
	,@AffectedDate DATETIME = NULL
	,@ModifiedBy INT = NULL
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
Task: create new working history
DB: NexHR
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

	INSERT INTO dbo.HistoryWork(
		EmpId
		,AffectedDate	
		,ModifiedBy
		,JobtitleId
		,GroupId
		,ModifiedDate
		,JobtitleName
		,GroupName		
		)
	VALUES (
		@EmpId	
		,@AffectedDate
		,@ModifiedBy
		,@JobtitleId
		,@GroupId
		,@ModifiedDate
		,@JobtitleName
		,@GroupName	
		)
END

