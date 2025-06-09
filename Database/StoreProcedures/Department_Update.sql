USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Department_Update]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_Update] 
    @DepartmentId    INT
    ,@Code VARCHAR(20)
    ,@Name VARCHAR(100)
    ,@ParentId INT = NULL
    ,@ManagerId INT = NULL
    ,@IsDisabled BIT = NULL
    ,@ModifiedBy INT
    ,@ModifiedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Department.
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

    DECLARE @DepartmentLevel INT,
        @OldDepartmentLevel INT,
        @SubLevel INT

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE Code = @Code
                AND DepartmentId <> @DepartmentId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Department code' + @Code + ' has already exists'

        RETURN
    END

    SELECT @DepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @ParentId

    SELECT @OldDepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @DepartmentId

    IF(@DepartmentLevel IS NULL)
    BEGIN
        SET @DepartmentLevel = 0
    END
    ELSE
    BEGIN
        SET @DepartmentLevel = @DepartmentLevel + 1
    END

    UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
    SET 
        [Code] = @Code,
        [Name] = @Name,
        [ParentId] = @ParentId,
        [ManagerId] = @ManagerId,
        [DepartmentLevel] = @DepartmentLevel,
        [IsDisabled] = @IsDisabled,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedDate] = @ModifiedDate
    WHERE DepartmentId = @DepartmentId

    SET @SubLevel = @DepartmentLevel - @OldDepartmentLevel
    
    UPDATE dp 
    SET [DepartmentLevel] = (d.DepartmentLevel + @SubLevel)
    FROM dbo.Department AS dp WITH (ROWLOCK, UPDLOCK)
    INNER JOIN dbo.RelatedDepartment(@DepartmentId) AS d ON dp.DepartmentId = d.DepartmentId

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update department failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END

GO
