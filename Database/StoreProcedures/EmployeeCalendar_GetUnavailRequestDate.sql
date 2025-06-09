USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_GetUnavailRequestDate]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetUnavailRequestDate] -- 245, '6/1/2014', '9/30/2014'
	@EmpId INT
	,@fromDate DATETIME
	,@toDate DATETIME
	,@requestId BIGINT = NULL
AS
/*
Created: 2018/12/10@Quinn.Vo
Task: Get unavailable date when create/edit request.
DB: NexHR
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxDate DATETIME
		,@MinDate DATETIME
		,@fromDate_LC DATETIME
		,@toDate_LC DATETIME

	CREATE TABLE #tmpCalendar (WorkDate DATETIME)

	SELECT @fromDate = CONVERT(NVARCHAR(20), @fromDate, 111)
		,@toDate = CONVERT(NVARCHAR(20), @toDate, 111)

	SELECT @fromDate_LC = @fromDate
		,@toDate_LC = @toDate

	WHILE (@fromDate_LC <= @toDate_LC)
	BEGIN
		INSERT INTO #tmpCalendar
		VALUES (@fromDate_LC)

		SET @fromDate_LC = DATEADD(DD, 1, @fromDate_LC)
	END

	DELETE #tmpCalendar
	WHERE WorkDate IN (
			SELECT WorkDate
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE (
					WorkDate BETWEEN @fromDate
						AND @toDate
					)
				AND EmpID = @EmpId
				AND NOT (
					IsOffDay = 1
					OR IsHoliday = 1
					OR ShiftId IS NULL
					OR (
						IsLeaveDay = 1
						AND LeaveDay = 1
						)
					)
			)

	-- AND NOT (((IsHoliday = 1 OR IsOffDay = 1) AND ShiftId IS NULL) OR (IsLeaveDay = 1 and LeaveDay=1))
	IF (@requestId IS NOT NULL)
	BEGIN
		DELETE #tmpCalendar
		WHERE WorkDate IN (
				SELECT cl.WorkDate
				FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
					AND DATEDIFF(DD, rq.FromDate, cl.WorkDate) >= 0
					AND DATEDIFF(DD, cl.WorkDate, rq.ToDate) >= 0
					AND rq.TotalDays > 0.5
					AND rq.Id = @requestId
				)
	END

	SELECT WorkDate
	FROM #tmpCalendar

	DROP TABLE #tmpCalendar
END

GO
