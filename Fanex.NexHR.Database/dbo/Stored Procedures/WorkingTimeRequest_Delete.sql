
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Delete    Script Date: 3/13/2016 3:21:43 PM ******/
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
    -- Add the parameters for the stored procedure here
    @id int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    delete WorkingTimeRequest
    where Id = @id and Status != 2
END

