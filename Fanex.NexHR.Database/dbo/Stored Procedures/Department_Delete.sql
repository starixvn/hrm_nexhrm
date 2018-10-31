
CREATE PROCEDURE [dbo].[Department_Delete] 
    @DepartmentId    INT
    ,@ModifiedBy INT
    ,@ModifiedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140610@Quinn.Vo
Task: delete Department.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DepartmentLevel INT

    IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE DepartmentId = @DepartmentId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department does not exists '

        RETURN
    END

    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Department WITH (NOLOCK)
            WHERE [ParentId] = @DepartmentId
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department has child departments '

        RETURN
    END
    
    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Department AS dep WITH (NOLOCK)
            INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON dep.DepartmentId = gr.DepartmentId
            WHERE dep.DepartmentId = @DepartmentId AND gr.Deleted != 1
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department has teams '

        RETURN
    END
    
    DELETE FROM dbo.Department
    WHERE DepartmentId = @DepartmentId

	SELECT @ErrCode = 0, @ErrMsg = ''
END

