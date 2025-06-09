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
