SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.Report_GetLateInEarlyOutDetail    Script Date: 3/13/2016 3:21:42 PM ******/

ALTER PROCEDURE [dbo].[Report_GetLateInEarlyOutDetail] --'2015-08-01', '2015-08-31'
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
    ,@EmployeeId INT = NULL
    ,@IncludeLeaveDeduction BIT = 0
AS
/*
    Created: 20150805@Quinn.Vo
    Task: Get late in/ early out detail report
    DB: NexEPA
    Original: 

    Revisions:  
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
        la.WorkDate
        ,la.[Minutes]
        ,la.[Type] AS TypeId
        ,la.LeaveDeductionId
        ,la.ModifiedDate
        ,la.EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.Name
    FROM dbo.LateInEarlyOutTrans AS la WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON la.EmpId = e.Id
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
    WHERE 
        e.GroupId NOT IN (1081,40)
        AND DATEDIFF(D, @FromDate, la.WorkDate) >= 0
        AND DATEDIFF(D, la.WorkDate, @ToDate) >= 0
        AND (@IncludeLeaveDeduction = 1 OR la.[Type] <> 0)
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
END

GO
