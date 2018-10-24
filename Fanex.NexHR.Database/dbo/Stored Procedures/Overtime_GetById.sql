
/****** Object:  Stored Procedure dbo.Overtime_GetById    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_GetById] @Id INT
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
    FROM dbo.Overtime ot WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE ot.OvertimeId = @Id;

    SELECT otd.OvertimeId AS Id
        ,otd.[Hour]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.OvertimeDetail otd WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE otd.OvertimeId = @Id;
END;

