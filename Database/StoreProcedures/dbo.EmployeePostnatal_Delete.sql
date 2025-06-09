USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeePostnatal_Delete]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    26/02/2016
-- Task:            Implement "Employee Postnatal"
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_Delete] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
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
        DELETE dbo.EmployeePostnatal
        WHERE dbo.EmployeePostnatal.EmpId = @EmpId
            AND dbo.EmployeePostnatal.FromDate = @FromDate
            AND dbo.EmployeePostnatal.ToDate = @ToDate

        IF @@ERROR <> 0
        BEGIN
            SELECT @ErrCode = 1
                ,@ErrMsg = N'Delete Employee Postnatal failed.';
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
