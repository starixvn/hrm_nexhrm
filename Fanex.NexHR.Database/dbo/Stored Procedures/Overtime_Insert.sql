
/****** Object:  Stored Procedure dbo.Overtime_Insert    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_Insert] @Date DATETIME
    ,@CreatedBy INT
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
    SET NOCOUNT ON;

    DECLARE @OTRequestId INT
        ,@EmployeeId INT
        ,@Hour NUMERIC(18, 1);

    CREATE TABLE #OverTimeSheet (
        EmployeeId INT
        ,[Hour] NUMERIC(18, 1)
        );

    IF (
            NOT EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                    LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON g.[Id] = e.GroupId
                WHERE e.Id = @CreatedBy
                    AND (
                       e.Roles like '%1%'
						OR e.Roles like '%2%'
						OR e.Roles like '%4%'
                        OR g.Code = 'PM'
                        )
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'PM team, Assistant Leader, Leader must have a permission to create the OT request';

        RETURN;
    END;

    INSERT INTO #OverTimeSheet
    SELECT SUBSTRING(val, 1, CHARINDEX(':', val) - 1)
        ,SUBSTRING(val, CHARINDEX(':', val) + 1, 20)
    FROM dbo.[fnc_strSplit](@OverTimeSheet, ',');

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Overtime (
            [Date]
            ,[Description]
            ,[Status]
            ,CreatedBy
            ,CreatedDate
            )
        VALUES (
            @Date
            ,@Description
            ,@Status
            ,@CreatedBy
            ,GETDATE()
            );

        SELECT @OTRequestId = SCOPE_IDENTITY();

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
                            LEFT JOIN dbo.Overtime AS ot WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
                        WHERE EmployeeId = @EmployeeId
                            AND [Date] = @Date
							AND ot.Status IN (1, 2)
                        )
                    )
            BEGIN
                SELECT @ErrCode = 2
                    ,@ErrMsg = 'Employee ' + NickName + ' already has worked over time on the day'
                FROM Employee
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
                @OTRequestId
                ,@EmployeeId
                ,@Hour
                );

            DELETE
            FROM #OverTimeSheet
            WHERE EmployeeId = @EmployeeId;
        END;

        IF(@Status = 2)
        BEGIN
             EXECUTE dbo.Overtime_InsertTransaction @OTRequestId, @Date, @CreatedBy
        END

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Insert Overtime request failed';

        ROLLBACK TRANSACTION;
    END CATCH;

    DROP TABLE #OverTimeSheet;
END;

