USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LeaveDeduction_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveDeduction_Delete] 
	@LeaveDeductionId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Leave deduction.
DB: NexEPA
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
	DECLARE @OldTotalDays NUMERIC(10, 1), @OldIsAnnual BIT, @EmployeeId INT, @DeductDate DATETIME

	SELECT @OldTotalDays = LeaveDays, @OldIsAnnual = IsAnnual, @EmployeeId = EmpId, @DeductDate = [Date]
	FROM dbo.LeaveConversion WITH (NOLOCK)
	WHERE Id = @LeaveDeductionId 

	SET XACT_ABORT ON
	BEGIN TRAN
	BEGIN TRY
		DELETE FROM dbo.LeaveConversion WHERE Id = @LeaveDeductionId
		DELETE FROM dbo.LateInEarlyOutTrans WHERE LeaveDeductionId = @LeaveDeductionId
		DELETE FROM dbo.LeaveTrans WHERE LeaveConversionId = @LeaveDeductionId

		UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
		SET [Minutes] = [Minutes] + (@OldTotalDays * 8 * 60), ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
		WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@DeductDate)

		COMMIT
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK

			SELECT @ErrCode = 100
				,@ErrMsg = 'There is an error when deleting leave deduction.'
		END
	END CATCH
END
GO
