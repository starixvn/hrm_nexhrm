CREATE PROCEDURE [dbo].[Overtime_InsertConversion]
    @createdBy      INT
    ,@empId    INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 20200326@Merlin.Le
Task: Enhance Nex Leave
DB: NexHRM
Original: 

Revisions:  20200723Dwight.Tran: Enhance NEX Request. Get overtime conversion yearly
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldMinutes INT
        ,@HRCode    VARCHAR (2) = 'HR'
        ,@MGTCode   VARCHAR (3) = 'MGT'
		,@year INT = YEAR(@deductDate);

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @createdBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = @MGTCode
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to pay NEX balance';

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
            ,@errMsg = 'NEX balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

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
            ,@errMsg = 'Not enough NEX balance to pay';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[OvertimeConversion_138194] (
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
            ,@errMsg = 'Pay NEX balance fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;