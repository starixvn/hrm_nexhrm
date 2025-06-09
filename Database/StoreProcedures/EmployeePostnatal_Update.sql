USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeePostnatal_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.EmployeePostnatal_Update    Script Date: 3/13/2016 3:21:41 PM ******/

-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    26/02/2016
-- Task:            Implement "Employee Postnatal"
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_Update] @OldEmpId INT
    ,@OldFromDate DATETIME
    ,@OldToDate DATETIME
    ,@NewEmpId INT
    ,@NewFromDate DATETIME
    ,@NewToDate DATETIME
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
                WHERE ep.EmpId = @OldEmpId
                    AND ep.FromDate = @OldFromDate
                    AND ep.ToDate = @OldToDate
                )
            )
    BEGIN
        UPDATE dbo.EmployeePostnatal
        SET dbo.EmployeePostnatal.EmpId = @NewEmpId
            ,dbo.EmployeePostnatal.FromDate = @NewFromDate
            ,dbo.EmployeePostnatal.ToDate = @NewToDate
            ,dbo.EmployeePostnatal.ModifiedBy = @ModifiedBy
            ,dbo.EmployeePostnatal.ModifiedDate = dbo.fnc_GetClientDateTime()
            ,dbo.EmployeePostnatal.Minutes = @Minutes
            ,dbo.EmployeePostnatal.Type = @Type
        WHERE dbo.EmployeePostnatal.EmpId = @OldEmpId
            AND dbo.EmployeePostnatal.FromDate = @OldFromDate
            AND dbo.EmployeePostnatal.ToDate = @OldToDate

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
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'Cannot find this Employee Postnatal !'
    END
END;

GO
