CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetOpenAndCloseBalance] @OpenDate DATETIME
	,@CloseDate DATETIME
AS
/*
Created: 20141229@Quinn.Vo
Task: Get current is open and close balance. 
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsClosedBalance BIT
		,@IsOpenedBalance BIT

	SET @IsClosedBalance = 0
	SET @IsOpenedBalance = 0

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveTrans WITH (NOLOCK)
				WHERE FromDate = @OpenDate
					AND IsOpenBalance = 1
				)
			)
	BEGIN
		SET @IsOpenedBalance = 1
	END

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveTrans WITH (NOLOCK)
				WHERE FromDate = @CloseDate
					AND IsCloseBalance = 1
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wk.EmpId = e.Id AND e.isDeleted = 0
				WHERE YEAR(FromDate) = YEAR(@CloseDate)
					AND RequestType = 6
					AND LeaveTypeId IN (2, 30) 
					AND [Status] = 1
				)
			)
	BEGIN
		SET @IsClosedBalance = 1
	END

	SELECT @IsOpenedBalance AS IsOpenedBalance
		,@IsClosedBalance AS IsClosedBalance
END
GO
