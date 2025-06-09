USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Leave_ClearExpiredTransfer]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Leave_ClearExpiredTransfer]
    @Year INT
AS
/*
Created: 20250411@HenryDinh
Task: Clear unsued transfer annual leave  
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransferDesc NVARCHAR(100) = 'Transfer AL from ' + CAST(@Year AS VARCHAR)

    CREATE TABLE #LeaveToClear (
        EmpId INT,
		NickName NVARCHAR(100),
        TotalTransferred NUMERIC(5,1),
        TotalUsed NUMERIC(5,1),
        Remaining NUMERIC(5,1),
        TransferId INT
    )

	;WITH CombinedLeaveTrans AS (
        SELECT lt.Id, EmpId, CreditDays, DeductDays, FromDate, ToDate, Description, Deleted
        FROM NexHRM.dbo.LeaveTrans AS lt WITH (NOLOCK)
		INNER JOIN NexHRM.dbo.Employee AS emp WITH (NOLOCK) ON emp.Id = lt.EmpId
        WHERE ISNULL(Deleted, 0) = 0 AND emp.IsSpecialUser = 0
        UNION ALL
        SELECT lt.Id, EmpId, CreditDays, DeductDays, FromDate, ToDate, Description, Deleted
        FROM LumiHRM.dbo.LeaveTrans AS lt WITH (NOLOCK) 
        INNER JOIN LumiHRM.dbo.Employee AS emp WITH (NOLOCK) ON emp.Id = lt.EmpId
        WHERE ISNULL(Deleted, 0) = 0 AND emp.IsSpecialUser = 0
    )
    INSERT INTO #LeaveToClear (EmpId, NickName, TotalTransferred, TotalUsed, Remaining, TransferId)
    SELECT
        lt.EmpId,
		e.NickName, 
        SUM(ISNULL(lt.CreditDays, 0)) AS TotalTransferred,
        (
            SELECT SUM(ISNULL(DeductDays, 0))
            FROM CombinedLeaveTrans AS used
            WHERE used.EmpId = lt.EmpId
                AND ISNULL(used.Deleted, 0) = 0
                AND used.DeductDays > 0
                AND used.FromDate >= lt.FromDate
                AND used.ToDate <= lt.ToDate
        ) AS TotalUsed,
        0 AS Remaining,
        MAX(lt.Id)
    FROM CombinedLeaveTrans lt
    INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.Id = lt.EmpId 
    WHERE lt.Description = @TransferDesc
		AND ISNULL(e.IsDeleted, 0) = 0
		AND e.Enabled = 1
        AND e.IsSpecialUser = 0
    GROUP BY lt.EmpId, e.NickName, lt.Description, lt.FromDate, lt.ToDate

    UPDATE t
    SET Remaining = ISNULL(TotalTransferred, 0) - ISNULL(TotalUsed, 0)
    FROM #LeaveToClear t

    INSERT INTO dbo.LeaveTrans (
        EmpId,
        RequestId,
        Description,
        FromDate,
        ToDate,
        DeductDays,
        CreditDays,
        Deleted,
        CreatedDate,
        CreatedBy,
		ModifiedDate,
		ModifiedBy,
        IsOpenBalance,
        IsCloseBalance
    )
    SELECT
        EmpId,
        NULL AS RequestId,
        'Auto clear unused transfer AL from ' + CAST(@Year AS VARCHAR),
        '2025-04-01 00:00:00',
        '2025-04-01 00:00:00',
        Remaining,
        0,
        NULL,
        '2025-04-01 00:00:00',
        NULL,
		'2025-04-01 00:00:00',
        NULL,
        0,
        0
    FROM #LeaveToClear
    WHERE Remaining > 0

    DROP TABLE #LeaveToClear
END
GO
