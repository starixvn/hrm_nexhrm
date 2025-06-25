CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetById] @Id INT
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

    SELECT ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedById
        ,e.NickName AS CreatedByNickName
        ,e.FullName AS CreatedByFullName
    FROM dbo.[Overtime] ot WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE ot.OvertimeId = @Id;

    SELECT otd.OvertimeId AS Id
        ,otd.[Hour]
		,otd.[Date]
		,otd.[NexHour]
		,otd.Note
		,otd.EmployeeId RecordEmployeeId
        ,otd.EmployeeId 
        ,e.NickName
        ,e.FullName
    FROM dbo.[OvertimeDetail] otd WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE otd.OvertimeId = @Id;
END;

GO
