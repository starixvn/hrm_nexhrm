CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetAllByEmpId] @EmpId INT,
                                                     @Year  INT
AS
     BEGIN
         SELECT lieo.Id,
                e.NickName,
                (CASE lieo.Type
                     WHEN 1
                     THEN 'Late in'
                     WHEN 2
                     THEN 'Early out'
                 END) 'Types',
                lieo.WorkDate,
                lieo.Minutes,
                lieo.ModifiedBy,
                lieo.ModifiedDate
         FROM dbo.LateInEarlyOut lieo
              JOIN dbo.Employee e ON lieo.EmpId = e.Id
         WHERE lieo.EmpId = @EmpId
               AND YEAR(lieo.WorkDate) = @Year
         ORDER BY lieo.WorkDate DESC;
     END;
GO
