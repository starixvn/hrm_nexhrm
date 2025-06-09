USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LogReachedWorkingTime_Insert]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[LogReachedWorkingTime_Insert] @EmployeeIds VARCHAR(MAX)
	,@WorkDate DATETIME
	,@InsertedType VARCHAR(20)
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
		,InsertedType
		)
	SELECT val
		,@WorkDate
		,GETDATE()
		,@InsertedType
	FROM dbo.fnc_strSplit(@EmployeeIds, ',')
END
GO
