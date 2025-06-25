CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_GetAll] @EmpId INT
    ,@Year INT
AS
BEGIN
    SELECT ep.EmpId
        ,e.NickName
        ,e.FullName
        ,ep.FromDate
        ,ep.ToDate
        ,e2.NickName AS 'ModifiedBy'
        ,ep.ModifiedDate
        ,ep.Minutes
        ,ep.Type
    FROM dbo.EmployeePostnatal ep
        JOIN dbo.Employee e ON ep.EmpId = e.Id
        LEFT JOIN dbo.Employee e2 ON ep.ModifiedBy = e2.Id
    WHERE (
            @EmpId = -1
            OR ep.EmpId = @EmpId
            )
        AND (
            @Year = - 1
            OR (
                YEAR(ep.FromDate) = @Year
                OR YEAR(ep.ToDate) = @Year
                )
            )
	ORDER BY ep.ToDate DESC
END;

GO
