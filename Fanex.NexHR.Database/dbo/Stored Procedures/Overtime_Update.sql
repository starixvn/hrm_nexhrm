
/****** Object:  Stored Procedure dbo.Overtime_Update    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_Update]
    @Id                 INT
    ,@ModifiedBy        INT
    ,@Description       NVARCHAR(4000)
    ,@OverTimeSheet     VARCHAR(MAX)
    ,@ErrCode           INT                 = 0 OUTPUT
    ,@ErrMsg            VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200302Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
		@EmployeeId     INT
        ,@Date          DATE
        ,@FromTime      VARCHAR (8)
        ,@ToTime        VARCHAR (8)
        ,@Hour          NUMERIC (18, 1)
		,@NexHour		NUMERIC	(18, 1)
        ,@Note          NVARCHAR (250)
        ,@DocHandler    INT
        ,@MGTCode       VARCHAR (3)     = 'MGT'
        ,@PendingStatus INT             = 1;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
		,[NexHour]		NUMERIC	(18, 1)
        ,[Note]         NVARCHAR (250)
    );

	IF (
		NOT EXISTS (
			SELECT 1 FROM [dbo].[Overtime_138194] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'NEX request not exist';

        RETURN;
	END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Overtime_138194] ot WITH (NOLOCK)
            WHERE
				ot.[CreatedBy] = @ModifiedBy
				AND ot.[OvertimeId] = @Id
        )
    )
    BEGIN
        SELECT
            @ErrCode = 2
            ,@ErrMsg = 'You only have a permission to update your NEX request';

        RETURN;
    END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Overtime_138194] ot WITH (NOLOCK)
            WHERE ot.[OvertimeId] = @Id
                AND ot.[Status] = @PendingStatus
        )
    )
    BEGIN
        SELECT
            @ErrCode = 3
            ,@ErrMsg = 'You only can update pending NEX request';

        RETURN;
    END;

    EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeSheet;
     
    INSERT INTO #OvertimeSheet
    SELECT
        [EmployeeId]
        ,[Date]
        ,[Hour]
		,[NexHour]
        ,[Note]
    FROM OPENXML(@DocHandler, '/Root/OvertimeRecord', 1)
    WITH (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
		,[NexHour]		NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[Overtime_138194] WITH (ROWLOCK, UPDLOCK)
        SET
			[Description] = @Description
            ,[ModifiedBy] = @ModifiedBy
            ,[ModifiedDate] = GETDATE()
		WHERE [OvertimeId] = @Id

		DELETE FROM [dbo].[OvertimeDetail_138194]
			WHERE [OvertimeId] = @Id

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
                ,@Hour      = [Hour]
				,@NexHour	= [NexHour]
                ,@Note      = [Note]
            FROM #OvertimeSheet;

            IF (
                EXISTS (
                    SELECT 1
                    FROM [dbo].[OverTimeDetail_138194] AS otd WITH (NOLOCK)
                        LEFT JOIN [dbo].[Overtime_138194] AS ot WITH (NOLOCK) ON ot.[OvertimeId] = otd.[OvertimeId]
                    WHERE otd.[EmployeeId] = @EmployeeId
						AND DATEDIFF(DAY, @Date, otd.[Date]) = 0
                        AND ot.Status IN (1, 2)
                )
            )
            BEGIN
                SELECT
                    @ErrCode = 4
                    ,@ErrMsg = 'Employee ' + NickName + ' has already worked over time on ' + CONVERT(VARCHAR, @Date, 106)
                FROM Employee WITH (NOLOCK)
                WHERE Id = @EmployeeId;

                ROLLBACK TRANSACTION;
                RETURN;
            END;

            INSERT INTO [dbo].[OvertimeDetail_138194] (
                [OvertimeId]
                ,[EmployeeId]
                ,[Date]
                ,[Hour]
				,[NexHour]
                ,[Note]
				,[Verified]
            )
            VALUES (
                @Id
                ,@EmployeeId
                ,@Date
                ,@Hour
				,@NexHour
                ,@Note
				,0
            );

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, @Date, [Date]) = 0
        END;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Update NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;

