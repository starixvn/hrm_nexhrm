CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Update] @Id INT
    ,@WorkDate DATETIME
    ,@Type TINYINT
    ,@Minutes TINYINT
    ,@EmpId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE dbo.LateInEarlyOut
    SET dbo.LateInEarlyOut.WorkDate = @WorkDate
        ,-- date
        dbo.LateInEarlyOut.Type = @Type
        ,-- tinyint
        dbo.LateInEarlyOut.Minutes = @Minutes
        ,-- tinyint
        dbo.LateInEarlyOut.EmpId = @EmpId
        ,-- int
        dbo.LateInEarlyOut.ModifiedBy = @ModifiedBy
        ,-- int
        dbo.LateInEarlyOut.ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE dbo.LateInEarlyOut.Id = @Id;

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Late in/Early out failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;

GO
