
CREATE PROCEDURE [dbo].[Overtime_DeleteTransactionFromLeaveRequest] @RequestId INT
	,@EmployeeId INT
	,@Date DATE
	,@TotalDays NUMERIC(18, 1)
	,@ModifiedBy INT
AS
/*
Created: 20160314@Quinn.Vo
Task: Delete OT transaction from Leave request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.OvertimeTrans
	WHERE LeaveConversionId = @RequestId

	UPDATE dbo.OvertimeYearly
	SET [Minutes] = ([Minutes] + (@TotalDays * 480))
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE [Year] >= YEAR(@Date)
		AND EmpId = @EmployeeId
END
