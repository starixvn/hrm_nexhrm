
/****** Object:  Stored Procedure dbo.LookupData_Insert    Script Date: 3/13/2016 3:21:42 PM ******/


-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LookupData_Insert]
    @LookupCode VARCHAR(20),
    @Name NVARCHAR(100),
    @ErrMsg NVARCHAR(500) = '',
    @ErrCode INT = null        
AS
BEGIN

    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @lookupId int
    SELECT @lookupId = Id FROM Lookup WHERE Code = @LookupCode    

    IF (EXISTS(SELECT * FROM LookupData l WITH (NOLOCK)                 
            WHERE l.LookupId=@lookupId AND l.Name=@Name))
    BEGIN
        SELECT @ErrCode=100,
                @ErrMsg = 'The job title already exist'
        RETURN
    END

    INSERT LookupData(LookupId,Code,Name,Value)
    VALUES(@lookupId,NULL,@Name,NULL)

    SET @ErrCode=0
END



