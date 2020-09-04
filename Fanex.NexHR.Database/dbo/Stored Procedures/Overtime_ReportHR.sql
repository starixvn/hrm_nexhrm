
/****** Object:  Stored Procedure dbo.Overtime_ReportSummary    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_ReportHR] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@CreatedBy INT = NULL
    ,@Status INT = 0
AS
/*
Created: 20200903Dwight.Tran
Task: Enhance NEX Request.
DB: NexHRM
Original:

Revisions: 
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF OBJECT_ID('#TempOvertime') IS NOT NULL
	BEGIN
		DROP TABLE #TempOvertime
	END

	CREATE TABLE #TempOvertime (
		Id INT
		,[Status] NVARCHAR (250)
		,[Description] NVARCHAR (250)
		,[CreatedDate] DATE
		,[CreatedId] INT
		,[NickName] NVARCHAR (250)
		,[FullName] NVARCHAR (250)
		)

	INSERT INTO #TempOvertime (
		Id 
		,[Status] 
		,[Description] 
		,[CreatedDate] 
		,[CreatedId] 
		,[NickName] 
		,[FullName]
		)
    SELECT DISTINCT  ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime_138194] AS ot WITH (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail_138194] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            (@Status = 0
            OR ot.[Status] = @Status)
			AND ot.[Status] IN (2, 4, 5)	--approved, verified, void
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );

	SELECT Id 
		,[Status] 
		,[Description] 
		,[CreatedDate] 
		,[CreatedId] 
		,[NickName] 
		,[FullName]
	FROM #TempOvertime

    SELECT otd.OvertimeId
        ,otd.[Hour]
		,otd.[Date]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime_138194] AS ot WITH (NOLOCK)
		INNER JOIN #TempOvertime as temp WITH (NOLOCK) ON ot.[OvertimeId] = temp.[Id]
        INNER JOIN dbo.[OvertimeDetail_138194] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId

	DROP TABLE #TempOvertime
END;