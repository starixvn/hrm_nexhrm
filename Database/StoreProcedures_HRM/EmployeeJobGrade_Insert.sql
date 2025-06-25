USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeJobGrade_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeJobGrade_Insert] 
    @EmpId INT
    ,@JobGradeId INT
    ,@OldJobGradeId INT
    ,@IsPromotion BIT
    ,@EffectiveDate DATETIME
    ,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Insert Promotion
DB: NexHRM
Original: 
Revisions:  
    20252602@Henry.Dinh: Set others IsCurrentUsed to 0
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
	SET IsCurrentUsed = 0
	WHERE EmpId = @EmpId AND (IsDeleted = 0 OR IsDeleted IS NULL);

    IF EXISTS (
        SELECT 1
        FROM dbo.EmployeeJobGrade WITH (NOLOCK)
        WHERE EmpId = @EmpId 
        AND JobGradeId = @JobGradeId
        AND (IsDeleted = 0 OR IsDeleted IS NULL)
        AND (AffectedDate = @EffectiveDate)
    )
    BEGIN
        UPDATE dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
        SET 
            IsPromotion = @IsPromotion,
            Note = @Note,
            OldJobGradeId = @OldJobGradeId,
			IsCurrentUsed = 1,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE 
            EmpId = @EmpId 
            AND JobGradeId = @JobGradeId
            AND (IsDeleted = 0 OR IsDeleted IS NULL)
            AND (AffectedDate = @EffectiveDate)
        SELECT @ErrCode = 0, @ErrMsg = ''
        RETURN
    END
    
    ELSE
	BEGIN
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
			,@OldJobGradeId
			,@EffectiveDate
			,@ModifiedBy
			,dbo.fnc_GetClientDateTime()
			,1 
			,@Note
			,NULL
			,@IsPromotion
		);
	END
    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO
