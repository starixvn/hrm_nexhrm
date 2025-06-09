CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_InsertTransaction] 
    @EmpId INT
    ,@WorkDate DATE
    ,@LateIn INT
    ,@EarlyOut INT
AS
/*
Created: 20150805@Quinn.Vo
Task: Insert late in/early out transaction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @OldLateIn INT, @OldEarlyOut INT, @MinutesPreviousYear INT

    IF(@LateIn > 60 OR @EarlyOut > 60)
    BEGIN
        SELECT @LateIn = 0, @EarlyOut = 0
    END
    ELSE
    BEGIN
        IF (@LateIn > 0)
        BEGIN
            IF(@LateIn <= 30)
            BEGIN
                SET @LateIn = 30
            END
            ELSE
            BEGIN 
                SET @LateIn = 60
            END
        END

        IF (@EarlyOut > 0)
        BEGIN
            IF(@EarlyOut <= 30)
            BEGIN
                SET @EarlyOut = 30
            END
            ELSE
            BEGIN 
                SET @EarlyOut = 60
            END
        END
    END

    SELECT @OldLateIn = [Minutes]
    FROM dbo.LateInEarlyOutTrans WITH (NOLOCK)
    WHERE EmpId = @EmpId 
    AND DATEDIFF(DD, WorkDate, @WorkDate) = 0
    AND [Type] = 1
	AND ISNULL(IsManual, 0) = 0

    SELECT @OldEarlyOut = [Minutes]
    FROM dbo.LateInEarlyOutTrans WITH (NOLOCK)
    WHERE EmpId = @EmpId 
    AND DATEDIFF(DD, WorkDate, @WorkDate) = 0
    AND [Type] = 2
	AND ISNULL(IsManual, 0) = 0

    SELECT @OldLateIn = ISNULL(@OldLateIn, 0), @OldEarlyOut = ISNULL(@OldEarlyOut, 0)
    IF(ISNULL(@OldLateIn, 0) <> ISNULL(@LateIn, 0) OR ISNULL(@OldEarlyOut, 0) <> ISNULL(@EarlyOut, 0))
    BEGIN
        IF(ISNULL(@OldLateIn, 0) <> ISNULL(@LateIn, 0))
        BEGIN
            IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmpId AND [Year] = YEAR(@WorkDate)))
            BEGIN
                UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
                SET [Minutes] = [Minutes] - @OldLateIn + @LateIn, ModifiedDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @EmpId AND [Year] >= YEAR(@WorkDate)
            END
            ELSE
            BEGIN
                SELECT @MinutesPreviousYear = [Minutes]
                FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
                WHERE EmpId = @EmpId 
                    AND [Year] = YEAR(@WorkDate) - 1

                INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
                VALUES (@EmpId, YEAR(@WorkDate), (ISNULL(@MinutesPreviousYear, 0) - @OldLateIn + @LateIn), dbo.fnc_GetClientDateTime())
            END

            DELETE FROM dbo.LateInEarlyOutTrans 
			WHERE EmpId = @EmpId 
			AND WorkDate = @WorkDate 
			AND [Type] = 1
			AND ISNULL(IsManual, 0) = 0
            
            IF(@LateIn <> 0)
            BEGIN
                INSERT INTO dbo.LateInEarlyOutTrans (EmpId, WorkDate, [Type], [Minutes], ModifiedDate)
                VALUES (@EmpId, @WorkDate, 1, @LateIn, dbo.fnc_GetClientDateTime())
            END
        END
        
        IF(ISNULL(@OldEarlyOut, 0) <> ISNULL(@EarlyOut, 0))
        BEGIN
            IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmpId AND [Year] = YEAR(@WorkDate)))
            BEGIN
                UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
                SET [Minutes] = [Minutes] - @OldEarlyOut + @EarlyOut, ModifiedDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @EmpId AND [Year] >= YEAR(@WorkDate)
            END
            ELSE
            BEGIN
                SELECT @MinutesPreviousYear = [Minutes]
                FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
                WHERE EmpId = @EmpId 
                    AND [Year] = YEAR(@WorkDate) - 1

                INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
                VALUES (@EmpId, YEAR(@WorkDate), (ISNULL(@MinutesPreviousYear, 0) - @OldEarlyOut + @EarlyOut), dbo.fnc_GetClientDateTime())
            END

            DELETE FROM dbo.LateInEarlyOutTrans 
			WHERE EmpId = @EmpId 
			AND WorkDate = @WorkDate 
			AND [Type] = 2
			AND ISNULL(IsManual, 0) = 0

            IF(@EarlyOut <> 0)
            BEGIN
                INSERT INTO dbo.LateInEarlyOutTrans (EmpId, WorkDate, [Type], [Minutes], ModifiedDate)
                VALUES (@EmpId, @WorkDate, 2, @EarlyOut, dbo.fnc_GetClientDateTime())
            END
        END
    END
END

GO
