USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_GetWeekWorking]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWeekWorking] 
	-- Add the parameters for the stored procedure here
	@id int,
	@date datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from EmployeeCalendar
	where EmpId = @id and datediff(week,WorkDate,@date)=0
	order by WorkDate
END

GO
