USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_InsertTransactionFromLeaveRequest]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_InsertTransactionFromLeaveRequest] @RequestId INT
	,@EmployeeId INT
	,@Date DATE
	,@TotalDays NUMERIC(18, 1)
	,@ModifiedBy INT
AS
/*
Created: 20160314@Quinn.Vo
Task: Insert OT transaction from Leave request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.OvertimeTrans (
		EmpId
		,[Date]
		,[Minutes]
		,LeaveConversionId
		,[Type]
		,ModifiedBy
		,ModifiedDate
		)
	SELECT @EmployeeId
		,@Date
		,@TotalDays * 480
		,@RequestId
		,0
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.OvertimeYearly WITH (NOLOCK)
				WHERE [Year] = YEAR(@Date)
					AND EmpId = @EmployeeId
				)
			)
	BEGIN
		UPDATE dbo.OvertimeYearly
		SET [Minutes] = ([Minutes] - (@TotalDays * 480))
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = dbo.fnc_GetClientDateTime()
		WHERE [Year] >= YEAR(@Date)
		AND EmpId = @EmployeeId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.OvertimeYearly (
			EmpId
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@Date)
			,[Minutes] - (@TotalDays * 480)
			,@ModifiedBy
			,dbo.fnc_GetClientDateTime()
		FROM dbo.OvertimeYearly WITH (NOLOCK)
		WHERE EmpId = @EmployeeId
			AND ([Year] = YEAR(@Date) - 1)
	END
END
GO
