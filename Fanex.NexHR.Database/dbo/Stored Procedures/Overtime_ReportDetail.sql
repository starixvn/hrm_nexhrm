
/****** Object:  Stored Procedure dbo.Overtime_ReportDetail    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_ReportDetail] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@Status INT
    ,@CreatedBy INT = NULL
    ,@EmployeeId INT = NULL
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
        ,otd.[Hour]
        ,otd.EmployeeId
        ,e2.NickName
        ,e2.FullName
        ,ot.CreatedBy AS CreatedId
        ,e1.NickName
        ,e1.FullName
    FROM dbo.Overtime ot WITH (NOLOCK)
        LEFT JOIN dbo.OvertimeDetail otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        LEFT JOIN Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        LEFT JOIN Employee e2 WITH (NOLOCK) ON e2.Id = otd.EmployeeId
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
            )
        AND (
            @EmployeeId IS NULL
            OR otd.EmployeeId = @EmployeeId
            );
END;

