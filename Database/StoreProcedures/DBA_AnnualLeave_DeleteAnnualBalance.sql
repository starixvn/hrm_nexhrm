CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_DeleteAnnualBalance]
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
BEGIN
	DELETE dbo.LeaveTrans
        WHERE (EmpId = @EmpID OR @EmpID IS NULL) AND FromDate= @FromDate AND CreditDays > 0    AND [Description] = @Description AND RequestId IS NULL
END
GO
