
/****** Object:  Stored Procedure dbo.EmployeeCalendar_Update    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE PROCEDURE [dbo].[EmployeeCalendar_Update] @EmployeeId INT
    ,@Date DATETIME
    ,@ShiftId INT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*  
Created: 2012/05/10@Dan  
Task: Update working calendar for an employee in a day  
DB: NexHR  
Original:   
  
Revisions:      
Param's Explanation:  
 @EmpId: employee ID  
 @date: the date to update  
 @ShiftID: shift id (null if not work)  
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @fromdate DATETIME
        ,@todate DATETIME
        ,@isHoliday BIT

    SET @Date = CONVERT(VARCHAR(10), @Date, 111)

    UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
    SET @isHoliday = CASE 
            WHEN EXISTS (
                    SELECT 1
                    FROM dbo.Holiday
                    WHERE WorkDate BETWEEN FromDate
                            AND ToDate
                    ) THEN 1
            ELSE 0
            END
        ,IsHoliday = CASE 
            WHEN @ShiftId IS NULL THEN @isHoliday
            ELSE 0
            END
        ,ShiftId = @ShiftId
        ,isOffDay = CASE 
            WHEN @ShiftID IS NULL
                AND @isHoliday = 0
                AND LeaveTypeId IS NULL THEN 1
            ELSE 0
            END
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = GETDATE()
    WHERE empId = @EmployeeId
        AND DATEDIFF(d, WorkDate, @date) = 0

    EXEC EmployeeAttendance_ProcessingAndInsert @EmployeeId
        ,@Date
        ,@Date
        ,1

      SELECT @ErrCode = 0
END

