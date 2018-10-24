/****** Object:  Stored Procedure dbo.Overtime_DeleteTransaction    Script Date: 3/13/2016 9:45:23 AM ******/

CREATE PROCEDURE [dbo].[Overtime_DeleteTransaction] @OvertimeId INT
	,@Date DATE
	,@ModifiedBy INT
AS
/*
Created: 20160309@Quinn.Vo
Task: Delete OT transaction.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #OvertimeDetail (
		EmpId INT
		,[Date] DATE
		,[Minutes] INT
		)

	INSERT INTO #OvertimeDetail (
		EmpId
		,[Date]
		,[Minutes]
		)
	SELECT otd.EmployeeId
		,ot.[Date]
		,otd.[Hour] * 60
	FROM dbo.Overtime AS ot WITH (NOLOCK)
		INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	WHERE ot.OvertimeId = @OvertimeId

	DELETE
	FROM dbo.OvertimeTrans
	WHERE OvertimeId = @OvertimeId

	UPDATE oty
	SET oty.[Minutes] = (oty.[Minutes] - otd.[Minutes])
	FROM #OvertimeDetail AS otd
		INNER JOIN dbo.OvertimeYearly AS oty WITH (UPDLOCK, ROWLOCK) ON oty.EmpId = otd.EmpId
		AND [Year] >= YEAR(@Date)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.OvertimeYearly WITH (NOLOCK)
			WHERE EmpId = otd.EmpId
				AND [Year] = YEAR(@Date)
			)

	DROP TABLE #OvertimeDetail
END