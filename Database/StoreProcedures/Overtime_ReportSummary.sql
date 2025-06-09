CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportSummary] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@CreatedBy INT = NULL
    ,@Status INT = 0
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions: 20200903Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT  ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedById
        ,e.NickName AS CreatedByNickName
        ,e.FullName AS CreatedByFullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );

    SELECT otd.OvertimeId
        ,otd.[Hour]
		,otd.[Date]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
        INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );
END;

GO
