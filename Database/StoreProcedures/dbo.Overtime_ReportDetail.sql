USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_ReportDetail]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportDetail]
    @FromDate       DATETIME
    ,@ToDate        DATETIME
    ,@Status        INT
    ,@CreatedBy     INT = NULL
    ,@EmployeeId    INT = NULL
	,@OvertimeId	INT = NULL
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions:
    - 20200227@Merlin.Le: Enhance NEX leave
	- 20200903Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT
        ot.OvertimeId 
        ,otd.[Date]          AS OnDate
        ,ot.[Status]
        ,ot.[Description]
        ,otd.[Hour]
		,otd.[Note]
        ,otd.EmployeeId
        ,e2.NickName
        ,e2.FullName
        ,e1.NickName AS CreatedBy
    FROM dbo.[Overtime] ot WITH (NOLOCK)
        LEFT JOIN dbo.OvertimeDetail otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        LEFT JOIN Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        LEFT JOIN Employee e2 WITH (NOLOCK) ON e2.Id = otd.EmployeeId
    WHERE
		(
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
        )
		AND (
			(@OvertimeId IS NULL AND DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
			AND DATEDIFF(DAY, @ToDate, otd.[Date]) <=0)
			OR otd.OvertimeId = @OvertimeId
		);
END;



GO
