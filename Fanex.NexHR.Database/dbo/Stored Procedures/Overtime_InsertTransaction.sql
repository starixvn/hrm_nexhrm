
/****** Object:  Stored Procedure dbo.Overtime_InsertTransaction    Script Date: 3/13/2016 3:21:44 PM ******/

CREATE PROCEDURE [dbo].[Overtime_InsertTransaction] @OvertimeId INT
    ,@Date DATE
    ,@ModifiedBy INT
AS
/*
Created: 20160309@Quinn.Vo
Task: Insert OT transaction.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.OvertimeYearly WITH (NOLOCK)
                WHERE [Year] = YEAR(@Date)
                )
            )
    BEGIN
        INSERT INTO dbo.OvertimeYearly (
            EmpId
            ,[Year]
            ,[Minutes]
            ,[ModifiedBy]
            ,[ModifiedDate]
            )
        SELECT prev.EmpId
            ,YEAR(@Date)
            ,ISNULL(prev.[Minutes], 0)
            ,@ModifiedBy
            ,GETDATE()
        FROM (
            SELECT EmpId
                ,[Minutes]
            FROM dbo.OvertimeYearly WITH (NOLOCK)
            WHERE ([Year] = YEAR(@Date) - 1)
            ) AS prev
        WHERE NOT EXISTS (
                SELECT 1
                FROM dbo.OvertimeYearly WITH (NOLOCK)
                WHERE EmpId = prev.EmpId
                    AND [Year] = YEAR(@Date)
                )
    END

    CREATE TABLE #OvertimeDetail (
        EmpId INT
        ,[Date] DATE
        ,[Minutes] INT
        )

    INSERT INTO #OvertimeDetail (
        EmpId
        ,[Date]
        ,[Minutes]
        )
    SELECT otd.EmployeeId
        ,ot.[Date]
        ,otd.[Hour] * 60
    FROM dbo.Overtime AS ot WITH (NOLOCK)
        INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
    WHERE ot.OvertimeId = @OvertimeId

    INSERT INTO dbo.OvertimeTrans (
        EmpId
        ,[Date]
        ,[Minutes]
        ,OvertimeId
        ,[Type]
        ,ModifiedBy
        ,ModifiedDate
        )
    SELECT EmpId
        ,[Date]
        ,[Minutes]
        ,@OvertimeId
        ,1
        ,@ModifiedBy
        ,GETDATE()
    FROM #OvertimeDetail

    UPDATE oty
    SET oty.[Minutes] = (oty.[Minutes] + otd.[Minutes])
    FROM #OvertimeDetail AS otd
        INNER JOIN dbo.OvertimeYearly AS oty WITH (UPDLOCK, ROWLOCK) ON oty.EmpId = otd.EmpId
        AND [Year] >= YEAR(@Date)
    WHERE EXISTS (
            SELECT 1
            FROM dbo.OvertimeYearly WITH (NOLOCK)
            WHERE EmpId = otd.EmpId
                AND [Year] = YEAR(@Date)
            )

    INSERT INTO dbo.OvertimeYearly (
        EmpId
        ,[Year]
        ,[Minutes]
        ,[ModifiedBy]
        ,[ModifiedDate]
        )
    SELECT otd.EmpId
        ,YEAR(otd.[Date])
        ,ISNULL(prev.[Minutes], 0) + otd.[Minutes]
        ,@ModifiedBy
        ,GETDATE()
    FROM #OvertimeDetail AS otd
        LEFT JOIN (
        SELECT EmpId
            ,[Minutes]
        FROM dbo.OvertimeYearly WITH (NOLOCK)
        WHERE ([Year] = YEAR(@Date) - 1)
        ) AS prev ON otd.EmpId = prev.EmpId
    WHERE NOT EXISTS (
            SELECT 1
            FROM dbo.OvertimeYearly WITH (NOLOCK)
            WHERE EmpId = otd.EmpId
                AND [Year] = YEAR(@Date)
            )

    DROP TABLE #OvertimeDetail
END

