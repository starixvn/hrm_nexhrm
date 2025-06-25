CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetById] @Id INT
AS
BEGIN
    SELECT lieo.Id,
                e.NickName,
                lieo.Type,
                lieo.WorkDate,
                lieo.Minutes,
                lieo.ModifiedBy,
                lieo.ModifiedDate
         FROM dbo.LateInEarlyOut lieo
              JOIN dbo.Employee e ON lieo.EmpId = e.Id
         WHERE lieo.Id = @Id;
END

GO
