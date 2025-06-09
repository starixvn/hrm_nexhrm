USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeCalendar_GetShiftByDate]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetShiftByDate]
	-- Add the parameters for the stored procedure here
	@employeeId int,
	@date datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from Shift
	where Id = (select top(1) ShiftId from EmployeeCalendar
				where	EmpId = @employeeId and
						DATEDIFF(day,WorkDate,@date)=0)
END

GO
