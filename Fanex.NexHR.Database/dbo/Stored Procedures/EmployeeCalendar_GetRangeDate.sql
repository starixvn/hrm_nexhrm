CREATE PROCEDURE [dbo].[EmployeeCalendar_GetRangeDate] 
    @EmpId INT
    ,@fromDate DATETIME
    ,@toDate DATETIME
	,@RequestId BIGINT = NULL
AS
/*
Created: 2018/12/10@Quinn.Vo
Task: Get number of days when create/edit leave request.
DB: NexHR
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
   SET NOCOUNT ON;

	DECLARE @TotalDay NUMERIC(5,1)

	IF (@RequestId IS NULL)
	BEGIN
		SELECT @TotalDay = COUNT(1)
		FROM dbo.EmployeeCalendar WITH (NOLOCK)
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN @fromDate
				AND @toDate
			AND IsHoliday = 0
			AND IsOffDay = 0
			AND IsLeaveDay = 0
	END
	ELSE
	BEGIN
		SELECT @TotalDay = COUNT(1)
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN @fromDate
					AND @toDate
				AND IsHoliday = 0
				AND IsOffDay = 0
	END

	SELECT @TotalDay AS TotalDay
END

