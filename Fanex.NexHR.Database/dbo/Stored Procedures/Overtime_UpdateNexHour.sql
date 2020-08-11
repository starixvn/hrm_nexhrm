CREATE PROCEDURE [dbo].[Overtime_UpdateNexHour]
    @modifiedBy     INT
	,@overtimeId	INT
    ,@employeeId    INT
    ,@date          DATE
    ,@adjustHour    NUMERIC (18, 1)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 20200318@Merlin.Le
Task: Enhance Nex Leave
DB: NexHRM
Original: 

Revisions:  20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldAdjustMinutes   INT			= 0
        ,@HRCode            VARCHAR (2) = 'HR';

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Code] = @HRCode
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to adjust NEX hour';

        RETURN;
    END;
    
    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[OvertimeDetail_138194] WITH (NOLOCK)
        WHERE [EmployeeId] = @employeeId
            AND DATEDIFF(DAY, @date, [Date]) = 0
			AND [OvertimeId] = @overtimeId
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'NEX Request not exist';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

		IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.OvertimeYearly WITH (NOLOCK)
                WHERE [Year] = YEAR(@date) AND [EmpId] = @employeeId
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
			SELECT @employeeId
				,YEAR(@Date)
				,ISNULL(prev.[Minutes], 0)
				,@ModifiedBy
				,GETDATE()
			FROM (
				SELECT	EmpId
						,[Minutes]
				FROM dbo.OvertimeYearly WITH (NOLOCK)
				WHERE ([Year] = YEAR(@date) - 1) AND [EmpId] = @employeeId
				) AS prev
			WHERE NOT EXISTS (
					SELECT 1
					FROM dbo.OvertimeYearly WITH (NOLOCK)
					WHERE EmpId = prev.EmpId
						AND [Year] = YEAR(@date)
					)
		END


		IF EXISTS (
			SELECT 1 FROM [dbo].[OvertimeTrans]
			WHERE [EmpId] = @employeeId
				AND [OvertimeId] = @overtimeId
				AND [Date] = @date
		)
		BEGIN
			SELECT @oldAdjustMinutes = [Minutes]
			FROM [dbo].[OvertimeTrans] WITH (NOLOCK)
			WHERE [EmpId] = @employeeId
				AND DATEDIFF(DAY, @date, [Date]) = 0
				AND [OvertimeId] = @overtimeId

			UPDATE [dbo].[OvertimeTrans] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]	= @adjustHour * 60
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = GETDATE()
			WHERE [EmpId] = @employeeId
				AND DATEDIFF(DAY, @date, [Date]) = 0
				AND [OvertimeId] = @overtimeId;

			UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]       = [Minutes] + @adjustHour * 60 - @oldAdjustMinutes
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = GETDATE()
			WHERE [EmpId] = @employeeId
				AND [Year] >= YEAR(@date)
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
					@overtimeId
					,@employeeId
					,@date
					,@adjustHour * 60
					,1
					,@modifiedBy
					,GETDATE()

			UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]       = [Minutes] + @adjustHour * 60
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = GETDATE()
			WHERE [EmpId] = @employeeId
				AND [Year] >= YEAR(@date)
		END

		UPDATE [dbo].[OvertimeDetail_138194] WITH (ROWLOCK, UPDLOCK)
		SET
			[Verified] = 1
			,[NexHour] = @adjustHour
		Where [EmployeeId] = @employeeId
			AND [OvertimeId] = @overtimeId
			AND DATEDIFF(DAY, @date, [Date]) = 0

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Update NEX hour fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;