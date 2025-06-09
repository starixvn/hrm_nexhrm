USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeManagementGrade_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeManagementGrade_Insert] 
    @EmpId INT
    ,@ManagementGradeId INT
    ,@OldManagementGradeId INT
    ,@IsPromotion BIT
    ,@EffectiveDate DATETIME
    ,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20252602@Henry.Dinh
Task: Insert Management Grade
DB: NexHRM
Original: 
Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.EmployeeManagementGrade WITH (ROWLOCK, UPDLOCK)
	SET IsCurrentUsed = 0
	WHERE EmpId = @EmpId AND (IsDeleted = 0 OR IsDeleted IS NULL);

    IF EXISTS (
        SELECT 1
        FROM dbo.EmployeeManagementGrade WITH (NOLOCK)
        WHERE EmpId = @EmpId 
        AND ManagementGradeId = @ManagementGradeId
        AND (IsDeleted = 0 OR IsDeleted IS NULL)
        AND (AffectedDate = @EffectiveDate)
    )
    BEGIN
        UPDATE dbo.EmployeeManagementGrade WITH (ROWLOCK, UPDLOCK)
        SET 
            IsPromotion = @IsPromotion,
            Note = @Note,
            OldManagementGradeId = @OldManagementGradeId,
			IsCurrentUsed = 1,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE 
            EmpId = @EmpId 
            AND ManagementGradeId = @ManagementGradeId
            AND (IsDeleted = 0 OR IsDeleted IS NULL)
            AND (AffectedDate = @EffectiveDate)
        SELECT @ErrCode = 0, @ErrMsg = ''
        RETURN
    END
    
    ELSE
	BEGIN
		INSERT INTO dbo.EmployeeManagementGrade(
			EmpId
			,ManagementGradeId
			,OldManagementGradeId
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
			,@ManagementGradeId
			,@OldManagementGradeId
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
