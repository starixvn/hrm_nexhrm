USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[JobGrade_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[JobGrade_Insert] 
    @EmpId INT
    ,@JobGradeId INT
	,@EffectiveDate DATETIME
	,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240102@Lilac.Nguyen
Task: Insert JobGrade
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.EmployeeJobGrade(
		EmpId
		,JobGradeId
		,OldJobGradeId
		,AffectedDate
		,ModifiedBy
		,ModifiedDate
		,IsCurrentUsed
		,Note
		,IsDeleted
		,IsPromotion
        )

    VALUES (
        @EmpId
		,@JobGradeId
		,@JobGradeId
		,@EffectiveDate
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()
		,1
		,@Note
		,NULL
		,0
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
