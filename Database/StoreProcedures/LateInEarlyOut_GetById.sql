USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[LateInEarlyOut_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Stored Procedure dbo.LateInEarlyOut_GetById    Script Date: 3/13/2016 3:21:41 PM ******/
-- =============================================
-- Author:        Harvey.Nguyen
-- Create date:    19/02/2016
-- Description:    Task "LateIn/EarlyOut Notification"
-- =============================================
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
