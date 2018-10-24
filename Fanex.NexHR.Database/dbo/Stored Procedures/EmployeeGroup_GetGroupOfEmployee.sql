-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[EmployeeGroup_GetGroupOfEmployee]
	-- Add the parameters for the stored procedure here
	@employeeId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from EmployeeGroup
	where Id= (	select top(1) GroupId from Employee 
				where Id = @employeeId)
END
