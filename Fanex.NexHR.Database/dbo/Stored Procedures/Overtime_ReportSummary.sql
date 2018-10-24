
/****** Object:  Stored Procedure dbo.Overtime_ReportSummary    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_ReportSummary] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@CreatedBy INT = NULL
    ,@Status INT = 0
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ot.OvertimeId AS Id
        ,ot.[Date] AS OnDate
        ,ot.[Status]
        ,ot.[Description]
        ,ot.CreatedBy AS CreatedId
        ,e.NickName
        ,e.FullName
    FROM dbo.Overtime AS ot WITH (NOLOCK)
        LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE ot.[Status] IN (1, 2)
       AND @FromDate <= ot.[Date]
        AND ot.[Date] <= @ToDate
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );

    SELECT otd.OvertimeId AS Id
        ,otd.[Hour]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.OverTime AS ot WITH (NOLOCK)
        INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE ot.[Status] IN (1, 2)
       AND @FromDate <= ot.[Date]
        AND ot.[Date] <= @ToDate
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );
        --SELECT ot.Id,
        --       ot.[Date] AS OnDate,
        --       ot.[Status],
        --       ot.[Description],
        --       Total.Hour AS Hour,
        --       Total.Member AS TotalMember,
        --       ot.CreatedBy AS CreatedId,
        --       e1.NickName AS CreatedNickName,
        --       e1.FullName AS CreatedFullName
        --FROM OvertimeRequest ot WITH (NOLOCK)
        --     LEFT JOIN Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        --     LEFT JOIN
        --(
        --    SELECT OvertimeRequestId,
        --           SUM(Hour) AS Hour,
        --           COUNT(*) AS Member
        --    FROM OvertimeDetail WITH (NOLOCK)
        --    GROUP BY OvertimeRequestId
        --) AS Total ON Total.OvertimeRequestId = ot.Id
        --WHERE @FromDate <= ot.[Date]
        --      AND ot.[Date] <= @ToDate
        --      AND (@Status = 0
        --           OR ot.[Status] = @Status)
        --      AND (@CreatedBy IS NULL
        --           OR ot.CreatedBy = @CreatedBy);
END;

