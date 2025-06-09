USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_GetGroupOfEmployee]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroupOfEmployee]
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
GO
