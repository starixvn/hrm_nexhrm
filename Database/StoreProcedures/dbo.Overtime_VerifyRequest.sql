USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_VerifyRequest]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_VerifyRequest]
    @Id					INT
	,@ModifiedBy		INT
    ,@OvertimeSheet		NVARCHAR (MAX)
    ,@ErrCode			INT                 = 0 OUTPUT
    ,@ErrMsg			VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20200904Dwight.Tran
Task: Enhance NEX Request.
DB: NexHRM
Original: 

Revisions:   
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @EmployeeId			INT
        ,@Date				DATE
		,@NexdHour			NUMERIC (18, 1)
		,@OldNexMinute		INT
		,@VerifiedStatus	INT = 4
        ,@DocHandler		INT;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[NexHour]      NUMERIC (18, 1)
    );

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND e.Roles IN (3, 4) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to verify NEX request'

		RETURN;
	END;

    EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeSheet;
     
    INSERT INTO #OvertimeSheet
    SELECT
        [EmployeeId]
        ,[Date]
        ,[NexHour]
    FROM OPENXML(@DocHandler, '/Root/OvertimeRecord', 1)
    WITH (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[NexHour]      NUMERIC (18, 1)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        WHILE (
            EXISTS (
                SELECT 1
                FROM #OvertimeSheet
            )
        )
        BEGIN
            SELECT TOP (1)
                @EmployeeId = [EmployeeId]
                ,@Date      = [Date]
				,@NexdHour	= [NexHour]
            FROM #OvertimeSheet;
			IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.OvertimeYearly WITH (NOLOCK)
					WHERE [Year] = YEAR(@date) AND [EmpId] = @EmployeeId
					)
				)
			BEGIN
				IF (NOT EXISTS (
						SELECT 1
						FROM dbo.OvertimeYearly WITH (NOLOCK)
						WHERE EmpId = @EmployeeId
							AND [Year] = (YEAR(@Date) - 1)
						))
				BEGIN
					INSERT INTO dbo.OvertimeYearly (
						EmpId
						,[Year]
						,[Minutes]
						,[ModifiedBy]
						,[ModifiedDate]
						)
					SELECT @EmployeeId
						,YEAR(@Date)
						,@NexdHour * 60
						,@ModifiedBy
						,GETDATE()
				END
				ELSE
				BEGIN
					INSERT INTO dbo.OvertimeYearly (
						EmpId
						,[Year]
						,[Minutes]
						,[ModifiedBy]
						,[ModifiedDate]
						)
					SELECT @EmployeeId
						,YEAR(@Date)
						,ISNULL(prev.[Minutes], 0) + (@NexdHour * 60)
						,@ModifiedBy
						,GETDATE()
					FROM (
						SELECT	EmpId
								,[Minutes]
						FROM dbo.OvertimeYearly WITH (NOLOCK)
						WHERE ([Year] = YEAR(@date) - 1) AND [EmpId] = @EmployeeId
						) AS prev
					WHERE NOT EXISTS (
							SELECT 1
							FROM dbo.OvertimeYearly WITH (NOLOCK)
							WHERE EmpId = prev.EmpId
								AND [Year] = YEAR(@Date)
							)
					END
			END

			IF EXISTS (
				SELECT 1 FROM [dbo].[OvertimeTrans]
				WHERE [EmpId] = @EmployeeId
					AND [OvertimeId] = @Id
					AND [Date] = @Date
			)
			BEGIN
				SELECT @OldNexMinute = [Minutes]
				FROM [dbo].[OvertimeTrans] WITH (NOLOCK)
				WHERE [EmpId] = @EmployeeId
					AND DATEDIFF(DAY, @Date, [Date]) = 0
					AND [OvertimeId] = @Id

				UPDATE [dbo].[OvertimeTrans] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]	= @NexdHour * 60
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = GETDATE()
				WHERE [EmpId] = @EmployeeId
					AND DATEDIFF(DAY, @Date, [Date]) = 0
					AND [OvertimeId] = @Id;

				UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]       = [Minutes] + @NexdHour * 60 - @OldNexMinute
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = GETDATE()
				WHERE [EmpId] = @EmployeeId
					AND [Year] >= YEAR(@Date)
			END
			ELSE
			BEGIN
				INSERT INTO dbo.OvertimeTrans(
						OvertimeId
						,EmpId
						,[Date]
						,[Minutes]
						,[Type]
						,ModifiedBy
						,ModifiedDate
						)
						SELECT 
						@Id
						,@EmployeeId
						,@Date
						,@NexdHour * 60
						,1
						,@ModifiedBy
						,GETDATE()

				UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]       = [Minutes] + @NexdHour * 60
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = GETDATE()
				WHERE [EmpId] = @EmployeeId
					AND [Year] >= YEAR(@Date)
			END
			UPDATE [dbo].[OvertimeDetail] WITH (ROWLOCK, UPDLOCK)
			SET [NexHour] = @NexdHour
			WHERE 
				[EmployeeId] = @EmployeeId
				AND DATEDIFF(DAY, @Date, [Date]) = 0
				AND [OvertimeId] = @Id

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, [Date], @Date) = 0;
        END;

		UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
		SET
			[Status] = @VerifiedStatus,
			[ModifiedBy] = @ModifiedBy,
			[ModifiedDate] = GETDATE()
		WHERE [OvertimeId] = @Id

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Verify NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;



GO
