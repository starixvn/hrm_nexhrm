
/****** Object:  Stored Procedure dbo.EmployeeGroup_Update    Script Date: 3/13/2016 3:21:41 PM ******/

CREATE PROCEDURE [dbo].[EmployeeGroup_Update] @Id INT
    ,@Code VARCHAR(20)
    ,@Name NVARCHAR(50)
    ,@DepartmentId INT = NULL
    ,@ParentId INT = NULL
    ,@LeaderId INT
    ,@AssistantLeaderId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: update group 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup WITH (NOLOCK)
                WHERE Id <> @Id
                    AND (
                        Code = @Code
                        OR NAME = @Name
                        )
                    AND ISNULL(Deleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This team has already existed.'

        RETURN
    END

    IF OBJECT_ID('tempdb..#RoleID') IS NOT NULL
    BEGIN
        DROP TABLE #RoleID
    END

    CREATE TABLE #RoleID (RoleID INT)

    DECLARE @OldLeaderID INT = NULL
        ,@OldAssistantLeaderID INT = NULL
        ,@Roles VARCHAR(20) = NULL
        ,@tmpRoles VARCHAR(20)

    SELECT @OldLeaderID = g.LeaderId
        ,@OldAssistantLeaderID = g.AssistantLeaderId
    FROM [dbo].[EmployeeGroup] g WITH (NOLOCK)
    WHERE g.Id = @Id

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Code = @Code
        ,[Name] = @Name
        ,DepartmentId = @DepartmentId
        ,ParentId = @ParentId
        ,LeaderId = @LeaderId
        ,AssistantLeaderId = @AssistantLeaderId
        ,ModifiedDate = GETDATE()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id

    IF EXISTS (
            SELECT TOP 1 e.Id
            FROM [dbo].[Employee] e WITH (NOLOCK)
            WHERE e.Id = @LeaderId
            )
    BEGIN
        SET @Roles = (
                SELECT TOP 1 e.Roles
                FROM [dbo].[Employee] e WITH (NOLOCK)
                WHERE e.Id = @LeaderId
                )

        IF @Roles IS NOT NULL
        BEGIN
            DELETE
            FROM #RoleID

            INSERT INTO #RoleID (RoleID)
            SELECT val
            FROM fnc_strSplit(@Roles, ',')

            IF NOT EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r WITH (NOLOCK)
                    WHERE r.RoleID = 4
                    )
                AND NOT EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r WITH (NOLOCK)
                    WHERE r.RoleID = 2
                    )
            BEGIN
                SET @tmpRoles = NULL

                INSERT INTO #RoleID (RoleID)
                VALUES (2)

                SELECT @tmpRoles = COALESCE(@tmpRoles + ',', '') + CONVERT(VARCHAR, r.RoleID)
                FROM #RoleID r WITH (NOLOCK)
                ORDER BY r.RoleID

                UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
                SET Roles = @tmpRoles
                WHERE Id = @LeaderId
            END
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
            SET Roles = 2
            WHERE Id = @LeaderId
        END
    END

    IF EXISTS (
            SELECT TOP 1 e.Id
            FROM [dbo].[Employee] e WITH (NOLOCK)
            WHERE e.Id = @AssistantLeaderId
            )
    BEGIN
        SET @Roles = (
                SELECT TOP 1 e.Roles
                FROM [dbo].[Employee] e WITH (NOLOCK)
                WHERE e.Id = @AssistantLeaderId
                )

        IF @Roles IS NOT NULL
        BEGIN
            DELETE
            FROM #RoleID

            INSERT INTO #RoleID (RoleID)
            SELECT val
            FROM fnc_strSplit(@Roles, ',')

            IF NOT EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r WITH (NOLOCK)
                    WHERE r.RoleID = 4
                    )
                AND NOT EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r WITH (NOLOCK)
                    WHERE r.RoleID = 1
                    )
            BEGIN
                SET @tmpRoles = NULL

                INSERT INTO #RoleID (RoleID)
                VALUES (1)

                SELECT @tmpRoles = COALESCE(@tmpRoles + ',', '') + CONVERT(VARCHAR, r.RoleID)
                FROM #RoleID r WITH (NOLOCK)
                ORDER BY r.RoleID

                UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
                SET Roles = @tmpRoles
                WHERE Id = @AssistantLeaderId
            END
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
            SET Roles = 1
            WHERE Id = @AssistantLeaderId
        END
    END

    SELECT @ErrCode = 0

    IF OBJECT_ID('tempdb..#RoleID') IS NOT NULL
    BEGIN
        DROP TABLE #RoleID
    END
END

