
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_DeleteAnnualBalance]
    @Year INT,
    @EmpID INT=NULL,
    @FromDate datetime,

    @Description NVARCHAR(1000)='Add annual leave days for',
    @ErrCode int = 0 OUTPUT,
    @ErrMsg varchar(200) = '' OUTPUT    
AS
/*
Created: Quinn.Vo@2014.12.26
Task: delete annual balance leave.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET @Description = 'Add annual leave days for ' + CONVERT(NVARCHAR(10),@Year) + '.'
BEGIN TRY
    BEGIN TRANSACTION;

        UPDATE e 
        SET AnnualLeaveBalance = e.AnnualLeaveBalance - lt.CreditDays
        FROM dbo.Employee AS e 
        INNER JOIN LeaveTrans lt 
            ON e.Id =  lt.EmpId AND FromDate= @FromDate AND ISNULL(lt.Deleted,0) = 0
        WHERE (e.Id = @EmpID OR @EmpID IS NULL)
        AND lt.[Description] = @Description
        AND lt.RequestId IS NULL
        
        DELETE dbo.LeaveTrans
        WHERE (EmpId = @EmpID OR @EmpID IS NULL) AND FromDate= @FromDate AND CreditDays > 0    AND [Description] = @Description AND RequestId IS NULL
        
    COMMIT TRANSACTION;
    SELECT @ErrCode=0,@ErrCode=''
END TRY
BEGIN CATCH
            ROLLBACK TRANSACTION;
            SELECT @ErrCode = 100, @ErrMsg = N'delete annual balance failed.'
            RETURN
END CATCH;
