CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Insert] @WorkDate DATETIME
    ,@Type TINYINT
    ,@Minutes TINYINT
    ,@EmpId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.LateInEarlyOut lieo
                WHERE lieo.WorkDate = @WorkDate
                    AND lieo.Type = @Type
                    AND lieo.EmpId = @EmpId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This request has already existed !';

        RETURN;
    END;

    INSERT INTO dbo.LateInEarlyOut (
        --Id - this column value is auto-generated
        WorkDate
        ,[Type]
        ,[Minutes]
        ,EmpId
        ,ModifiedBy
        ,ModifiedDate
        )
    VALUES (
        @WorkDate
        ,@Type
        ,--1: LateIn - 2: EarlyOut
        @Minutes
        ,@EmpId
        ,@ModifiedBy
        ,dbo.fnc_GetClientDateTime()
        );

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create Late in/Early out failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;

GO
