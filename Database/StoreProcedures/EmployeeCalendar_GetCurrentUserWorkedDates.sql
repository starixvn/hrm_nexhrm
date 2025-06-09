USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_GetCurrentUserWorkedDates]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetCurrentUserWorkedDates]
    @UserId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
	Created: 20180614@Oliver.Dang
	Task: Get employee working dates by date range.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250327@Henry: Update MGT Code -> MGT/COR code
		- 20250506@Henry: Add HR-ADM group code
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsNotShow BIT = 0

	IF(
		EXISTS (
			SELECT 1 FROM 
				dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
			WHERE e.Id = @UserId
				AND 
					(e.Roles LIKE '%2%'
						OR e.Roles LIKE '%3%'
						OR e.Roles LIKE '%4%'
						OR g.Code IN ('HR', 'EXP','FA','HR-ADM','PM', 'MGT', 'COR', 'ITNW')
					)
		)
	)
	BEGIN
		SET @IsNotShow = 1
	END

	CREATE TABLE #EmployeeCalendar2HalfDay
	(
		WorkDate DATE
	)

	INSERT INTO #EmployeeCalendar2HalfDay(WorkDate)
	SELECT CAST(rq.FromDate AS DATE)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
	WHERE rq.EmpId = @UserId
		AND rq.[Status] IN (1, 2)
		AND rq.TotalDays = 0.5
	GROUP BY rq.EmpId
		,CAST(rq.FromDate AS DATE)
	HAVING SUM(TotalDays) = 1

    SELECT cl.WorkDate
    FROM  dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
		AND cl.WorkDate BETWEEN rq.FromDate AND rq.ToDate
		AND rq.[Status] = 1
		AND rq.TotalDays >= 1
	LEFT JOIN #EmployeeCalendar2HalfDay AS hd ON cl.WorkDate = hd.WorkDate
    WHERE cl.EmpId = @UserId
		AND (cl.WorkDate between @FromDate and @ToDate)
		AND (cl.IsOffDay <> 1 AND ISNULL(cl.LeaveDay, 0) < 1 AND cl.IsHoliday <> 1)
		AND rq.Id IS NULL
		AND hd.WorkDate IS NULL
		AND @IsNotShow = 0

	DROP TABLE #EmployeeCalendar2HalfDay
END

SET ANSI_NULLS ON
GO
