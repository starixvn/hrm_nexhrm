/****** Object:  Stored Procedure dbo.Overtime_DeleteTransaction    Script Date: 3/13/2016 9:45:23 AM ******/

CREATE PROCEDURE [dbo].[Overtime_DeleteTransaction]
	@OvertimeId INT
	,@ModifiedBy INT
AS
/*
Created: 20160309@Quinn.Vo
Task: Delete OT transaction.
DB: NexHRM
Original: 

Revisions:  20200729Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(
		SELECT 1
				FROM [dbo].[OvertimeTrans] WITH (NOLOCK)
				WHERE [OvertimeId] = @OvertimeId
	)
	BEGIN
		IF OBJECT_ID('##OvertimeDetail') IS NOT NULL
		BEGIN
			DROP TABLE #OvertimeDetail
		END

		CREATE TABLE #OvertimeDetail (
			EmpId INT
			,[Date] DATETIME
			,[Minutes] INT
			)

		INSERT INTO #OvertimeDetail (
			EmpId
			,[Date]
			,[Minutes]
			)
		SELECT otd.EmployeeId
			,otd.[Date]
			,otd.[NexHour] * 60
		FROM dbo.[Overtime_138194] AS ot WITH (NOLOCK)
			INNER JOIN dbo.[OvertimeDetail_138194] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
		WHERE ot.OvertimeId = @OvertimeId

		DELETE
		FROM dbo.OvertimeTrans
		WHERE OvertimeId = @OvertimeId

		UPDATE oty
		SET oty.[Minutes] = (oty.[Minutes] - otd.[Minutes])
			,oty.[ModifiedBy] = @ModifiedBy
			,oty.[ModifiedDate] = GETDATE()
		FROM #OvertimeDetail AS otd
			INNER JOIN dbo.OvertimeYearly AS oty WITH (UPDLOCK, ROWLOCK) ON oty.EmpId = otd.EmpId
			AND [Year] >= YEAR(otd.[Date])
		WHERE EXISTS (
				SELECT 1
				FROM dbo.OvertimeYearly WITH (NOLOCK)
				WHERE EmpId = otd.EmpId
					AND [Year] = YEAR(otd.[Date])
				)

		DROP TABLE #OvertimeDetail
	END;
END