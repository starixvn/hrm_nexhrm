
/****** Object:  Stored Procedure dbo.LateInEarlyOut_GetAllByEmpId    Script Date: 3/13/2016 3:21:41 PM ******/
-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    19/02/2016
-- Description:    Task "LateIn/EarlyOut Notification"
-- =============================================
CREATE PROCEDURE [dbo].[LateInEarlyOut_GetAllByEmpId] @EmpId INT,
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
                 END) 'Type',
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
