CREATE OR ALTER PROCEDURE [dbo].[Promotion_Insert] 
    @EmpId INT
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
Task: Insert Promotion
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
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
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Promotion has already exists.'

        RETURN
    END

	IF (dbo.fnc_GetClientDateTime() >= @EffectiveDate)
    BEGIN
        UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
		SET 
			IsCurrentUsed = 0
		WHERE EmpId = @EmpId  AND dbo.fnc_GetClientDateTime() >= AffectedDate AND  @EffectiveDate > AffectedDate
    END

    INSERT INTO HRM.dbo.EmployeeJobGrade(
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
		,CASE
			WHEN @EffectiveDate > dbo.fnc_GetClientDateTime() THEN 0
			ELSE 1
			END
		,@Note
		,NULL
		,1
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
