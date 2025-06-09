USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_UpdateConversion]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[Overtime_UpdateConversion]
    @modifiedBy     INT
    ,@id            INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate	DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
    Created: 20200723@Dwight.Tran
    Task: Enhance NEX request
    DB: NexHRM
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldYearlyMinutes   INT
        ,@empId				INT
        ,@year              INT = Year(@deductDate)
		,@oldYear			INT
        ,@oldMinutes        INT
		,@HRCode    VARCHAR (2) = 'HR-ADM';

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = 'COR'
                OR eg.[Code] = 'MGT'
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to update OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId = [EmpId]
		,@oldYear = Year([DeductDate])
		,@oldMinutes = [Deduct]
    FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    SELECT @oldYearlyMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly] as oty WITH (NOLOCK)
    WHERE oty.[EmpId] = @empId
        AND oty.[Year] = @year;

    IF (@oldYearlyMinutes + @oldMinutes < @deduct * 60.0)
    BEGIN
        SELECT
            @errCode = 4
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		UPDATE [dbo].[OvertimeConversion] WITH (ROWLOCK, UPDLOCK)
		SET
			[Deduct]        = @deduct * 60
			,[DeductDate] = @deductDate
			,[Description]  = @description
			,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			,[ModifiedBy] = @modifiedBy
		WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes - (@deduct * 60.0)
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

		IF(@oldYear < @year)
		BEGIN
			UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]       = [Minutes] + @oldMinutes
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			WHERE [EmpId] = @empId
				AND [Year] >= @oldYear AND [Year] < @year;
		END;
		COMMIT TRANSACTION;
        
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Update OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO
