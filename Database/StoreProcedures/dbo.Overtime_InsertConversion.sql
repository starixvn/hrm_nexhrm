USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_InsertConversion]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[Overtime_InsertConversion]
    @createdBy      INT
    ,@empId    INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate DATETIME
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
        @oldMinutes INT
        ,@HRCode    VARCHAR (2) = 'HR-ADM'
		,@year INT = YEAR(@deductDate);

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @createdBy
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
            ,@errMsg = 'Only role HR have a permission to deduct NEX balance';

        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

	
    SELECT @oldMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly]
    WHERE [EmpId] = @empId
        AND [Year] = @year;

    IF (@oldMinutes < (@deduct * 60.0))
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[OvertimeConversion] (
            [EmpId]
            ,[Deduct]
            ,[DeductDate]
            ,[Description]
			,[ModifiedBy]
			,[ModifiedDate]
        )
        VALUES (
            @empId
            ,@deduct * 60.0
            ,@deductDate
            ,@description
			,@createdBy,
			GETDATE()
        )

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] - (@deduct * 60.0)
            ,[ModifiedBy]   = @createdBy
            ,[ModifiedDate] = GETDATE()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Deduct NEX balance fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO
