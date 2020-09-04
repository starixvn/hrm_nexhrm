
/****** Object:  Stored Procedure dbo.Overtime_GetById    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_GetById] @Id INT
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

    SELECT ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime_138194] ot WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE ot.OvertimeId = @Id;

    SELECT otd.OvertimeId AS Id
        ,otd.[Hour]
		,otd.[Date]
		,otd.[NexHour]
		,otd.Note
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.[OvertimeDetail_138194] otd WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE otd.OvertimeId = @Id;
END;

