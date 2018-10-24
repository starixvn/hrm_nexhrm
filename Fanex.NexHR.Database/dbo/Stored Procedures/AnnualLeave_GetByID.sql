
CREATE PROCEDURE [dbo].[AnnualLeave_GetByID] @LeaveTransactionId INT
AS
/*
Created: 20160223@Quinn.Vo
Task: Get leave transaction by id.
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
    WHERE t.[Id] = @LeaveTransactionId
        AND RequestId IS NULL
        AND ISNULL(Deleted, 0) = 0
END

