
CREATE FUNCTION [dbo].[fnc_CheckValidAnnualLeaveBalanceByRequest] (
	@EmpId INT
	,@ToDate DATETIME
	,@TotalDayOfRequest NUMERIC(18, 1)
	,@RequestId BIGINT = NULL
	)
RETURNS BIT
AS
BEGIN
	DECLARE @IsValid BIT
		,@LimitLeaveBalanceAfterRequest NUMERIC(18, 1)
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

	-- SELECT @MinLeaveDayMustHaveAfterRequest = CASE WHEN MONTH(@ToDate) < 7 THEN (7 - MONTH(@ToDate)) ELSE 0 END

	SET @IsValid = CASE WHEN @LeaveDayBalance - @TotalDayOfRequest - @LimitLeaveBalanceAfterRequest >= 0 THEN 1 ELSE 0 END

	RETURN @IsValid
END