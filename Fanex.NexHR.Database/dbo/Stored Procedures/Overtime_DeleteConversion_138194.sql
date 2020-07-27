CREATE PROCEDURE [dbo].[Overtime_DeleteConversion_138194]
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
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @empId     INT
        ,@year          INT
        ,@oldMinutes    INT
		,@HRCode    VARCHAR (2) = 'HR'
        ,@MGTCode   VARCHAR (3) = 'MGT';

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = @MGTCode
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = '1'
            ,@errMsg = 'Only role HR have a permission to delete OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion_138194] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = '1'
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId     = [EmpId]
        ,@year          = Year([DeductDate])
        ,@oldMinutes    = [Deduct]
    FROM [dbo].[OvertimeConversion_138194] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = '3'
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [dbo].[OvertimeConversion_138194]
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
            @errCode = '1000'
            ,@errMsg = 'Delete OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;