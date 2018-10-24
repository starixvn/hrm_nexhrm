
/****** Object:  Stored Procedure dbo.LookupData_Update    Script Date: 3/13/2016 3:21:42 PM ******/


-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LookupData_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @ErrMsg NVARCHAR(500) = '',
    @ErrCode INT = null        
AS
BEGIN

    SET NOCOUNT ON;    

    IF (EXISTS(SELECT * FROM LookupData l WITH (NOLOCK)                 
            WHERE l.Id=@Id))
    BEGIN
        UPDATE LookupData SET Name=@Name
        WHERE Id=@Id
        RETURN
    END
    
    SELECT @ErrCode=100,
                @ErrMsg = 'The job title already exist'
        RETURN
END



