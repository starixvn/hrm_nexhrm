
CREATE PROCEDURE [dbo].[AnnualLeave_GetTransByEmp] @EmployeeId INT
    ,@Year INT
    ,@excludedRequestTrans BIT
AS
/*
Created: 2012/05/29@Dan
Task: show annual leave tranasaction by employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT t.ID AS LeaveTransactionId
        ,RequestID
        ,FromDate AS AffectedFrom
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS [ToTalDays]
        ,Description
        ,t.CreatedDate
        ,t.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.LeaveTrans AS t WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = t.EmpID
    WHERE EmpId = @EmployeeId
        AND YEAR(fromdate) = @Year
        AND ISNULL(Deleted, 0) = 0
        AND (
            @excludedRequestTrans = 0
            OR RequestId IS NULL
            )
END

