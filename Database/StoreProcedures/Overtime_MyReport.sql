CREATE OR ALTER PROCEDURE [dbo].[Overtime_MyReport]
    @FromDate       DATETIME
    ,@ToDate        DATETIME
    ,@EmployeeId    INT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions: 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT
        ot.OvertimeId   AS Id
        ,otd.[Date]      AS OnDate
        ,ot.[Status]
        ,ot.[Description]
        ,otd.[Hour]
        ,otd.EmployeeId
		,e.NickName 
        ,e.FullName
        ,e1.NickName AS CreatedBy

    FROM dbo.[Overtime] ot WITH (NOLOCK)
        INNER JOIN dbo.Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        INNER JOIN dbo.[OvertimeDetail] otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON otd.EmployeeId = e.Id
    WHERE ot.[Status] IN (1, 2)
		AND DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND otd.EmployeeId = @EmployeeId;
END;
GO
