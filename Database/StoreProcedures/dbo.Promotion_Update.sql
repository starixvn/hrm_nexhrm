USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Promotion_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [dbo].[Promotion_Update] 
    @Id INT
	,@EmpId INT
    ,@JobGradeId INT
	,@OldJobGradeId INT
    ,@EffectiveDate DATETIME
	,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Update a promotion
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
	IF (
            EXISTS (
                SELECT 1
                FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
                WHERE EmpId = @EmpId 
				AND ((OldJobGradeId = @OldJobGradeId AND JobGradeId =  @JobGradeId ) OR @EffectiveDate = AffectedDate)
				AND (IsDeleted = 0 OR IsDeleted IS NULL)
				AND IsPromotion = 1
				AND Id <> @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Promotion has already exists.'

        RETURN
    END

	
	IF (GETDATE() >= @EffectiveDate)
    BEGIN
        UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
		SET 
			IsCurrentUsed = 0
		WHERE EmpId = @EmpId AND Id <> @Id AND GETDATE() >= AffectedDate AND  @EffectiveDate > AffectedDate
    END

    UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
    SET 
		JobGradeId = @JobGradeId
		,OldJobGradeId = @OldJobGradeId
		,AffectedDate = @EffectiveDate
		,IsCurrentUsed = CASE
							WHEN @EffectiveDate > GETDATE() THEN 0
							ELSE 1
							END
		,Note = @Note
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = GETDATE()
    WHERE Id = @Id
  
    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update promotion failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END

GO
