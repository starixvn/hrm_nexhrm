
CREATE PROCEDURE [dbo].[WorkingTimeRequest_AvaibleLeaveBalance] 
	@EmpId INT
	,@EffectedDate DATETIME = NULL
	,@RequestId BIGINT = NULL
AS
/*
Created: Quinn.Vo
Task: Get  Avaible Balance Leave From Request ID
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @AnnualLeaveBalance NUMERIC(18, 2)
		,@AnnualLeaveCanBeUsed NUMERIC(18, 2)
		,@EndYear DATETIME

	IF @EffectedDate IS NULL
	BEGIN
		SELECT @EffectedDate = CONVERT(DATETIME, CONVERT(NVARCHAR(10), YEAR(GETDATE())) + '/12/31')
	END

	SET @EndYear = CONVERT(DATETIME, CONVERT(NVARCHAR(10), YEAR(@EffectedDate)) + '/12/31')

	SELECT @AnnualLeaveBalance = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @EndYear, @RequestID)
		,@AnnualLeaveCanBeUsed = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @EffectedDate, @RequestID)

	SELECT MONTH(@EffectedDate) AS [Month]
		,YEAR(@EffectedDate) AS [Year]
		,@AnnualLeaveBalance AS AnnualLeaveBalance
		,@AnnualLeaveCanBeUsed AS AnnualLeaveCanBeUsed
END