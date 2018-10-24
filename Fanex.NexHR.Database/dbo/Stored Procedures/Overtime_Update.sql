
/****** Object:  Stored Procedure dbo.Overtime_Update    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_Update] @Id INT
    ,@Date DATETIME
    ,@ModifiedBy INT
    ,@Description NVARCHAR(4000)
    ,@Status INT
    ,@OverTimeSheet VARCHAR(MAX)
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    DECLARE @EmployeeId INT
        ,@MinutesPreviousYear INT
        ,@Hour NUMERIC(18, 1)
        ,@CreatedBy INT;

    CREATE TABLE #OverTimeSheet (
        EmployeeId INT
        ,[Hour] NUMERIC(18, 1)
        );

    SELECT @CreatedBy = CreatedBy
    FROM dbo.Overtime WITH (NOLOCK)
    WHERE OvertimeId = @Id;

    IF (
            @CreatedBy = @ModifiedBy
            OR EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                WHERE e.Id = @ModifiedBy
                    AND e.Roles = 4
                )
            )
    BEGIN
        INSERT INTO #OverTimeSheet
        SELECT SUBSTRING(val, 1, CHARINDEX(':', val) - 1)
            ,SUBSTRING(val, CHARINDEX(':', val) + 1, 20)
        FROM dbo.[fnc_strSplit](@OverTimeSheet, ',');

        BEGIN TRY
            BEGIN TRANSACTION;

            UPDATE dbo.Overtime WITH (ROWLOCK, UPDLOCK)
            SET [Date] = @Date
                ,[Description] = @Description
                ,[Status] = @Status
                ,ModifiedBy = @ModifiedBy
                ,ModifiedDate = GETDATE()
            WHERE OvertimeId = @Id;

            -- Delete old data and insert new data
            DELETE
            FROM dbo.OvertimeDetail
            WHERE OvertimeId = @Id;

            WHILE (
                    EXISTS (
                        SELECT 1
                        FROM #OverTimeSheet
                        )
                    )
            BEGIN
                SELECT TOP 1 @EmployeeId = EmployeeId
                    ,@Hour = [Hour]
                FROM #OverTimeSheet;

                IF (
                        EXISTS (
                            SELECT 1
                            FROM dbo.OverTimeDetail AS otd WITH (NOLOCK)
                                INNER JOIN dbo.Overtime AS ot WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
                            WHERE EmployeeId = @EmployeeId
                                AND [Date] = @Date
								AND ot.Status IN (1, 2)
                            )
                        )
                BEGIN
                    SELECT @ErrCode = 3
                        ,@ErrMsg = 'Employee ' + NickName + ' already has worked over time on the day'
                    FROM Employee WITH (NOLOCK)
                    WHERE Id = @EmployeeId;

                    ROLLBACK TRANSACTION;

                    RETURN;
                END;

                INSERT INTO dbo.OverTimeDetail (
                    OvertimeId
                    ,EmployeeId
                    ,[Hour]
                    )
                VALUES (
                    @Id
                    ,@EmployeeId
                    ,@Hour
                    );

                DELETE
                FROM #OverTimeSheet
                WHERE EmployeeId = @EmployeeId;
            END;

            IF (@Status = 2)
            BEGIN
                EXECUTE dbo.Overtime_InsertTransaction @Id
                    ,@Date
                    ,@ModifiedBy
            END

            COMMIT TRANSACTION;
        END TRY

        BEGIN CATCH
            SELECT @ErrCode = 1000
                ,@ErrMsg = 'Update Overtime request failed';

            ROLLBACK TRANSACTION;
        END CATCH;

        DROP TABLE #OverTimeSheet;
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The OT request is only updated by its owner';

        RETURN;
    END;
END;

