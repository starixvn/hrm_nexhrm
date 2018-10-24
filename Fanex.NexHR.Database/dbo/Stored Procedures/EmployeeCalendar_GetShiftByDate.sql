
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetShiftByDate]
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

