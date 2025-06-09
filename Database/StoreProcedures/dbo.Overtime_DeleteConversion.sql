USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_DeleteConversion]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[Overtime_DeleteConversion]
    @modifiedBy     INT
    ,@id            INT
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
        @empId     INT
        ,@year          INT
        ,@oldMinutes    INT
		,@HRCode    VARCHAR (2) = 'HR-ADM'

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
            ,@errMsg = 'Only role HR have a permission to delete OT conversion';

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
        @empId     = [EmpId]
        ,@year          = Year([DeductDate])
        ,@oldMinutes    = [Deduct]
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

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [dbo].[OvertimeConversion]
        WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = GETDATE()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Delete OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO
