CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetNumberRequestInMonth]
    @empId INT
    ,@fromDate DATETIME
    ,@toDate DATETIME
    ,@requestType INT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get numbers request in month.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    DECLARE @NumberRequestHalfDay NUMERIC(5, 1), @NumberRequest NUMERIC(5, 1)
    IF (@requestType = 1)
    BEGIN
        SELECT @NumberRequest = COUNT(1) 
        FROM dbo.WorkingTimeRequest WITH (NOLOCK)
        WHERE EmpId = @empId
        AND RequestType IN (1, 2)
        AND [Status] IN (1, 2)
        AND DATEDIFF (D, @fromDate, FromDate) >= 0
        AND DATEDIFF (D, FromDate, @toDate) >= 0
    END
    ELSE IF (@requestType = 6)
    BEGIN
        SELECT @NumberRequestHalfDay = SUM(TotalDays)
        FROM dbo.WorkingTimeRequest WITH (NOLOCK)
        WHERE EmpId = @empId
        AND RequestType = 6
        AND [Status] IN (1, 2)
        AND TotalDays = 0.5
        AND DATEDIFF (D, @fromDate, FromDate) >= 0
        AND DATEDIFF (D, ToDate, @toDate) >= 0

        SELECT @NumberRequest = COUNT(1)
        FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) 
            ON (DATEDIFF (D, FromDate, cl.WorkDate) >= 0
            AND DATEDIFF (D, cl.WorkDate, ToDate) >= 0)
            AND wk.EmpId = cl.EmpId
        WHERE wk.EmpId = @empId
        AND RequestType = 6
        AND [Status] IN (1, 2)
        AND TotalDays <> 0.5
        AND DATEDIFF (D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF (D, cl.WorkDate, @toDate) >= 0
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1

        SELECT @NumberRequest = (ISNULL(@NumberRequestHalfDay, 0) + ISNULL(@NumberRequest, 0))
    END

    SELECT ISNULL(@NumberRequest, 0) AS NumberRequest
END

GO
