
CREATE PROCEDURE [dbo].[Department_Insert] 
    @Code VARCHAR(20)
    ,@Name VARCHAR(100)
    ,@ParentId INT = NULL
    ,@ManagerId INT = NULL
    ,@CreatedBy INT
    ,@CreatedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Department.
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
            EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE Code = @Code
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Department code ' + @Code + ' has already exists'

        RETURN
    END

    SELECT @DepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @ParentId

    IF(@DepartmentLevel IS NULL)
    BEGIN
        SET @DepartmentLevel = 0
    END
    ELSE
    BEGIN
        SET @DepartmentLevel = @DepartmentLevel + 1
    END

    INSERT INTO dbo.Department(
        [Code]
        ,[Name]
        ,[ParentId]
        ,[ManagerId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[IsDisabled]
        ,DepartmentLevel
        )
    VALUES (
        @Code
        ,@Name
        ,@ParentId
        ,@ManagerId
        ,@CreatedBy
        ,@CreatedDate
        ,0
        ,@DepartmentLevel
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END

