
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetRangeDate    Script Date: 3/13/2016 3:21:40 PM ******/

-- =============================================
-- Author:        <Author,,Quinn.vo>
-- Create date: <Create Date,,2012-08-09>
-- Description:    <Description,,get work days in range date>
-- =============================================
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetRangeDate] 
    -- Add the parameters for the stored procedure here
    @EmpId INT
    ,@fromDate DATETIME
    ,@toDate DATETIME
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT COUNT(1) AS TotalDay
    FROM dbo.EmployeeCalendar WITH (NOLOCK)
    WHERE EmpID = @EmpId
        AND WorkDate BETWEEN @fromDate
            AND @toDate
        AND IsHoliday = 0
        AND IsOffDay = 0
        AND IsLeaveDay = 0
END

