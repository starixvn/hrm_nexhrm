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
