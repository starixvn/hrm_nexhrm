
CREATE FUNCTION [dbo].[fnc_GetAnnualLeaveBalanceByDate] (
	@EmpId INT
	,@ToDate DATETIME
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 1)
AS
BEGIN
	DECLARE @LimitLeaveBalanceAfterRequest NUMERIC(18, 1)
		,@LeaveDayBalance NUMERIC(18, 1)
		,@CreditDays NUMERIC(18, 1)
		,@LeaveDayOfPendingRequest NUMERIC(18, 1)
		,@AnnualLeaveType INT = 2
		,@UnplannedLeaveType INT = 30

	SELECT @LeaveDayOfPendingRequest = SUM(TotalDays)
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND [Status] = 1
		AND RequestType = 6
		AND (
			LeaveTypeID = @AnnualLeaveType
			OR LeaveTypeID = @UnplannedLeaveType
			)
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND (@RequestId IS NULL OR id <> @RequestId)

	SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0)
		,@CreditDays = SUM(ISNULL(CreditDays, 0))
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND ISNULL(Deleted, 0) = 0
		AND YEAR(FromDate) = YEAR(@ToDate)

	SELECT @LimitLeaveBalanceAfterRequest = @CreditDays - (FLOOR(@CreditDays * MONTH(@ToDate) / 12) + 5)
	IF (@LimitLeaveBalanceAfterRequest < 0)
	BEGIN
		SET @LimitLeaveBalanceAfterRequest = 0
	END

	SET @LeaveDayBalance = @LeaveDayBalance - @LimitLeaveBalanceAfterRequest

	IF(@LeaveDayBalance < 0)
	BEGIN
		SET @LeaveDayBalance = 0
	END

	RETURN ISNULL(@LeaveDayBalance, 0)
END