
CREATE PROCEDURE [dbo].[LogReachedWorkingTime_Insert] @EmployeeIds VARCHAR(MAX)
	,@WorkDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190516@Quinn.Vo
Task: insert log for remind reached working time
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.LogReachedWorkingTime (
		EmpId
		,WorkDate
		,InsertedDate
		)
	SELECT val
		,@WorkDate
		,GETDATE()
	FROM dbo.fnc_strSplit(@EmployeeIds, ',')
END