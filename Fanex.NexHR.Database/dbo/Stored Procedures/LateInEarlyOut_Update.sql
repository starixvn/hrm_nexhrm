
/****** Object:  Stored Procedure dbo.LateInEarlyOut_Update    Script Date: 3/13/2016 3:21:41 PM ******/

-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    19/02/2016
-- Description:    Task "LateIn/EarlyOut Notification"
-- =============================================
CREATE PROCEDURE [dbo].[LateInEarlyOut_Update] @Id INT
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
        dbo.LateInEarlyOut.ModifiedDate = GETDATE()
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

