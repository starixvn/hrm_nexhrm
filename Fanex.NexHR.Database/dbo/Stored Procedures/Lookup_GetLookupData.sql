
/****** Object:  Stored Procedure dbo.Lookup_GetLookupData    Script Date: 3/13/2016 3:21:41 PM ******/


-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Lookup_GetLookupData]
    -- Add the parameters for the stored procedure here
    @lookupCode varchar(20)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    declare @lookupId int
    select @lookupId = Id from Lookup where Code = @lookupCode
    
    select * from LookupData where LookupId = @lookupId
    order by [Name]
END



