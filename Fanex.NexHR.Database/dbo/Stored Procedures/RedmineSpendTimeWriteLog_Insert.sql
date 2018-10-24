
CREATE PROCEDURE [dbo].[RedmineSpendTimeWriteLog_Insert] 
    @EmpId INT
    ,@IssueId INT
    ,@SpentOn DATE
    ,@Hours FLOAT
    ,@Comments NVARCHAR(255) = null
	,@Activity INT
    ,@CreatedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180620@Oliver.Dang
Task: Insert log when log time redmine from HRM
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RedmineSpendTimeLog(
        [EmpId]
        ,[IssueId]
        ,[SpentOn]
        ,[Hours]
        ,[Comments]
        ,[Activity]
        ,[CreatedDate]
        )
    VALUES (
        @EmpId
        ,@IssueId
        ,@SpentOn
        ,@Hours
        ,@Comments
		,@Activity
        ,@CreatedDate
        )

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Insert log failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END

