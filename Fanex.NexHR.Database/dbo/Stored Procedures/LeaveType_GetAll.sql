
/****** Object:  Stored Procedure dbo.LeaveType_GetAll    Script Date: 3/13/2016 3:21:41 PM ******/
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LeaveType_GetAll]
    @ExcludeNotShown bit = null
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    select * from LeaveType
    where (@ExcludeNotShown is null or isShown=1)
    order by [name] 
END

