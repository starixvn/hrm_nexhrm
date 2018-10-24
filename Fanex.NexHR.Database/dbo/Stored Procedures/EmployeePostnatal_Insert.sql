
/****** Object:  Stored Procedure dbo.EmployeePostnatal_Insert    Script Date: 3/13/2016 3:21:41 PM ******/

-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    26/02/2016
-- Task:            Implement "Employee Postnatal"
-- =============================================
CREATE PROCEDURE [dbo].[EmployeePostnatal_Insert] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@Minutes INT
    ,@Type tinyint
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeePostnatal ep
                WHERE ep.EmpId = @EmpId
                    AND ep.FromDate = @FromDate
                    AND ep.ToDate = @ToDate
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This request has already existed !'

        RETURN
    END

    INSERT INTO dbo.EmployeePostnatal (
        EmpId
        ,FromDate
        ,ToDate
        ,ModifiedBy
        ,ModifiedDate
        ,[Minutes]
        ,[Type]
        )
    VALUES (
        @EmpId
        ,@FromDate
        ,@ToDate
        ,@ModifiedBy
        ,GETDATE()
        ,@Minutes
        ,@Type
        );

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create new Employee Postnatal failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;
