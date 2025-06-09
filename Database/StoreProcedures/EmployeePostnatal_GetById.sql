USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeePostnatal_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.EmployeePostnatal_GetById    Script Date: 3/13/2016 3:21:41 PM ******/

-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    26/02/2016
-- Task:            Implement "Employee Postnatal"
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_GetById] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
BEGIN
    SELECT ep.EmpId
        ,e.NickName
        ,ep.FromDate
        ,ep.ToDate
        ,ep.ModifiedBy
        ,ep.ModifiedDate
        ,ep.Minutes
        ,ep.Type
    FROM dbo.EmployeePostnatal ep
        JOIN dbo.Employee e ON ep.EmpId = e.Id
    WHERE ep.EmpId = @EmpId
        AND ep.FromDate = @FromDate
        AND ep.ToDate = @ToDate
END;

GO
