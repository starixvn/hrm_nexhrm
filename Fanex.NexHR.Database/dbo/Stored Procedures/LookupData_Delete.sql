
/****** Object:  Stored Procedure dbo.LookupData_Delete    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[LookupData_Delete] @Id INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160224@Quinn.Vo
Task: Get lookup data.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                WHERE e.JobTitleId = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The employee is not empty so cannot delete it'

        RETURN
    END

    DELETE LookupData
    WHERE Id = @Id

    SET @ErrCode = 0
END

