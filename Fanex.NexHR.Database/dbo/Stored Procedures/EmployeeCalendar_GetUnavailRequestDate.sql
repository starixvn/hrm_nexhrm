
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetUnavailRequestDate    Script Date: 3/13/2016 3:21:40 PM ******/

-- =============================================
-- Author:        <Author,,Quinn.vo>
-- Create date: <Create Date,,2012-08-09>
-- Description:    <Description,,get list days which is not Work>
-- =============================================
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetUnavailRequestDate] -- 245, '6/1/2014', '9/30/2014'
    -- Add the parameters for the stored procedure here
    @EmpId int,
    @fromDate datetime,
    @toDate datetime
AS
BEGIN
    ---- SET NOCOUNT ON added to prevent extra result sets from
    ---- interfering with SELECT statements.
    --SET NOCOUNT ON;

 --   -- Insert statements for procedure here
    --SELECT WorkDate 
    --FROM dbo.EmployeeCalendar WITH(NOLOCK)
    --WHERE EmpID= @EmpId AND WorkDate BETWEEN @fromDate AND @toDate
    --AND (IsHoliday = 1 OR IsOffDay = 1 OR (IsLeaveDay = 1 and LeaveDay=1))
    
    SET NOCOUNT ON;
 DECLARE @MaxDate datetime, @MinDate datetime,@fromDate_LC datetime,@toDate_LC datetime

 CREATE TABLE #tmpCalendar(WorkDate DATETIME)

 SELECT @fromDate=CONVERT(NVARCHAR(20),@fromDate,111),@toDate=CONVERT(NVARCHAR(20),@toDate,111)

 SELECT  @fromDate_LC = @fromDate  , @toDate_LC = @toDate
 WHILE (@fromDate_LC<=@toDate_LC)
  BEGIN
   INSERT INTO #tmpCalendar
   VALUES (@fromDate_LC)

   SET @fromDate_LC = DATEADD(DD,1,@fromDate_LC)
  END

 DELETE #tmpCalendar
 WHERE WorkDate IN (
  SELECT WorkDate 
  FROM dbo.EmployeeCalendar WITH(NOLOCK)
  WHERE (WorkDate BETWEEN @fromDate AND @toDate)
   AND EmpID= @EmpId 
   AND NOT (IsOffDay = 1 OR IsHoliday = 1 OR ShiftId IS NULL OR (IsLeaveDay = 1 AND LeaveDay = 1))
   -- AND NOT (((IsHoliday = 1 OR IsOffDay = 1) AND ShiftId IS NULL) OR (IsLeaveDay = 1 and LeaveDay=1))
  )
 
 SELECT WorkDate 
 FROM #tmpCalendar

 DROP TABLE #tmpCalendar
END


