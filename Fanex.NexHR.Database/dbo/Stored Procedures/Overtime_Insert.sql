
/****** Object:  Stored Procedure dbo.Overtime_Insert    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Overtime_Insert]
    @CreatedBy			INT
    ,@Description       NVARCHAR (4000)
    ,@OvertimeSheet		NVARCHAR (MAX)
    ,@ErrCode			INT                 = 0 OUTPUT
    ,@ErrMsg			VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
    - 20200227@Merlin.Le: Enhance NEX leave
	- 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @OTRequestId    INT
        ,@EmployeeId    INT
        ,@Date          DATE
        ,@FromTime      VARCHAR (8)
        ,@ToTime        VARCHAR (8)
        ,@Hour          NUMERIC (18, 1)
		,@NexdHour		NUMERIC (18, 1)
        ,@Note          NVARCHAR (250)
        ,@DocHandler    INT;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
        ,[NexHour]      NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
                LEFT JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON eg.[Id] = e.[GroupId]
            WHERE e.[Id] = @CreatedBy
                AND (
                    e.[Roles] like '%1%'		-- Assistant leader
                    OR e.[Roles] like '%2%'		-- Leader
                    OR e.[Roles] like '%4%'		-- SA
                    OR eg.[Code] = 'PM'
                )
        )
    )
    BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'PM team, Assistant Leader, Leader must have a permission to create the NEX request';

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
        ,[NexHour]      NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Overtime_138194] (
            [Description]
            ,[Status]
            ,[CreatedBy]
            ,[CreatedDate]
        )
        VALUES (
            @Description
            ,1
            ,@CreatedBy
            ,GETDATE()
        );

        SELECT @OTRequestId = SCOPE_IDENTITY();

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
				,@NexdHour	= [NexHour]
                ,@Note      = [Note]
            FROM #OvertimeSheet;

            IF (
                EXISTS (
                    SELECT 1
                    FROM [dbo].[OvertimeDetail_138194] AS otd WITH (NOLOCK)
                        LEFT JOIN [dbo].[Overtime_138194] AS ot WITH (NOLOCK) ON ot.[OvertimeId] = otd.[OvertimeId]
                    WHERE otd.[EmployeeId] = @EmployeeId
						AND DATEDIFF(DAY, otd.[Date], @Date) = 0
                        AND ot.Status IN (1, 2)
                )
            )
            BEGIN
                SELECT
                    @ErrCode = 2
                    ,@ErrMsg = 'Employee ' + NickName + ' has already worked over time on ' + CAST(@Date AS VARCHAR (10))
                FROM Employee WITH (NOLOCK)
                WHERE Id = @EmployeeId;

                ROLLBACK TRANSACTION;
                RETURN;
            END;

            INSERT INTO [dbo].[OverTimeDetail_138194] (
                [OvertimeId]
                ,[EmployeeId]
                ,[Date]
                ,[Hour]
				,[NexHour]
                ,[Note]
				,[Verified]
            )
            VALUES (
                @OTRequestId
                ,@EmployeeId
                ,@Date
                ,@Hour
				,@NexdHour
                ,@Note
				,0
            );

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, [Date], @Date) = 0
        END;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Insert NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;

