
/****** Object:  Stored Procedure dbo.Overtime_MyReport    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_MyReport] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT
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
        ,e.NickName
        ,e.FullName
        ,ot.CreatedBy AS CreatedId
        ,e1.NickName
        ,e1.FullName
    FROM dbo.Overtime ot WITH (NOLOCK)
        INNER JOIN dbo.Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        INNER JOIN dbo.OvertimeDetail otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON otd.EmployeeId = e.Id
    WHERE ot.[Status] IN (1, 2)
       AND @FromDate <= ot.[Date]
        AND ot.[Date] <= @ToDate
        AND otd.EmployeeId = @EmployeeId;
END;

