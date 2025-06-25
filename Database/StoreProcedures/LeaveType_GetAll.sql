CREATE OR ALTER PROCEDURE [dbo].[LeaveType_GetAll]
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

GO
